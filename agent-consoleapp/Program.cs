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
            Console.WriteLine("Hello World!");
            Connection conn = new();

            // Here we don't have anything else to do..
            Thread.Sleep(10000);
        }
    }
}
