using System;

namespace ipsc6.agent.client
{
    public class AgentStateWorkType : IEquatable<AgentStateWorkType>, ICloneable
    {
        public readonly AgentState AgentState;
        public readonly WorkType WorkType;

        public AgentStateWorkType(AgentState agentState, WorkType workType)
        {
            AgentState = agentState;
            WorkType = workType;
        }

        public bool Equals(AgentStateWorkType other)
        {
            return (
                (this.AgentState == other.AgentState) &&
                (this.WorkType == other.WorkType)
            );
        }

        public object Clone()
        {
            return new AgentStateWorkType(AgentState, WorkType);
        }
    }
}
