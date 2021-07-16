namespace ipsc6.agent.config
{
    public class Phone
    {
        public ushort LocalPort { get; set; } = 5060;
        public string LocalAddress { get; set; }
        public string PublicAddress { get; set; }
        public string RingerWaveFile { get; set; }
    }
}
