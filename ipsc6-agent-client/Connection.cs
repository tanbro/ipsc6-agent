using System;
using System.Collections.Concurrent;
using System.Linq;
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
            eventThread = new Thread(new ParameterizedThreadStart(EventThreadStarter));
            Initialize();
        }

        public Connection(ushort localPort = 0, string address = "", Encoding encoding = null)
        {
            connector = network.Connector.CreateInstance(localPort, address);
            Encoding = encoding ?? Encoding.Default;
            eventThread = new Thread(new ParameterizedThreadStart(EventThreadStarter));
            Initialize();
        }

        ~Connection()
        {
            Dispose(false);
        }

        #region IDisposable
        // Flag: Has Dispose already been called?
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                logger.InfoFormat("{0} Dispose", this);
                // Check to see if Dispose has already been called.
                if (this.disposed)
                {
                    logger.ErrorFormat("{0} Dispose has already been called.", this);
                }
                else
                {
                    logger.DebugFormat("{0} Dispose - Stop event thread", this);
                    eventThreadCancelSource.Cancel();
                    eventThread.Join();
                    eventThreadCancelSource.Dispose();
                    logger.DebugFormat("{0} Dispose - Deallocate connector", this);
                    network.Connector.DeallocateInstance(connector);
                    // Note disposing has been done.
                    disposed = true;
                    logger.DebugFormat("{0} Dispose - disposed", this);
                }
            }
        }
        #endregion

        private void Initialize()
        {
            logger.InfoFormat("{0} Initialize", this);
            connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;
            connector.OnConnected += Connector_OnConnected;
            connector.OnDisconnected += Connector_OnDisconnected;
            connector.OnConnectionLost += Connector_OnConnectionLost;
            connector.OnAgentMessageReceived += Connector_OnAgentMessageReceived;
            eventThread.Start(eventThreadCancelSource.Token);
        }

        private readonly network.Connector connector;

        int agentId = -1;
        public int AgentId
        {
            get
            {
                if (state != ConnectionState.Ok)
                {
                    throw new InvalidOperationException(string.Format("{0}", state));
                }
                return agentId;
            }
        }

        private ConnectionState state = ConnectionState.Init;
        public ConnectionState State => state;
        private void SetState(ConnectionState newState)
        {
            var e = new ConnectionStateChangedEventArgs<ConnectionState>(state, newState);
            state = newState;
            Task.Run(() => OnConnectionStateChanged?.Invoke(this, e));
        }

        private TaskCompletionSource<object> connectTcs;
        private TaskCompletionSource<object> disconnectTcs;
        private TaskCompletionSource<int> logInTcs;
        private TaskCompletionSource<object> logOutTcs;
        private MessageType hangingReqType = MessageType.NONE;
        private TaskCompletionSource<ServerSentMessage> reqTcs;
        private readonly ConcurrentQueue<object> eventQueue = new ConcurrentQueue<object>();
        private readonly CancellationTokenSource eventThreadCancelSource = new CancellationTokenSource();
        private readonly Thread eventThread;

        string remoteHost;
        public string RemoteHost => remoteHost;
        ushort remotePort;
        public ushort RemotePort => remotePort;

        public bool Connected => connector.Connected;

        public event ServerSentEventHandler OnServerSentEvent;
        public event ClosedEventHandler OnClosed;
        public event LostEventHandler OnLost;
        public event ConnectionStateChangedEventHandler OnConnectionStateChanged;

        private void EventThreadStarter(object obj)
        {
            var token = (CancellationToken)obj;
            while (!token.IsCancellationRequested)
            {
                while (eventQueue.TryDequeue(out object msg))
                {
                    ProcessEventMessage(msg);
                }
                Thread.Sleep(50);
            }
        }

        private void ProcessEventMessage(object msg)
        {
            bool isResponse;  // 是否 Response ?
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
                    }
                    else
                    {
                        SetState(ConnectionState.Lost);
                    }
                }
                if (msg is ConnectorDisconnectedEventArgs)
                {
                    OnClosed?.Invoke(this);
                }
                else
                {
                    OnLost?.Invoke(this);
                }
                if (prevState == ConnectionState.Closing)
                {
                    disconnectTcs.SetResult(null);
                }
                else if (prevState == ConnectionState.Opening)
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
                    if (msg_.N1 < 1)
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
                            agentId = msg_.N2;
                            SetState(ConnectionState.Ok);
                        }
                        logInTcs.SetResult(msg_.N2);
                    }
                }
                else if (msg_.Type == MessageType.REMOTE_MSG_RELEASE)
                {
                    if (msg_.N1 < 1)
                    {
                        // 注销失败
                        ConnectionState prevState;
                        lock (connectLock)
                        {
                            prevState = state;
                            SetState(ConnectionState.Ok);
                        }
                        logOutTcs.SetException(new ErrorResponse(msg_));
                    }
                    else
                    {
                        // 注销成功实际上并不会发生，因为服务器会直接断开
                        logOutTcs.SetResult(null);
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
                        if (msg_.N1 < 1)
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

        private void Connector_OnAgentMessageReceived(object sender, network.AgentMessageReceivedEventArgs e)
        {
            var data = new ServerSentMessage(e, Encoding);
            logger.DebugFormat("{0} AgentMessageReceived: {1}", this, data);
            eventQueue.Enqueue(data);
        }

        private void Connector_OnConnectionLost(object sender, EventArgs e)
        {
            logger.ErrorFormat("{0} OnConnectionLost", this);
            eventQueue.Enqueue(new ConnectorConnectionLostEventArgs());
        }

        private void Connector_OnDisconnected(object sender, EventArgs e)
        {
            logger.WarnFormat("{0} OnDisconnected", this);
            eventQueue.Enqueue(new ConnectorDisconnectedEventArgs());
        }

        private void Connector_OnConnected(object sender, network.ConnectedEventArgs e)
        {
            logger.InfoFormat("{0} OnConnected", this);
            eventQueue.Enqueue(new ConnectorConnectedEventArgs());
        }

        private void Connector_OnConnectAttemptFailed(object sender, EventArgs e)
        {
            logger.ErrorFormat("{0} OnConnectAttemptFailed", this);
            eventQueue.Enqueue(new ConnectorConnectAttemptFailedEventArgs());
        }

        public readonly Encoding Encoding;

        private readonly object connectLock = new object();

        public void Send(AgentRequestMessage value)
        {
            connector.SendAgentMessage((int)value.Type, value.N, value.S);
        }

        public const int DefaultRequestTimeoutMilliseconds = 15000;
        public const int DefaultKeepAliveTimeoutMilliseconds = 5000;

        public async Task<int> Open(string remoteHost, ushort remotePort, string workerNumber, string password, uint keepAliveTimeout = DefaultKeepAliveTimeoutMilliseconds, int requestTimeout = DefaultRequestTimeoutMilliseconds, int flag = 0)
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
            logger.InfoFormat("{0} Open(remoteHost={1}, remotePort={2}) ...", this, remoteHost, remoteHost);
            connectTcs = new TaskCompletionSource<object>();
            if (remotePort > 0)
            {
                connector.Connect(remoteHost, remotePort, keepAliveTimeout);
            }
            else
            {
                connector.Connect(remoteHost);
            }
            this.remoteHost = remoteHost;
            this.remotePort = remotePort;
            await connectTcs.Task;
            ///
            /// 登录
            logger.InfoFormat("{0} Open(remoteHost={1}, remotePort={2}) ... Log-in(flag={4}) as workerNumber \"{3}\"", this, remoteHost, remoteHost, workerNumber, flag);
            var reqData = new AgentRequestMessage(MessageType.REMOTE_MSG_LOGIN, flag, string.Format("{0}|{1}|1|0|{0}", workerNumber, password));
            logInTcs = new TaskCompletionSource<int>();
            Send(reqData);
            var task = await Task.WhenAny(logInTcs.Task, Task.Delay(requestTimeout));
            if (task != logInTcs.Task)
            {
                logger.ErrorFormat("{0} Open(remoteHost={1}, remotePort={2}) ... Log in timeout. Closing the connector", this, remoteHost, remoteHost);
                connector.Disconnect();
                throw new ConnectionTimeoutException();
            }

            var agentid = await logInTcs.Task;
            logger.InfoFormat("{0} Open(remoteHost={1}, remotePort={2}) ... Succeed: workerNumber=\"{3}\", AgentId={4}", this, remoteHost, remoteHost, workerNumber, agentid);
            return agentid;
        }

        public async Task<int> Open(string remoteHost, string workerNumber, string password, uint keepAliveTimeout = DefaultKeepAliveTimeoutMilliseconds, int flag = 0)
        {
            return await Open(remoteHost, 0, workerNumber, password, keepAliveTimeout, flag);
        }

        public async Task Close(bool graceful = true, int requestTimeout = DefaultRequestTimeoutMilliseconds, int flag = 0)
        {
            ConnectionState[] closedStates = { ConnectionState.Closed, ConnectionState.Failed, ConnectionState.Lost };
            ConnectionState[] allowedStates = { ConnectionState.Opening, ConnectionState.Ok };
            Task task;

            lock (connectLock)
            {
                if (!closedStates.Any(m => m == State))
                {
                    logger.DebugFormat("{0} Close(graceful) ... Already closed.", this);
                    return;
                }
                if (!allowedStates.Any(m => m == State))
                {
                    throw new InvalidOperationException(string.Format("{0}", State));
                }
                SetState(ConnectionState.Closing);
                disconnectTcs = new TaskCompletionSource<object>();
            }

            if (graceful)
            {
                logOutTcs = new TaskCompletionSource<object>();
                var timeoutTask = Task.Delay(requestTimeout);
                logger.InfoFormat("{0} Close(graceful) ...", this);
                Send(new AgentRequestMessage(MessageType.REMOTE_MSG_RELEASE, flag));
                task = await Task.WhenAny(logOutTcs.Task, disconnectTcs.Task, timeoutTask);
                if (task == timeoutTask)
                {
                    throw new DisconnectionTimeoutException();
                }
                else
                {
                    await task;
                }
            }
            else
            {
                var timeoutTask = Task.Delay(requestTimeout);
                logger.InfoFormat("{0} Close(force) ...", this);
                connector.Disconnect();
                task = await Task.WhenAny(disconnectTcs.Task, timeoutTask);
                if (task == timeoutTask)
                {
                    throw new ConnectionTimeoutException();
                }
                else
                {
                    await task;
                }
            }
        }

        private static readonly object requestLock = new object();

        public async Task<ServerSentMessage> Request(AgentRequestMessage args, int millisecondsTimeout = DefaultRequestTimeoutMilliseconds)
        {
            lock (connectLock)
            {
                if (state != ConnectionState.Ok)
                {
                    throw new InvalidOperationException(string.Format("Can not send a request when state is {0}", state));
                }
            }
            lock (requestLock)
            {
                if (hangingReqType != MessageType.NONE)
                {
                    throw new InvalidOperationException(string.Format("A hanging request exists: {0}", hangingReqType));
                }
                hangingReqType = args.Type;
            }
            try
            {
                logger.InfoFormat("{0} Request({1})", this, args);
                reqTcs = new TaskCompletionSource<ServerSentMessage>();
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

        public override string ToString()
        {
            return string.Format(
                "<{0} at 0x{1:x8} Local={2}, Remote={3}:{4}, State={4}>",
                GetType().Name, GetHashCode(), connector.BoundAddress, RemoteHost, RemotePort, State);
        }

    }
}
