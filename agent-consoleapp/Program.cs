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

        static async Task Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            // Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Console.OutputEncoding: {0}", Console.OutputEncoding);
            Console.WriteLine("Default Encoding: {0}", Encoding.Default.EncodingName);
            Console.WriteLine("汉字测试。");

            Connector.Initial();
            try
            {
                do
                {
                    using Connection conn = new();
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
                        break;
                    }
                    await conn.LogIn("1001", "1001");
                    Console.WriteLine("LogIn OK");

                    while (true)
                    {
                        var s = Console.ReadLine();
                    }

                } while (false);

            }
            finally
            {
                Connector.Release();
            }
        }
    }
}
