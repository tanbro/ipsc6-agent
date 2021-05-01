using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class AgentGroup: IEquatable<AgentGroup>
    {
        public readonly string Id;
        public string Name;
        public AgentGroup(string id , string name="")
        {
            Id = id;
            Name = name;
        }

        public bool Equals(AgentGroup other)
        {
            return Id == other.Id;
        }
    }
}
