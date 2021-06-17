using System;

using org.pjsip.pjsua2;


namespace ipsc6.agent.client
{
    public class SipCallInfo
    {
        public SipCallInfo(Sip.Call call)
        {
            if (call is null)
            {
                throw new ArgumentNullException(nameof(call));
            }
            var info = call.getInfo();
            Id = info.id;
            LocalUri = info.localUri;
            RemoteUri = info.remoteUri;
            State = info.state;
        }

        public int Id { get; }
        public string LocalUri { get; }
        public string RemoteUri { get; }
        public pjsip_inv_state State { get; }
    }
}
