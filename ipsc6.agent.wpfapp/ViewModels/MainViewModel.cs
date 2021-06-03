using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Toolkit.Mvvm.Input;

namespace ipsc6.agent.wpfapp.ViewModels
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class MainViewModel : Utils.SingletonObservableObject<MainViewModel>
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainViewModel));

        public Models.Cti.AgentBasicInfo AgentBasicInfo => Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.RingInfo RingInfo => Models.Cti.RingInfo.Instance;

        internal void RefreshCanExecute()
        {
            IRelayCommand[] commands =
            {
                onHookCommand, offHookCommand, setStateCommand, skillSignCommand
            };
            App.TaskFactory.StartNew(() =>
            {
                foreach (var command in commands)
                {
                    command.NotifyCanExecuteChanged();
                }
            });
        }


        #region Command 打开状态弹出窗
        static bool isOpenStatePopup = false;
        public bool IsOpenStatePopup
        {
            get => isOpenStatePopup;
            set => SetProperty(ref isOpenStatePopup, value);
        }
        #endregion

        #region OpenStatePopup Command
        static readonly IRelayCommand openStatePopupCommand = new RelayCommand(DoOpenStatePopup, CanOpenStatePopup);
        public IRelayCommand OpenStatePopupCommand => openStatePopupCommand;
        static void DoOpenStatePopup()
        {
            isOpenStatePopup = true;
        }

        static bool CanOpenStatePopup()
        {
            if (doingSetState) return false;
            var agent = Controllers.AgentController.Agent;
            if (agent == null) return false;
            return true;
        }
        #endregion

        #region Command 签入/出
        static readonly IRelayCommand skillSignCommand = new RelayCommand<object>(DoSkillSign, CanSkillSign);
        public IRelayCommand SkillSignCommand => skillSignCommand;

        static async void DoSkillSign(object parameter)
        {
            var skillId = parameter as string;
            var agent = Controllers.AgentController.Agent;
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

        static bool CanSkillSign(object _)
        {
            var agent = Controllers.AgentController.Agent;
            if (agent == null) return false;
            return true;
        }

        #endregion

        #region Command 修改状态
        static readonly IRelayCommand setStateCommand = new RelayCommand<object>(DoSetState, CanSetState);
        public IRelayCommand SetStateCommand => setStateCommand;
        static bool doingSetState = false;
        static async void DoSetState(object parameter)
        {
            doingSetState = true;
            try
            {
                setStateCommand.NotifyCanExecuteChanged();
                logger.DebugFormat("设置状态: {0}", parameter);
                var st = parameter as AgentStateWorkType;
                var agent = Controllers.AgentController.Agent;
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
            finally
            {
                doingSetState = false;
                setStateCommand.NotifyCanExecuteChanged();
            }
        }

        static bool CanSetState(object parameter)
        {
            if (doingSetState) return false;
            var agent = Controllers.AgentController.Agent;
            if (agent == null) return false;
            client.AgentState[] vals = { client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            if (!vals.Any(x => x == agent.AgentState)) return false;
            if (parameter != null)
            {
                var st = parameter as AgentStateWorkType;
                if (st.Item2 == agent.WorkType) return false;
            }
            return true;
        }
        #endregion

        #region Command Offhook
        static bool doingOffHook = false;

        static readonly IRelayCommand offHookCommand = new RelayCommand(DoOffHook, CanOffHook);
        public IRelayCommand OffHookCommand => offHookCommand;

        static async void DoOffHook()
        {
            doingOffHook = true;
            try
            {
                offHookCommand.NotifyCanExecuteChanged();
                logger.Debug("摘机");
                var agent = Controllers.AgentController.Agent;
                await agent.OffHook();
            }
            finally
            {
                doingOffHook = false;
                offHookCommand.NotifyCanExecuteChanged();
            }
        }

        static bool CanOffHook()
        {
            if (doingOffHook) return false;
            var agent = Controllers.AgentController.Agent;
            if (agent == null) return false;
            //if (agent.IsOffHookRequesting) return false;
            if (agent.AgentState != client.AgentState.Idle) return false;
            if (agent.TeleState == client.TeleState.OffHook) return false;
            return true;
        }
        #endregion


        #region Command Hangup
        static readonly IRelayCommand onHookCommand = new RelayCommand(DoOnHook, CanOnHook);
        public IRelayCommand OnHookCommand => onHookCommand;
        static bool doingOnHook = false;
        static async void DoOnHook()
        {
            doingOnHook = true;
            try
            {
                onHookCommand.NotifyCanExecuteChanged();
                logger.Debug("挂机");
                var agent = Controllers.AgentController.Agent;
                await agent.OnHook();
            }
            finally
            {
                doingOnHook = false;
                onHookCommand.NotifyCanExecuteChanged();
            }
        }

        static bool CanOnHook()
        {
            if (doingOnHook) return false;
            var agent = Controllers.AgentController.Agent;
            if (agent == null) return false;
            if (agent.IsOffHookRequesting) return false;
            if (agent.TeleState == client.TeleState.OnHook) return false;
            return true;
        }
        #endregion


    }
}
