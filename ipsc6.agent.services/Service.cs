using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[assembly: InternalsVisibleTo("ipsc6.agent.wpfapp")]
namespace ipsc6.agent.services
{
#pragma warning disable VSTHRD200
    public class Service : IDisposable
    {
        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DestroyAgent();
                }
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Service()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Demo methods
        public string Echo(string message)
        {
            OnEchoTriggered?.Invoke(this, new Events.EchoTriggeredEventArgs { Message = message });
            return message;
        }

        public async Task<string> EchoWithDelay(string message, int milliseconds)
        {
            OnEchoTriggered?.Invoke(this, new Events.EchoTriggeredEventArgs { Message = message });
            await Task.Delay(milliseconds);
            return message;
        }

        public event EventHandler OnEchoTriggered;

        public static void ThrowAnException(string message)
        {
            throw new InvalidOperationException(message);
        }
        #endregion

        #region Create/Release

        private client.Agent agent;
        private bool disposedValue;
        private readonly Models.Model Model = new();

        internal static void Initial()
        {
            client.Agent.Initial();
        }

        internal static void Release()
        {
            client.Agent.Release();
        }

        private void DestroyAgent()
        {
            if (agent == null)
            {
                return;
            }

            agent.Dispose();
            agent = null;
        }

        internal void CreateAgent(IEnumerable<string> addresses, ushort localPort, string localAddress)
        {
            if (agent != null)
            {
                DestroyAgent();
            }
            agent = new client.Agent(addresses, localPort, localAddress);

            agent.OnAgentDisplayNameReceived += Agent_OnAgentDisplayNameReceived;

            agent.OnConnectionStateChanged += Agent_OnConnectionStateChanged;
            agent.OnMainConnectionChanged += Agent_OnMainConnectionChanged;

            agent.OnAgentStateChanged += Agent_OnAgentStateChanged;
            agent.OnTeleStateChanged += Agent_OnTeleStateChanged;

            agent.OnGroupReceived += Agent_OnGroupReceived;
            agent.OnSignedGroupsChanged += Agent_OnSignedGroupsChanged;

            agent.OnSipRegistrarListReceived += Agent_OnSipRegistrarListReceived;
            agent.OnSipRegisterStateChanged += Agent_OnSipRegisterStateChanged;
            agent.OnSipCallStateChanged += Agent_OnSipCallStateChanged;

            agent.OnHoldInfoReceived += Agent_OnHoldInfoReceived;
            agent.OnRingInfoReceived += Agent_OnRingInfoReceived;

            agent.OnQueueInfoReceived += Agent_OnQueueInfoReceived;

            agent.OnStatsChanged += Agent_OnStatsChanged;

            ReloadCtiServers();
        }
        #endregion

        #region status

        public event EventHandler OnLoginCompleted;

        private void Agent_OnAgentDisplayNameReceived(object sender, client.AgentDisplayNameReceivedEventArgs e)
        {
            Model.DisplayName = agent.DisplayName;
            Model.WorkerNum = agent.WorkerNum;
            OnLoginCompleted?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<Events.StatusChangedEventArgs> OnStatusChanged;

        private void Agent_OnAgentStateChanged(object sender, client.AgentStateChangedEventArgs e)
        {
            lock (Model)
            {
                Model.State = e.NewState.AgentState;
                Model.WorkType = e.NewState.WorkType;
                ReloadCalls();
            }
            OnStatusChanged?.Invoke(this, new Events.StatusChangedEventArgs
            {
                OldState = e.OldState.AgentState,
                OldWorkType = e.OldState.WorkType,
                NewState = e.NewState.AgentState,
                NewWorkType = e.NewState.WorkType,
            });
        }


        public event EventHandler<Events.TeleStateChangedEventArgs> OnTeleStateChanged;

        private void Agent_OnTeleStateChanged(object sender, client.TeleStateChangedEventArgs e)
        {
            Model.TeleState = e.NewState;
            ReloadCalls();
            OnTeleStateChanged?.Invoke(this, new Events.TeleStateChangedEventArgs
            {
                OldState = e.OldState,
                NewState = e.NewState,
            });
        }

        internal async Task LogInAsync(string workerNum, string password)
        {
            await agent.StartUpAsync(workerNum, password);
        }

        public IReadOnlyList<string> GetWorkerNum()
        {
            return new List<string> { Model.WorkerNum, Model.DisplayName };
        }

        public IReadOnlyList<int> GetStatus()
        {
            return new List<int> { (int)Model.State, (int)Model.WorkType };
        }

        public async Task SetBusy(client.WorkType workType = client.WorkType.PauseBusy)
        {
            await agent.SetBusyAsync(workType);
        }

        public async Task SetIdle()
        {
            await agent.SetIdleAsync();
        }

        public Models.Model GetModel()
        {
#pragma warning disable SYSLIB0011
            using MemoryStream ms = new();
            BinaryFormatter formatter = new();
            lock (Model)
            {
                formatter.Serialize(ms, Model);
            }
            ms.Position = 0;
            return (Models.Model)formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
        }
        #endregion

        #region Connection
        private void Agent_OnConnectionStateChanged(object sender, client.ConnectionInfoStateChangedEventArgs e)
        {
            ReloadCtiServers();
            OnCtiConnectionStateChanged?.Invoke(this, new Events.CtiConnectionStateChangedEventArgs
            {
                CtiIndex = agent.GetConnetionIndex(e.ConnectionInfo),
                OldState = e.OldState,
                NewState = e.NewState,
            });
        }

        private void Agent_OnMainConnectionChanged(object sender, EventArgs e)
        {
            ReloadCtiServers();
            OnMainCtiConnectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnMainCtiConnectionChanged;
        public event EventHandler<Events.CtiConnectionStateChangedEventArgs> OnCtiConnectionStateChanged;

        private void ReloadCtiServers()
        {
            lock (Model)
            {
                Model.CtiServers = (
                    from t in agent.CtiServers.Select((value, index) => (index, value))
                    select new Models.CtiServer
                    {
                        Host = t.value.Host,
                        Port = t.value.Port,
                        IsMain = t.index == agent.MainConnectionIndex,
                        State = agent.GetConnectionState(t.index),
                    }
                ).ToList();
            }
        }

        public IReadOnlyCollection<Models.CtiServer> GetCtiServers()
        {
            return GetModel().CtiServers;
        }

        #endregion

        #region AgentGroup

        public event EventHandler OnSignedGroupsChanged;

        private void Agent_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            ReloadGroups();
        }

        private void ReloadGroups()
        {
            lock (Model)
            {
                Model.Groups = (
                    from x in agent.Groups
                    select new Models.Group { Id = x.Id, Name = x.Name, IsSigned = x.IsSigned }
                ).ToList();
            }
            OnSignedGroupsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Agent_OnGroupReceived(object sender, EventArgs e)
        {
            ReloadGroups();
        }

        public async Task SignGroups(bool isSignIn = true)
        {
            if (isSignIn)
            {
                await agent.SignInAsync();
            }
            else
            {
                await agent.SignOutAsync();
            }
        }

        public async Task SignGroup(string id, bool isSignIn = true)
        {
            if (isSignIn)
            {
                await agent.SignInAsync(id);
            }
            else
            {
                await agent.SignOutAsync(id);
            }
        }

        public async Task SignGroups(IEnumerable<string> ids, bool isSignIn = true)
        {
            if (isSignIn)
            {
                await agent.SignInAsync(ids);
            }
            else
            {
                await agent.SignOutAsync(ids);
            }
        }

        public IReadOnlyCollection<Models.Group> GetGroups()
        {
            return GetModel().Groups;
        }

        #endregion

        #region SIP

        private void Agent_OnSipRegistrarListReceived(object sender, client.SipRegistrarListReceivedEventArgs e)
        {
            ReloadSipAccounts();
            OnSipRegisterStateChanged?.Invoke(this, EventArgs.Empty);
        }
        private void Agent_OnSipCallStateChanged(object sender, EventArgs e)
        {
            ReloadSipAccounts();
            ReloadCalls();
            OnSipCallStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Agent_OnSipRegisterStateChanged(object sender, EventArgs e)
        {
            ReloadSipAccounts();
            OnSipRegisterStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnSipRegisterStateChanged;
        public event EventHandler OnSipCallStateChanged;

        private void ReloadSipAccounts()
        {
            lock (Model)
            {
                if (agent != null && agent.SipAccounts != null)
                {
                    Model.SipAccounts = (
                        from a in agent.SipAccounts
                        select new Models.SipAccount
                        {
                            CtiIndex = a.ConnectionIndex,
                            IsValid = a.IsValid,
                            Uri = a.Uri,
                            IsRegisterActive = a.IsRegisterActive,
                            LastRegisterError = a.LastRegisterError,
                            Calls = (
                                from c in a.Calls
                                select new Models.SipCall
                                {
                                    Id = c.Id,
                                    LocalUri = c.LocalUri,
                                    RemoteUri = c.RemoteUri,
                                    State = c.State,
                                }
                            ).ToList(),
                        }
                    ).ToList();
                }
            }
        }

        public IReadOnlyCollection<Models.SipAccount> GetSipAccounts()
        {
            return GetModel().SipAccounts;
        }

        public async Task Answer()
        {
            await agent.AnswerAsync();
        }

        public async Task Hangup()
        {
            await agent.HangupAsync();
        }

        #endregion

        #region Calls

        private void Agent_OnRingInfoReceived(object sender, client.RingInfoReceivedEventArgs e)
        {
            var callInfo = CreateCallInfo(e.Value);
            ReloadCalls();
            OnRingCallReceived?.Invoke(this, new Events.CallInfoEventArgs { Call = callInfo });
        }

        private void Agent_OnHoldInfoReceived(object sender, client.HoldInfoEventArgs e)
        {
            var callInfo = CreateCallInfo(e.Value);
            ReloadCalls();
            OnHeldCallReceived?.Invoke(this, new Events.CallInfoEventArgs { Call = callInfo });
        }

        private Models.CallInfo CreateCallInfo(client.CallInfo value)
        {
            return new Models.CallInfo()
            {
                CtiIndex = agent.GetConnetionIndex(value.CtiServer),
                Channel = value.Channel,
                Direction = value.CallDirection,
                IsHeld = value.IsHeld,
                HoldType = value.HoldType,
                RemoteTelNum = value.RemoteTelNum,
                RemoteLoc = value.RemoteLocation,
                H24CallCount = value.H24CallCount,
                H48CallCount = value.H48CallCount,
                WorkerNum = value.WorkerNum,
                GroupId = value.GroupId,
                IvrPath = value.IvrPath,
                CustomString = value.CustomString,
            };
        }

        private void ReloadCalls()
        {
            lock (Model)
            {
                Model.Calls = (
                    from primeCallInfo in agent.Calls
                    select CreateCallInfo(primeCallInfo)
                ).ToList();
            }
        }

        public event EventHandler<Events.CallInfoEventArgs> OnRingCallReceived;
        public event EventHandler<Events.CallInfoEventArgs> OnHeldCallReceived;

        public IReadOnlyCollection<Models.CallInfo> GetCalls()
        {
            return GetModel().Calls;
        }

        public IReadOnlyCollection<Models.CallInfo> GetHeldCalls()
        {
            return (
                from callInfo in GetModel().Calls
                where callInfo.IsHeld
                select callInfo
            ).ToList();
        }

        public async Task Hold()
        {
            await agent.HoldAsync();
        }

        public async Task UnHold()
        {
            await agent.UnHoldAsync();
        }

        public async Task UnHold(int ctiIndex, int channel)
        {
            await agent.UnHoldAsync(ctiIndex, channel);
        }

        #endregion

        #region QueueInfo

        private void Agent_OnQueueInfoReceived(object sender, client.QueueInfoEventArgs e)
        {
            lock (Model)
            {
                Model.QueueInfos = (
                    from obj in agent.QueueInfos
                    select CreateQueueInfo(obj)
                ).ToList();
            }
            OnQueueInfoEvent?.Invoke(this, new Events.QueueInfoEventArgs { QueueInfo = CreateQueueInfo(e.Value) });
        }

        private Models.QueueInfo CreateQueueInfo(client.QueueInfo obj)
        {
            return new Models.QueueInfo
            {
                CtiIndex = agent.GetConnetionIndex(obj.CtiServer),
                Channel = obj.Channel,
                Id = obj.Id,
                State = obj.EventType,
                Type = obj.Type,
                ProcessId = obj.ProcessId,
                CallingTelNum = obj.CallingTelNum,
                WorkerNum = obj.WorkerNum,
                CustomeString = obj.CustomeString,
                Groups = (
                    from x in obj.Groups
                    select new Models.Group { Id = x.Id, Name = x.Name, IsSigned = x.IsSigned }
                ).ToList(),
            };
        }

        public event EventHandler<Events.QueueInfoEventArgs> OnQueueInfoEvent;

        public IReadOnlyCollection<Models.QueueInfo> GetQueueInfos()
        {
            return GetModel().QueueInfos;
        }

        public async Task Dequeue(int ctiIndex, int channel)
        {
            await agent.DequeueAsync(ctiIndex, channel);
        }

        #endregion

        #region 拨号、转接、咨询
        public async Task Dial(string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            await agent.DialAsync(calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task Xfer(int ctiIndex, int channel, string groupId, string workerNum = "", string customString = "")
        {
            await agent.XferAsync(ctiIndex, channel, groupId, workerNum, customString);
        }

        public async Task Xfer(string groupId, string workerNum = "", string customString = "")
        {
            await agent.XferAsync(groupId, workerNum, customString);
        }

        public async Task XferConsult(string groupId, string workerNum = "", string customString = "")
        {
            await agent.XferConsultAsync(groupId, workerNum, customString);
        }

        public async Task XferExt(int ctiIndex, int channel, string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            await agent.XferExtAsync(ctiIndex, channel, calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task XferExt(string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            await agent.XferExtAsync(calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task XferExtConsult(string calledTelNum, string callingTelNum = "", string channelGroup = "", string option = "")
        {
            await agent.XferExtConsultAsync(calledTelNum, callingTelNum, channelGroup, option);
        }

        public async Task CallIvr(string ivrId, client.IvrInvokeType invokeType = client.IvrInvokeType.Keep, string customString = "")
        {
            await agent.CallIvrAsync(ivrId, invokeType, customString);
        }

        public async Task Monitor(int ctiIndex, string workerNum)
        {
            await agent.MonitorAsync(ctiIndex, workerNum);
        }

        public async Task UnMonitor(int ctiIndex, string workerNum)
        {
            await agent.UnMonitorAsync(ctiIndex, workerNum);
        }

        public async Task Intercept(int ctiIndex, string workerNum)
        {
            await agent.InterceptAsync(ctiIndex, workerNum);
        }

        public async Task Interrupt(int ctiIndex, string workerNum)
        {
            await agent.InterruptAsync(ctiIndex, workerNum);
        }

        public async Task Hangup(int ctiIndex, string workerNum)
        {
            await agent.HangupAsync(ctiIndex, workerNum);
        }

        public async Task SetBusy(string workerNum, client.WorkType workType = client.WorkType.PauseBusy)
        {
            await agent.SetBusyAsync(workerNum, workType);
        }

        public async Task SetIdle(string workerNum)
        {
            await agent.SetIdleAsync(workerNum);
        }

        public async Task SignOut(string workerNum, string groupId)
        {
            await agent.SignOutAsync(workerNum, groupId);
        }

        public async Task KickOut(string workerNum)
        {
            await agent.KickOutAsync(workerNum);
        }
        #endregion

        #region Stats - 通话统计信息

        private void Agent_OnStatsChanged(object sender, EventArgs e)
        {
            var agent = sender as client.Agent;
            lock (this)
            {
                Model.Stats.CopyFromAgent(agent);
            }
            OnStatsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnStatsChanged;

        public Models.Stats GetStats()
        {
            return Model.Stats;
        }

        #endregion

    }
#pragma warning restore VSTHRD200
}
