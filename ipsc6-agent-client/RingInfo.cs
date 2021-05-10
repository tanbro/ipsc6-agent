namespace ipsc6.agent.client
{
    public class RingInfo : ServerSideData
    {
        public int WorkingChannel { get; }
        public string CustomString { get; }
        public RingInfo(ConnectionInfo connectionInfo, int workingChannel, string customString = "") : base(connectionInfo)
        {
            WorkingChannel = workingChannel;
            CustomString = customString;
        }
    }
}
