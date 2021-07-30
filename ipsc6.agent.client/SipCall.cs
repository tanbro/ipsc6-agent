using org.pjsip.pjsua2;
using System;


namespace ipsc6.agent.client
{
    public struct SipCall
    {
        public SipCall(Sip.MyPjCall call)
        {
            call = call is not null ? call : throw new ArgumentNullException(nameof(call));
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
