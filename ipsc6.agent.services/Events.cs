using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace ipsc6.agent.services.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class EchoTriggeredEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class StatusChangedEventArgs : EventArgs
    {
        public client.AgentState OldState { get; set; }
        public client.AgentState NewState { get; set; }
        public client.WorkType OldWorkType { get; set; }
        public client.WorkType NewWorkType { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CtiConnectionStateChangedEventArgs : EventArgs
    {
        public int CtiIndex { get; set; }
        public client.ConnectionState OldState { get; set; }
        public client.ConnectionState NewState { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TeleStateChangedEventArgs : EventArgs
    {
        public client.TeleState OldState { get; set; }
        public client.TeleState NewState { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CallInfoEventArgs : EventArgs
    {
        public Models.CallInfo Call { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class QueueInfoEventArgs : EventArgs
    {
        public Models.QueueInfo QueueInfo { get; set; }
    }
}
