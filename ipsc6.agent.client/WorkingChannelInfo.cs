namespace ipsc6.agent.client
{
    public class WorkingChannelInfo : ServerSideData
    {
        public int Channel { get; }
        public string CustomString { get; }
        public WorkingChannelInfo(CtiServer ctiServer, int channel, string customString = "") : base(ctiServer)
        {
            Channel = channel;
            CustomString = customString;
        }

    }
}
