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

    public class Window
    {
        public bool NoStartupLoginDialog { get; set; }
        public MainWindowStartupMode MainWindowStartupMode { get; set; }
    }
}
