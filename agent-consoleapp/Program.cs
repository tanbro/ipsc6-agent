using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ipsc6.agent.network;

namespace agent_consoleapp
{
    class Program
    {
        static Connector connector;

        static async Task Main(string[] args)
        {

            Console.WriteLine("Connector.Initial ...");
            Connector.Initial();
            Console.WriteLine("Connector.CreateInstance ...");

            connector = Connector.CreateInstance();
            connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;
            connector.OnConnected += Connector_OnConnected;

            Console.WriteLine("Connector.Connect ...");
            var connectId = await ConnectAsync("192.168.2.107", 13920);
            Console.WriteLine("_connectId={0}", connectId);

            Console.WriteLine("Connector.DeallocateInstance ...");
            Connector.DeallocateInstance(connector);
            Console.WriteLine("Connector.Release ...");
            Connector.Release();
        }

        static TaskCompletionSource<int> ctConnect;

        static Task<int> ConnectAsync(string host, UInt16 port = 13920)
        {
            ctConnect = new();
            connector.Connect(host, port);
            return ctConnect.Task;
        }

        private static void Connector_OnConnected(int connectionId)
        {
            ctConnect.SetResult(connectionId);
        }

        private static void Connector_OnConnectAttemptFailed()
        {
            ctConnect.SetException(new SystemException("ConnectAttemptFailed"));
        }
    }
}
