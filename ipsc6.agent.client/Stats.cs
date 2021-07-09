using System;

namespace ipsc6.agent.client
{
    public class Stats
    {
        public uint DailyCallCount { get; internal set; }
        public TimeSpan DailyCallDuration { get; internal set; }
    }
}
