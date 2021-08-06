using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct QueueInfo
    {
        public int CtiIndex { get; internal set; }
        public int Channel { get; internal set; }
        public string Id { get; internal set; }
        public client.QueueEventType State { get; internal set; }
        public client.QueueInfoType Type { get; internal set; }
        public long ProcessId { get; internal set; }
        public string CallingTelNum { get; internal set; }
        public string WorkerNum { get; internal set; }
        public string CustomeString { get; internal set; }
        public IReadOnlyCollection<Group> Groups { get; internal set; }
    }
}
