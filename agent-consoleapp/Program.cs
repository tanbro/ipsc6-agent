using System;
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
            connector.OnConnected += Connector_OnConnected;

            Console.WriteLine("Connector.Connect ...");
            connector.Connect("192.168.2.107", 13920);

            Console.CancelKeyPress += Console_CancelKeyPress;
            isCancelKeyPressed = false;
            while (!isCancelKeyPressed)
            {
                Console.ReadLine();
            }

            Console.WriteLine("Connector.DeallocateInstance ...");
            Connector.DeallocateInstance(connector);
            Console.WriteLine("Connector.Release ...");
            Connector.Release();
        }

        static bool isCancelKeyPressed = false;

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            isCancelKeyPressed = true;
            e.Cancel = true;
        }

        private static void Connector_OnConnected(int connectionId)
        {
            Console.WriteLine("Connector_OnConnected!!! connectionId={0}", connectionId);
        }

        private static void Connector_OnConnectAttemptFailed()
        {
            Console.WriteLine("Connector_OnConnectAttemptFailed!!!");
        }
    }
}
