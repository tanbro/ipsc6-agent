using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Toolkit.Mvvm.Input;

using org.pjsip.pjsua2;

#pragma warning disable VSTHRD100

namespace ipsc6.agent.wpfapp.ViewModels
{
#pragma warning disable IDE0065
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;
#pragma warning restore IDE0065

    public class MainViewModel : Utils.SingletonObservableObject<MainViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainViewModel));

        #region 主窗口
        private static bool pinned = true;
        public bool Pinned
        {
            get => pinned;
            set => SetProperty(ref pinned, value);
        }

        private static readonly IRelayCommand pinCommand = new RelayCommand(DoPin);
        public IRelayCommand PinCommand => pinCommand;
        private static void DoPin()
        {
            Instance.Pinned = !pinned;
        }

        private static bool snapped = false;
        public bool Snapped
        {
            get => snapped;
            set
            {
                var window = Application.Current.MainWindow;
                if (value)
                {
                    window.Height = 8;
                    window.Top = 0;
                    MainPanelVisibility = Visibility.Collapsed;
                }
                else
                {
                    window.Height = 80;
                    MainPanelVisibility = Visibility.Visible;
                }
                SetProperty(ref snapped, value);
            }
        }

        private static double mainWindowHeight;
        public double MainWindowHeight
        {
            get => mainWindowHeight;
            set => SetProperty(ref mainWindowHeight, value);
        }

        private static double mainWindowTop;
        public double MainWindowTop
        {
            get => mainWindowTop;
            set => SetProperty(ref mainWindowTop, value);
        }

        private static Visibility mainPanelVisibility = Visibility.Visible;
        public Visibility MainPanelVisibility
        {
            get => mainPanelVisibility;
            set => SetProperty(ref mainPanelVisibility, value);
        }
        #endregion

        #region ctor
        private static void RefreshExecutables()
        {
            IRelayCommand[] commands =
            {
                groupPopupCommand,
                statePopupCommand, setStateCommand,
                signGroupCommand,
                holdCommand, unHoldCommand,
            };
            foreach (var command in commands)
            {
                Application.Current.Dispatcher.Invoke(command.NotifyCanExecuteChanged);
            }
        }

        static MainViewModel()
        {
            App.mainService.OnLoginCompleted += MainService_OnLoginCompleted;
            App.mainService.OnStatusChanged += MainService_OnStatusChanged;
            App.mainService.OnSignedGroupsChanged += MainService_OnSignedGroupsChanged;
            App.mainService.OnRingCallReceived += MainService_OnRingCallReceived;
            App.mainService.OnHeldCallReceived += MainService_OnHeldCallReceived;
            App.mainService.OnTeleStateChanged += MainService_OnTeleStateChanged;
            App.mainService.OnSipRegisterStateChanged += MainService_OnSipRegisterStateChanged;
            App.mainService.OnSipCallStateChanged += MainService_OnSipCallStateChanged;
        }
        #endregion

        #region Agent Status
        private static void MainService_OnLoginCompleted(object sender, EventArgs e)
        {
            var svc = App.mainService;
            var ss = svc.GetWorkerNum();
            Instance.WorkerNumber = ss[0];
            Instance.DisplayName = ss[1];
            RefreshExecutables();
        }

        private static void MainService_OnStatusChanged(object sender, services.Events.StatusChangedEventArgs e)
        {
            Instance.Status = new AgentStateWorkType(e.NewState, e.NewWorkType);
            RefreshExecutables();
        }

        private static string workerNum;
        public string WorkerNumber
        {
            get => workerNum;
            set => SetProperty(ref workerNum, value);
        }
        private static string displayName;
        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }

        private static AgentStateWorkType status;
        public AgentStateWorkType Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        #endregion

        #region Group

        private static bool isGroupPopupOpened;
        public bool IsGroupPopupOpened
        {
            get => isGroupPopupOpened;
            set => SetProperty(ref isGroupPopupOpened, value);
        }
        private static readonly IRelayCommand groupPopupCommand = new RelayCommand(DoGroupPopup, CanGroupPopup);
        public IRelayCommand GroupPopupCommand => groupPopupCommand;
        private static void DoGroupPopup()
        {
            Instance.IsGroupPopupOpened = !isGroupPopupOpened;
        }

        private static bool CanGroupPopup()
        {
            if (groups?.Count == 0) return false;
            return true;
        }

        private static void MainService_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            var svc = App.mainService;
            Instance.Groups = svc.GetGroups();
            RefreshExecutables();
        }

        private static IReadOnlyCollection<services.Models.Group> groups;
        public IReadOnlyCollection<services.Models.Group> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        private static readonly IRelayCommand signGroupCommand = new RelayCommand<object>(DoSignGroup, CanSignGroup);
        public IRelayCommand SignGroupCommand => signGroupCommand;

        private static async void DoSignGroup(object parameter)
        {
            string groupId = parameter as string;
            var svc = App.mainService;

            using (await Utils.CommandGuard.CreateAsync(signGroupCommand))
            {
                var group = groups.First(x => x.Id == groupId);
                bool isSignIn = !group.IsSigned;
                if (isSignIn)
                    logger.DebugFormat("签入 {0}", groupId);
                else
                    logger.DebugFormat("签出 {0}", groupId);

                await svc.SignGroup(groupId, isSignIn);
            }
        }

        private static bool CanSignGroup(object _)
        {
            //if (Utils.CommandGuard.IsGuarding) return false;
            client.AgentState[] allowedAgentStates = { client.AgentState.OnLine, client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            if (!allowedAgentStates.Any(x => x == status.Item1)) return false;
            return true;
        }

        #endregion

        #region Command 打开状态弹出窗
        private static bool isStatePopupOpened;
        public bool IsStatePopupOpened
        {
            get => isStatePopupOpened;
            set => SetProperty(ref isStatePopupOpened, value);
        }

        private static readonly IRelayCommand statePopupCommand = new RelayCommand(DoOpenStatePopup, CanOpenStatePopup);
        public IRelayCommand StatePopupCommand => statePopupCommand;

        private static void DoOpenStatePopup()
        {
            Instance.IsStatePopupOpened = !isStatePopupOpened;
        }

        private static bool CanOpenStatePopup()
        {
            client.AgentState[] allowedAgentStates = { client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            if (!allowedAgentStates.Any(x => x == status.Item1)) return false;
            return true;
        }
        #endregion

        #region Command 修改状态
        private static readonly List<AgentStateWorkType> stateOperationItems = new()
        {
            new AgentStateWorkType(client.AgentState.Idle, client.WorkType.Unknown),
            new AgentStateWorkType(client.AgentState.Pause, client.WorkType.PauseBusy),
            new AgentStateWorkType(client.AgentState.Pause, client.WorkType.PauseLeave),
            new AgentStateWorkType(client.AgentState.Pause, client.WorkType.PauseTyping),
            new AgentStateWorkType(client.AgentState.Pause, client.WorkType.PauseSnooze),
            new AgentStateWorkType(client.AgentState.Pause, client.WorkType.PauseDinner),
            new AgentStateWorkType(client.AgentState.Pause, client.WorkType.PauseTrain),
        };
        public IReadOnlyCollection<AgentStateWorkType> StateOperationItems => stateOperationItems;

        private static readonly IRelayCommand setStateCommand = new RelayCommand<object>(DoSetState, CanSetState);
        public IRelayCommand SetStateCommand => setStateCommand;
        private static async void DoSetState(object parameter)
        {
            logger.DebugFormat("设置状态: {0}", parameter);

            var st = parameter as AgentStateWorkType;
            var svc = App.mainService;
            using (await Utils.CommandGuard.CreateAsync(signGroupCommand))
            {
                if (st.Item1 == client.AgentState.Idle)
                {
                    await svc.SetIdle();
                }
                else if (st.Item1 == client.AgentState.Pause)
                {
                    await svc.SetBusy(st.Item2);
                }
                else if (st.Item1 == client.AgentState.Leave)
                {
                    await svc.SetBusy();
                }
            }
        }

        private static bool CanSetState(object parameter)
        {
            //if (Utils.CommandGuard.IsGuarding) return false;
            client.AgentState[] allowedAgentStates = { client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            if (!allowedAgentStates.Any(x => x == status.Item1)) return false;

            if (parameter != null)
            {
                AgentStateWorkType t = (AgentStateWorkType)parameter;
                if (t.Item1 == status.Item1 && t.Item2 == status.Item2) return false;
            }

            return true;
        }
        #endregion

        #region Tele State
        private static void MainService_OnTeleStateChanged(object sender, services.Events.TeleStateChangedEventArgs e)
        {
            Instance.TeleState = e.NewState;
        }

        private static client.TeleState teleState;
        public client.TeleState TeleState
        {
            get => teleState;
            set => SetProperty(ref teleState, value);
        }

        #endregion

        #region SIP UAC
        private static void MainService_OnSipRegisterStateChanged(object sender, EventArgs e)
        {
            ReloadSipAccounts();
        }

        private static void MainService_OnSipCallStateChanged(object sender, EventArgs e)
        {
            ReloadSipAccounts();
        }

        private static void ReloadSipAccounts()
        {
            var svc = App.mainService;
            Instance.SipAccounts = svc.GetSipAccounts();
            // UI 上的电话状态Icon/Label的转换结果由“TeleState”和注册状态共同计算得出，但是 bingding 只有 TeleState(不规范)，所以这里强行传播给绑定
            Instance.OnPropertyChanged("TeleState");

            IRelayCommand[] commands =
            {
                answerCommand, hangupCommand
            };
            foreach (var command in commands)
            {
                Application.Current.Dispatcher.Invoke(command.NotifyCanExecuteChanged);
            }
        }

        private static IReadOnlyCollection<services.Models.SipAccount> sipAccounts = new services.Models.SipAccount[] { };
        public IReadOnlyCollection<services.Models.SipAccount> SipAccounts
        {
            get => sipAccounts;
            set => SetProperty(ref sipAccounts, value);
        }
        private static readonly IRelayCommand answerCommand = new RelayCommand(DoAnswer, CanAnswer);
        public IRelayCommand AnswerCommand => answerCommand;

        private static async void DoAnswer()
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.CreateAsync(answerCommand))
            {
                logger.Debug("摘机");
                await svc.Answer();
            }
        }

        private static bool CanAnswer()
        {
            //if (Utils.CommandGuard.IsGuarding) return false;
            var callsIter = sipAccounts.SelectMany(m => m.Calls);
            if (!callsIter.Any(x => x.State == pjsip_inv_state.PJSIP_INV_STATE_CONNECTING)) return false;
            return true;
        }

        private static readonly IRelayCommand hangupCommand = new RelayCommand(DoHangup, CanHangup);
        public IRelayCommand HangupCommand => hangupCommand;

        private static async void DoHangup()
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.CreateAsync(hangupCommand))
            {
                logger.Debug("挂机");
                await svc.Hangup();
            }
        }

        private static bool CanHangup()
        {
            //if (Utils.CommandGuard.IsGuarding) return false;
            var callsIter = sipAccounts.SelectMany(m => m.Calls);
            var states = new pjsip_inv_state[] { pjsip_inv_state.PJSIP_INV_STATE_NULL, pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED };
            if (!callsIter.Any(x => states.Contains(x.State))) return false;
            return true;
        }

        #endregion

        #region Call 保持, 取消保持, 保持列表

        private static void MainService_OnHeldCallReceived(object sender, services.Events.CallInfoEventArgs e)
        {
            ReloadCalls();
        }

        private static void MainService_OnRingCallReceived(object sender, services.Events.CallInfoEventArgs e)
        {
            ReloadCalls();
        }

        private static void ReloadCalls()
        {
            var svc = App.mainService;
            Instance.Calls = svc.GetCalls();
            Instance.HeldCalls = Instance.Calls.Where(x => x.IsHeld).ToList();
            RefreshExecutables();
        }

        private static IReadOnlyCollection<services.Models.CallInfo> calls = new services.Models.CallInfo[] { };
        public IReadOnlyCollection<services.Models.CallInfo> Calls
        {
            get => calls;
            set => SetProperty(ref calls, value);
        }

        private static IReadOnlyCollection<services.Models.CallInfo> heldCalls = new services.Models.CallInfo[] { };
        public IReadOnlyCollection<services.Models.CallInfo> HeldCalls
        {
            get => heldCalls;
            set => SetProperty(ref heldCalls, value);
        }

        private static readonly IRelayCommand holdCommand = new RelayCommand(DoHold, CanHold);
        public IRelayCommand HoldCommand => holdCommand;

        private static async void DoHold()
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.CreateAsync(holdCommand))
            {
                await svc.Hold();
            }
        }
        static bool CanHold()
        {
            //if (Utils.CommandGuard.IsGuarding) return false;
            if (status.Item1 != client.AgentState.Work) return false;
            if (!calls.Any(x => !x.IsHeld)) return false;
            return true;
        }

        private static readonly IRelayCommand unHoldCommand = new RelayCommand<object>(DoUnHold, CanUnHold);
        public IRelayCommand UnHoldCommand => unHoldCommand;
        private static async void DoUnHold(object parameter)
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.CreateAsync(unHoldCommand).ConfigureAwait(true))
            {
                if (parameter == null)
                {
                    await svc.UnHold();
                }
                else
                {
                    var callInfo = (services.Models.CallInfo)parameter;
                    await svc.UnHold(callInfo.CtiIndex, callInfo.Channel);
                }
            }
        }
        static bool CanUnHold(object parameter)
        {
            var svc = App.mainService;
            if (status.Item1 != client.AgentState.Work) return false;
            if (parameter == null)
            {
                if (!calls.Any(x => x.IsHeld)) return false;
            }
            else
            {
                var callInfo = (services.Models.CallInfo)parameter;
                if (!callInfo.IsHeld) return false;
            }
            return true;
        }

        private static bool isHoldPopupOpened;
        public bool IsHoldPopupOpened
        {
            get => isHoldPopupOpened;
            set => SetProperty(ref isHoldPopupOpened, value);
        }

        private static readonly IRelayCommand holdPopupCommand = new RelayCommand(DoHoldPopup);
        public IRelayCommand HoldPopupCommand => holdPopupCommand;

        private static void DoHoldPopup()
        {
            Instance.IsHoldPopupOpened = !isHoldPopupOpened;
        }
        #endregion

        /*
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

        static readonly IRelayCommand dequeueCommand = new AsyncRelayCommand<object>(DoDequeueAsync);
        public IRelayCommand DequeueCommand => dequeueCommand;
        static async Task DoDequeueAsync(object paramter)
        {
            var queueInfo = paramter as client.QueueInfo;
            var agent = Controllers.AgentController.Agent;
            await agent.DequeueAsync(queueInfo);
        }
        #endregion
*/

        #region 座席咨询
        static readonly IRelayCommand xferConsultCommand = new RelayCommand(DoXferConsult);
        public IRelayCommand XferConsultCommand => xferConsultCommand;
        static async void DoXferConsult()
        {
            var svc = App.mainService;

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

            await svc.agent.XferConsultAsync(groupId.Trim(), workerNum.Trim());
        }
        #endregion

        #region 座席转移
        private static readonly IRelayCommand xferCommand = new RelayCommand(DoXfer);
        public IRelayCommand XferCommand => xferCommand;
        static async void DoXfer()
        {
            var svc = App.mainService;

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

            await svc.agent.XferAsync(groupId.Trim(), workerNum.Trim());
        }
        #endregion

        /*
        #region 外乎
        static readonly IRelayCommand dialCommand = new AsyncRelayCommand(DoDialAsync);
        public IRelayCommand DialCommand => dialCommand;
        static async Task DoDialAsync()
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
            await agent.DialAsync(inputText);
        }
        #endregion

        #region 外转
        static readonly IRelayCommand xferExtCommand = new AsyncRelayCommand(DoXferExtAsync);
        public IRelayCommand XferExtCommand => xferExtCommand;
        static async Task DoXferExtAsync()
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
            await agent.XferExtAsync(inputText);
        }
        #endregion

        #region 外咨
        static readonly IRelayCommand xferExtConsultCommand = new AsyncRelayCommand(DoXferExtConsultAsync);
        public IRelayCommand XferExtConsultCommand => xferExtConsultCommand;
        static async Task DoXferExtConsultAsync()
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
            await agent.XferExtConsultAsync(inputText);
        }
        #endregion

        #region 转 IVR
        static readonly IRelayCommand callIvrCommand = new AsyncRelayCommand(DoCallIvrAsync);
        public IRelayCommand CallIvrCommand => callIvrCommand;
        static async Task DoCallIvrAsync()
        {
            var agent = Controllers.AgentController.Agent;

            string ivrId;
            client.IvrInvokeType ivrType;
            string ivrString;

            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "转 IVR" },
                        { "Label", "输入 IVR 的 ID" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                ivrId = dialog.InputText;
            }
            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "转 IVR" },
                        { "Label", "输入 IVR 的 类型。 0 or Keep: (Default)不释放; 1 or Over: 释放" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                if (string.IsNullOrWhiteSpace(dialog.InputText))
                    ivrType = client.IvrInvokeType.Keep;
                else
                    ivrType = (client.IvrInvokeType)Enum.Parse(typeof(client.IvrInvokeType), dialog.InputText);
            }
            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "转 IVR" },
                        { "Label", "输入 IVR 的 文本参数" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                ivrString = dialog.InputText;
            }
            await agent.CallIvrAsync(ivrId, ivrType, ivrString);
        }
        #endregion

        #region btnAdv
        static readonly IRelayCommand advCommand = new AsyncRelayCommand(DoAdvCommandAsync);
        public IRelayCommand AdvCommand => advCommand;
        static async Task DoAdvCommandAsync()
        {
            var agent = Controllers.AgentController.Agent;

            int connIndex;
            client.MessageType msgTyp;
            int n;
            string s;

            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "发送 CTI 命令" },
                        { "Label", "输入 CTI 服务器节点序号" },
                        { "InputText", "0" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                connIndex = int.Parse(dialog.InputText);
            }

            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "发送 CTI 命令" },
                        { "Label", "输入 CTI 命令名称" },
                        { "InputText", "REMOTE_MSG_LISTEN" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                msgTyp = (client.MessageType)Enum.Parse(typeof(client.MessageType), dialog.InputText);
            }

            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "发送 CTI 命令" },
                        { "Label", "输入 CTI 命令参数的整数部分" },
                        { "InputText", "-1" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                n = int.Parse(dialog.InputText);
            }

            {
                var dialog = new Dialogs.PromptDialog()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "发送 CTI 命令" },
                        { "Label", "输入 CTI 命令参数的字符串部分" },
                        { "InputText", "" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                s = dialog.InputText;
            }

            switch (msgTyp)
            {
                case client.MessageType.REMOTE_MSG_LISTEN:
                    await agent.MonitorAsync(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_STOPLISTEN:
                    await agent.UnMonitorAsync(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEIDLE:
                    await agent.SetIdleAsync(s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEPAUSE:
                    {
                        var parts = s.Split(new char[] { '|' });
                        await agent.SetBusyAsync(
                            parts[0],
                            (client.WorkType)Enum.Parse(typeof(client.WorkType), parts[1])
                        );
                    }
                    break;
                case client.MessageType.REMOTE_MSG_INTERCEPT:
                    await agent.InterceptAsync(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEINSERT:
                    await agent.InterruptAsync(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEHANGUP:
                    await agent.HangupAsync(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCESIGNOFF:
                    await agent.SignOutAsync(s);
                    break;
                case client.MessageType.REMOTE_MSG_KICKOUT:
                    await agent.KickOutAsync(s);
                    break;
                default:
                    MessageBox.Show($"还没有实现 {msgTyp}");
                    break;
            }
        }
        #endregion
*/
    }
}

#pragma warning restore VSTHRD100
