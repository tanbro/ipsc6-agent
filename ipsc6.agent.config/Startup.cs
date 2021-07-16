using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.config
{
    public enum MainWindowStartupMode
    {
        Normal,
        Snapped,
        Hide,
    }

    public class Startup
    {
        public bool LoginNotRequired { get; set; }
        public MainWindowStartupMode MainWindowStartupMode { get; set; }
    }
}
