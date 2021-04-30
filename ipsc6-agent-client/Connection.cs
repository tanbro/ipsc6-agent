using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ipsc6.agent.client
{
    public class Connection : IDisposable
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        public Connection(network.Connector connector, Encoding encoding = null)
        {
            this.connector = connector;
            Encoding = encoding ?? Encoding.Default;
            Initialize();
        }

        public Connection(ushort localPort = 0, string address = "", Encoding encoding = null)
        {
            connector = network.Connector.CreateInstance(localPort, address);
            Encoding = encoding ?? Encoding.Default;
            Initialize();
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;


        private readonly network.Connector connector;

        private ConnectionState state = ConnectionState.Init;
        private void SetState(ConnectionState newState)
        {
            var e = new ConnectionStateChangedEventArgs(state, newState);
            state = newState;
            Task.Run(() => OnConnectionStateChanged?.Invoke(this, e));
        }

        public ConnectionState State
        {
            get { return state; }
        }

        private TaskCompletionSource<object> connectTcs;
        private TaskCompletionSource<object> logInTcs;
        private MessageType hangingReqType = MessageType.NONE;
        private TaskCompletionSource<ServerSentMessage> reqTcs;
        private ConcurrentQueue<object> eventQueue;
        private Thread eventThread;
        private readonly CancellationTokenSource eventThreadCancelSource = new CancellationTokenSource();

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
                    logger.DebugFormat("{0} Dispose - Stop event thread", connector.BoundAddress);
                    eventThreadCancelSource.Cancel();
                    eventThread.Join();
                    eventThreadCancelSource.Dispose();
                    logger.DebugFormat("{0} Dispose - Deallocate connector", connector.BoundAddress);
                    network.Connector.DeallocateInstance(connector);
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

        public event ServerSentEventHandler OnServerSentEvent;
        public event ClosedEventHandler OnClosed;
        public event LostEventHandler OnLost;
        public event ConnectionStateChangedEventHandler OnConnectionStateChanged;

        private void EventThreadStarter(object obj)
        {
            var token = (CancellationToken)obj;
            bool isResponse;  // 是否 Response ?
            try
            {
                while (!token.IsCancellationRequested)
                {
                    while (eventQueue.TryDequeue(out object msg))
                    {
                        if (msg is ConnectorConnectedEventArgs)
                        {
                            connectTcs.SetResult(msg);
                        }
                        else if (msg is ConnectorConnectAttemptFailedEventArgs)
                        {
                            lock (connectLock)
                            {
                                SetState(ConnectionState.Failed);
                            }
                            connectTcs.SetException(new ConnectionFailedException());
                        }
                        else if ((msg is ConnectorDisconnectedEventArgs) || (msg is ConnectorConnectionLostEventArgs))
                        {
                            ConnectionState prevState;
                            lock (connectLock)
                            {
                                prevState = state;
                                if (msg is ConnectorDisconnectedEventArgs)
                                {
                                    SetState(ConnectionState.Closed);
                                    OnClosed?.Invoke(this);
                                }
                                else if (msg is ConnectorConnectionLostEventArgs)
                                {
                                    SetState(ConnectionState.Lost);
                                    OnLost?.Invoke(this);
                                }
                            }
                            if (prevState == ConnectionState.Opening)
                            {
                                if (msg is ConnectorDisconnectedEventArgs)
                                {
                                    try
                                    {
                                        connectTcs.SetException(new ConnectionClosedException());
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        logInTcs.SetException(new ConnectionClosedException());
                                    }
                                }
                                else if (msg is ConnectorConnectionLostEventArgs)
                                {
                                    try
                                    {
                                        connectTcs.SetException(new ConnecttionLostException());
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        logInTcs.SetException(new ConnecttionLostException());
                                    }
                                }
                            }
                        }
                        else if (msg is ServerSentMessage)
                        {
                            var msg_ = msg as ServerSentMessage;

                            if (msg_.Type == MessageType.REMOTE_MSG_LOGIN)
                            {
                                if ((int)msg_.N1 < 1)
                                {
                                    var err = new ErrorResponse(msg_);
                                    logger.ErrorFormat("Login failed: {0}. Connector will be closed.", err);
                                    logInTcs.SetException(new ErrorResponse(msg_));
                                    connector.Disconnect();
                                    // 登录失败算一种连接失败
                                    lock (connectLock)
                                    {
                                        SetState(ConnectionState.Failed);
                                    }
                                }
                                else
                                {
                                    lock (connectLock)
                                    {
                                        SetState(ConnectionState.Ok);
                                    }
                                    logInTcs.SetResult(msg_);
                                }
                            }
                            else
                            {
                                lock (requestLock)
                                {
                                    isResponse = (hangingReqType == msg_.Type);
                                }
                                if (isResponse)
                                {
                                    if ((int)msg_.N1 < 1)
                                    {
                                        var err = new ErrorResponse(msg_);
                                        logger.ErrorFormat("{0}", err);
                                        reqTcs.SetException(new ErrorResponse(msg_));
                                    }
                                    else
                                    {
                                        reqTcs.SetResult(msg_);
                                    }
                                }
                                else
                                {
                                    /// server->client event
                                    OnServerSentEvent?.Invoke(this, new ServerSentEventArgs(msg_));
                                }
                            }
                        }
                    }
                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException) { }
        }

        private void Connector_OnAgentMessageReceived(object sender, network.AgentMessageReceivedEventArgs e)
        {
            /* UTF-8 转当前编码 */
            var data = new ServerSentMessage(e);
            logger.DebugFormat("{0} AgentMessageReceived: {1}", connector.BoundAddress, data);
            eventQueue.Enqueue(data);
        }

        private void Connector_OnConnectionLost(object sender)
        {
            logger.ErrorFormat("{0} OnConnectionLost", connector.BoundAddress);
            eventQueue.Enqueue(new ConnectorConnectionLostEventArgs());
        }

        private void Connector_OnDisconnected(object sender)
        {
            logger.WarnFormat("{0} OnDisconnected", connector.BoundAddress);
            eventQueue.Enqueue(new ConnectorDisconnectedEventArgs());
        }

        private void Connector_OnConnected(object sender, network.ConnectedEventArgs e)
        {
            logger.InfoFormat("{0} OnConnected", connector.BoundAddress);
            eventQueue.Enqueue(new ConnectorConnectedEventArgs());
        }

        private void Connector_OnConnectAttemptFailed(object sender)
        {
            logger.ErrorFormat("{0} OnConnectAttemptFailed", connector.BoundAddress);
            eventQueue.Enqueue(new ConnectorConnectAttemptFailedEventArgs());
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
            eventQueue = new ConcurrentQueue<object>();
            eventThread = new Thread(new ParameterizedThreadStart(EventThreadStarter));
            eventThread.Start(eventThreadCancelSource.Token);
        }

        private readonly object connectLock = new object();

        public void Send(AgentRequestMessage value)
        {
            connector.SendAgentMessage((int)value.Type, value.N, value.S);
        }

        public const int DefaultTimeoutMilliseconds = 5000;

        public async Task Open(string remoteHost, ushort remotePort, string username, string password, int millisecondsTimeout = DefaultTimeoutMilliseconds)
        {
            ConnectionState[] allowStates = { ConnectionState.Init, ConnectionState.Closed, ConnectionState.Failed, ConnectionState.Lost };
            lock (connectLock)
            {
                if (allowStates.Any(p => p == State))
                {
                    SetState(ConnectionState.Opening);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0}", State));
                }
            }
            logger.InfoFormat("{0} Open(remoteHost={1}, remotePort={2}) ...", connector.BoundAddress, remoteHost, remoteHost);
            connectTcs = new TaskCompletionSource<object>();
            if (remotePort > 0)
            {
                connector.Connect(remoteHost, remotePort);
            }
            else
            {
                connector.Connect(remoteHost);
            }
            await connectTcs.Task;
            ///
            /// 登录
            logger.InfoFormat("{0} Open(remoteHost={1}, remotePort={2}) ... Log in as user \"{3}\"", connector.BoundAddress, remoteHost, remoteHost, username);
            var reqData = new AgentRequestMessage(MessageType.REMOTE_MSG_LOGIN, string.Format("{0}|{1}|1|0|{0}", username, password));
            logInTcs = new TaskCompletionSource<object>();
            Send(reqData);
            var task = await Task.WhenAny(logInTcs.Task, Task.Delay(millisecondsTimeout));
            if (task != logInTcs.Task)
            {
                logger.ErrorFormat("{0} Open(remoteHost={1}, remotePort={2}) ... Log in timeout. Closing the connector", connector.BoundAddress, remoteHost, remoteHost);
                connector.Disconnect();
                throw new ConnectionTimeoutException();
            }
            logger.Info("Log in succeed");
            await logInTcs.Task;
        }

        public async Task Open(string remoteHost, string username, string password, int millisecondsTimeout = DefaultTimeoutMilliseconds)
        {
            await Open(remoteHost, 0, username, password, millisecondsTimeout);
        }


        public void Close()
        {
            ConnectionState[] allowedStates = { ConnectionState.Opening, ConnectionState.Ok };
            lock (connectLock)
            {
                if (!allowedStates.Any(m => m == State))
                {
                    throw new InvalidOperationException(string.Format("{0}", State));
                }
                logger.InfoFormat("{0} Close ...", connector.BoundAddress);
                SetState(ConnectionState.Closing);
                connector.Disconnect();
            }
        }

        private static readonly object requestLock = new object();

        public async Task<ServerSentMessage> Request(AgentRequestMessage args, int millisecondsTimeout = DefaultTimeoutMilliseconds)
        {
            lock (requestLock)
            {
                if (hangingReqType != MessageType.NONE)
                {
                    throw new InvalidOperationException(string.Format("A hanging request exists: {0}", hangingReqType));
                }
                hangingReqType = args.Type;
            }
            logger.InfoFormat("{0} Request({1})", connector.BoundAddress, args);
            reqTcs = new TaskCompletionSource<ServerSentMessage>();
            try
            {
                connector.SendAgentMessage((int)args.Type, args.N, args.S);
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
                    hangingReqType = MessageType.NONE;
                }
            }
        }

    }
}
