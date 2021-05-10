using System;

namespace ipsc6.agent.client
{
    public class AgentGroup : IEquatable<AgentGroup>
    {
        public string Id { get; }
        public string Name { get; set; }

        private bool signed = false;
        public bool Signed { get => signed; set => signed = value; }

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
