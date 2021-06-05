using System;
using System.Collections.Generic;


namespace ipsc6.agent.wpfapp.Models.Cti
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class AgentBasicInfo : Utils.SingletonObservableObject<AgentBasicInfo>
    {

        string workerNumber;
        public string WorkerNumber
        {
            get => workerNumber;
            set => SetProperty(ref workerNumber, value);
        }

        string displayName;
        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }

        AgentStateWorkType agentStateWorkType = new AgentStateWorkType(client.AgentState.OffLine, client.WorkType.Unknown);
        public AgentStateWorkType AgentStateWorkType
        {
            get => agentStateWorkType;
            set => SetProperty(ref agentStateWorkType, value);
        }

        IList<client.AgentGroup> skillsGroup = new List<client.AgentGroup>();
        public IList<client.AgentGroup> SkillGroups
        {
            get => skillsGroup;
            set => SetProperty(ref skillsGroup, value);
        }

        IList<AgentStateWorkType> stateOperationItems = AgentStateOperations.Instance.Items;
        public IList<AgentStateWorkType> StateOperationItems
        {
            get => stateOperationItems;
            set => SetProperty(ref stateOperationItems, value);
        }

        client.TeleState teleState = client.TeleState.OnHook;
        public client.TeleState TeleState
        {
            get => teleState;
            set => SetProperty(ref teleState, value);
        }

        IList<client.CallInfo> callList = new List<client.CallInfo>();
        public IList<client.CallInfo> CallList
        {
            get => callList;
            set => SetProperty(ref callList, value);
        }

        IList<client.CallInfo> holdList = new List<client.CallInfo>();
        public IList<client.CallInfo> HoldList
        {
            get => holdList;
            set => SetProperty(ref holdList, value);
        }

    }
}
