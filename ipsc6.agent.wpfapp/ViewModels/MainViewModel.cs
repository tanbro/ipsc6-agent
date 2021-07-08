using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private CancellationTokenSource timerCanceller;
        private Task timerTask;

        internal void StartTimer()
        {
            var dispatcher = Application.Current.Dispatcher;
            timerCanceller = new();
            timerTask = dispatcher.InvokeAsync(async () =>
            {
                while (!timerCanceller.IsCancellationRequested)
                {
                    DoOnTimer();
                    try
                    {
                        await Task.Delay(360, timerCanceller.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }).Task;
        }

        internal void StopTimer()
        {
            using (timerCanceller)
            {
                timerCanceller.Cancel();
#pragma warning disable VSTHRD002
                timerTask.Wait();
#pragma warning restore VSTHRD002
            }
        }

        private static void DoOnTimer()
        {
            if (lastStatusTime != null)
            {
                Instance.StatusDuration = DateTime.UtcNow - lastStatusTime;
            }
        }
        #endregion

        #region ctor
        private static IRelayCommand[] GetStateRelativeCommands()
        {
            return new IRelayCommand[] {
                statePopupCommand, setStateCommand,
                groupPopupCommand, signGroupCommand,
                holdPopupCommand, holdCommand, unHoldCommand
            };
        }

        private static void NotifyStateRelativeCommandsExecutable()
        {
            foreach (var command in GetStateRelativeCommands())
            {
                if (command != null)
                {
                    Application.Current.Dispatcher.Invoke(command.NotifyCanExecuteChanged);
                }
            }
        }

        static MainViewModel()
        {
            App.mainService.OnCtiConnectionStateChanged += MainService_OnCtiConnectionStateChanged;
            App.mainService.OnLoginCompleted += MainService_OnLoginCompleted;
            App.mainService.OnStatusChanged += MainService_OnStatusChanged;
            App.mainService.OnSignedGroupsChanged += MainService_OnSignedGroupsChanged;
            App.mainService.OnRingCallReceived += MainService_OnRingCallReceived;
            App.mainService.OnHeldCallReceived += MainService_OnHeldCallReceived;
            App.mainService.OnTeleStateChanged += MainService_OnTeleStateChanged;
            App.mainService.OnSipRegisterStateChanged += MainService_OnSipRegisterStateChanged;
            App.mainService.OnSipCallStateChanged += MainService_OnSipCallStateChanged;
            App.mainService.OnQueueInfoEvent += MainService_OnQueueInfoEvent;
        }

        #endregion

        #region Agent Status
        private static void MainService_OnCtiConnectionStateChanged(object sender, services.Events.CtiConnectionStateChangedEventArgs e)
        {
            var svc = App.mainService;
            Instance.CtiServices = svc.GetCtiServers();
            ResetStatusTimeSpan();
        }

        private static void ResetStatusTimeSpan()
        {
            lastStatusTime = DateTime.UtcNow;
            Instance.StatusDuration = new();
        }

        private static IReadOnlyCollection<services.Models.CtiServer> ctiServices = Array.Empty<services.Models.CtiServer>();
        public IReadOnlyCollection<services.Models.CtiServer> CtiServices
        {
            get => ctiServices;
            set
            {
                if (SetProperty(ref ctiServices, value))
                    NotifyStateRelativeCommandsExecutable();
            }
        }

        private static void MainService_OnLoginCompleted(object sender, EventArgs e)
        {
            var svc = App.mainService;
            var ss = svc.GetWorkerNum();
            Instance.WorkerNumber = ss[0];
            Instance.DisplayName = ss[1];
        }

        private static bool IsMainConnectionOk => ctiServices.Any(x => x.State == client.ConnectionState.Ok && x.IsMain);

        private static void MainService_OnStatusChanged(object sender, services.Events.StatusChangedEventArgs e)
        {
            Instance.Status = new AgentStateWorkType(e.NewState, e.NewWorkType);
            ResetStatusTimeSpan();
        }

        private static string workerNum;
        public string WorkerNumber
        {
            get => workerNum;
            set
            {
                if (SetProperty(ref workerNum, value))
                    NotifyStateRelativeCommandsExecutable();
            }
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
            set
            {
                if (SetProperty(ref status, value))
                    NotifyStateRelativeCommandsExecutable();
            }
        }

        private static DateTime lastStatusTime;
        private static TimeSpan statusDuration;
        public TimeSpan StatusDuration
        {
            get => statusDuration;
            set => SetProperty(ref statusDuration, value);
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
            if (status.Item1 is client.AgentState.NotExist or client.AgentState.OffLine) return false;
            return true;
        }

        private static void MainService_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            var svc = App.mainService;
            Instance.Groups = svc.GetGroups();
        }

        private static IReadOnlyCollection<services.Models.Group> groups;
        public IReadOnlyCollection<services.Models.Group> Groups
        {
            get => groups;
            set
            {
                if (SetProperty(ref groups, value))
                    NotifyStateRelativeCommandsExecutable();
            }
        }

        private static readonly IRelayCommand signGroupCommand = new RelayCommand<object>(DoSignGroup, CanSignGroup);
        public IRelayCommand SignGroupCommand => signGroupCommand;

        private static async void DoSignGroup(object parameter)
        {
            var svc = App.mainService;

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            string groupId = parameter as string;

            using (await Utils.CommandGuard.EnterAsync(GetStateRelativeCommands()))
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
            if (!IsMainConnectionOk) return false;
            if (Utils.CommandGuard.IsGuarding)
                return false;
            client.AgentState[] allowedAgentStates = { client.AgentState.OnLine, client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            if (!allowedAgentStates.Any(x => x == status.Item1))
                return false;
            return true;
        }
        #endregion

        #region Command 状态
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
            using (await Utils.CommandGuard.EnterAsync(GetStateRelativeCommands()))
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
            if (!IsMainConnectionOk) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            client.AgentState[] allowedAgentStates = { client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            if (!allowedAgentStates.Any(x => x == status.Item1)) return false;

            if (parameter != null)
            {
                var t = parameter as AgentStateWorkType;
                if (t.Item1 == status.Item1 && t.Item2 == status.Item2) return false;
            }

            return true;
        }
        #endregion

        #region Tele State
        private static void MainService_OnTeleStateChanged(object sender, services.Events.TeleStateChangedEventArgs e)
        {
            Instance.TeleState = e.NewState;
            ReloadCalls();
        }

        private static client.TeleState teleState;
        public client.TeleState TeleState
        {
            get => teleState;
            set
            {
                if (SetProperty(ref teleState, value))
                    NotifyStateRelativeCommandsExecutable();
            }
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
            ReloadCalls();
        }

        private static void ReloadSipAccounts()
        {
            var svc = App.mainService;
            var dispatcher = Application.Current.Dispatcher;
            Instance.SipAccounts = svc.GetSipAccounts();
            IRelayCommand[] commands = { answerCommand, hangupCommand };
            dispatcher.Invoke(() =>
            {
                foreach (var command in commands)
                {
                    dispatcher.Invoke(command.NotifyCanExecuteChanged);
                }
                // UI 上的电话状态Icon/Label的转换结果由“TeleState”和注册状态共同计算得出，但是 bingding 只有 TeleState(不规范)，所以这里强行传播给绑定
                Instance.OnPropertyChanged("TeleState");
            });
        }

        private static IReadOnlyCollection<services.Models.SipAccount> sipAccounts = Array.Empty<services.Models.SipAccount>();
        public IReadOnlyCollection<services.Models.SipAccount> SipAccounts
        {
            get => sipAccounts;
            set
            {
                if (SetProperty(ref sipAccounts, value))
                    NotifyStateRelativeCommandsExecutable();
            }
        }
        private static readonly IRelayCommand answerCommand = new RelayCommand(DoAnswer, CanAnswer);
        public IRelayCommand AnswerCommand => answerCommand;

        private static async void DoAnswer()
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.EnterAsync(answerCommand))
            {
                logger.Debug("摘机");
                await svc.Answer();
            }
        }

        private static bool CanAnswer()
        {
            var callsIter = sipAccounts.SelectMany(m => m.Calls);
            if (!callsIter.Any(x => x.State == pjsip_inv_state.PJSIP_INV_STATE_INCOMING)) return false;
            return true;
        }

        private static readonly IRelayCommand hangupCommand = new RelayCommand(DoHangup, CanHangup);
        public IRelayCommand HangupCommand => hangupCommand;

        private static async void DoHangup()
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.EnterAsync(hangupCommand))
            {
                logger.Debug("挂机");
                await svc.Hangup();
            }
        }

        private static bool CanHangup()
        {
            var states = new pjsip_inv_state[] {
                pjsip_inv_state.PJSIP_INV_STATE_CALLING,pjsip_inv_state.PJSIP_INV_STATE_INCOMING,
                pjsip_inv_state.PJSIP_INV_STATE_EARLY,pjsip_inv_state.PJSIP_INV_STATE_CONNECTING,
                pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED
            };
            var callsIter = sipAccounts.SelectMany(m => m.Calls);
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
        }

        private static IReadOnlyCollection<services.Models.CallInfo> calls = new services.Models.CallInfo[] { };
        public IReadOnlyCollection<services.Models.CallInfo> Calls
        {
            get => calls;
            set
            {
                if (SetProperty(ref calls, value))
                    NotifyStateRelativeCommandsExecutable();
            }
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
            using (await Utils.CommandGuard.EnterAsync(GetStateRelativeCommands()))
            {
                await svc.Hold();
            }
        }

        private static bool CanHold()
        {
            if (!IsMainConnectionOk) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            if (status.Item1 != client.AgentState.Work) return false;
            if (!calls.Any(x => !x.IsHeld)) return false;
            return true;
        }

        private static readonly IRelayCommand unHoldCommand = new RelayCommand<object>(DoUnHold, CanUnHold);
        public IRelayCommand UnHoldCommand => unHoldCommand;
        private static async void DoUnHold(object parameter)
        {
            var svc = App.mainService;
            using (await Utils.CommandGuard.EnterAsync(GetStateRelativeCommands()))
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

        private static bool CanUnHold(object parameter)
        {
            if (!IsMainConnectionOk) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
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

        private static readonly IRelayCommand holdPopupCommand = new RelayCommand(DoHoldPopup, CanHoldPopup);
        public IRelayCommand HoldPopupCommand => holdPopupCommand;

        private static void DoHoldPopup()
        {
            Instance.IsHoldPopupOpened = !isHoldPopupOpened;
        }

        private static bool CanHoldPopup()
        {
            return heldCalls.Count > 0;
        }
        #endregion

        #region 排队列表

        private static void MainService_OnQueueInfoEvent(object sender, services.Events.QueueInfoEventArgs e)
        {
            var svc = App.mainService;
            Instance.QueueInfos = svc.GetQueueInfos();
        }

        private static bool isQueuePopupOpened;
        public bool IsQueuePopupOpened
        {
            get => isQueuePopupOpened;
            set => SetProperty(ref isQueuePopupOpened, value);
        }
        private static readonly IRelayCommand queuePopupCommand = new RelayCommand(DoQueuePopup, CanQueuePopup);
        public IRelayCommand QueuePopupCommand => queuePopupCommand;
        private static void DoQueuePopup()
        {
            Instance.IsQueuePopupOpened = !isQueuePopupOpened;
        }
        private static bool CanQueuePopup()
        {
            return queueInfos.Count > 0;
        }

        private static IReadOnlyCollection<services.Models.QueueInfo> queueInfos = new services.Models.QueueInfo[] { };
        public IReadOnlyCollection<services.Models.QueueInfo> QueueInfos
        {
            get => queueInfos;
            set
            {
                if (SetProperty(ref queueInfos, value))
                {
                    Application.Current.Dispatcher.Invoke(queuePopupCommand.NotifyCanExecuteChanged);
                }
            }
        }

        private static readonly IRelayCommand dequeueCommand = new RelayCommand<object>(DoDequeue, CanDequeue);
        public IRelayCommand DequeueCommand => dequeueCommand;
        private static async void DoDequeue(object paramter)
        {
            var queueInfo = (services.Models.QueueInfo)paramter;
            var svc = App.mainService;
            using (await Utils.CommandGuard.EnterAsync())
            {
                await svc.Dequeue(queueInfo.CtiIndex, queueInfo.Channel);
            }
        }
        private static bool CanDequeue(object paramter)
        {
            if (!IsMainConnectionOk) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            return true;
        }
        #endregion

        #region 座席咨询
        static readonly IRelayCommand xferConsultCommand = new RelayCommand(DoXferConsult);
        public IRelayCommand XferConsultCommand => xferConsultCommand;

        private static async void DoXferConsult()
        {
            var svc = App.mainService;

            Dialogs.PromptDialog dialog = new()
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

            await svc.XferConsult(groupId.Trim(), workerNum.Trim());
        }
        #endregion

        #region 座席转移
        private static readonly IRelayCommand xferCommand = new RelayCommand(DoXfer);
        public IRelayCommand XferCommand => xferCommand;

        private static async void DoXfer()
        {
            var svc = App.mainService;

            Dialogs.PromptDialog dialog = new()
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

            await svc.Xfer(groupId.Trim(), workerNum.Trim());
        }
        #endregion

        #region 外呼
        private static readonly IRelayCommand dialCommand = new RelayCommand(DoDial);
        public IRelayCommand DialCommand => dialCommand;

        private static async void DoDial()
        {
            var svc = App.mainService;
            Dialogs.PromptDialog dialog = new()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "拨号" },
                    { "Label", "输入拨打的号码" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;
            await svc.Dial(inputText);
        }
        #endregion

        #region 外转
        private static readonly IRelayCommand xferExtCommand = new RelayCommand(DoXferExt);
        public IRelayCommand XferExtCommand => xferExtCommand;

        private static async void DoXferExt()
        {
            var svc = App.mainService;
            Dialogs.PromptDialog dialog = new()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "向外转移" },
                    { "Label", "输入拨打的号码" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;
            await svc.XferExt(inputText);
        }
        #endregion

        #region 外咨
        private static readonly IRelayCommand xferExtConsultCommand = new RelayCommand(DoXferExtConsult);
        public IRelayCommand XferExtConsultCommand => xferExtConsultCommand;

        private static async void DoXferExtConsult()
        {
            var svc = App.mainService;
            Dialogs.PromptDialog dialog = new()
            {
                DataContext = new Dictionary<string, object> {
                    { "Title", "向外咨询" },
                    { "Label", "输入拨打的号码" }
                }
            };
            if (dialog.ShowDialog() != true) return;
            var inputText = dialog.InputText;
            await svc.XferExtConsult(inputText);
        }
        #endregion

        #region 转 IVR
        private static readonly IRelayCommand callIvrCommand = new RelayCommand(DoCallIvr);
        public IRelayCommand CallIvrCommand => callIvrCommand;

        private static async void DoCallIvr()
        {
            var svc = App.mainService;

            string ivrId;
            client.IvrInvokeType ivrType;
            string ivrString;

            {
                Dialogs.PromptDialog dialog = new()
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
                Dialogs.PromptDialog dialog = new()
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
                Dialogs.PromptDialog dialog = new()
                {
                    DataContext = new Dictionary<string, object> {
                        { "Title", "转 IVR" },
                        { "Label", "输入 IVR 的 文本参数" },
                    }
                };
                if (dialog.ShowDialog() != true) return;
                ivrString = dialog.InputText;
            }
            await svc.CallIvr(ivrId, ivrType, ivrString);
        }

        #endregion

        #region btnAdv
        private static readonly IRelayCommand advCommand = new RelayCommand(DoAdvCommand);
        public IRelayCommand AdvCommand => advCommand;

        private static async void DoAdvCommand()
        {
            var svc = App.mainService;

            int connIndex;
            client.MessageType msgTyp;
            int n;
            string s;

            {
                Dialogs.PromptDialog dialog = new()
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
                Dialogs.PromptDialog dialog = new()
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
                Dialogs.PromptDialog dialog = new()
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
                Dialogs.PromptDialog dialog = new()
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
                    await svc.Monitor(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_STOPLISTEN:
                    await svc.UnMonitor(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEIDLE:
                    await svc.SetIdle(s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEPAUSE:
                    {
                        var parts = s.Split(new char[] { '|' });
                        await svc.SetBusy(
                            parts[0],
                            (client.WorkType)Enum.Parse(typeof(client.WorkType), parts[1])
                        );
                    }
                    break;
                case client.MessageType.REMOTE_MSG_INTERCEPT:
                    await svc.Intercept(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEINSERT:
                    await svc.Interrupt(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCEHANGUP:
                    await svc.Hangup(connIndex, s);
                    break;
                case client.MessageType.REMOTE_MSG_FORCESIGNOFF:
                    {
                        var parts = s.Split(new char[] { '|' });
                        await svc.SetBusy(
                            parts[0],
                            (client.WorkType)Enum.Parse(typeof(client.WorkType), parts[1])
                        );
                        await svc.SignOut(parts[0], parts[1]);
                    }
                    break;
                case client.MessageType.REMOTE_MSG_KICKOUT:
                    await svc.KickOut(s);
                    break;
                default:
                    MessageBox.Show($"还没有实现 {msgTyp}");
                    break;
            }
        }
        #endregion

    }
}

#pragma warning restore VSTHRD100
