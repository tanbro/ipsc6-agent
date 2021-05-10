using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        public class ConnectionInfo
        {
            public string Host { get; }
            public ushort Port { get; }

            public ConnectionInfo(string host, ushort port = 0)
            {
                Host = host;
                Port = port;
            }
        }

        static void Main(string[] args)
        {
            var ci = new ConnectionInfo("localhost", 8080);
            var s = JsonSerializer.Serialize(ci);
            Console.WriteLine(s);
        }
    }
}
