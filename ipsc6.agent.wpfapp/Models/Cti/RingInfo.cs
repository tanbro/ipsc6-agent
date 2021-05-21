using System.ComponentModel;

namespace ipsc6.agent.wpfapp.Models.Cti
{
    public class RingInfo : Utils.SingletonModelBase<RingInfo>, INotifyPropertyChanged
    {
        private string teleNum;
        public string TeleNum
        {
            get => teleNum;
            set => SetField(ref teleNum, value);
        }

        private string location;
        public string Location
        {
            get => location;
            set => SetField(ref location, value);
        }

        private string ivrPath;
        public string IvrPath
        {
            get => ivrPath;
            set => SetField(ref ivrPath, value);
        }

        private string bizName;
        public string BizName
        {
            get => bizName;
            set => SetField(ref bizName, value);
        }

    }
}
