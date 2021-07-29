using org.pjsip.pjsua2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace ipsc6.agent.client
{
    public class Agent : IDisposable
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Agent));

        public Agent(ushort localPort = 0, string localAddress = "")
        {
            LocalPort = localPort;
            LocalAddress = localAddress;
        }

        // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        //~Agent()
        //{
        //  不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //  Dispose(disposing: false);
        //}

        // Flag: Has Dispose already been called?
        private bool disposedValue;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    DisposePjSipAccounts();
                    DisposeConnections();
                }
                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                //
                disposedValue = true;
            }
        }

        internal static Endpoint SipEndpoint;
        internal static TaskFactory SyncFactory;
        public ushort LocalPort { get; private set; }
        public string LocalAddress { get; private set; }

        private static SipConfigArgs _sipConfigArgs;

        public static bool IsInitialized { get; private set; }

        public static void Initial(SipConfigArgs sipConfigArgs = null)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Initialized already");
            logger.Info("Initial");

            /// Get a Synchronized TaskFactory
            SyncFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

            /// Init PJ SIP UA
            logger.Info("create pjsua2 endpoint.");
            _sipConfigArgs = sipConfigArgs ?? new SipConfigArgs();
            if (_sipConfigArgs.TransportConfigArgsCollection.Count() == 0)
            {
                _sipConfigArgs.TransportConfigArgsCollection = new SipTransportConfigArgs[] { new() };
            }
            SipEndpoint = new Endpoint();
            SipEndpoint.libCreate();
            using (var epCfg = new EpConfig())
            {
                epCfg.uaConfig.maxCalls = 1;
                //epCfg.logConfig.level = 3;
                //epCfg.logConfig.msgLogging = 0;
                //epCfg.logConfig.writer = SipLogWriter.Instance;
                SipEndpoint.libInit(epCfg);
                foreach (var args in _sipConfigArgs.TransportConfigArgsCollection)
                {
                    logger.DebugFormat("pjsua2 endpoint craete transport: {0}", args);
                    using TransportConfig cfg = new() { port = args.Port, portRange = args.PortRange };
                    if (!string.IsNullOrWhiteSpace(args.BoundAddress))
                    {
                        cfg.boundAddress = args.BoundAddress;
                    }
                    if (!string.IsNullOrWhiteSpace(args.PublicAddress))
                    {
                        cfg.publicAddress = args.PublicAddress;
                    }
                    SipEndpoint.transportCreate(args.TransportType, cfg);
                }
            }
            logger.Debug("pjsua2 endpoint start...");
            SipEndpoint.libStart();
            logger.Debug("pjsua2 endpoint start ok!");

            /// Init our Raknet library
            logger.Debug("network.Connector.Initial()");
            network.Connector.Initial();

            /// OK!
            IsInitialized = true;
        }

        public static void Release()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Not initialized yet");
            logger.Info("Release");

            logger.Debug("network.Connector.Release()");
            network.Connector.Release();

            logger.Debug("destory pjsua2 Endpoint");
            SipEndpoint.libDestroy();
            SipEndpoint.Dispose();
            IsInitialized = false;
        }

        private readonly RequestGuard requestGuard = new();

        public AgentRunningState RunningState { get; private set; } = AgentRunningState.Stopped;

        public string WorkerNum { get; private set; }

        private string password;

        private readonly List<Connection> connections = new();
        private readonly List<CtiServer> ctiServers = new();
        public IReadOnlyList<CtiServer> CtiServers => ctiServers;
        public int GetConnetionIndex(CtiServer connectionInfo) => ctiServers.IndexOf(connectionInfo);
        private Connection GetConnection(int index) => connections[index];
        private Connection GetConnection(CtiServer connectionInfo) => connections[GetConnetionIndex(connectionInfo)];
        public ConnectionState GetConnectionState(int index) => GetConnection(index).State;
        public ConnectionState GetConnectionState(CtiServer ctiServer)
        {
            var index = ctiServers.IndexOf(ctiServer);
            return connections[index].State;
        }

        public int MainConnectionIndex { get; private set; } = -1;
        private Connection MainConnection => connections[MainConnectionIndex];
        public CtiServer MainConnectionInfo => (MainConnectionIndex < 0) ? null : ctiServers[MainConnectionIndex];

        public event EventHandler OnMainConnectionChanged;
        public int AgentId => MainConnection.AgentId;

        public string DisplayName { get; private set; }
        public AgentChannelInfo AgentChannelInfo { get; private set; }
        public AgentState AgentState { get; private set; } = AgentState.NotExist;
        public WorkType WorkType { get; private set; } = WorkType.Unknown;
        public AgentStateWorkType AgentStateWorkType
        {
            get => new(AgentState, WorkType);
            private set
            {
                AgentState = value.AgentState;
                WorkType = value.WorkType;
            }
        }
        public event EventHandler<AgentStateChangedEventArgs> OnAgentStateChanged;

        public TeleState TeleState { get; private set; } = TeleState.OnHook;
        public event EventHandler<TeleStateChangedEventArgs> OnTeleStateChanged;

        private readonly HashSet<QueueInfo> queueInfos = new();
        public IReadOnlyCollection<QueueInfo> QueueInfos => queueInfos;
        public event EventHandler<QueueInfoEventArgs> OnQueueInfoReceived;

        public event EventHandler<HoldInfoEventArgs> OnHoldInfoReceived;

        private readonly HashSet<CallInfo> calls = new();
        public IReadOnlyCollection<CallInfo> Calls => calls;

        public IReadOnlyCollection<CallInfo> HeldCalls
        {
            get
            {
                lock (this)
                {
                    return (
                        from m in calls
                        where m.IsHeld
                        select m
                    ).ToList();
                }
            }
        }

        private void Conn_OnServerSend(object sender, ServerSentEventArgs e)
        {

            var conn = sender as Connection;
            var msg = e.Message;
            var index = connections.IndexOf(conn);
            var ctiServer = ctiServers[index];
            switch (msg.Type)
            {
                case MessageType.REMOTE_MSG_SETSTATE:
                    /// 状态改变
                    ProcessStateChangedMessage(ctiServer, msg);
                    break;
                case MessageType.REMOTE_MSG_SETTELESTATE:
                    /// 电话状态改变
                    ProcessTeleStateChangedMessage(ctiServer, msg);
                    break;
                case MessageType.REMOTE_MSG_QUEUEINFO:
                    /// 排队信息
                    ProcessQueueInfoMessage(ctiServer, msg);
                    break;
                case MessageType.REMOTE_MSG_HOLDINFO:
                    /// 保持信息
                    ProcessHoldInfoMessage(ctiServer, msg);
                    break;
                case MessageType.REMOTE_MSG_SENDDATA:
                    /// 其他各种数据
                    ProcessDataMessage(ctiServer, msg);
                    break;
                default:
                    break;
            }
        }

        private AgentStateWorkType cachedStateWorkType;

        private void ProcessStateChangedMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            AgentStateWorkType newState;
            var newWorkType = (WorkType)msg.N2;
            if (newWorkType == WorkType.OffHooked)
            {
                // 特殊处理，不改变 WorkType!
                newState = new AgentStateWorkType((AgentState)msg.N1, AgentStateWorkType.WorkType);
            }
            else
            {
                newState = new AgentStateWorkType((AgentState)msg.N1, (WorkType)msg.N2);
            }
            logger.DebugFormat("StateChanged - {0}", newState);
            var currIndex = ctiServers.FindIndex(ci => ci == ctiServer);
            AgentStateWorkType oldState = null;
            Connection oldMainConnObj = null;
            lock (this)
            {
                if (AgentStateWorkType != newState)
                {
                    oldState = AgentStateWorkType.Clone() as AgentStateWorkType;
                    AgentStateWorkType = newState;
                    // 缓存状态
                    if (!isEverLostAllConnections)
                    {
                        if (AgentState is AgentState.Idle or AgentState.Pause
                            && availableSetBusyWorkTypes.Contains(WorkType))
                        {
                            cachedStateWorkType = newState;
                        }
                    }
                }
                // 如果是工作状态且主连接不是当前的，需要切换！
                if (newState.AgentState is AgentState.Ring or AgentState.Work
                    && currIndex != MainConnectionIndex)
                {
                    oldMainConnObj = MainConnection;
                    // 先修改 index
                    MainConnectionIndex = currIndex;
                    logger.InfoFormat("切换主服务节点到 {0}", MainConnection);
                }
            }

            if (oldMainConnObj != null)
            {
                // 通知原来的主，无论能否通知成功
                Task.Run(async () =>
                {
                    AgentRequestMessage req = new(MessageType.REMOTE_MSG_TAKENAWAY);
                    await oldMainConnObj.RequestAsync(req);
                });
                // fire the event
                OnMainConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
            if (oldState != null)
            {
                AgentStateChangedEventArgs ev = new(oldState, newState);
                OnAgentStateChanged?.Invoke(this, ev);
            }
        }

        private void ProcessTeleStateChangedMessage(CtiServer _, ServerSentMessage msg)
        {
            TeleState oldState;
            TeleState newState = (TeleState)msg.N1;
            logger.DebugFormat("TeleStateChanged - {0} --> {1}", TeleState, newState);
            TeleStateChangedEventArgs ev = null;

            lock (this)
            {
                oldState = TeleState;
                if (oldState != newState)
                {
                    TeleState = newState;
                    ev = new TeleStateChangedEventArgs(oldState, newState);
                }
                // 如果挂机了， Call List 必须清空
                if (newState == TeleState.OnHook)
                {
                    calls.Clear();
                    WorkingChannelInfo = null;
                }
            }

            // Offhook 请求的对应的自动摘机
            if (offHookSemaphore.CurrentCount < 1)
            {
                switch (newState)
                {
                    case TeleState.OffHook:
                        logger.Debug("TeleStateChangedMessage - Server side Offhooking Succees");
                        offHookServerTcs?.TrySetResult(null);
                        break;
                    default:
                        logger.Debug("TeleStateChangedMessage - Server side Offhooking Fail");
                        offHookServerTcs?.TrySetCanceled();
                        break;
                }
            }

            if (ev != null)
            {
                OnTeleStateChanged?.Invoke(this, ev);
            }
        }

        private static readonly QueueEventType[] aliveQueueEventTypes = { QueueEventType.Join, QueueEventType.Wait };

        private void ProcessQueueInfoMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            QueueInfo queueInfo = new(ctiServer, msg, groups);
            bool isAlive = aliveQueueEventTypes.Contains(queueInfo.EventType);
            logger.DebugFormat("QueueInfoMessage - {0}", queueInfo);
            // 记录 QueueInfo
            lock (this)
            {
                // 修改之前,一律先删除
                queueInfos.Remove(queueInfo);
                if (isAlive)
                    queueInfos.Add(queueInfo);
            }
            OnQueueInfoReceived?.Invoke(this, new(ctiServer, queueInfo));
        }

        private void ProcessHoldInfoMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            var channel = msg.N1;
            var holdEventType = (HoldEventType)msg.N2;
            var isHeld = holdEventType != HoldEventType.Cancel;
            CallInfo callInfo;
            lock (this)
            {
                callInfo = calls.First(x => x.CtiServer == ctiServer && x.Channel == channel);
                callInfo.IsHeld = isHeld;
                callInfo.HoldType = holdEventType;
            }
            logger.DebugFormat("HoldInfoMessage - {0}: {1}", isHeld ? "UnHold" : "Hold", callInfo);
            OnHoldInfoReceived?.Invoke(this, new(ctiServer, callInfo));
        }

        private void ProcessDataMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            switch ((ServerSentMessageSubType)msg.N1)
            {
                case ServerSentMessageSubType.AgentId:
                    // NOTE: AgentID 实际上采用的客户端自行计算的。由于算法与服务器保持了统一，故不采用服务器告知的ID
                    DoOnAgentId(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.Channel:
                    DoOnChannel(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.SipRegistrarList:
                    DoOnSipRegistrarList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.AgentInfo:
                    DoOnAgentInfo(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.TodayWorkInfo:
                    DoOnTodayWorkInfo(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.GroupIdList:
                    DoOnGroupIdList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.GroupNameList:
                    DoOnGroupNameList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.PrivilegeList:
                    DoOnPrivilegeList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.PrivilegeExternList:
                    DoOnPrivilegeExternList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.SignedGroupIdList:
                    DoOnSignedGroupIdList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.WorkingChannel:
                    DoOnWorkingChannel(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.Ring:
                    DoOnRing(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.IvrData:
                    DoOnIvrData(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.CustomString:
                    DoOnCustomString(ctiServer, msg);
                    break;
                default:
                    break;
            }
        }

        private void DoOnAgentInfo(CtiServer _, ServerSentMessage msg)
        {
            var jo = JsonSerializer.Deserialize<AgentInfoJson>(msg.S);

            ResetDisplayName(jo.DisplayName);
            ResetPrivilegeList(jo.Power);
            ResetPrivilegeExternList(jo.PowerExt);
            ResetGroupIdList(jo.GroupIdIdList);
            ResetGroupNameList(jo.GroupIdIdList);
            AppendAllGroupList(jo.AppendedGroupList);
        }

        public event EventHandler OnAllGroupListChanged;

        private readonly List<Group> allGroups = new();
        public IReadOnlyCollection<Group> AllGroups
        {
            get
            {
                IReadOnlyCollection<Group> result;
                lock (this)
                {
                    result = allGroups.ToList();
                }
                return result;
            }
        }

        private void AppendAllGroupList(IList<Tuple<string, string>> appendedGroupList)
        {
            lock (this)
            {
                foreach (var pair in appendedGroupList)
                {
                    var obj = allGroups.SingleOrDefault(x => x.Id == pair.Item1);
                    if (obj != null)
                    {
                        obj.Name = pair.Item2;
                    }
                    else
                    {
                        allGroups.Add(new Group(pair.Item1, pair.Item2));
                    }
                }
            }
            OnAllGroupListChanged?.Invoke(this, EventArgs.Empty);
        }

        public Stats Stats { get; } = new();
        public event EventHandler OnStatsChanged;
        private void DoOnTodayWorkInfo(CtiServer _, ServerSentMessage msg)
        {
            Stats.DailyCallCount = (uint)msg.N2;
            Stats.DailyCallDuration = TimeSpan.FromSeconds(
#pragma warning disable CA1305
                double.Parse(msg.S)
#pragma warning restore CA1305
            );
            OnStatsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnSignedGroupsChanged;
        private void DoOnSignedGroupIdList(CtiServer _, ServerSentMessage msg)
        {
            var signed = Convert.ToBoolean(msg.N2);
            var ids = msg.S.Split(Constants.VerticalBarDelimiter);
            lock (this)
            {
                foreach (var id in ids)
                {
                    var groupObj = groups.FirstOrDefault(m => m.Id == id);
                    if (groupObj == null)
                    {
                        logger.ErrorFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0}\"> 不存在", id);
                    }
                    else
                    {
                        logger.DebugFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0}\" signed={1}>", id, signed);
                        groupObj.IsSigned = signed;
                    }
                }
            }
            OnSignedGroupsChanged?.Invoke(this, EventArgs.Empty);
        }

        private readonly SemaphoreSlim pjSemaphore = new(1);


        public event EventHandler<SipRegistrarListReceivedEventArgs> OnSipRegistrarListReceived;
        public event EventHandler OnSipRegisterStateChanged;
        public event EventHandler<SipCallEventArgs> OnSipCallStateChanged;
        private void DoOnSipRegistrarList(CtiServer connectionInfo, ServerSentMessage msg)
        {
            var connectionIndex = GetConnetionIndex(connectionInfo);
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var val = msg.S.Split(Constants.VerticalBarDelimiter);
            SipRegistrarListReceivedEventArgs evt = new(val);

            SyncFactory.StartNew(async () =>
            {
                await pjSemaphore.WaitAsync();
                try
                {
                    foreach (var addr in evt.Value)
                    {
                        logger.DebugFormat("处理 SipAccount 地址 {0} ...", addr);
                        var uri = $"sip:{WorkerNum}@{addr}";
                        Sip.Account acc;
                        acc = sipAccountCollection.FirstOrDefault(x => x.getInfo().uri == uri);
                        if (acc != null)
                        {
                            logger.DebugFormat("SipAccount 释放已存在帐户 {0} ...", uri);
                            sipAccountCollection.Remove(acc);
                            acc.Dispose();
                        }
                        logger.DebugFormat("SipAccount 新建帐户 {0} ...", uri);
                        using var sipAuthCred = new AuthCredInfo("digest", "*", WorkerNum, 0, "hesong");
                        using var cfg = new AccountConfig { idUri = uri };
                        cfg.regConfig.timeoutSec = 60;
                        cfg.regConfig.retryIntervalSec = 30;
                        cfg.regConfig.randomRetryIntervalSec = 10;
                        cfg.regConfig.firstRetryIntervalSec = 15;
                        cfg.regConfig.registrarUri = $"sip:{addr}";
                        cfg.sipConfig.authCreds.Add(sipAuthCred);
                        acc = new Sip.Account(connectionIndex, _sipConfigArgs.RingerWaveFile);
                        acc.OnRegisterStateChanged += Acc_OnRegisterStateChanged;
                        acc.OnIncomingCall += Acc_OnIncomingCall;
                        acc.OnCallDisconnected += Acc_OnCallStateChanged;
                        acc.OnCallStateChanged += Acc_OnCallStateChanged;
                        acc.create(cfg);
                        sipAccountCollection.Add(acc);
                    }
                    /// reload PJ-SIP data
                    lock (this)
                    {
                        ReloadSipAccountCollection();
                    }
                    /// Fire the event
                    await Task.Run(() => OnSipRegistrarListReceived?.Invoke(this, evt));
                }
                finally
                {
                    pjSemaphore.Release();
                }

            });
        }

        private void ReloadSipAccountCollection()
        {
            sipAccounts.Clear();
            sipAccounts.UnionWith(
                from m in sipAccountCollection
                select new SipAccount(m)
            );
        }

        private void Acc_OnRegisterStateChanged(object sender, EventArgs e)
        {
            SyncFactory.StartNew(async () =>
            {
                await pjSemaphore.WaitAsync();
                try
                {
                    lock (this)
                    {
                        ReloadSipAccountCollection();
                    }
                    /// Fire the event
                    await Task.Run(() => OnSipRegisterStateChanged?.Invoke(this, EventArgs.Empty));
                }
                finally
                {
                    pjSemaphore.Release();
                }
            });
        }

        private void Acc_OnIncomingCall(object sender, Sip.CallEventArgs e)
        {
            SipCall callObj = new(e.Call);

            SyncFactory.StartNew(async () =>
            {
                await pjSemaphore.WaitAsync();
                try
                {
                    // 处理外呼等服务器回呼请求
                    if (offHookSemaphore.CurrentCount < 1)
                    {
                        logger.Debug("SipUA_OnIncomingCall - Client side Offhooking: Answer ...");
                        try
                        {
                            using (var prm = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK })
                            {
                                e.Call.answer(prm);
                            }
                            logger.Debug("SipUA_OnIncomingCall - Client side Offhooking: Answer ok");
                            offHookClientTcs?.TrySetResult(null);
                        }
                        catch (Exception exce)
                        {
                            offHookClientTcs?.TrySetException(exce);
                            throw;
                        }
                    }
                    lock (this)
                    {
                        ReloadSipAccountCollection();
                    }
                    // fire the event
                    await Task.Run(() => OnSipCallStateChanged?.Invoke(this, new(callObj)));
                }
                finally
                {
                    pjSemaphore.Release();
                }
            });
        }

        private void Acc_OnCallStateChanged(object sender, Sip.CallEventArgs e)
        {
            SipCall callObj = new(e.Call);

            SyncFactory.StartNew(async () =>
            {
                await pjSemaphore.WaitAsync();
                try
                {
                    lock (this)
                    {
                        ReloadSipAccountCollection();
                    }
                    await Task.Run(() => OnSipCallStateChanged?.Invoke(this, new(callObj)));

                }
                finally
                {
                    pjSemaphore.Release();
                }
            });

        }

        public event EventHandler<CustomStringReceivedEventArgs> OnCustomStringReceived;
        private void DoOnCustomString(CtiServer ctiServer, ServerSentMessage msg)
        {
            ServerSentCustomString val = new(ctiServer, msg.N2, msg.S);
            CustomStringReceivedEventArgs evt = new(ctiServer, val);
            OnCustomStringReceived?.Invoke(this, evt);
        }

        public event EventHandler<IvrDataReceivedEventArgs> OnIvrDataReceived;
        private void DoOnIvrData(CtiServer ctiServer, ServerSentMessage msg)
        {
            IvrData val = new(ctiServer, msg.N2, msg.S);
            IvrDataReceivedEventArgs evt = new(ctiServer, val);
            OnIvrDataReceived?.Invoke(this, evt);
        }

        public event EventHandler<RingInfoReceivedEventArgs> OnRingInfoReceived;
        private void DoOnRing(CtiServer ctiServer, ServerSentMessage msg)
        {
            WorkingChannelInfo workingChannelInfo = new(ctiServer, msg.N2);
            CallInfo callInfo = new(ctiServer, workingChannelInfo.Channel, msg.S);
            logger.DebugFormat("OnRing - {0}", callInfo);
            lock (this)
            {
                WorkingChannelInfo = workingChannelInfo;
                calls.Remove(callInfo);
                calls.Add(callInfo);
            }
            OnWorkingChannelInfoReceived?.Invoke(this, new(ctiServer, workingChannelInfo));
            OnRingInfoReceived?.Invoke(this, new(ctiServer, callInfo));
        }

        public WorkingChannelInfo WorkingChannelInfo { get; private set; }
        public event EventHandler<WorkingChannelInfoReceivedEventArgs> OnWorkingChannelInfoReceived;
        private void DoOnWorkingChannel(CtiServer ctiServer, ServerSentMessage msg)
        {
            WorkingChannelInfo workingChannelInfo = new(ctiServer, msg.N2, msg.S);
            logger.DebugFormat("OnWorkingChannel - {0}: {1}", ctiServer, workingChannelInfo.Channel);
            lock (this)
            {
                WorkingChannelInfo = workingChannelInfo;
            }
            OnWorkingChannelInfoReceived?.Invoke(this, new(ctiServer, workingChannelInfo));
        }

        private readonly HashSet<Privilege> privilegeCollection = new();
        public IReadOnlyCollection<Privilege> PrivilegeCollection => privilegeCollection;
        public event EventHandler OnPrivilegeCollectionReceived;
        private void DoOnPrivilegeList(CtiServer _, ServerSentMessage msg)
        {
            ResetPrivilegeList(msg.S);
        }

        private void ResetPrivilegeList(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            var it = from s in data.Split(Constants.VerticalBarDelimiter)
                     select (Privilege)Enum.Parse(typeof(Privilege), s);
            ResetPrivilegeList(it);
        }

        private void ResetPrivilegeList(IEnumerable<Privilege> data)
        {
            lock (this)
            {
                privilegeCollection.UnionWith(data);
            }
            OnPrivilegeCollectionReceived?.Invoke(this, EventArgs.Empty);
        }

        private readonly HashSet<int> privilegeExternCollection = new();
        public IReadOnlyCollection<int> PrivilegeExternCollection => privilegeExternCollection;
        public event EventHandler OnPrivilegeExternCollectionReceived;
        private void DoOnPrivilegeExternList(CtiServer _, ServerSentMessage msg)
        {
            ResetPrivilegeExternList(msg.S);
        }

        private void ResetPrivilegeExternList(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            var values = from s in data.Split(Constants.VerticalBarDelimiter)
#pragma warning disable CA1305
                         select int.Parse(s);
#pragma warning restore CA1305
            lock (this)
            {
                privilegeExternCollection.UnionWith(values);
            }
            OnPrivilegeExternCollectionReceived?.Invoke(this, EventArgs.Empty);
        }

        private readonly List<Group> groups = new();
        public IReadOnlyList<Group> Groups
        {
            get
            {
                IReadOnlyList<Group> result;
                lock (this)
                {
                    result = groups.ToList();
                }
                return result;
            }
        }
        public event EventHandler OnGroupReceived;

        private List<Group> cachedGroups = new();

        private void DoOnGroupIdList(CtiServer _, ServerSentMessage msg)
        {
            ResetGroupIdList(msg.S);
        }

        private void ResetGroupIdList(IEnumerable<string> data)
        {
            var itGroups = from id in data select new Group(id);
            lock (this)
            {
                groups.Clear();
                groups.AddRange(itGroups);
            }
        }

        private void ResetGroupIdList(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            var idArray = data.Split(Constants.VerticalBarDelimiter);
            ResetGroupIdList(idArray);
        }

        private void DoOnGroupNameList(CtiServer _, ServerSentMessage msg)
        {
            ResetGroupNameList(msg.S);
        }
        private void ResetGroupNameList(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            var nameArray = data.Split(Constants.VerticalBarDelimiter);
            ResetGroupNameList(nameArray);

        }

        private void ResetGroupNameList(IEnumerable<string> data)
        {
            bool isRestore = false;
            lock (this)
            {
                foreach (var pair in groups.Zip(data, (first, second) => (first, second)))
                {
                    pair.first.Name = pair.second;
                }
                if (isEverLostAllConnections)
                {
                    isRestore = true;
                    isEverLostAllConnections = false;
                }
            }
            OnGroupReceived?.Invoke(this, EventArgs.Empty);
            if (isRestore)
            {
                var groupIdList = (
                    from m in cachedGroups
                    where (
                        m.IsSigned
                        && groups.Any(n => n.Id == m.Id && !n.IsSigned)
                    )
                    select m.Id
                ).ToList();
                if (groupIdList.Count > 0)
                {
                    logger.Info("DoOnGroupNameList - 从丢失全部 CTI 节点的连接的情况中恢复，还原之前的技能与状态");
                    AgentStateWorkType stateWorkType = null;
                    if (cachedStateWorkType != null)
                        stateWorkType = cachedStateWorkType.Clone() as AgentStateWorkType;
                    Task.Run(async () =>
                    {
                        logger.Debug("还原技能");
                        await SignInAsync(groupIdList);
                        if (stateWorkType != null)
                        {
                            if (stateWorkType.AgentState == AgentState.Idle)
                            {
                                logger.Debug("还原示闲");
                                await SetIdleAsync();
                            }
                            else if (stateWorkType.AgentState == AgentState.Pause)
                            {
                                logger.DebugFormat("还原示忙 {0}", stateWorkType.WorkType);
                                await SetBusyAsync(stateWorkType.WorkType);
                            }
                        }
                    });
                }
            }
            else
            {
                cachedGroups = groups.ToList();
            }
        }

        public event EventHandler<ChannelAssignedEventArgs> OnChannelAssigned;
        private void DoOnChannel(CtiServer ctiServer, ServerSentMessage msg)
        {
            AgentChannelInfo value = new(ctiServer, msg.N2);
            lock (this)
            {
                AgentChannelInfo = value;
            }
            OnChannelAssigned?.Invoke(this, new(ctiServer, value));
        }

        public event EventHandler<AgentDisplayNameReceivedEventArgs> OnAgentDisplayNameReceived;
        private void DoOnAgentId(CtiServer _, ServerSentMessage msg)
        {
            ResetDisplayName(msg.S);
        }

        private void ResetDisplayName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            lock (this)
            {
                DisplayName = s;
            }
            OnAgentDisplayNameReceived?.Invoke(this, new(s));
        }

        public event EventHandler<ConnectionInfoStateChangedEventArgs> OnConnectionStateChanged;

        private static readonly ConnectionState[] disconntedStates = { ConnectionState.Lost, ConnectionState.Failed, ConnectionState.Closed };
        private static readonly AgentState[] workingStates = { AgentState.Ring, AgentState.Work };

        private bool isEverLostAllConnections;
        private void Conn_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            var conn = sender as Connection;
            var connIdx = connections.IndexOf(conn);
            var ctiServer = ctiServers[connIdx];
            Random rand = new();
            ConnectionInfoStateChangedEventArgs evtStateChanged = new(ctiServer, e.OldState, e.NewState);
            EventArgs evtMainConnChanged = null;
            Action action = null;
            logger.DebugFormat("{0}: {1} --> {2}", conn, e.OldState, e.NewState);
            lock (this)
            {
                if (RunningState == AgentRunningState.Started)
                {
                    if (!isEverLostAllConnections)
                    {
                        if (!connections.Any(x => x.State == ConnectionState.Ok))
                        {
                            isEverLostAllConnections = true;
                            logger.Warn("丢失了所有 CTI 服务节点的连接!");
                        }
                    }
                }
                //////////
                // 断线处理
                if (disconntedStates.Contains(e.NewState))
                {
                    bool isCancelled = false;
                    var isMain = connIdx == MainConnectionIndex;
                    if (isMain)
                    {
                        if (RunningState != AgentRunningState.Started)
                        {
                            // 处于启动过程中！
                            logger.WarnFormat(
                                "主服务节点 [{0}]({1}) 连接断开 (NewState={2} RunningState={3}) 放弃重连.",
                                connIdx, ctiServers[connIdx], e.NewState, RunningState
                            );
                        }
                        else
                        {
                            // 其它连接还有连接上了的吗?
                            var indices = (
                                from vi in connections.Select((value, index) => (value, index))
                                where vi.index != MainConnectionIndex && vi.value.State == ConnectionState.Ok
                                select vi.index
                            ).ToList();
                            if (indices.Count > 0)
                            {
                                // 有的选,但是如果在工作状态,不能变!
                                if (workingStates.Contains(AgentState))
                                {
                                    logger.WarnFormat("主服务节点 [{0}]({1}) 连接断开. 但是由于处于工作状态, 将继续使用该主节点并发起重连", connIdx, ctiServer);
                                }
                                else
                                {
                                    MainConnectionIndex = indices[rand.Next(indices.Count)];
                                    logger.WarnFormat(
                                        "主服务节点 [{0}]({1}) 连接断开. 切换主节点到 [{2}]({3})",
                                        connIdx, ctiServer, MainConnectionIndex, MainConnectionInfo
                                    );
                                }
                            }
                            else
                            {
                                // 没得选
                                switch (e.NewState)
                                {
                                    case ConnectionState.Closed:
                                        {
                                            // 被主动关闭
                                            ConnectionState[] exceptStats = { ConnectionState.Closed, ConnectionState.Closing };
                                            var indices2 = (
                                                from vi in connections.Select((value, index) => (value, index))
                                                where vi.index != MainConnectionIndex && !exceptStats.Contains(vi.value.State)
                                                select vi.index
                                            ).ToList();
                                            if (indices2.Count > 0)
                                            {
                                                MainConnectionIndex = indices2[rand.Next(indices2.Count)];
                                                logger.WarnFormat(
                                                    "主服务节点 [{0}]({1}) 连接被关闭. 且找不到其它活动连接, 但仍切换主节点到 [{2}]({3})",
                                                    connIdx, ctiServer, MainConnectionIndex, MainConnectionInfo
                                                );
                                            }
                                            else
                                            {
                                                logger.WarnFormat(
                                                    "主服务节点 [{0}]({1}) 连接被关闭. 且找不到其它未被主动关闭的连接, 将放弃重连",
                                                    connIdx, ctiServer
                                                );
                                                isCancelled = true;
                                            }
                                        }
                                        break;
                                    default:
                                        logger.WarnFormat(
                                            "主服务节点 [{0}]({1}) 连接断开. 但是由于找不到其它可用服务节点连接, 将继续使用该主节点并发起重连",
                                            connIdx, ctiServer
                                        );
                                        break;
                                }
                            }
                            ///
                            if (!isCancelled)
                            {
                                if (MainConnectionIndex != connIdx)
                                {
                                    action = new Action(async () =>
                                    {
                                        logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接 ... ", connIdx, ctiServer);
                                        try
                                        {
                                            await conn.OpenAsync(ctiServer.Host, ctiServer.Port, WorkerNum, password, flag: 0);
                                            logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接成功", connIdx, ctiServer);
                                        }
                                        catch (ConnectionException ex)
                                        {
                                            logger.ErrorFormat("从服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, ctiServer, $"{ex.GetType()} {ex.Message}");
                                        };
                                    });
                                    // 需要抛出切换新的主服务节事件
                                    evtMainConnChanged = EventArgs.Empty;
                                }
                                else
                                {
                                    action = new Action(async () =>
                                    {
                                        logger.InfoFormat("主服务节点 [{0}]({1}) 重新连接 ... ", connIdx, ctiServer);
                                        try
                                        {
                                            await conn.OpenAsync(ctiServer.Host, ctiServer.Port, WorkerNum, password, flag: 1);
                                            logger.InfoFormat("主服务节点 [{0}]({1}) 重新连接成功", connIdx, ctiServer);
                                        }
                                        catch (ConnectionException ex)
                                        {
                                            logger.ErrorFormat("主服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, ctiServer, $"{ex.GetType()} {ex.Message}");
                                        };
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (RunningState != AgentRunningState.Started)
                        {
                            logger.WarnFormat(
                                "从服务节点 [{0}]({1}) 连接断开 ({2} {3}). 放弃重连.",
                                connIdx, ctiServer, e.NewState, RunningState
                            );
                        }
                        else if (e.NewState == ConnectionState.Closed)
                        {
                            logger.WarnFormat("从服务节点 [{0}]({1}) 连接被关闭. 将放弃重连", connIdx, ctiServer);
                        }
                        else
                        {
                            logger.WarnFormat("从服务节点 [{0}]({1}) 连接丢失. 将发起重连", connIdx, ctiServer);
                            action = new Action(async () =>
                            {
                                logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接 ... ", connIdx, ctiServer);
                                try
                                {
                                    await conn.OpenAsync(ctiServer.Host, ctiServer.Port, WorkerNum, password, flag: 0);
                                    logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接成功", connIdx, ctiServer);
                                }
                                catch (ConnectionException ex)
                                {
                                    logger.ErrorFormat("从服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, ctiServer, $"{ex.GetType()} {ex.Message}");
                                }
                            });
                        }
                    }
                }
            }
            // 抛出连接状态变化事件
            OnConnectionStateChanged?.Invoke(this, evtStateChanged);
            // 抛出切换新的主服务节事件
            if (evtMainConnChanged != null)
            {
                OnMainConnectionChanged?.Invoke(this, evtMainConnChanged);
            }
            // 执行重连
            if (action != null)
            {
                Task.Run(action);
            }
        }

        public async Task StartUpAsync(IEnumerable<string> addresses, string workerNum, string password)
        {
            foreach (var addr in addresses)
            {
                if (string.IsNullOrWhiteSpace(addr))
                {
                    throw new ArgumentException("地址不得为空", nameof(addresses));
                }
                var c = (from s in addresses where s == addr select s).Count();
                if (c > 1)
                {
                    throw new ArgumentException("重复的地址", nameof(addresses));
                }
            }

            logger.InfoFormat(
                "StartUpAsync - workerNum=\"{0}\", addresses=\"{1}\"",
                workerNum, string.Join(", ", addresses)
            );

            using (requestGuard.TryEnter())
            {
                connections.Clear();
                ctiServers.Clear();

                AgentRunningState savedRunningState;
                IEnumerable<int> minorIndices;
                Random rand = new();
                lock (this)
                {
                    if (RunningState != AgentRunningState.Stopped)
                    {
                        throw new InvalidOperationException($"Invalid state: {RunningState}");
                    }
                    savedRunningState = RunningState;
                    RunningState = AgentRunningState.Starting;
                }

                foreach (var s in addresses)
                {
                    CtiServer ctiServer = new(s);
                    Connection conn = new(LocalPort, LocalAddress);
                    conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                    conn.OnServerSentEvent += Conn_OnServerSend;
                    connections.Add(conn);
                    ctiServers.Add(ctiServer);
                }
                WorkerNum = workerNum;
                this.password = password;

                // 首先，随机选择一个主节点
                // 然后尝试连接主节点。如果主节点无法连接，换别的主节点。如果循环一次之后全部无法连接，就认为连接失败。
                // 连接主节点成功后尝试连接其它节点，无论是否成功。
                Exception lastestException = null;
                var unusedIndices = Enumerable.Range(0, ctiServers.Count).ToList();
                try
                {
                    for (int i = 0; unusedIndices.Count > 0 && MainConnectionIndex < 0; ++i)
                    {
                        // 从没有用过的列表里面，随机选择一个主节点
                        var index = rand.Next(0, unusedIndices.Count);
                        MainConnectionIndex = unusedIndices[index];
                        unusedIndices.RemoveAt(index);
                        logger.InfoFormat("StartUpAsync - [0] 连接主服务节点 [{1}]({2}|{3}) ... ", i, MainConnectionIndex, MainConnectionInfo.Host, MainConnectionInfo.Port);
                        try
                        {
                            await MainConnection.OpenAsync(
                                MainConnectionInfo.Host, MainConnectionInfo.Port,
                                workerNum, password,
                                flag: 1
                            );
                        }
                        catch (Exception exception)
                        {
                            MainConnectionIndex = -1;
                            lastestException = exception;
                            if (exception is not ConnectionException)
                            {
                                throw;
                            }
                        }
                    }
                    if (MainConnectionIndex >= 0)
                    {
                        lock (this)
                        {
                            RunningState = AgentRunningState.Started;
                        }
                    }
                    else if (lastestException != null)
                    {
                        throw lastestException;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                catch
                {
                    lock (this)
                    {
                        RunningState = savedRunningState;
                    }
                    throw;
                }
                // 然后其他节点
                minorIndices = from i in Enumerable.Range(0, ctiServers.Count)
                               where i != MainConnectionIndex
                               select i;
                foreach (var i in minorIndices)
                {
                    var conn = connections[i];
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    Task.Run(async () =>
                    {
                        await conn.OpenAsync(
                           ctiServers[i].Host, ctiServers[i].Port,
                           workerNum, password,
                           flag: 0
                       );
                    });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                }
            }
        }

        public async Task ShutDownAsync(bool force = false)
        {
            using (requestGuard.TryEnter())
            {
                AgentRunningState savedRunningState;
                var graceful = !force;
                bool isMainNotConnected;
                lock (this)
                {
                    if (RunningState != AgentRunningState.Started)
                    {
                        throw new InvalidOperationException($"Invalid state: {RunningState}");
                    }
                    savedRunningState = RunningState;
                    RunningState = AgentRunningState.Stopping;
                    isMainNotConnected = !MainConnection.Connected;
                }
                try
                {
                    // 首先，主节点
                    if (isMainNotConnected)
                    {
                        logger.WarnFormat("主节点连接 {0} 已经关闭！", MainConnection, graceful);
                    }
                    else
                    {
                        logger.DebugFormat("关闭主节点连接 {0} graceful={1}...", MainConnection, graceful);
                        await MainConnection.CloseAsync(graceful, flag: 1);
                        logger.DebugFormat("关闭主节点连接 {0} graceful={1} 完毕.", MainConnection, graceful);
                    }

                    // 然后其他节点
                    var itConnObj =
                        from x in connections.Select((value, index) => (value, index))
                        where x.index != MainConnectionIndex
                        select x.value;
                    await Task.WhenAll(
                        from conn in itConnObj
                        select Task.Run(async () =>
                        {
                            if (graceful)
                            {
                                try
                                {
                                    logger.DebugFormat("关闭从节点连接 {0} graceful={1}...", conn, graceful);
                                    await conn.CloseAsync(graceful, flag: 0);
                                    logger.DebugFormat("关闭从节点连接 {0} graceful={1} 完毕.", conn, graceful);
                                }
                                catch (Exception exce)
                                {
                                    switch (exce)
                                    {
                                        case DisconnectionTimeoutException:
                                        case ErrorResponse:
                                        case InvalidOperationException:
                                            logger.DebugFormat("关闭从节点连接 {0} 失败, 将强行关闭: {1}", conn, exce);
                                            await conn.CloseAsync(graceful: false, flag: 0);
                                            break;
                                        default:
                                            throw;
                                    }
                                }
                            }
                            else
                            {
                                await conn.CloseAsync(graceful: false, flag: 0);
                            }
                        })
                    );
                    lock (this)
                    {
                        RunningState = AgentRunningState.Stopped;
                    }
                }
                catch
                {
                    lock (this)
                    {
                        RunningState = savedRunningState;
                    }
                    throw;
                }

                lock (this)
                {
                    DisposePjSipAccounts();
                    DisposeConnections();
                }
            }
        }

        private void DisposeConnections()
        {
            foreach (var conn in connections)
            {
                logger.DebugFormat("Dispose {0}", conn);
                conn.Dispose();
            }
            connections.Clear();
            ctiServers.Clear();
        }

        private void DisposePjSipAccounts()
        {
            foreach (var acc in sipAccountCollection)
            {
                logger.DebugFormat("Dispose {0}", acc);
                acc.OnIncomingCall -= Acc_OnIncomingCall;
                acc.OnRegisterStateChanged -= Acc_OnRegisterStateChanged;
                acc.OnCallDisconnected -= Acc_OnCallStateChanged;
                acc.OnCallStateChanged -= Acc_OnCallStateChanged;
                acc.Dispose();
            }
            sipAccountCollection.Clear();
            sipAccounts.Clear();
        }

        public bool IsRequesting => requestGuard.IsEntered;

        public async Task<ServerSentMessage> RequestAsync(int connectionIndex, AgentRequestMessage args, int timeout = Connection.DefaultRequestTimeoutMilliseconds)
        {
            using (requestGuard.TryEnter())
            {
                var conn = connections[connectionIndex];
                return await conn.RequestAsync(args, timeout);
            }
        }

        public async Task<ServerSentMessage> RequestAsync(AgentRequestMessage args, int timeout = Connection.DefaultRequestTimeoutMilliseconds)
        {
            using (requestGuard.TryEnter())
            {
                return await MainConnection.RequestAsync(args, timeout);
            }
        }

        public async Task SignInAsync()
        {
            await SignInAsync(Array.Empty<string>());
        }

        public async Task SignInAsync(string id)
        {
            await SignInAsync(new string[] { id });
        }

        public async Task SignInAsync(IEnumerable<string> ids)
        {
            using (requestGuard.TryEnter())
            {
                var s = string.Join("|", ids);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_SIGNON, s);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task SignOutAsync()
        {
            await SignOutAsync(Array.Empty<string>());
        }

        public async Task SignOutAsync(string id)
        {
            await SignOutAsync(new string[] { id });
        }

        public async Task SignOutAsync(IEnumerable<string> ids)
        {
            using (requestGuard.TryEnter())
            {
                var s = string.Join("|", ids);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_SIGNOFF, s);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task SetIdleAsync()
        {
            using (requestGuard.TryEnter())
            {
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_CONTINUE);
                await MainConnection.RequestAsync(req);
            }
        }

        private static readonly WorkType[] availableSetBusyWorkTypes = {
            WorkType.PauseBusy,
            WorkType.PauseLeave,
            WorkType.PauseTyping,
            WorkType.PauseForce,
            WorkType.PauseDisconnect,
            WorkType.PauseSnooze,
            WorkType.PauseDinner,
            WorkType.PauseTrain,
        };

        public async Task SetBusyAsync(WorkType workType = WorkType.PauseBusy)
        {
            if (!availableSetBusyWorkTypes.Contains(workType))
                throw new ArgumentOutOfRangeException(nameof(workType), $"Invalid work type {workType}");
            using (requestGuard.TryEnter())
            {
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_PAUSE, (int)workType);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task DialAsync(string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = MainConnection;
                var s = $"{calledTelNum}|{callingTelNum}|{channelGroup}|{option}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_DIAL, 0, s);
                await OffHookAsync(conn);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferAsync(CtiServer connectionInfo, int channel, string groupId, string workerNum = "", string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var s = $"{workerNum}|{groupId}|{customString}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_TRANSFER, channel, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferAsync(int connectionIndex, int channel, string groupId, string workerNum = "", string customString = "")
        {
            var connectionInfo = ctiServers[connectionIndex];
            await XferAsync(connectionInfo, channel, groupId, workerNum, customString);
        }

        public async Task XferAsync(CallInfo callInfo, string groupId, string workerNum = "", string customString = "")
        {
            await XferAsync(callInfo.CtiServer, callInfo.Channel, groupId, workerNum, customString);
        }

        public async Task XferAsync(string groupId, string workerNum = "", string customString = "")
        {
            var connectionInfo = WorkingChannelInfo.CtiServer;
            await XferAsync(connectionInfo, -1, groupId, workerNum, customString);
        }

        public async Task XferConsultAsync(string groupId, string workerNum = "", string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(WorkingChannelInfo.CtiServer);
                var s = $"{workerNum}|{groupId}|{customString}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_CONSULT, -1, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferExtAsync(CtiServer connectionInfo, int channel, string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var s = $"{calledTelNum}|{callingTelNum}|{channelGroup}|{option}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_TRANSFER_EX, channel, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferExtAsync(int connectionIndex, int channel, string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            var connectionInfo = ctiServers[connectionIndex];
            await XferExtAsync(connectionInfo, channel, calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task XferExtAsync(CallInfo callInfo, string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            var connectionInfo = callInfo.CtiServer;
            var channel = callInfo.Channel;
            await XferExtAsync(connectionInfo, channel, calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task XferExtAsync(string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            var connInfo = WorkingChannelInfo.CtiServer;
            await XferExtAsync(connInfo, -1, calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task XferExtConsultAsync(string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(WorkingChannelInfo.CtiServer);
                var s = $"{calledTelNum}|{callingTelNum}|{channelGroup}|{option}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_CONSULT_EX, -1, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task CallIvrAsync(CtiServer connectionInfo, int channel, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var s = $"{ivrId}|{(int)invokeType}|{customString}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_CALLSUBFLOW, channel, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task CallIvrAsync(int connectionIndex, int channel, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connectionInfo = ctiServers[connectionIndex];
            await CallIvrAsync(connectionInfo, channel, ivrId, invokeType, customString);
        }

        public async Task CallIvrAsync(CallInfo callInfo, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connectionInfo = callInfo.CtiServer;
            var channel = callInfo.Channel;
            await CallIvrAsync(connectionInfo, channel, ivrId, invokeType, customString);
        }

        public async Task CallIvrAsync(string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connInfo = WorkingChannelInfo.CtiServer;
            await CallIvrAsync(connInfo, -1, ivrId, invokeType, customString);
        }

        public async Task HoldAsync()
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(WorkingChannelInfo.CtiServer);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_HOLD);
                await conn.RequestAsync(req);
            }
        }

        public async Task UnHoldAsync(CtiServer connectionInfo, int channel)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_RETRIEVE, channel);
                await conn.RequestAsync(req);
            }
        }

        public async Task UnHoldAsync(int connectionIndex, int channel)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await UnHoldAsync(connectionInfo, channel);
        }

        public async Task UnHoldAsync(CallInfo callInfo)
        {
            await UnHoldAsync(callInfo.CtiServer, callInfo.Channel);
        }

        public async Task UnHoldAsync()
        {
            var callInfo = HeldCalls.FirstOrDefault();
            if (callInfo == null)
            {
                throw new InvalidOperationException("没有任何被保持的呼叫，无法执行取消保持操作");
            }
            await UnHoldAsync(callInfo);
        }

        public async Task BreakAsync(int channel = -1, string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(WorkingChannelInfo.CtiServer);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_BREAK_SESS, channel, customString);
                await conn.RequestAsync(req);
            }
        }


        public async Task AnswerAsync()
        {
            logger.Debug("Answer");

            if (offHookSemaphore.CurrentCount < 1)
            {
                throw new InvalidOperationException("Can not perform a SIP answer when in a Off-Hook requestion");
            }

            await SyncFactory.StartNew(async () =>
            {
                await pjSemaphore.WaitAsync();
                try
                {
                    var iterIncomingCalls =
                        from c in sipAccountCollection.SelectMany(x => x.Calls)
                        let ci = c.getInfo()
                        where ci.state == pjsip_inv_state.PJSIP_INV_STATE_INCOMING
                        select c;
                    using CallOpParam cop = new() { statusCode = pjsip_status_code.PJSIP_SC_OK };
                    foreach (var call in iterIncomingCalls)
                    {
                        call.answer(cop);
                    }
                }
                finally
                {
                    pjSemaphore.Release();
                }
            });
        }

        public async Task HangupAsync()
        {
            logger.Debug("Hangup");

            if (offHookSemaphore.CurrentCount < 1)
            {
                throw new InvalidOperationException("Can not perform a SIP hangup when in a Off-Hook requestion");
            }

            await SyncFactory.StartNew(async () =>
            {
                await pjSemaphore.WaitAsync();
                try
                {
                    SipEndpoint.hangupAllCalls();
                    lock (this)
                    {
                        // 主动挂机了， Call List 也来一个清空动作
                        calls.Clear();
                    }
                }
                finally
                {
                    pjSemaphore.Release();
                }
            });
        }

        private readonly SemaphoreSlim offHookSemaphore = new(1);
        private TaskCompletionSource<object> offHookServerTcs;
        private TaskCompletionSource<object> offHookClientTcs;

        public const int DefaultOffHookTimeoutMilliSeconds = 5000;

        internal async Task OffHookAsync(Connection connection, int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            await offHookSemaphore.WaitAsync();
            try
            {
                bool isContinue;
                lock (this)
                {
                    isContinue = TeleState != TeleState.OffHook;
                }
                if (!isContinue)
                {
                    logger.DebugFormat("OffHook - 服务节点 [{0}] 上该座席已经处于 OffHook 状态，无需回呼请求", connection);
                    return;
                }
                logger.DebugFormat("OffHook - 服务节点 [{0}] 回呼请求开始", connection);
                offHookServerTcs = new();
                offHookClientTcs = new();
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_OFFHOOK);
                await connection.RequestAsync(req);
                var timeoutTask = Task.Delay(millisecondsTimeout);
                Task[] waitingTasks = { offHookServerTcs.Task, offHookClientTcs.Task };
                var completeTask = Task.WhenAll(offHookServerTcs.Task, offHookClientTcs.Task);
                var task = await Task.WhenAny(completeTask, timeoutTask);
                if (task == timeoutTask)
                {
                    logger.ErrorFormat("OffHook - 服务节点 [{0}] 回呼超时", connection);
                    offHookServerTcs.TrySetCanceled();
                    offHookClientTcs.TrySetCanceled();
                    throw new TimeoutException();
                }
                await task;
                if (waitingTasks.Any(x => x.Status != TaskStatus.RanToCompletion))
                {
                    logger.ErrorFormat("OffHook - 服务节点 [{0}] 回呼失败", connection);
                    throw new InvalidOperationException();
                }
                logger.DebugFormat("OffHook - 服务节点 [{0}] 回呼成功", connection);
            }
            finally
            {
                offHookSemaphore.Release();
            }
        }

        internal async Task OffHookAsync(CtiServer connectionInfo, int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            var connection = GetConnection(connectionInfo);
            await OffHookAsync(connection, millisecondsTimeout);
        }

        internal async Task OffHookAsync(int connectionIndex, int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            var connection = GetConnection(connectionIndex);
            await OffHookAsync(connection, millisecondsTimeout);
        }

        internal async Task OffHookAsync(int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            var connectionInfo = MainConnectionInfo;
            await OffHookAsync(connectionInfo, millisecondsTimeout);
        }

        public async Task DequeueAsync(QueueInfo queueInfo)
        {
            var connectionIndex = GetConnetionIndex(queueInfo.CtiServer);
            await DequeueAsync(connectionIndex, queueInfo.Channel);
        }

        public async Task DequeueAsync(int connectionIndex, int channel)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionIndex);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_GETQUEUE, channel);
                await conn.RequestAsync(req);
            }
        }

        public async Task MonitorAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_LISTEN, -1, workerNum);
                await OffHookAsync(conn);
                await conn.RequestAsync(req);
            }
        }


        public async Task MonitorAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await MonitorAsync(connectionInfo, workerNum);
        }

        public async Task InterceptAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_INTERCEPT, -1, workerNum);
                await conn.RequestAsync(req);
            }
        }

        public async Task InterceptAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await InterceptAsync(connectionInfo, workerNum);
        }

        public async Task UnMonitorAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_STOPLISTEN, workerNum);
                await conn.RequestAsync(req);
            }
        }

        public async Task UnMonitorAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await UnMonitorAsync(connectionInfo, workerNum);
        }

        public async Task InterruptAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_FORCEINSERT, -1, workerNum);
                await OffHookAsync(conn);
                await conn.RequestAsync(req);
            }
        }

        public async Task InterruptAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await InterruptAsync(connectionInfo, workerNum);
        }

        public async Task HangupAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_FORCEHANGUP, workerNum);
                await conn.RequestAsync(req);
            }
        }

        public async Task HangupAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await HangupAsync(connectionInfo, workerNum);
        }

        public async Task SignOutAsync(string workerNum, string groupId)
        {
            using (requestGuard.TryEnter())
            {
                var s = $"{workerNum}|{groupId}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_FORCESIGNOFF, -1, s);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task SetIdleAsync(string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_FORCEIDLE, -1, workerNum);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task SetBusyAsync(string workerNum, WorkType workType = WorkType.PauseBusy)
        {
            using (requestGuard.TryEnter())
            {
                var s = $"{workerNum}|{(int)workType}";
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_FORCEPAUSE, -1, s);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task KickOutAsync(string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_KICKOUT, -1, workerNum);
                await MainConnection.RequestAsync(req);
            }
        }

        public async Task TakenAwayAsync(int index)
        {
            using (requestGuard.TryEnter())
            {
                Connection connObj;
                // 先修改 index
                lock (this)
                {
                    if (MainConnectionIndex < 0)
                    {
                        throw new InvalidOperationException();
                    }
                    if (index < 0 || index >= connections.Count || index == MainConnectionIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, "");
                    }
                    if (connections[index].State != ConnectionState.Ok)
                    {
                        throw new InvalidOperationException($"Invalid state: {connections[index].State}");
                    }
                    connObj = MainConnection;
                    MainConnectionIndex = index;
                    logger.InfoFormat("切换主服务节点到 {0}", MainConnection);
                }
                OnMainConnectionChanged?.Invoke(this, EventArgs.Empty);
                // 再通知原来的主，无论能否通知成功
                AgentRequestMessage req = new(MessageType.REMOTE_MSG_TAKENAWAY);
                await connObj.RequestAsync(req);
            }
        }

        private readonly HashSet<Sip.Account> sipAccountCollection = new();
        private readonly HashSet<SipAccount> sipAccounts = new();
        public IReadOnlyCollection<SipAccount> SipAccounts => sipAccounts;

    }
}
