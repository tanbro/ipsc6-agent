using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class SipConfigArgs
    {
        public string RingerWaveFile { get; set; }
        public IEnumerable<SipTransportConfigArgs> TransportConfigArgsCollection { get; set; }
    }
}
