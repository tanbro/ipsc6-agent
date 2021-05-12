using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class AgentGroup : IEquatable<AgentGroup>, ICloneable
    {
        public string Id { get; }
        public string Name { get; set; }

        private bool signed = false;
        public bool Signed { get => signed; set => signed = value; }

        public AgentGroup(string id)
        {
            Id = id;
            Name = "";
        }

        public AgentGroup(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AgentGroup);
        }

        public bool Equals(AgentGroup other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public object Clone()
        {
            return new AgentGroup(Id, Name);
        }

        public static bool operator ==(AgentGroup left, AgentGroup right)
        {
            return EqualityComparer<AgentGroup>.Default.Equals(left, right);
        }

        public static bool operator !=(AgentGroup left, AgentGroup right)
        {
            return !(left == right);
        }
    }
}
