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

        IReadOnlyList<client.Group> skillGroups = new List<client.Group>();
        public IReadOnlyList<client.Group> SkillGroups
        {
            get => skillGroups;
            set => SetProperty(ref skillGroups, value);
        }

        IReadOnlyCollection<AgentStateWorkType> stateOperationItems = AgentStateOperations.Instance.Items;
        public IReadOnlyCollection<AgentStateWorkType> StateOperationItems
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

        IReadOnlyCollection<client.Call> callList = new HashSet<client.Call>();
        public IReadOnlyCollection<client.Call> CallList
        {
            get => callList;
            set => SetProperty(ref callList, value);
        }

        IReadOnlyCollection<client.Call> holdList = new HashSet<client.Call>();
        public IReadOnlyCollection<client.Call> HoldList
        {
            get => holdList;
            set => SetProperty(ref holdList, value);
        }

        IReadOnlyCollection<client.QueueInfo> queueList = new HashSet<client.QueueInfo>();
        public IReadOnlyCollection<client.QueueInfo> QueueList
        {
            get => queueList;
            set => SetProperty(ref queueList, value);
        }

        IList<client.SipAccount> sipAccountList = new List<client.SipAccount>();
        public IList<client.SipAccount> SipAccountList
        {
            get => sipAccountList;
            set
            {
                SetProperty(ref sipAccountList, value);
                OnPropertyChanged("TeleState");
            }
        }
    }
}
