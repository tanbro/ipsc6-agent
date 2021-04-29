using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ipsc6.agent.network;


namespace ipsc6.agent.client
{
    public class Connection : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool disposed = false;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        private readonly Connector connector;
        private TaskCompletionSource<object> tcsConnect;
        private AgentMessageEnum msgtypRequest;
        private TaskCompletionSource<AgentMessageReceivedEventArgs> tcsRequest;
        private ConcurrentQueue<object> agentMessageQueue;
        private Thread agentMessageThread;

        private void Initialize()
        {
            logger.InfoFormat("Initialize: BoundAddress={0}", connector.BoundAddress);
            connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;
            connector.OnConnected += Connector_OnConnected;
            connector.OnDisconnected += Connector_OnDisconnected;
            connector.OnConnectionLost += Connector_OnConnectionLost;
            connector.OnAgentMessageReceived += Connector_OnAgentMessageReceived;
            msgtypRequest = AgentMessageEnum.UNKNOWN;
            agentMessageQueue = new ConcurrentQueue<object>();
            agentMessageThread = new Thread(AgentMessageReceiveThreadStarter);
            agentMessageThread.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.InfoFormat("Dispose: BoundAddress={0}", connector.BoundAddress);
            if (disposing)
            {
                // Check to see if Dispose has already been called.
                if (!this.disposed)
                {
                    agentMessageThread.Abort();
                    agentMessageThread.Join();
                    Connector.DeallocateInstance(connector);
                    // Note disposing has been done.
                    disposed = true;
                }
            }
        }

        public bool Connected
        {
            get { return connector.Connected; }
        }

        public event ServerSendEventHandler OnServerSendEventReceived;
        public event DisconnectedEventHandler OnDisconnected;
        public event ConnectionLostEventHandler OnConnectionLost;

        private void AgentMessageReceiveThreadStarter(object _)
        {
            try
            {
                while (true)
                {
                    while (agentMessageQueue.TryDequeue(out object msg))
                    {
                        if (msg is AgentMessageReceivedEventArgs)
                        {
                            var e = msg as AgentMessageReceivedEventArgs;
                            /// 是否 Response ?
                            var isResponse = false;
                            lock (requestLock)
                            {
                                isResponse = (int)msgtypRequest == e.CommandType;
                            }
                            if (isResponse)
                            {
                                if (e.CommandType == (int)msgtypRequest)
                                {
                                    /// response
                                    if (e.N1 == 1)
                                    {
                                        tcsRequest.SetResult(e);
                                    }
                                    else
                                    {
                                        var errMsg = string.Format("ErrorResponse. Code: {0}", e.N1);
                                        tcsRequest.SetException(new ErrorResponseException(errMsg));
                                    }

                                }
                                else if (e.CommandType < 1)
                                {
                                    var errMsg = string.Format("ServerSendError. Code: {0}", e.N1);
                                    tcsRequest.SetException(new ServerSendError(errMsg));
                                }
                            }
                            else
                            {
                                /// server->client event
                                OnServerSendEventReceived?.Invoke(this, e);
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
            var utfBytes = Encoding.Default.GetBytes(e.S);
            e.S = Encoding.UTF8.GetString(utfBytes, 0, utfBytes.Length);
            logger.DebugFormat("{0} AgentMessageReceived: {1} {2} {3} {4}", connector.BoundAddress, e.CommandType, e.N1, e.N2, e.S);
            agentMessageQueue.Enqueue(e);
        }

        private void Connector_OnConnectionLost(object sender)
        {
            logger.WarnFormat("{0} OnConnectionLost", connector.BoundAddress);
            if (tcsConnect != null)
            {
                if (tcsConnect.Task.Status == TaskStatus.Running)
                {
                    Task.Run(() =>
                    {
                        tcsConnect.SetException(new AgentConnectFailedException("ConnectionLost"));
                    });
                }
            }
            else
            {
                /// 连接丢失事件
                Task.Run(() => OnConnectionLost?.Invoke(this));
            }
        }

        private void Connector_OnDisconnected(object sender)
        {
            logger.InfoFormat("{0} OnDisconnected", connector.BoundAddress);
            if (tcsConnect != null)
            {
                if (tcsConnect.Task.Status == TaskStatus.Running)
                {
                    Task.Run(() =>
                    {
                        tcsConnect.SetException(new AgentConnectFailedException("Disconnected"));
                    });
                }
            }
            else
            {
                /// 连接丢失事件
                Task.Run(() => OnDisconnected?.Invoke(this));
            }
        }

        private void Connector_OnConnected(object sender, ConnectedEventArgs e)
        {
            logger.InfoFormat("{0} OnConnected", connector.BoundAddress);
            Task.Run(() => tcsConnect.SetResult(null));
        }

        private void Connector_OnConnectAttemptFailed(object sender)
        {
            logger.InfoFormat("{0} OnConnectAttemptFailed", connector.BoundAddress);
            Task.Run(() => tcsConnect.SetException(new AgentConnectFailedException("ConnectAttemptFailed")));
        }

        private readonly Encoding encoding;

        public Connection(Connector connector, Encoding encoding=null)
        {
            this.connector = connector;
            this.encoding = encoding ?? Encoding.Default;
            Initialize();
        }

        public Connection(ushort localPort = 0, string address = "", Encoding encoding=null)
        {
            connector = Connector.CreateInstance(localPort, address);
            this.encoding = encoding ?? Encoding.Default;
            Initialize();
        }

        private readonly object connectLock = new object();

        public async Task Open(string host, ushort port = 0)
        {
            port = port > 0 ? port : Connector.DEFAULT_REMOTE_PORT;
            logger.InfoFormat("Open(host={0}, port={1}) ...", host, port);
            lock (connectLock)
            {
                if (tcsConnect != null)
                {
                    throw new InvalidOperationException(string.Format("{0}", tcsConnect.Task.Status));
                }
                tcsConnect = new TaskCompletionSource<object>();
            }
            try
            {
                lock (connectLock)
                {
                    connector.Connect(host, port);
                }
                await tcsConnect.Task;
            }
            finally
            {
                lock (connectLock)
                {
                    tcsConnect = null;
                }
            }
        }

        public void Close()
        {
            connector.Disconnect();
        }

        private static readonly object requestLock = new object();

        public async Task<AgentMessageReceivedEventArgs> Request(AgentRequestArgs args, int millisecondsTimeout = 5000)
        {
            lock (requestLock)
            {
                if (tcsRequest != null)
                {
                    throw new InvalidOperationException(string.Format("{0}", tcsRequest.Task.Status));
                }
                tcsRequest = new TaskCompletionSource<AgentMessageReceivedEventArgs>();
            }
            try
            {
                lock (requestLock)
                {
                    msgtypRequest = args.Type;
                    connector.SendAgentMessage((int)args.Type, args.N, args.S);
                }
                var task = await Task.WhenAny(tcsRequest.Task, Task.Delay(millisecondsTimeout));
                if (task != tcsRequest.Task)
                {
                    throw new AgentRequestTimeoutException();
                }
                return await tcsRequest.Task;
            }
            finally
            {
                lock (requestLock)
                {
                    msgtypRequest = AgentMessageEnum.UNKNOWN;
                    tcsRequest = null;
                }
            }
        }

        public async Task LogIn(string username, string password)
        {
            await Request(new AgentRequestArgs(
                AgentMessageEnum.REMOTE_MSG_LOGIN,
                0,
                string.Format("{0}|{1}|1|0|{0}", username, password)
            ));
        }

        public async Task LogOut()
        {
            await Request(new AgentRequestArgs(AgentMessageEnum.REMOTE_MSG_RELEASE));
        }

        public async Task SignOn(string groupId)
        {
            await Request(new AgentRequestArgs(
                AgentMessageEnum.REMOTE_MSG_SIGNON,
                0,
                groupId
            ));
        }

    }
}
