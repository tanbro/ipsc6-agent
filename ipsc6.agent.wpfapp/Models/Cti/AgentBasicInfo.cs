using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace ipsc6.agent.wpfapp.Models.Cti
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class AgentBasicInfo : Utils.SingletonModelBase<AgentBasicInfo>, INotifyPropertyChanged
    {

        string workerNumber;
        public string WorkerNumber
        {
            get => workerNumber;
            set => SetField(ref workerNumber, value);
        }

        string displayName;
        public string DisplayName
        {
            get => displayName;
            set => SetField(ref displayName, value);
        }

        AgentStateWorkType agentStateWorkType= new AgentStateWorkType(client.AgentState.OffLine, client.WorkType.Unknown);
        public AgentStateWorkType AgentStateWorkType
        {
            get => agentStateWorkType;
            set => SetField(ref agentStateWorkType, value);
        }

        IList<client.AgentGroup> skillsGroup = new List<client.AgentGroup>();
        public IList<client.AgentGroup> SkillGroups
        {
            get => skillsGroup;
            set => SetField(ref skillsGroup, value);
        }

    }
}
