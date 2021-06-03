using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace ipsc6.agent.wpfapp.Models.Cti
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class AgentBasicInfo : Utils.SingletonModelBase<AgentBasicInfo>
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

        AgentStateWorkType agentStateWorkType = new AgentStateWorkType(client.AgentState.OffLine, client.WorkType.Unknown);
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

        IList<AgentStateWorkType> stateOperationItems = AgentStateOperations.Instance.Items;
        public IList<AgentStateWorkType> StateOperationItems
        {
            get => stateOperationItems;
            set => SetField(ref stateOperationItems, value);
        }

        client.TeleState teleState = client.TeleState.OnHook;
        public client.TeleState TeleState
        {
            get => teleState;
            set => SetField(ref teleState, value);
        }

    }
}
