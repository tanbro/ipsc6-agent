using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class AgentGroup: IEquatable<AgentGroup>
    {
        public readonly string Id;
        public string Name;
        public bool Signed = false;
        public AgentGroup(string id)
        {
            Id = id;
        }

        public bool Equals(AgentGroup other)
        {
            return Id == other.Id;
        }
    }
}
