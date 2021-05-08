namespace ipsc6.agent.client
{
    public class WorkingChannelInfo
    {
        public readonly int Channel;
        public readonly string CustomString;
        public WorkingChannelInfo(int channel, string customString) : base()
        {
            Channel = channel;
            CustomString = customString;
        }

        public WorkingChannelInfo(int channel) : base()
        {
            Channel = channel;
            CustomString = "";
        }

    }
}
