namespace ipsc6.agent.config
{
    public class Phone
    {
        public ushort LocalSipPort { get; set; } = 5060;
        public string LocalAddress { get; set; }
        public string PublicAddress { get; set; }
    }
}
