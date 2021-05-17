using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class AgentStateWorkType : ICloneable, IEquatable<AgentStateWorkType>
    {
        public AgentState AgentState { get; }
        public WorkType WorkType { get; }

        public AgentStateWorkType(AgentState agentState, WorkType workType)
        {
            AgentState = agentState;
            WorkType = workType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            return new AgentStateWorkType(AgentState, WorkType);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AgentStateWorkType);
        }

        public bool Equals(AgentStateWorkType other)
        {
            return other != null &&
                   AgentState == other.AgentState &&
                   WorkType == other.WorkType;
        }

        public static bool operator ==(AgentStateWorkType left, AgentStateWorkType right)
        {
            return EqualityComparer<AgentStateWorkType>.Default.Equals(left, right);
        }

        public static bool operator !=(AgentStateWorkType left, AgentStateWorkType right)
        {
            return !(left == right);
        }
    }
}
