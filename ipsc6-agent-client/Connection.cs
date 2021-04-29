using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    public class Connection : IDisposable
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        public Connection(Connector connector, Encoding encoding = null)
        {
            this.connector = connector;
            Encoding = encoding ?? Encoding.Default;
            Initialize();
        }

        public Connection(ushort localPort = 0, string address = "", Encoding encoding = null)
        {
            connector = Connector.CreateInstance(localPort, address);
            Encoding = encoding ?? Encoding.Default;
            Initialize();
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;


        private readonly Connector connector;

        private ConnectionState state = ConnectionState.Init;
        private void SetState(ConnectionState newState)
        {
            var e = new ConnectionStateChangedEventArgs(state, newState);
            state = newState;
            Task.Run(()=>OnConnectionStateChanged?.Invoke(this, e));
        }

        public ConnectionState State
        {
            get { return state; }
        }

        private TaskCompletionSource<object> openTcs;
        private AgentMessage hangingReqType = AgentMessage.UNKNOWN;
        private TaskCompletionSource<AgentMessageReceivedEventArgs> reqTcs;
        private ConcurrentQueue<object> agentMsgQueue;
        private Thread agentMsgTrd;
        private readonly CancellationTokenSource agentMsgTrdCts = new CancellationTokenSource();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                logger.InfoFormat("{0} Dispose", connector.BoundAddress);
                // Check to see if Dispose has already been called.
                if (this.disposed)
                {
                    logger.ErrorFormat("{0} Dispose has already been called.", connector.BoundAddress);
                }
                else
                {
                    logger.DebugFormat("{0} Dispose - Cancel agent msg thred", connector.BoundAddress);
                    agentMsgTrdCts.Cancel();
                    agentMsgTrd.Join();
                    agentMsgTrdCts.Dispose();
                    logger.DebugFormat("{0} Dispose - Deallocate connector", connector.BoundAddress);
                    Connector.DeallocateInstance(connector);
                    // Note disposing has been done.
                    disposed = true;
                    logger.DebugFormat("{0} Dispose - disposed", connector.BoundAddress);
                }
            }
        }

        public bool Connected
        {
            get { return connector.Connected; }
        }

        public event ServerSendEventHandler OnServerSend;
        public event ClosedEventHandler OnClosed;
        public event LostEventHandler OnLost;
        public event ConnectionStateChangedEventHandler OnConnectionStateChanged;

        private void AgentMsgRcvTrdStarter(object obj)
        {
            var token = (CancellationToken)obj;
            bool isResponse;  // 是否 Response ?
            try
            {
                while (!token.IsCancellationRequested)
                {
                    while (agentMsgQueue.TryDequeue(out object msg))
                    {
                        if (msg is AgentMessageReceivedEventArgs)
                        {
                            var e = msg as AgentMessageReceivedEventArgs;
                            lock (requestLock)
                            {
                                isResponse = (hangingReqType == e.CommandType);
                            }
                            if (isResponse)
                            {
                                if ((int)e.N1 < 1)
                                {
                                    var err = new ErrorResponse(e);
                                    logger.ErrorFormat("{0}", err);
                                    reqTcs.SetException(new ErrorResponse(e));
                                }
                                else
                                {
                                    reqTcs.SetResult(e);
                                }
                            }
                            else
                            {
                                /// server->client event
                                OnServerSend?.Invoke(this, e);
                            }
                        }
                    }
                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException) { }
        }

        private void Connector_OnAgentMessageReceived(object sender, AgentMessageReceivedEventArgs e)
        {
            /* UTF-8 转当前编码 */
            var utfBytes = Encoding.GetBytes(e.S);
            e.S = Encoding.UTF8.GetString(utfBytes, 0, utfBytes.Length);
            logger.DebugFormat("{0} AgentMessageReceived: {1}", connector.BoundAddress, e);
            agentMsgQueue.Enqueue(e);
        }

        private void Connector_OnConnectionLost(object sender)
        {
            logger.ErrorFormat("{0} OnConnectionLost", connector.BoundAddress);
            lock (connectLock)
            {
                SetState(ConnectionState.Lost);
            }
            if (openTcs?.Task.Status == TaskStatus.Running)
            {
                Task.Run(() =>
                {
                    openTcs.SetException(new ConnectLostException());
                });
            }
            else
            {
                /// 连接丢失事件
                Task.Run(() => OnLost?.Invoke(this));
            }
        }

        private void Connector_OnDisconnected(object sender)
        {
            logger.WarnFormat("{0} OnDisconnected", connector.BoundAddress);
            lock (connectLock)
            {
                SetState(ConnectionState.Closed);
            }

            if (openTcs?.Task.Status == TaskStatus.Running)
            {
                Task.Run(() =>
                {
                    openTcs.SetException(new ConnectionClosedException());
                });
            }
            else
            {
                /// 连接丢失事件
                Task.Run(() => OnClosed?.Invoke(this));
            }
        }

        private void Connector_OnConnected(object sender, ConnectedEventArgs e)
        {
            logger.InfoFormat("{0} OnConnected", connector.BoundAddress);
            lock(connectLock)
            {
                SetState(ConnectionState.Ok);
            }
            Task.Run(() => openTcs.SetResult(null));
        }

        private void Connector_OnConnectAttemptFailed(object sender)
        {
            logger.ErrorFormat("{0} OnConnectAttemptFailed", connector.BoundAddress);
            lock (connectLock)
            {
                SetState(ConnectionState.Failed);
            }
            Task.Run(() => openTcs.SetException(new ConnectionFailedException()));
        }

        public readonly Encoding Encoding;

        private void Initialize()
        {
            logger.InfoFormat("{0} Initialize", connector.BoundAddress);
            connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;
            connector.OnConnected += Connector_OnConnected;
            connector.OnDisconnected += Connector_OnDisconnected;
            connector.OnConnectionLost += Connector_OnConnectionLost;
            connector.OnAgentMessageReceived += Connector_OnAgentMessageReceived;
            hangingReqType = AgentMessage.UNKNOWN;
            agentMsgQueue = new ConcurrentQueue<object>();
            agentMsgTrd = new Thread(new ParameterizedThreadStart(AgentMsgRcvTrdStarter));
            agentMsgTrd.Start(agentMsgTrdCts.Token);
        }

        private readonly object connectLock = new object();

        public async Task Open(string remoteHost, ushort remotePort = 0)
        {
            remotePort = remotePort > 0 ? remotePort : Connector.DEFAULT_REMOTE_PORT;
            try
            {
                lock (connectLock)
                {
                    ConnectionState[] states = {
                        ConnectionState.Init, ConnectionState.Closed, ConnectionState.Failed, ConnectionState.Lost
                    };
                    if (!states.Any(p => p == State))
                    {
                        throw new InvalidOperationException(string.Format("ConnectionState is {0}", State));
                    }
                    if (openTcs != null)
                    {
                        throw new InvalidOperationException(string.Format("{0}", openTcs.Task.Status));
                    }
                    logger.InfoFormat("{0} Open(remoteHost={1}, remotePort={2}) ...", connector.BoundAddress, remoteHost, remoteHost);
                    openTcs = new TaskCompletionSource<object>();
                    SetState(ConnectionState.Opening);
                    connector.Connect(remoteHost, remotePort);
                }
                await openTcs.Task;
            }
            finally
            {
                lock (connectLock)
                {
                    openTcs = null;
                }
            }
        }

        public void Close()
        {
            lock (connectLock)
            {
                ConnectionState[] states = { ConnectionState.Opening, ConnectionState.Ok };
                if (!states.Any(m => m == State))
                {
                    throw new InvalidOperationException(string.Format("ConnectionState is {0}", State));
                }
                logger.InfoFormat("{0} Close ...", connector.BoundAddress);
                SetState(ConnectionState.Closing);
                connector.Disconnect();
            }
        }

        private static readonly object requestLock = new object();

        public async Task<AgentMessageReceivedEventArgs> Request(AgentRequestArgs args, int millisecondsTimeout = 5000)
        {
            lock (requestLock)
            {
                if (reqTcs != null)
                {
                    throw new InvalidOperationException(string.Format("Invalide request state: {0}", reqTcs.Task.Status));
                }
                if (hangingReqType != AgentMessage.UNKNOWN)
                {
                    throw new InvalidOperationException(string.Format("Invalide hanging request type: {0}", hangingReqType));
                }
                reqTcs = new TaskCompletionSource<AgentMessageReceivedEventArgs>();
                hangingReqType = args.Type;
                logger.InfoFormat("{0} Request({1})", connector.BoundAddress, args);
                connector.SendAgentMessage((int)args.Type, args.N, args.S);
            }
            try
            {
                var task = await Task.WhenAny(reqTcs.Task, Task.Delay(millisecondsTimeout));
                if (task != reqTcs.Task)
                {
                    throw new RequestTimeoutError();
                }
                return await reqTcs.Task;
            }
            finally
            {
                lock (requestLock)
                {
                    hangingReqType = AgentMessage.UNKNOWN;
                    reqTcs = null;
                }
            }
        }

        public async Task LogIn(string username, string password)
        {
            await Request(new AgentRequestArgs(
                AgentMessage.REMOTE_MSG_LOGIN,
                0,
                string.Format("{0}|{1}|1|0|{0}", username, password)
            ));
        }

        public async Task LogOut()
        {
            var data = new AgentRequestArgs(AgentMessage.REMOTE_MSG_RELEASE);
            try
            {
                await Request(data, 100);
            }
            catch (RequestTimeoutError) { }
        }

    }
}
