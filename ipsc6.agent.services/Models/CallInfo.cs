using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CallInfo
    {
        public int CtiIndex { get; internal set; }
        public int Channel { get; internal set; }
        public client.CallDirection Direction { get; internal set; }
        public bool IsHeld { get; internal set; }
        public client.HoldEventType HoldType { get; internal set; }
        public string RemoteTelNum { get; internal set; }
        public string RemoteLoc { get; internal set; }
        public int H24CallCount { get; internal set; }
        public int H48CallCount { get; internal set; }
        public string WorkerNum { get; internal set; }
        public string GroupId { get; internal set; }
        public string IvrPath { get; internal set; }
        public string CustomString { get; internal set; }

    }
}
