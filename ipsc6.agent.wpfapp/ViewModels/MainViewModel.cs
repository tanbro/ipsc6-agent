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

        //internal void RefreshAgentExecutables()
        //{
        //    IRelayCommand[] commands =
        //    {
        //        answerCommand, hangupCommand,
        //        statePopupCommand, setStateCommand,
        //        skillSignCommand,
        //        holdCommand, unHoldCommand,
        //    };
        //    var _ = App.TaskFactory.StartNew(() =>
        //    {
        //        foreach (var command in commands)
        //        {
        //            command.NotifyCanExecuteChanged();
        //        }
        //    });
        //}

        static MainViewModel()
        {
            App.mainService.OnLoginCompleted += MainService_OnLoginCompleted;
            App.mainService.OnStatusChanged += MainService_OnStatusChanged;
            App.mainService.OnSignedGroupsChanged += MainService_OnSignedGroupsChanged;
        }

        private static void MainService_OnLoginCompleted(object sender, EventArgs e)
        {
            var m = App.mainService.Model;
            Instance.WorkerNumber = m.WorkerNumber;
            Instance.DisplayName = m.DisplayName;
        }

        private static void MainService_OnStatusChanged(object sender, services.Events.StatusChangedEventArgs e)
        {
            Instance.Status = new AgentStateWorkType(e.NewState, e.NewWorkType);
        }

        private static void MainService_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            Instance.Groups = App.mainService.Model.Groups.ToList();
        }

        #region Agent Status
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

        private static client.AgentState teleStats;
        public client.AgentState TeleStats
        {
            get => teleStats;
            set => SetProperty(ref teleStats, value);
        }

        #endregion

        #region 技能组 Popup
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
            return true;
        }
        #endregion

        #region 座席组

        private static IReadOnlyCollection<services.Models.Group> groups;
        public IReadOnlyCollection<services.Models.Group> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        private static readonly IAsyncRelayCommand signGroupCommand = new AsyncRelayCommand<object>(DoSignGroupAsync, CanSignGroup);
        public IAsyncRelayCommand SignGroupCommand => signGroupCommand;

        private static async Task DoSignGroupAsync(object parameter)
        {
            string groupId = parameter as string;
            var group = groups.First(x => x.Id == groupId);
            bool isSignIn = !group.IsSigned;
            if (isSignIn)
                logger.DebugFormat("签入 {0}", groupId);
            else
                logger.DebugFormat("签出 {0}", groupId);
            await App.mainService.SignGroup(groupId, isSignIn);
        }

        private static bool CanSignGroup(object _)
        {
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
            //var agent = Controllers.AgentController.Agent;
            //client.AgentState[] allowedAgentStates = { client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            //if (!allowedAgentStates.Any(x => x == agent.AgentState)) return false;
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

        private static readonly IRelayCommand setStateCommand = new AsyncRelayCommand<object>(DoSetStateAsync, CanSetState);
        public IRelayCommand SetStateCommand => setStateCommand;
        private static async Task DoSetStateAsync(object parameter)
        {
            logger.DebugFormat("设置状态: {0}", parameter);
            var st = parameter as AgentStateWorkType;
            var svr = App.mainService;
            if (st.Item1 == client.AgentState.Idle)
            {
                await svr.SetIdle();
            }
            else if (st.Item1 == client.AgentState.Pause)
            {
                await svr.SetBusy(st.Item2);
            }
            else if (st.Item1 == client.AgentState.Leave)
            {
                await svr.SetBusy();
            }
        }

        private static bool CanSetState(object parameter)
        {
            //if (doingSetState) return false;
            //var agent = Controllers.AgentController.Agent;
            //if (agent == null) return false;
            //if (agent.IsRequesting) return false;
            //client.AgentState[] vals = { client.AgentState.Idle, client.AgentState.Pause, client.AgentState.Leave };
            //if (!vals.Any(x => x == agent.AgentState)) return false;
            //if (parameter != null)
            //{
            //    var st = parameter as AgentStateWorkType;
            //    if (st.Item2 == agent.WorkType) return false;
            //}
            return true;
        }
        #endregion

        #region SIP/Tele
        private static client.TeleState teleState;
        public client.TeleState TeleState
        {
            get => teleState;
            set => SetProperty(ref teleState, value);
        }

        #endregion
        /*
                #region Answer
                static readonly IRelayCommand answerCommand = new AsyncRelayCommand(DoAnswerAsync, CanAnswer);
                public IRelayCommand AnswerCommand => answerCommand;
                static bool doingAnswer = false;

                static async Task DoAnswerAsync()
                {
                    doingAnswer = true;
                    try
                    {
                        answerCommand.NotifyCanExecuteChanged();
                        logger.Debug("摘机");
                        var agent = Controllers.AgentController.Agent;
                        await agent.AnswerAsync();
                    }
                    finally
                    {
                        doingAnswer = false;
                        answerCommand.NotifyCanExecuteChanged();
                    }
                }

                static bool CanAnswer()
                {
                    if (doingAnswer) return false;
                    var agent = Controllers.AgentController.Agent;
                    if (agent == null) return false;

                    var callCnt = (
                        from c in agent.SipAccounts.SelectMany(x => x.Calls)
                        where c.State == org.pjsip.pjsua2.pjsip_inv_state.PJSIP_INV_STATE_INCOMING
                        select c
                    ).Count();
                    if (callCnt < 1) return false;

                    return true;
                }
                #endregion

                #region Command Hangup
                static readonly IRelayCommand hangupCommand = new AsyncRelayCommand(DoHangupAsync, CanHangup);
                public IRelayCommand HangupCommand => hangupCommand;
                static bool doingHangup = false;
                static async Task DoHangupAsync()
                {
                    doingHangup = true;
                    try
                    {
                        hangupCommand.NotifyCanExecuteChanged();
                        logger.Debug("挂机");
                        var agent = Controllers.AgentController.Agent;
                        await agent.HangupAsync();
                    }
                    finally
                    {
                        doingHangup = false;
                        hangupCommand.NotifyCanExecuteChanged();
                    }
                }

                static bool CanHangup()
                {
                    if (doingHangup) return false;

                    var sipAccountList = Instance.AgentBasicInfo.SipAccountList;
                    var callCnt = (
                        from c in sipAccountList.SelectMany(x => x.Calls)
                        select c
                    ).Count();
                    if (callCnt < 1) return false;

                    return true;
                }
                #endregion

                #region 座席咨询
                static readonly IRelayCommand xferConsultCommand = new AsyncRelayCommand(DoXferConsultAsync);
                public IRelayCommand XferConsultCommand => xferConsultCommand;
                static async Task DoXferConsultAsync()
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

                    await agent.XferConsultAsync(groupId.Trim(), workerNum.Trim());
                }
                #endregion

                #region 座席转移
                static readonly IRelayCommand xferCommand = new AsyncRelayCommand(DoXferAsync);
                public IRelayCommand XferCommand => xferCommand;
                static async Task DoXferAsync()
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

                    await agent.XferAsync(groupId.Trim(), workerNum.Trim());
                }
                #endregion

                #region 保持
                static readonly IRelayCommand holdCommand = new AsyncRelayCommand(DoHoldAsync, CanHold);
                public IRelayCommand HoldCommand => holdCommand;
                static async Task DoHoldAsync()
                {
                    var agent = Controllers.AgentController.Agent;
                    await agent.HoldAsync();
                    Instance.RefreshAgentExecutables();
                }
                static bool CanHold()
                {
                    var agent = Controllers.AgentController.Agent;
                    if (agent.IsRequesting) return false;
                    if (agent.AgentState != client.AgentState.Work) return false;

                    var calls = Instance.AgentBasicInfo.CallList;

                    if (calls.Count == 0) return false;
                    if (calls.All(x => x.IsHeld)) return false;
                    return true;
                }
                #endregion

                #region 取消保持
                static readonly IRelayCommand unHoldCommand = new AsyncRelayCommand<object>(DoUnHoldAsync, CanUnHold);
                public IRelayCommand UnHoldCommand => unHoldCommand;
                static async Task DoUnHoldAsync(object parameter)
                {
                    var agent = Controllers.AgentController.Agent;
                    if (parameter == null)
                    {
                        await agent.UnHoldAsync();
                    }
                    else
                    {
                        await agent.UnHold(parameter as client.Call);
                    }
                    Instance.RefreshAgentExecutables();
                }
                static bool CanUnHold(object parameter)
                {
                    var agent = Controllers.AgentController.Agent;
                    if (agent.IsRequesting) return false;
                    if (agent.AgentState != client.AgentState.Work) return false;
                    if (parameter == null)
                    {
                        if (agent.HeldCalls.Count == 0) return false;
                    }
                    else
                    {
                        var callInfo = parameter as client.Call;
                        if (!callInfo.IsHeld) return false;
                    }
                    return true;
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

                static readonly IRelayCommand dequeueCommand = new AsyncRelayCommand<object>(DoDequeueAsync);
                public IRelayCommand DequeueCommand => dequeueCommand;
                static async Task DoDequeueAsync(object paramter)
                {
                    var queueInfo = paramter as client.QueueInfo;
                    var agent = Controllers.AgentController.Agent;
                    await agent.DequeueAsync(queueInfo);
                }
                #endregion

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
