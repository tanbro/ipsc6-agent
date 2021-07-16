using org.pjsip.pjsua2;

namespace ipsc6.agent.client
{
    /// ref: https://www.pjsip.org/docs/book-latest/html/reference.html#_CPPv4N2pj15TransportConfigE
    public class SipTransportConfigArgs
    {
        public pjsip_transport_type_e TransportType { get; set; } = pjsip_transport_type_e.PJSIP_TRANSPORT_UDP;
        public uint Port { get; set; }
        public uint PortRange { get; set; }
        public string PublicAddress { get; set; }
        public string BoundAddress { get; set; }

        public override string ToString()
        {
            return $"<{GetType().Name}@{GetHashCode():x8} Port={Port}, PortRange={PortRange}, PublicAddress=\"{PublicAddress}\", BoundAddress=\"{BoundAddress}\">";
        }
    }
}
