using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct CtiServer
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public bool IsMain { get; set; }
        public client.ConnectionState State { get; set; }
    }
}
