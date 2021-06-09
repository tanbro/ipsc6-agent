using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.wpfapp.Config
{
    public class Ipsc
    {
        public ushort LocalPort { get; set; }
        public string LocalAddress { get; set; }

        public IList<string> ServerList { get; set; }

        public string Name { get; set; }
    }
}
