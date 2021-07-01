using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using org.pjsip.pjsua2;

namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SipCall
    {
        public int Id { get; internal set; }
        public string LocalUri { get; internal set; }
        public string RemoteUri { get; internal set; }
        public pjsip_inv_state State { get; internal set; }
    }
}
