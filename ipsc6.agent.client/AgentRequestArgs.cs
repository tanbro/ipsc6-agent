using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
    public class AgentRequestArgs
    {
        public AgentMessageEnum Type;
        public int N;
        public string S;

        public AgentRequestArgs(AgentMessageEnum type, int n=0, string s="")
        {
            Type = type;
            N = n;
            S = s;
        }
    }
}
