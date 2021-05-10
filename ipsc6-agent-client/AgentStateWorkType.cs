using System;

namespace ipsc6.agent.client
{
    public class AgentStateWorkType : IEquatable<AgentStateWorkType>, ICloneable
    {
        public AgentState AgentState { get; }
        public WorkType WorkType { get; }

        public AgentStateWorkType(AgentState agentState, WorkType workType)
        {
            AgentState = agentState;
            WorkType = workType;
        }

        public bool Equals(AgentStateWorkType other)
        {
            return (
                (AgentState == other.AgentState) &&
                (WorkType == other.WorkType)
            );
        }
        public override bool Equals(object obj)
        {
            var that = obj as AgentStateWorkType;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(AgentStateWorkType lhs, AgentStateWorkType rhs)
        {
            return (lhs is object) && (rhs is object) && lhs.Equals(rhs);
        }

        public static bool operator !=(AgentStateWorkType lhs, AgentStateWorkType rhs)
        {
            return !(lhs == rhs);
        }

        public object Clone()
        {
            return new AgentStateWorkType(AgentState, WorkType);
        }
    }
}
