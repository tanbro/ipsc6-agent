using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SipAccount
    {
        public int CtiIndex { get; internal set; }
        public bool IsValid { get; internal set; }
        public int Id { get; internal set; }
        public string Uri { get; internal set; }
        public bool IsRegisterActive { get; internal set; }
        public int LastRegisterError { get; internal set; }
        public IReadOnlyCollection<SipCall> Calls { get; internal set; }
    }
}
