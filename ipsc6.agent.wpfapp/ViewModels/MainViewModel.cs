using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

namespace ipsc6.agent.wpfapp.ViewModels
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class MainViewModel : Utils.SingletonModelBase<MainViewModel>, INotifyPropertyChanged
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainViewModel));

        public Models.Cti.AgentBasicInfo AgentBasicInfo => Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.RingInfo RingInfo => Models.Cti.RingInfo.Instance;


        #region Command 打开状态弹出窗
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

        #region Command 签入/出
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
            var agent = Enties.Cti.AgentController.Agent;
            return true;
        }
        #endregion

        #region Command 修改状态
        static Utils.RelayCommand setStateCommand = new Utils.RelayCommand(x => DoSetState(x), x => CanSetState(x));
        public ICommand SetStateCommand => setStateCommand;

        static async void DoSetState(object parameter)
        {
            logger.DebugFormat("设置状态: {0}", parameter);
            var st = parameter as AgentStateWorkType;
            var agent = Enties.Cti.AgentController.Agent;

            try
            {
                if (st.Item1 == client.AgentState.Idle)
                {
                    await agent.SetIdle();
                }
                else if (st.Item1 == client.AgentState.Pause)
                {
                    await agent.SetBusy(st.Item2);
                }
                else if (st.Item1 == client.AgentState.Leave)
                {
                    await agent.SetBusy();
                }
            }
            catch (client.BaseRequestError err)
            {
                MessageBox.Show($"{err}", "坐席客户端远程调用失败");
            }
        }

        static bool CanSetState(object _)
        {
            var agent = Enties.Cti.AgentController.Agent;
            return true;
        }
        #endregion

        #region Command Offhook
        static Utils.RelayCommand offhookCommand = new Utils.RelayCommand(x => DoOffhook(x), x => CanOffhook(x));
        public ICommand OffhookCommand => offhookCommand;

        static async void DoOffhook(object _)
        {
            logger.DebugFormat("摘机");
            var agent = Enties.Cti.AgentController.Agent;
            await agent.OffHook();
        }

        static bool CanOffhook(object _) => true;
        #endregion


        #region Command Hangup
        static Utils.RelayCommand hangupCommand = new Utils.RelayCommand(x => DoHangup(x), x => CanHangup(x));
        public ICommand HangupCommand => hangupCommand;

        static async void DoHangup(object _)
        {
            logger.DebugFormat("挂机");
            var agent = Enties.Cti.AgentController.Agent;
            await agent.HangUp();
        }

        static bool CanHangup(object _) => true;
        #endregion


    }
}
