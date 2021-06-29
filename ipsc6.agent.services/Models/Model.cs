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
    public class Model
    {
        public string WorkerNumber { get; internal set; }
        public string DisplayName { get; internal set; }
        public IReadOnlyCollection<Group> Groups { get; internal set; }
        public IReadOnlyCollection<CtiServer> CtiServers { get; internal set; }
        public client.AgentState State { get; internal set; }
        public client.WorkType WorkType { get; internal set; }
        public client.TeleState TeleState { get; internal set; }
    }
}
