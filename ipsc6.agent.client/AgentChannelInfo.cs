namespace ipsc6.agent.client
{
    public class AgentChannelInfo : ServerSideData
    {
        public int Channel { get; }
        public AgentChannelInfo(CtiServer ctiServer, int channel) : base(ctiServer)
        {
            Channel = channel;
        }
    }
}
