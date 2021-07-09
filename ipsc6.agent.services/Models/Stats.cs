using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Stats
    {
        public uint DailyCallCount { get; internal set; }
        public TimeSpan DailyCallDuration { get; internal set; }

        internal static Stats FromAgent(client.Agent agent)
        {
            Stats instance = new();
            instance.CopyFromAgent(agent);
            return instance;
        }

        internal void CopyFromAgent(client.Agent agent)
        {
            var stats = agent.Stats;
            DailyCallCount = stats.DailyCallCount;
            DailyCallDuration = stats.DailyCallDuration;
        }
    }
}
