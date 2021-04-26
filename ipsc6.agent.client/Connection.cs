using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ipsc6.agent.network;


namespace ipsc6.agent.client
{
    public class Connection : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool disposed = false;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        private Connector connector;
        private TaskCompletionSource tcsConnect;
        private AgentMessageEnum msgtypRequest;
        private TaskCompletionSource<AgentMessageReceivedEventArgs> tcsRequest;

        private void Initialize(Connector connector)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.connector = connector;
            this.connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;
            this.connector.OnConnected += Connector_OnConnected;
            this.connector.OnDisconnected += Connector_OnDisconnected;
            this.connector.OnConnectionLost += Connector_OnConnectionLost;
            this.connector.OnAgentMessageReceived += Connector_OnAgentMessageReceived;
            msgtypRequest = AgentMessageEnum.UNKNOWN;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.Info("Dispose");
            if (disposing)
            {
                // Check to see if Dispose has already been called.
                if (!this.disposed)
                {
                    Connector.DeallocateInstance(connector);
                    connector = null;
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

        private void Connector_OnAgentMessageReceived(object sender, AgentMessageReceivedEventArgs e)
        {
            //var utfBytes = Console.OutputEncoding.GetBytes(e.S);
            //e.S = Encoding.Default.GetString(utfBytes, 0, utfBytes.Length);
            logger.DebugFormat("{0} OnAgentMessageReceived: {1} {2} {3} {4}", connector.BoundAddress, e.CommandType, e.N1, e.N2, e.S);
            if (null != tcsRequest)
            {
                if (e.CommandType == (int)msgtypRequest)
                {
                    /// response
                    if (e.N1 == 1)
                    {
                        Task.Run(() => tcsRequest.SetResult(e));
                    }
                    else
                    {
                        var errMsg = string.Format("ErrorResponse. Code: {0}", e.N1);
                        Task.Run(() => tcsRequest.SetException(new ErrorResponseException(errMsg)));
                    }

                }
                else if (e.CommandType < 1)
                {
                    var errMsg = string.Format("ServerSendError. Code: {0}", e.N1);
                    Task.Run(() => tcsRequest.SetException(new ServerSendError(errMsg)));
                }
            }
            else
            {
                /// server->client event
                Task.Run(() => OnServerSendEventReceived?.Invoke(this, e));
            }
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
                /// 连接断开事件
                Task.Run(() => OnDisconnected?.Invoke(this));
            }
        }

        private void Connector_OnConnected(object sender, ConnectedEventArgs e)
        {
            logger.InfoFormat("{0} OnConnected", connector.BoundAddress);
            Task.Run(() => tcsConnect.SetResult());
        }

        private void Connector_OnConnectAttemptFailed(object sender)
        {
            logger.InfoFormat("{0} OnConnectAttemptFailed", connector.BoundAddress);
            Task.Run(() => tcsConnect.SetException(new AgentConnectFailedException("ConnectAttemptFailed")));
        }

        public Connection(Connector connector)
        {
            Initialize(connector);
        }

        public Connection(ushort localPort = 0, string address = "")
        {
            logger.InfoFormat("{0} CreateInstance(localPort={1}, address={2})", typeof(Connector), localPort, address);
            var connector = Connector.CreateInstance(localPort, address);
            Initialize(connector);
        }

        private static readonly object connectLock = new();

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
                tcsConnect = new();
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

        private static readonly object requestLock = new();

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
