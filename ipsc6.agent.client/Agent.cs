using System;
using System.Threading;
using System.Threading.Tasks;

using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    class Agent
    {
        Connector connector;
        public void Connect(string host, UInt16 port = 13920)
        {
            connector = Connector.CreateInstance();
            connector.OnConnected += Connector_OnConnected;
            connector.Connect(host, port);
        }

        private void Connector_OnConnected(int connectionId)
        {
            ctConnect.SetResult(connectionId);
        }

        TaskCompletionSource<int> ctConnect;

        public Task<int> ConnectAsync(string host, UInt16 port = 13920, CancellationToken token)
        {
            ctConnect = new TaskCompletionSource<int>();
            Connect(host, port);
            return ctConnect.Task;
        }

    }
}
