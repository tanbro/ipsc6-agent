using System.ComponentModel;

namespace ipsc6.agent.wpfapp.Models.Cti
{
    public class RingInfo : Utils.SingletonObservableObject<RingInfo>
    {
        private string teleNum;
        public string TeleNum
        {
            get => teleNum;
            set => SetProperty(ref teleNum, value);
        }

        private string location;
        public string Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }

        private string ivrPath;
        public string IvrPath
        {
            get => ivrPath;
            set => SetProperty(ref ivrPath, value);
        }

        private string bizName;
        public string BizName
        {
            get => bizName;
            set => SetProperty(ref bizName, value);
        }

    }
}
