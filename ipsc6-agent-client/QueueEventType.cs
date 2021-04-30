using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public enum QueueEventType
    {
        Join = 0,
        Wait = 1,
        PitchOn = 2,
        TimeOut = 3,
        Cancel = 4,
    }
}
