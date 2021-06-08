using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region 主窗口
        static bool pinned = true;
        public bool Pinned
        {
            get => pinned;
            set => SetProperty(ref pinned, value);
        }

        static readonly IRelayCommand pinCommand = new RelayCommand(DoPin);
        public IRelayCommand PinCommand => pinCommand;
        static void DoPin()
        {
            Instance.Pinned = !pinned;
        }

        static bool snapped = false;
        public bool Snapped
        {
            get => snapped;
            set
            {
                var win = Application.Current.MainWindow;
                if (value)
                {
                    win.Height = 8;
                    win.Top = 0;
                    MainPanelVisibility = Visibility.Collapsed;
                }
                else
                {
                    win.Height = 80;
                    MainPanelVisibility = Visibility.Visible;
                }
                SetProperty(ref snapped, value);
            }
        }

        static double mainWindowHeight;
        public double MainWindowHeight
        {
            get => mainWindowHeight;
            set => SetProperty(ref mainWindowHeight, value);
        }
        static double mainWindowTop;
        public double MainWindowTop
        {
            get => mainWindowTop;
            set => SetProperty(ref mainWindowTop, value);
        }
        static Visibility mainPanelVisibility = Visibility.Visible;
        public Visibility MainPanelVisibility
        {
            get => mainPanelVisibility;
            set => SetProperty(ref mainPanelVisibility, value);
        }
        #endregion

        public Models.Cti.AgentBasicInfo AgentBasicInfo => Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.RingInfo RingInfo => Models.Cti.RingInfo.Instance;

        internal void RefreshAgentExecutables()
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

        #region 技能组 Popup
        static bool isSkillPopupOpened;
        public bool IsSkillPopupOpened
        {
            get => isSkillPopupOpened;
            set => SetProperty(ref isSkillPopupOpened, value);
        }
        static readonly IRelayCommand skillPopupCommand = new RelayCommand(DoSkillPopup);
        public IRelayCommand SkillPopupCommand => skillPopupCommand;
        static void DoSkillPopup()
        {
            Instance.IsSkillPopupOpened = !isSkillPopupOpened;
        }
        #endregion

        #region Command 打开状态弹出窗
        static bool isStatePopupOpened;
        public bool IsStatePopupOpened
        {
            get => isStatePopupOpened;
            set => SetProperty(ref isStatePopupOpened, value);
        }
        static readonly IRelayCommand statePopupCommand = new RelayCommand(DoOpenStatePopup, CanOpenStatePopup);
        public IRelayCommand StatePopupCommand => statePopupCommand;
        static void DoOpenStatePopup()
        {
            Instance.IsStatePopupOpened = !isStatePopupOpened;
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
        static readonly IRelayCommand offHookCommand = new RelayCommand(DoOffHook, CanOffHook);
        public IRelayCommand OffHookCommand => offHookCommand;
        static bool doingOffHook = false;

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
            if (agent.AgentState != client.AgentState.Idle
                && agent.AgentState != client.AgentState.Ring) return false;
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
            if (agent.TeleState == client.TeleState.OffHook) return true;
            if (agent.AgentState != client.AgentState.Work
                && agent.AgentState != client.AgentState.WorkPause
                && agent.AgentState != client.AgentState.Ring) return false;
            if (agent.TeleState == client.TeleState.OnHook
                && agent.AgentState != client.AgentState.Ring) return false;
            return true;
        }
        #endregion

        #region 座席咨询
        static readonly IRelayCommand xferConsultCommand = new RelayCommand(DoXferConsult);
        public IRelayCommand XferConsultCommand => xferConsultCommand;
        static async void DoXferConsult()
        {
            var agent = Controllers.AgentController.Agent;

            var dialog = new Dialogs.PromptDialog()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "转接" },
                    { "Label", "输入要转接的目标。格式： 技能组ID:座席工号" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;

            string groupId, workerNum = "";
            var parts = inputText.Split(new char[] { ':' }, 2);
            if (parts.Length > 0)
                groupId = parts[0];
            else
                return;
            if (parts.Length > 1)
                workerNum = parts[1];

            await agent.XferConsult(groupId.Trim(), workerNum.Trim());
        }
        #endregion

        #region 座席转移
        static readonly IRelayCommand xferCommand = new RelayCommand(DoXfer);
        public IRelayCommand XferCommand => xferCommand;
        static async void DoXfer()
        {
            var agent = Controllers.AgentController.Agent;

            var dialog = new Dialogs.PromptDialog()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "转接" },
                    { "Label", "输入要转接的目标。格式： 技能组ID:座席工号" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;

            string groupId, workerNum = "";
            var parts = inputText.Split(new char[] { ':' }, 2);
            if (parts.Length > 0)
                groupId = parts[0];
            else
                return;
            if (parts.Length > 1)
                workerNum = parts[1];

            await agent.Xfer(groupId.Trim(), workerNum.Trim());
        }
        #endregion

        #region 保持
        static readonly IRelayCommand holdCommand = new RelayCommand(DoHold);
        public IRelayCommand HoldCommand => holdCommand;
        static async void DoHold()
        {
            var agent = Controllers.AgentController.Agent;
            await agent.Hold();
        }
        #endregion

        #region 取消保持
        static readonly IRelayCommand unHoldCommand = new RelayCommand<object>(DoUnHold);
        public IRelayCommand UnHoldCommand => unHoldCommand;
        static async void DoUnHold(object parameter)
        {
            client.CallInfo callInfo = null;
            var agent = Controllers.AgentController.Agent;
            if (parameter == null)
            {
                callInfo = agent.CallCollection.First(x => x.IsHeld);
            }
            else
            {
                callInfo = parameter as client.CallInfo;
            }
            await agent.UnHold(callInfo);
        }
        #endregion

        #region 保持列表
        static bool isHoldPopupOpened;
        public bool IsHoldPopupOpened
        {
            get => isHoldPopupOpened;
            set => SetProperty(ref isHoldPopupOpened, value);
        }
        static readonly IRelayCommand holdPopupCommand = new RelayCommand(DoHoldPopup);
        public IRelayCommand HoldPopupCommand => holdPopupCommand;
        static void DoHoldPopup()
        {
            Instance.IsHoldPopupOpened = !isHoldPopupOpened;
        }
        #endregion

        #region 排队列表
        static bool isQueuePopupOpened;
        public bool IsQueuePopupOpened
        {
            get => isQueuePopupOpened;
            set => SetProperty(ref isQueuePopupOpened, value);
        }
        static readonly IRelayCommand queuePopupCommand = new RelayCommand(DoQueuePopup);
        public IRelayCommand QueuePopupCommand => queuePopupCommand;
        static void DoQueuePopup()
        {
            Instance.IsQueuePopupOpened = !isQueuePopupOpened;
        }

        static readonly IRelayCommand dequeueCommand = new RelayCommand<object>(DoDequeue);
        public IRelayCommand DequeueCommand => dequeueCommand;
        static async void DoDequeue(object paramter)
        {
            var queueInfo = paramter as client.QueueInfo;
            var agent = Controllers.AgentController.Agent;
            await agent.Dequeue(queueInfo);
        }
        #endregion

        #region 外乎
        static readonly IRelayCommand dialCommand = new RelayCommand(DoDial);
        public IRelayCommand DialCommand => dialCommand;
        static async void DoDial()
        {
            var agent = Controllers.AgentController.Agent;

            var dialog = new Dialogs.PromptDialog()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "拨号" },
                    { "Label", "输入拨打的号码" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;
            await agent.Dial(inputText);
        }
        #endregion

        #region 外转
        static readonly IRelayCommand xferExtCommand = new RelayCommand(DoXferExt);
        public IRelayCommand XferExtCommand => xferExtCommand;
        static async void DoXferExt()
        {
            var agent = Controllers.AgentController.Agent;
            var dialog = new Dialogs.PromptDialog()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "向外转移" },
                    { "Label", "输入拨打的号码" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;
            await agent.XferExt(inputText);
        }
        #endregion

        #region 外咨
        static readonly IRelayCommand xferExtConsultCommand = new RelayCommand(DoXferExtConsult);
        public IRelayCommand XferExtConsultCommand => xferExtConsultCommand;
        static async void DoXferExtConsult()
        {
            var agent = Controllers.AgentController.Agent;
            var dialog = new Dialogs.PromptDialog()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "向外咨询" },
                    { "Label", "输入拨打的号码" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;
            await agent.XferExtConsult(inputText);
        }
        #endregion
    }
}
