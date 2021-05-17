namespace ipsc6.agent.client
{
    public class WorkingChannelInfo
    {
        public int Channel { get; }
        public string CustomString { get; }
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
