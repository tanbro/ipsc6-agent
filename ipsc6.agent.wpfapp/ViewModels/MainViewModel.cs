using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace ipsc6.agent.wpfapp.ViewModels
{
    public class MainViewModel : Utils.SingletonModelBase<MainViewModel>, INotifyPropertyChanged
    {
        //static readonly Models.Cti.AgentBasicInfo agentBasicInfo = Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.AgentBasicInfo AgentBasicInfo => Models.Cti.AgentBasicInfo.Instance;

        //static readonly Models.Cti.RingInfo ringInfo = Models.Cti.RingInfo.Instance;
        public Models.Cti.RingInfo RingInfo => Models.Cti.RingInfo.Instance;

        static bool isOpenStatePopup = false;
        public bool IsOpenStatePopup
        {
            get => isOpenStatePopup;
            set => SetField(ref isOpenStatePopup, value);
        }

        #region OpenStatePopup Command
        static Utils.RelayCommand openStatePopupCommand = new Utils.RelayCommand(x => DoOpenStatePopup(x), x => CanOpenStatePopup(x));
        public ICommand OpenStatePopupCommand => openStatePopupCommand;
        static void DoOpenStatePopup(object _)
        {
            isOpenStatePopup = true;
        }

        static bool CanOpenStatePopup(object _)
        {
            return true;
        }
        #endregion

    }
}
