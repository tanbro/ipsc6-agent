using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class Group : IEquatable<Group>, ICloneable
    {
        public string Id { get; }
        public string Name { get; internal set; }
        public bool IsSigned { get; internal set; }

        public Group(string id)
        {
            Id = id;
        }

        public Group(string id, string name)
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
            return Equals(obj as Group);
        }

        public bool Equals(Group other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public object Clone()
        {
            return new Group(Id, Name) { IsSigned = IsSigned };
        }

        public static bool operator ==(Group left, Group right)
        {
            return EqualityComparer<Group>.Default.Equals(left, right);
        }

        public static bool operator !=(Group left, Group right)
        {
            return !(left == right);
        }
    }
}
