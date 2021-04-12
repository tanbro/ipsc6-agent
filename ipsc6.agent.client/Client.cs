using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ipsc6.agent.client
{
    class Client
    {
        private List<Connection> connections;

        private void Initialize((ushort, string)[] listenAddresses)
        {
            connections = new List<Connection>();
            foreach (var addr in listenAddresses)
            {
                var connection = new Connection(addr.Item1, addr.Item2);
                connections.Add(connection);
            }
        }

        public Client(ushort localPort, string address)
        {
            Initialize(new[] { (localPort, address) });
        }

    }
}
