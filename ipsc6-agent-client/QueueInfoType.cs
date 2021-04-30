using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public enum QueueInfoType
    {
        CallIn = 0,
        Transfer = 1,
        Consult = 2,
        Return = 3,
        CallOut = 4,
        Intercept = 5,
        InternalCall = 6
    }
}
