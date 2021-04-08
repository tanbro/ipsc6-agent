using System;
using System.Threading;
using org.pjsip.pjsua2;
using ipsc6.agent.network;

namespace agent_consoleapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connector.Initial ...");
            Connector.Initial();
            Console.WriteLine("Connector.CreateInstance ...");
            var connector = Connector.CreateInstance();

            connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;

            Console.WriteLine("Connector.Connect ...");
            connector.Connect("127.0.0.1", 13920);

            while (true)
            {
                var inputString = Console.ReadLine();
                if (String.Equals(inputString, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            Console.WriteLine("Connector.DeallocateInstance ...");
            Connector.DeallocateInstance(connector);
            Console.WriteLine("Connector.Release ...");
            Connector.Release();
        }

        private static void Connector_OnConnectAttemptFailed()
        {
            Console.WriteLine("Connector_OnConnectAttemptFailed!!!");
        }
    }
}
