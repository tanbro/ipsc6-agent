using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct QueueInfo
    {
        public int CtiIndex { get; internal set; }
        public int Channel { get; internal set; }
        public string Id { get; internal set; }
        public client.QueueInfoType Type { get; internal set; }
        public long ProcessId { get; internal set; }
        public string CallingNo { get; internal set; }
        public string WorkerNum { get; internal set; }
        public string CustomeString { get; internal set; }
        public IReadOnlyCollection<Group> Groups { get; internal set; }
    }
}
