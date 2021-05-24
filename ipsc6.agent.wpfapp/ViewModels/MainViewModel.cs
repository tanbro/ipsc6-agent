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
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainViewModel));

        public Models.Cti.AgentBasicInfo AgentBasicInfo => Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.RingInfo RingInfo => Models.Cti.RingInfo.Instance;


        #region 打开状态弹出窗
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
        #endregion

        #region 签入/出 Command
        static Utils.RelayCommand skillSignCommand = new Utils.RelayCommand(x => DoSkillSignGroup(x), x => CanSkillSignGroup(x));
        public ICommand SkillSignCommand => skillSignCommand;

        static async void DoSkillSignGroup(object parameter)
        {
            var skillId = parameter as string;
            var agent = Enties.Cti.AgentController.Agent;
            var sg = agent.GroupCollection.First((m) => m.Id == skillId);
            if (sg.Signed)
            {
                logger.DebugFormat("签出技能 [{0}]({1})", sg.Id, sg.Name);
                await agent.SignOut(skillId);
            }
            else
            {
                logger.DebugFormat("签入技能 [{0}]({1})", sg.Id, sg.Name);
                await agent.SignIn(skillId);
            }
        }

        static bool CanSkillSignGroup(object _)
        {
            return true;
        }
        #endregion

    }
}
