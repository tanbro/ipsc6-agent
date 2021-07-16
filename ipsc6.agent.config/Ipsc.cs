using System.Collections.Generic;

namespace ipsc6.agent.config
{
    public class Ipsc
    {
        public ushort LocalPort { get; set; }
        public string LocalAddress { get; set; }

        public IList<string> ServerList { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("<{0} ServerList={1}, LocalPort=\"{2}\", LocalAddress=\"{3}\">",
                GetType().Name,
                (ServerList == null) ? "<null>" : $"\"{string.Join(",", ServerList)}\"",
                LocalPort,
                LocalAddress
            );
        }
    }
}
