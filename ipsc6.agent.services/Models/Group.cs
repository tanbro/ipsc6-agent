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
    public class Group
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsSigned { get; internal set; }
    }
}
