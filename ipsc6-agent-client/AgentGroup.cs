using System;

namespace ipsc6.agent.client
{
    public class AgentGroup : IEquatable<AgentGroup>
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

        public override bool Equals(object obj)
        {
            var that = obj as AgentGroup;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(AgentGroup lhs, AgentGroup rhs)
        {
            return (lhs is object) && (rhs is object) && lhs.Equals(rhs);
        }

        public static bool operator !=(AgentGroup lhs, AgentGroup rhs)
        {
            return !(lhs == rhs);
        }

    }
}
