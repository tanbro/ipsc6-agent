using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ipsc6.agent.network;
using ipsc6.agent.client;

namespace agent_consoleapp
{
    class Program
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

        static async Task Main(string[] _)
        {
            log4net.Config.BasicConfigurator.Configure();
            Connector.Initial();
            try
            {
                using Connection conn = new();
                conn.OnServerSendEventReceived += Conn_OnServerSendEventReceived;
                var server = "192.168.2.108";
                Console.WriteLine("Connect ... {0}", server);
                try
                {
                    await conn.Connect(server);
                    Console.WriteLine("Connected ... AgentId={0}", conn.AgentId);
                }
                catch (AgentConnectException exce)
                {
                    Console.WriteLine("Connected ... Failed: {0}", exce);
                    Environment.Exit(1);
                }
                await conn.LogIn("1001", "1001");
                Console.WriteLine("LogIn OK");

                Console.WriteLine("CTRL+C to exit");
                bool exiting = false;
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    exiting = true;
                };
                while (!exiting)
                {
                    Thread.Sleep(1000);
                }

            }
            finally
            {
                Connector.Release();
            }
        }

        private static void Conn_OnServerSendEventReceived(object sender, AgentMessageReceivedEventArgs e)
        {
            log.InfoFormat("ServerSendEvent: {0} {1} {2}", e.N1, e.N2, e.S);
        }
    }
}
