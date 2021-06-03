using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

using Microsoft.Toolkit.Mvvm.Input;

namespace ipsc6.agent.wpfapp.ViewModels
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class MainViewModel : Utils.SingletonModelBase<MainViewModel>
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
        #endregion

        #region OpenStatePopup Command
        static IRelayCommand openStatePopupCommand = new RelayCommand(DoOpenStatePopup, CanOpenStatePopup);
        public ICommand OpenStatePopupCommand => openStatePopupCommand;
        static void DoOpenStatePopup()
        {
            isOpenStatePopup = true;
        }

        static bool CanOpenStatePopup()
        {
            return true;
        }
        #endregion

        #region Command 签入/出
        static IRelayCommand skillSignCommand = new RelayCommand<object>(DoSkillSignGroup, CanSkillSignGroup);
        public IRelayCommand SkillSignCommand => skillSignCommand;

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
        static IRelayCommand setStateCommand = new RelayCommand<object>(DoSetState, CanSetState);
        public IRelayCommand SetStateCommand => setStateCommand;

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
        static readonly IRelayCommand offHookCommand = new RelayCommand(DoOffHook, CanOffHook);
        public IRelayCommand OffHookCommand => offHookCommand;

        static async void DoOffHook()
        {
            logger.DebugFormat("摘机");
            var agent = Enties.Cti.AgentController.Agent;
            await agent.OffHook();
        }

        static bool CanOffHook()
        {
            var agent = Enties.Cti.AgentController.Agent;
            if (agent.HasActiveCall) return false;
            if (agent.AgentState != client.AgentState.Idle) return false;
            return true;
        }
        #endregion


        #region Command Hangup
        static IRelayCommand onHookCommand = new RelayCommand(DoOnHook, CanHangup);
        public ICommand OnHookCommand => onHookCommand;

        static async void DoOnHook()
        {
            logger.DebugFormat("挂机");
            var agent = Enties.Cti.AgentController.Agent;
            await agent.OnHook();
        }

        static bool CanHangup()
        {
            var agent = Enties.Cti.AgentController.Agent;
            return agent.HasActiveCall;
        }
        #endregion


    }
}
