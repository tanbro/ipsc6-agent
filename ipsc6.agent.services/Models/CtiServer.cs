using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ipsc6.agent.services.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CtiServer
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public bool IsMain { get; set; }
        public client.ConnectionState State { get; set; }
    }
}
