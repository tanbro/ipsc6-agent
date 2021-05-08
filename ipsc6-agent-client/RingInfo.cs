namespace ipsc6.agent.client
{
    public class RingInfo : ServerSideData
    {
        public readonly int WorkingChannel;
        public readonly string CustomString;
        public RingInfo(ConnectionInfo connectionInfo, int workingChannel, string customString = "") : base(connectionInfo)
        {
            WorkingChannel = workingChannel;
            CustomString = customString;
        }
    }
}
