using System;
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
            //set => SetField(ref workerNumber, value);
            set
            {
                workerNumber = value;
                OnPropertyChanged("WorkerNumber");
            }
        }

        string displayName;
        public string DisplayName
        {
            get => displayName;
            set => SetField(ref displayName, value);
        }

        AgentStateWorkType agentStateWorkType;
        public AgentStateWorkType AgentStateWorkType
        {
            get => agentStateWorkType;
            set => SetField(ref agentStateWorkType, value);
        }
    }
}
