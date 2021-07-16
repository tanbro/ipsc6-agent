using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

namespace ipsc6.agent.client
{
    public class SipConfigArgs
    {
        public string RingerWaveFile { get; set; }
        public IEnumerable<SipTransportConfigArgs> TransportConfigArgsCollection { get; set; }
    }
}
