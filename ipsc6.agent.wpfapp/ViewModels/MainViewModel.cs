using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Mvvm.Input;

using org.pjsip.pjsua2;

#pragma warning disable VSTHRD100

using LocalRpcTargetFunc = System.Func<EmbedIO.WebSockets.WebSocketModule, EmbedIO.WebSockets.IWebSocketContext, object>;

namespace ipsc6.agent.wpfapp.ViewModels
{
#pragma warning disable IDE0065
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;
#pragma warning restore IDE0065

    public class MainViewModel : Utils.SingletonObservableObject<MainViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainViewModel));

        private const double NormalWindowHeight = 72;
        private const double SnappedWindowHeight = 8;

        #region ctor, deor, initial, release ...
        internal services.Service MainService { get; private set; }
        internal GuiService GuiService { get; private set; }

        private server.Server rpcServer;
        private Task rpcServerRunningTask;
        private CancellationTokenSource rpcServerRunningCanceller;

        internal config.Ipsc cfgIpsc;
        internal config.Startup cfgStartup;
        internal config.Phone cfgPhone;
        internal config.LocalWebServer cfgLocalWebServer;

        internal bool Load()
        {
            // 注意这里尚未登录

            Exception iniErr = null;
            var mainWindow = Application.Current.MainWindow;

            mainWindow.Hide();

            try
            {
                /// 非 GUI 的初始化
                ReloadConfigure();

                // 如果没有设置服务器地址，就必须要设置
                cfgIpsc.ServerList ??= new string[] { };
                while (cfgIpsc.ServerList.Count < 1)
                {
                    var r = MessageBox.Show(
                        "由于没有设置 CTI 服务器地址，该程序无法工作。\r\n是否要进行设置？\r\n\r\n按“是”打开设置窗口，按“否”退出程序。",
                        mainWindow.Title,
                        MessageBoxButton.YesNo, MessageBoxImage.Question
                    );
                    if (r == MessageBoxResult.No)
                    {
                        logger.Warn("未配置 CTI 服务器地址，且选择不进行配置。");
                        return false;
                    }
                    DoShowConfigWindow();

                    ReloadConfigure();
                    cfgIpsc.ServerList ??= new string[] { };
                }

                MainService = services.Service.Create(cfgIpsc, cfgPhone);
                MainService.OnCtiConnectionStateChanged += MainService_OnCtiConnectionStateChanged;
                MainService.OnLoginCompleted += MainService_OnLoginCompleted;
                MainService.OnStatusChanged += MainService_OnStatusChanged;
                MainService.OnSignedGroupsChanged += MainService_OnSignedGroupsChanged;
                MainService.OnRingCallReceived += MainService_OnRingCallReceived;
                MainService.OnHeldCallReceived += MainService_OnHeldCallReceived;
                MainService.OnTeleStateChanged += MainService_OnTeleStateChanged;
                MainService.OnSipRegisterStateChanged += MainService_OnSipRegisterStateChanged;
                MainService.OnSipCallStateChanged += MainService_OnSipCallStateChanged;
                MainService.OnQueueInfoEvent += MainService_OnQueueInfoEvent;
                MainService.OnStatsChanged += MainService_OnStatsChanged;

                GuiService = new();

                LocalRpcTargetFunc[] localRpcCreators = {
                    (_, _) => MainService,
                    (_, _) => GuiService,
                };
                rpcServer = cfgLocalWebServer.Port == 0 ? (new(localRpcCreators)) : (new(localRpcCreators, cfgLocalWebServer.Port));
                rpcServerRunningCanceller = new();
                rpcServerRunningTask = Task.Run(() => rpcServer.RunAsync(rpcServerRunningCanceller.Token));

                /// 登录
                if (!cfgStartup.LoginNotRequired)
                {
                    // 默认的方式：直接显示登录对话窗
                    // 登录失败否则就退出函数返回假
                    if (!new Views.LoginWindow().ShowDialog().Value)
                    {
                        logger.Warn("已关闭登录对话窗，放弃登录");
                        return false;
                    }
                }

                mainWindow.Show();

                /// GUI 的一些初始化
                IsShowToolbar = true;
                RootGridVerticalAlignment = VerticalAlignment.Bottom;
                mainWindow.Height = NormalWindowHeight;

                switch (cfgStartup.MainWindowStartupMode)
                {
                    case config.MainWindowStartupMode.Normal:
                        mainWindow.Activate();
                        mainWindow.Focus();
                        break;
                    case config.MainWindowStartupMode.Snapped:
                        mainWindow.Top = -1;
                        break;
                    case config.MainWindowStartupMode.Hide:
                        mainWindow.Hide();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Initial: {0}", e);
                iniErr = e;
            }

            if (iniErr != null)
            {
                MessageBox.Show(
                    $"主窗口视图模型初始化过程中出现了无法处理的异常，程序即将退出。\r\n\r\n{iniErr}",
                    mainWindow.Title,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                return false;
            }

            return true;
        }

        internal void ReloadConfigure()
        {
            ConfigManager.GetAllSettings();
            var cfgRoot = ConfigManager.ConfigurationRoot;

            cfgIpsc = new();
            cfgRoot.GetSection(nameof(config.Ipsc)).Bind(cfgIpsc);
            cfgStartup = new();
            cfgRoot.GetSection(nameof(config.Startup)).Bind(cfgStartup);
            cfgPhone = new();
            cfgRoot.GetSection(nameof(config.Phone)).Bind(cfgPhone);
            cfgLocalWebServer = new();
            cfgRoot.GetSection(nameof(config.LocalWebServer)).Bind(cfgLocalWebServer);

            if (string.IsNullOrWhiteSpace(cfgPhone.RingerWaveFile))
            {
                cfgPhone.RingerWaveFile = @"Audios/electronic-phone-ringer.wav";
            }
        }

        internal void Release()
        {
            logger.Info("Release");
            StopTimer();

            logger.Debug("Release - Stop RPC server");
            rpcServerRunningCanceller?.Cancel();
            try
            {
#pragma warning disable VSTHRD002 // 避免有问题的同步等待
                rpcServerRunningTask?.Wait();
#pragma warning restore VSTHRD002 // 避免有问题的同步等待
            }
            catch (TaskCanceledException) { }

            logger.Debug("Release - Dispose Main Service");
            MainService?.Dispose();

            logger.Debug("Release - Dispose GUI Service");
            GuiService?.Dispose();

            logger.Debug("Release - Completed");
        }

        private static IRelayCommand[] StateRelativeCommands => new IRelayCommand[] {
            loginCommand, logoutCommand,
            statePopupCommand, setStateCommand,
            groupPopupCommand, signGroupCommand,
            holdPopupCommand, holdCommand, unHoldCommand,
            xferPopupCommand, xferConsultPopupCommand,
            dialCommand, xferExtCommand, xferExtConsultCommand,
        };

        private static void NotifyStateRelativeCommandsExecutable()
        {
            foreach (var command in StateRelativeCommands)
            {
                if (command != null)
                {
                    Application.Current.Dispatcher.Invoke(command.NotifyCanExecuteChanged);
                }
            }
        }

        #endregion

        #region UI
        internal void ShowMainWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows.OfType<Window>().Last() ?? Application.Current.MainWindow;
                if (window == null)
                    return;
                if (window is Views.MainWindow)
                {
                    Snapped = false;
                    window.Topmost = true;
                }
                window.Show();
                window.Activate();
                window.Focus();
            });
        }

        internal bool IsExiting { get; private set; }

        internal void CloseMainWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var window in Application.Current.Windows.OfType<Window>())
                {
                    if (window is not Views.MainWindow)
                    {
                        window.Close();
                    }
                }
                IsExiting = true;
                Application.Current.MainWindow?.Close();
            });
        }

        internal void HideMainWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.MainWindow?.Hide();
            });
        }

        private static bool pinned;
        public bool Pinned
        {
            get => pinned;

            set
            {
                if (!SetProperty(ref pinned, value))
                    return;

                if (value && snapFsm != null)
                {
                    StateMachines.SnapTopState[] states =
                    {
                        StateMachines.SnapTopState.Initial,
                        StateMachines.SnapTopState.Snapped,
                        StateMachines.SnapTopState.SnappedWithMouseEnter,
                        StateMachines.SnapTopState.UnsnappedWithMouseLeave,
                        StateMachines.SnapTopState.Unsnapped,
                    };
                    if (states.Contains(snapFsm.State))
                    {
                        snapFsm.Fire(StateMachines.SnapTopTrigger.MoveOut);
                    }
                }
            }
        }

        private static readonly IRelayCommand pinCommand = new RelayCommand(DoPin);
        public IRelayCommand PinCommand => pinCommand;
        private static void DoPin()
        {
            Instance.Pinned = !pinned;
        }

        private static bool snapped;
        public bool Snapped
        {
            get => snapped;
            set
            {
                if (!SetProperty(ref snapped, value))
                    return;

                var win = Application.Current.MainWindow;
                if (value)
                {
                    IsShowToolbar = false;
                    RootGridVerticalAlignment = VerticalAlignment.Top;
                    win.Height = SnappedWindowHeight;
                    win.Top = 0;
                }
                else
                {
                    IsShowToolbar = true;
                    RootGridVerticalAlignment = VerticalAlignment.Bottom;
                    win.Height = NormalWindowHeight;
                    if (win.Top < 0)
                        win.Top = 0;
                }
            }
        }

        private static VerticalAlignment rootGridVerticalAlignment;

        public VerticalAlignment RootGridVerticalAlignment
        {
            get => rootGridVerticalAlignment;
            set => SetProperty(ref rootGridVerticalAlignment, value);
        }

        private CancellationTokenSource snapTimerCanceller;
        private StateMachines.SnapTopStateMachine snapFsm;

        private void InitialSnappingStateMachine()
        {
            if (Pinned) return;

            var dispatcher = Application.Current.Dispatcher;

            snapFsm = new();

            /// 各个状态的UI动作
            snapFsm.OnTransitioned(trans =>
            {
                //logger.DebugFormat("SnappingStateMachine - {0} == [{1}] ==> {2}", trans.Source, trans.Trigger, trans.Destination);
                switch (trans.Destination)
                {
                    case StateMachines.SnapTopState.Final:
                        snapTimerCanceller?.Cancel();
                        Snapped = false;
                        break;

                    case StateMachines.SnapTopState.Initial:
                        throw new InvalidOperationException($"{trans.Destination}");

                    case StateMachines.SnapTopState.Snapped:
                        snapTimerCanceller?.Cancel();
                        Snapped = true;
                        break;

                    case StateMachines.SnapTopState.SnappedWithMouseEnter:
                        SetSnapTimer(250);
                        break;

                    case StateMachines.SnapTopState.Unsnapped:
                        snapTimerCanceller?.Cancel();
                        Snapped = false;
                        break;

                    case StateMachines.SnapTopState.UnsnappedWithMouseLeave:
                        SetSnapTimer(250);
                        break;

                    default:
                        throw new InvalidOperationException($"{trans.Destination}");
                }
            });

            /// Snap 的 Initial 状态，需要启动计时器!
            SetSnapTimer(250);
        }

        private void SetSnapTimer(int msecs)
        {
#pragma warning disable VSTHRD110 // 观察异步调用的结果
            Application.Current.Dispatcher.Invoke(async () =>
#pragma warning restore VSTHRD110 // 观察异步调用的结果
            {
                bool isCancelled = false;
                snapTimerCanceller = new();
                try
                {
                    await Task.Delay(msecs, snapTimerCanceller.Token);
                }
                catch (TaskCanceledException)
                {
                    isCancelled = false;
                }
                finally
                {
                    snapTimerCanceller.Dispose();
                    snapTimerCanceller = null;
                }
                if (!isCancelled)
                {
                    await snapFsm.FireAsync(StateMachines.SnapTopTrigger.Timer);
                }
            });
        }

        private static double top;
        public double Top  // OneWayToSource !!!
        {
            get => top;

            set
            {
                if (!SetProperty(ref top, value))
                    return;

                if (Top < 0)
                {
                    if (snapFsm == null || snapFsm.State == StateMachines.SnapTopState.Final)
                        InitialSnappingStateMachine();
                }
                else if (snapFsm != null && (!Snapped || Top > 0))
                {
                    StateMachines.SnapTopState[] states =
                    {
                        StateMachines.SnapTopState.Initial,
                        StateMachines.SnapTopState.Snapped,
                        StateMachines.SnapTopState.SnappedWithMouseEnter,
                        StateMachines.SnapTopState.UnsnappedWithMouseLeave,
                        StateMachines.SnapTopState.Unsnapped,
                    };
                    if (states.Contains(snapFsm.State))
                    {
                        snapFsm.Fire(StateMachines.SnapTopTrigger.MoveOut);
                    }
                }
            }
        }

        private static bool isShowToolbar;
        public bool IsShowToolbar
        {
            get => isShowToolbar;
            set
            {
                if (SetProperty(ref isShowToolbar, value))
                {
                    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged("IsHideToolbar"));
                }
            }
        }
        public bool IsHideToolbar => !isShowToolbar;

        internal void MouseEnter()
        {
            if (snapFsm == null) return;

            StateMachines.SnapTopState[] states =
            {
                StateMachines.SnapTopState.Snapped,
                StateMachines.SnapTopState.UnsnappedWithMouseLeave,
            };
            if (states.Contains(snapFsm.State))
            {
                snapFsm.Fire(StateMachines.SnapTopTrigger.MouseEnter);
            }
        }

        internal void MouseLeave()
        {
            if (snapFsm == null) return;

            StateMachines.SnapTopState[] states =
            {
                StateMachines.SnapTopState.SnappedWithMouseEnter,
                StateMachines.SnapTopState.Unsnapped,
            };
            if (states.Contains(snapFsm.State))
            {
                snapFsm.Fire(StateMachines.SnapTopTrigger.MouseLeave);
            }
        }


        private CancellationTokenSource timerCanceller;
        private Task timerTask;

        internal void StartTimer()
        {
            var dispatcher = Application.Current.Dispatcher;
            ResetStatusTimeSpan();
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

        private void StopTimer()
        {

            try
            {
                timerCanceller?.Cancel();
                try
                {
#pragma warning disable VSTHRD002
                    timerTask?.Wait();
#pragma warning restore VSTHRD002
                }
                catch (TaskCanceledException) { }
            }
            finally
            {
                timerCanceller?.Dispose();
            }
        }

        private static void DoOnTimer()
        {
            Instance.StatusDuration = DateTime.UtcNow - lastStatusTime;
        }
        #endregion

        #region Agent Status
        private static void MainService_OnCtiConnectionStateChanged(object sender, services.Events.CtiConnectionStateChangedEventArgs e)
        {
            var svc = Instance.MainService;
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
                if (!SetProperty(ref ctiServices, value)) return;
                NotifyStateRelativeCommandsExecutable();
            }
        }

        private static void MainService_OnLoginCompleted(object sender, EventArgs e)
        {
            var svc = Instance.MainService;
            var ss = svc.GetWorkerNum();
            Instance.WorkerNumber = ss[0];
            Instance.DisplayName = ss[1];
        }

        private static bool IsMainConnectionOk => ctiServices.Any(x => x.State == client.ConnectionState.Ok && x.IsMain);

        private static void MainService_OnStatusChanged(object sender, services.Events.StatusChangedEventArgs e)
        {
            Instance.Status = new AgentStateWorkType(e.NewState, e.NewWorkType);
            if (e.NewState != client.AgentState.Work)
            {
                Instance.CurrentCallInfo = null;
            }
            ResetStatusTimeSpan();
        }

        private static string workerNum;
        public string WorkerNumber
        {
            get => workerNum;
            set
            {
                if (!SetProperty(ref workerNum, value)) return;
                NotifyStateRelativeCommandsExecutable();
            }
        }
        private static string displayName;
        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }

        private static AgentStateWorkType status = Tuple.Create(client.AgentState.NotExist, client.WorkType.Unknown);
        public AgentStateWorkType Status
        {
            get => status;
            set
            {
                if (!SetProperty(ref status, value)) return;
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

        #region 信息面板
        private static void MainService_OnStatsChanged(object sender, EventArgs e)
        {
            var svc = Instance.MainService;
            Instance.Stats = svc.GetStats();
        }

        private static services.Models.CallInfo currentCallInfo;
        public services.Models.CallInfo CurrentCallInfo
        {
            get => currentCallInfo;
            set
            {
                if (!SetProperty(ref currentCallInfo, value)) return;
                IsCurrentCallActive = value != null;
                IsNotCurrentCallActive = !IsCurrentCallActive;
            }
        }
        private static bool isCurrentCallActive = false;
        public bool IsCurrentCallActive
        {
            get => isCurrentCallActive;
            set => SetProperty(ref isCurrentCallActive, value);
        }
        private static bool isNotCurrentCallActive = true;
        public bool IsNotCurrentCallActive
        {
            get => isNotCurrentCallActive;
            set => SetProperty(ref isNotCurrentCallActive, value);
        }

        private static services.Models.Stats stats;
        public services.Models.Stats Stats
        {
            get => stats;
            set => SetProperty(ref stats, value);
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
            var svc = Instance.MainService;
            Instance.Groups = svc.GetGroups();
        }

        private static IReadOnlyCollection<services.Models.Group> groups;
        public IReadOnlyCollection<services.Models.Group> Groups
        {
            get => groups;
            set
            {
                if (!SetProperty(ref groups, value)) return;
                NotifyStateRelativeCommandsExecutable();
            }
        }

        private static readonly IRelayCommand signGroupCommand = new RelayCommand<object>(DoSignGroup, CanSignGroup);
        public IRelayCommand SignGroupCommand => signGroupCommand;

        private static async void DoSignGroup(object parameter)
        {
            var svc = Instance.MainService;

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            string groupId = parameter as string;

            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
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
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
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
            if (!allowedAgentStates.Contains(status.Item1)) return false;
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
            if (e.NewState == client.TeleState.OnHook)
            {
                Instance.CurrentCallInfo = null;
            }
        }

        private static client.TeleState teleState;
        public client.TeleState TeleState
        {
            get => teleState;
            set
            {
                if (!SetProperty(ref teleState, value)) return;
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
            var svc = Instance.MainService;
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
                if (!SetProperty(ref sipAccounts, value)) return;
                NotifyStateRelativeCommandsExecutable();
            }
        }
        private static readonly IRelayCommand answerCommand = new RelayCommand(DoAnswer, CanAnswer);
        public IRelayCommand AnswerCommand => answerCommand;

        private static async void DoAnswer()
        {
            var svc = Instance.MainService;
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
            var svc = Instance.MainService;
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

        #region 振铃, 保持, 取消保持, 保持列表
        private static void MainService_OnHeldCallReceived(object sender, services.Events.CallInfoEventArgs e)
        {
            ReloadCalls();
        }

        private static void MainService_OnRingCallReceived(object sender, services.Events.CallInfoEventArgs e)
        {
            ReloadCalls();
            Instance.CurrentCallInfo = e.Call;
        }

        private static void ReloadCalls()
        {
            var svc = Instance.MainService;
            Instance.Calls = svc.GetCalls();
            Instance.HeldCalls = Instance.Calls.Where(x => x.IsHeld).ToList();
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
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
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
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
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
            if (status.Item1 != client.AgentState.Work) return false;
            if (parameter == null)
            {
                /// NOTE: 不看是否真的有保持的通话，一律以工作类型为准！
                /// 如果不是，可以用下面的注释代码顶替。
                /// if (!calls.Any(x => x.IsHeld)) return false;
                client.WorkType[] workTypes =
                {
                    client.WorkType.Hold,
                    client.WorkType.Consult,
                };
                if (!workTypes.Contains(status.Item2)) return false;
                // 如果有多个保持的，就得选中具体的call，不能用这个按钮
                if (heldCalls.Count > 1) return false;
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
            var svc = Instance.MainService;
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
            var svc = Instance.MainService;
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

        #region 座席咨询,转移
        private static readonly IRelayCommand xferConsultCommand = new RelayCommand(DoXferConsult);
        public IRelayCommand XferConsultCommand => xferConsultCommand;

        private static async void DoXferConsult()
        {
            var svc = Instance.MainService;

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

        private static readonly IRelayCommand xferCommand = new RelayCommand(DoXfer);
        public IRelayCommand XferCommand => xferCommand;

        private static async void DoXfer()
        {
            var svc = Instance.MainService;

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

        private static IReadOnlyCollection<services.Models.Group> allGroups;
        public IReadOnlyCollection<services.Models.Group> AllGroups
        {
            get => allGroups;
            set => SetProperty(ref allGroups, value);
        }

        #region Xfer Popup
        private static bool isXferPopuped;
        public bool IsXferPopuped
        {
            get => isXferPopuped;
            set
            {
                if (!SetProperty(ref isXferPopuped, value)) return;
                if (value)  // 如果打开，就加载数据！
                    AllGroups = MainService.GetAllGroups();
            }
        }
        private static readonly IRelayCommand xferPopupCommand = new RelayCommand(DoXferPopup, CanXferPopup);
        public IRelayCommand XferPopupCommand => xferPopupCommand;
        private static void DoXferPopup()
        {
            Instance.IsXferPopuped = !isXferPopuped;
        }
        #endregion


        #region XferConsult Popup
        private static bool isXferConsultPopuped;
        public bool IsXferConsultPopuped
        {
            get => isXferConsultPopuped;
            set
            {
                if (!SetProperty(ref isXferConsultPopuped, value)) return;
                if (value)  // 如果打开，就加载数据！
                    AllGroups = MainService.GetAllGroups();
            }
        }
        private static readonly IRelayCommand xferConsultPopupCommand = new RelayCommand(DoXferConsultPopup, CanXferPopup);
        public IRelayCommand XferConsultPopupCommand => xferConsultPopupCommand;
        private static void DoXferConsultPopup()
        {
            Instance.IsXferConsultPopuped = !isXferConsultPopuped;
        }
        #endregion

        private static bool CanXferPopup()
        {
            if (!IsMainConnectionOk) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            if (status.Item1 != client.AgentState.Work) return false;
            if (heldCalls.Count > 0) return false;
            return true;
        }

        private static readonly IRelayCommand selectGroupOkCommand = new RelayCommand<object>(DoSelectGroupOk);
        public IRelayCommand SelectGroupOkCommand => selectGroupOkCommand;

        private static async void DoSelectGroupOk(object data)
        {
            if (data == null) throw new InvalidOperationException();
            var groupId = (string)data;
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
            {
                if (Instance.IsXferPopuped)
                {
                    logger.DebugFormat("释放转到座席组 {0}", groupId);
                    await svc.Xfer(groupId);
                    Instance.IsXferPopuped = false;
                }
                else if (Instance.IsXferConsultPopuped)
                {
                    logger.DebugFormat("保持转到座席组 {0}", groupId);
                    await svc.XferConsult(groupId);
                    Instance.IsXferConsultPopuped = false;
                }
            }
        }
        #endregion

        #region 外呼, 外转, 外咨

        private static string inputTelNum;
        public string InputTelNum
        {
            get => inputTelNum;
            set
            {
                if (SetProperty(ref inputTelNum, value))
                {
                    foreach (var cmd in new IRelayCommand[]
                    {
                        dialCommand, xferExtCommand, xferExtConsultCommand
                    })
                    {
                        cmd.NotifyCanExecuteChanged();
                    }
                }
            }
        }

        private static readonly IRelayCommand dialCommand = new RelayCommand(DoDial, CanDial);
        public IRelayCommand DialCommand => dialCommand;

        private static async void DoDial()
        {
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
            {
                await svc.Dial(inputTelNum.Trim());
            }
        }

        public static bool CanDial()
        {
            if (!IsMainConnectionOk) return false;
            if (string.IsNullOrWhiteSpace(inputTelNum)) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            if (status.Item1 != client.AgentState.Idle) return false;
            return true;
        }

        private static readonly IRelayCommand xferExtCommand = new RelayCommand(DoXferExt, CanXferExt);
        public IRelayCommand XferExtCommand => xferExtCommand;

        private static async void DoXferExt()
        {
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
            {
                await svc.XferExt(inputTelNum.Trim());
            }
        }

        public static bool CanXferExt()
        {
            if (!IsMainConnectionOk) return false;
            if (string.IsNullOrWhiteSpace(inputTelNum)) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            if (status.Item1 != client.AgentState.Work) return false;
            if (heldCalls.Count > 0) return false;
            return true;
        }

        private static readonly IRelayCommand xferExtConsultCommand = new RelayCommand(DoXferExtConsult, CanXferExtConsult);
        public IRelayCommand XferExtConsultCommand => xferExtConsultCommand;

        private static async void DoXferExtConsult()
        {
            var svc = Instance.MainService;
            using (await Utils.CommandGuard.EnterAsync(StateRelativeCommands))
            {
                await svc.XferExtConsult(inputTelNum.Trim());
            }
        }

        public static bool CanXferExtConsult()
        {
            if (!IsMainConnectionOk) return false;
            if (string.IsNullOrWhiteSpace(inputTelNum)) return false;
            if (Utils.CommandGuard.IsGuarding) return false;
            if (status.Item1 != client.AgentState.Work) return false;
            if (heldCalls.Count > 0) return false;
            return true;
        }
        #endregion

        #region 转 IVR
        private static readonly IRelayCommand callIvrCommand = new RelayCommand(DoCallIvr);
        public IRelayCommand CallIvrCommand => callIvrCommand;

        private static async void DoCallIvr()
        {
            var svc = Instance.MainService;

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
            var svc = Instance.MainService;

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

        #region 设置
        private static readonly IRelayCommand showConfigWindowCmmand = new RelayCommand(DoShowConfigWindow);
        public IRelayCommand ShowConfigWindowCmmand => showConfigWindowCmmand;

        private static void DoShowConfigWindow()
        {
            new Views.ConfigWindow().ShowDialog();
        }

        private static readonly IRelayCommand loginCommand = new RelayCommand(DoLogin, CanLogin);
        public IRelayCommand LoginCommand => loginCommand;

        private static void DoLogin()
        {
            logger.Debug("登录");
            if (!new Views.LoginWindow().ShowDialog().Value)
            {
                logger.Warn("已关闭登录对话窗，放弃登录");
            }
        }

        private static bool CanLogin()
        {
            var svc = Instance.MainService;
            if (svc == null) return false;
            if (svc.GetAgentRunningState() != client.AgentRunningState.Stopped) return false;
            return true;
        }

        private static readonly IRelayCommand logoutCommand = new RelayCommand(DoLogout, CanLogout);
        public IRelayCommand LogoutCommand => logoutCommand;

        private static async void DoLogout()
        {
            logger.Debug("注销");

            var svc = Instance.MainService;
            await svc.LogOut();

            Instance.SipAccounts = Array.Empty<services.Models.SipAccount>();
            Instance.Groups = Array.Empty<services.Models.Group>();
            Instance.AllGroups = Array.Empty<services.Models.Group>();
            Instance.Calls = Array.Empty<services.Models.CallInfo>();
        }

        private static bool CanLogout()
        {
            var svc = Instance.MainService;

            if (svc == null) return false;

            if (svc.GetAgentRunningState() != client.AgentRunningState.Started) return false;
            if (status.Item1 == client.AgentState.Work) return false;

            return true;
        }
        #endregion
    }
}

#pragma warning restore VSTHRD100
