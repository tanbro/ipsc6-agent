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
        private TaskCompletionSource<int> tcsConnected;
        private AgentMessageEnum msgtypRequest;
        private TaskCompletionSource<AgentMessageReceivedEventArgs> tcsRequest = null;

        private void Initialize(Connector connector)
        {
            logger.Info("Initialize");
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

        public int AgentId
        {
            get { return connector.AgentId; }
        }

        public bool Connected
        {
            get { return connector.Connected; }
        }

        private void Connector_OnAgentMessageReceived(object sender, AgentMessageReceivedEventArgs e)
        {
            var utfBytes = Console.OutputEncoding.GetBytes(e.S);
            e.S = Encoding.Default.GetString(utfBytes, 0, utfBytes.Length);
            logger.DebugFormat("OnAgentMessageReceived 0 - {0} {1} {2} {3}", e.CommandType, e.N1, e.N2, e.S);
            lock (this)
            {
                if ((e.CommandType == (int)msgtypRequest) && (tcsRequest != null))
                {
                    /// response
                    Task.Run(() => tcsRequest.SetResult(e));
                    return;
                }
                else
                {
                    /// server->client event
                    // TODO: EVENT!
                }
            }
        }

        private void Connector_OnConnectionLost(object sender)
        {
            if (tcsConnected.Task.Status == TaskStatus.Running)
            {
                Task.Run(() => tcsConnected.SetException(new AgentConnectFailedException("ConnectionLost")));
                return;
            }
        }

        private void Connector_OnDisconnected(object sender)
        {
            if (tcsConnected.Task.Status == TaskStatus.Running)
            {
                Task.Run(() => tcsConnected.SetException(new AgentConnectFailedException("Disconnected")));
                return;
            }
        }

        private void Connector_OnConnected(object sender, ConnectedEventArgs e)
        {
            Task.Run(() => tcsConnected.SetResult(e.AgentId));
        }

        private void Connector_OnConnectAttemptFailed(object sender)
        {
            Task.Run(() => tcsConnected.SetException(new AgentConnectFailedException("ConnectAttemptFailed")));
        }

        public Connection(Connector connector)
        {
            Initialize(connector);
        }

        public Connection(ushort localPort = 0, string address = "")
        {
            var connector = Connector.CreateInstance(localPort, address);
            Initialize(connector);
        }

        public Task<int> Connect(string host, ushort port)
        {
            tcsConnected = new();
            connector.Connect(host, port);
            return tcsConnected.Task;
        }

        public Task<int> Connect(string host)
        {
            return Connect(host, Connector.DEFAULT_REMOTE_PORT);
        }

        public async Task<AgentMessageReceivedEventArgs> Request(AgentRequestArgs args, int millisecondsTimeout = 2500)
        {
            lock (this)
            {
                if (tcsRequest != null)
                {
                    throw new InvalidOperationException(string.Format("{0}", tcsRequest.Task.Status));
                }
                tcsRequest = new TaskCompletionSource<AgentMessageReceivedEventArgs>();
            }
            msgtypRequest = args.Type;
            try
            {
                try
                {
                    connector.SendAgentMessage(connector.AgentId, (int)args.Type, args.N, args.S);
                }
                catch
                {
                    msgtypRequest = AgentMessageEnum.UNKNOWN;
                    throw;
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
                msgtypRequest = AgentMessageEnum.UNKNOWN;
                tcsRequest = null;
            }
        }

        public async Task LogIn(string username, string password)
        {
            var s = string.Format("{0}|{1}|1|0|{0}", username, password);
            var msg = new AgentRequestArgs(AgentMessageEnum.REMOTE_MSG_LOGIN, 0, s);
            await Request(msg);
        }



    }
}
