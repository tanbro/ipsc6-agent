using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class WorkingChannelInfo
    {
        public readonly int Channel;
        public readonly string CustomString;
        public WorkingChannelInfo(int channel, string customString = "") : base()
        {
            Channel = channel;
            CustomString = customString;
        }
    }
}
