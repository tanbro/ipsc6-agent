using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ipsc6.agent.services.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Model
    {
        public string WorkerNum { get; internal set; }
        public string DisplayName { get; internal set; }
        public IReadOnlyCollection<Group> Groups { get; internal set; } = new List<Group>();
        public IReadOnlyCollection<CtiServer> CtiServers { get; internal set; } = new List<CtiServer>();
        public client.AgentState State { get; internal set; }
        public client.WorkType WorkType { get; internal set; }
        public client.TeleState TeleState { get; internal set; }
        public IReadOnlyCollection<SipAccount> SipAccounts { get; internal set; } = new List<SipAccount>();
        public IReadOnlyCollection<CallInfo> Calls { get; internal set; } = new List<CallInfo>();
    }
}
