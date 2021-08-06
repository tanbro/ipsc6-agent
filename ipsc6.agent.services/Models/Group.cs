using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSigned { get; set; }
    }
}
