using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ipsc6.agent.wpfapp.Controllers
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public static class AgentController
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AgentController));

        internal static client.Agent Agent = null;

        internal static client.Agent CreateAgent()
        {
            if (Agent != null) throw new InvalidOperationException("Data member `agent` is not null");

            var cfg = Config.Manager.ConfigurationRoot;
            var options = new Config.Ipsc();
            cfg.GetSection(nameof(Config.Ipsc)).Bind(options);

            logger.InfoFormat(
                "CreateAgent - ServerList: {0}, LocalPort: {1}, LocalAddress: \"{2}\"",
                (options.ServerList == null) ? "<null>" : $"\"{string.Join(",", options.ServerList)}\"",
                options.LocalPort,
                options.LocalAddress
            );

            Agent = new client.Agent(options.ServerList, options.LocalPort, options.LocalAddress);

            Agent.OnConnectionStateChanged += Agent_OnConnectionStateChanged;
            Agent.OnAgentDisplayNameReceived += Agent_OnAgentDisplayNameReceived;
            Agent.OnAgentStateChanged += Agent_OnAgentStateChanged;
            Agent.OnGroupCollectionReceived += Agent_OnGroupCollectionReceived;
            Agent.OnSignedGroupsChanged += Agent_OnSignedGroupsChanged;
            Agent.OnTeleStateChanged += Agent_OnTeleStateChanged;
            Agent.OnHoldInfo += Agent_OnHoldInfo;
            Agent.OnRingInfoReceived += Agent_OnRingInfoReceived;
            Agent.OnQueueInfo += Agent_OnQueueInfo;
            Agent.OnSipRegisterStateChanged += Agent_OnSipRegisterStateChanged;
            Agent.OnSipCallStateChanged += Agent_OnSipCallStateChanged;

            return Agent;
        }

        private static void Agent_OnConnectionStateChanged(object sender, client.ConnectionInfoStateChangedEventArgs e)
        {

        }

        private static void Agent_OnSipCallStateChanged(object sender, EventArgs e)
        {
            var vm = ViewModels.MainViewModel.Instance;
            ReloadSipAccountList();
            vm.RefreshAgentExecutables();
        }

        private static void Agent_OnSipRegisterStateChanged(object sender, EventArgs e)
        {
            ReloadSipAccountList();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        static void ReloadSipAccountList()
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.SipAccountList = Agent.SipAccountCollection.ToList();
        }

        private static void Agent_OnQueueInfo(object sender, client.QueueInfoEventArgs e)
        {
            ReloadQueueList();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        private static void ReloadQueueList()
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.QueueList = Agent.QueueInfoCollection.ToList();
        }

        private static void Agent_OnRingInfoReceived(object sender, client.RingInfoReceivedEventArgs e)
        {
            ReloadCallList();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        private static void Agent_OnHoldInfo(object sender, client.HoldInfoEventArgs e)
        {
            logger.Debug("Agent_OnHoldInfo");
            ReloadCallList();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        static void ReloadCallList()
        {
            logger.Debug("ReloadCallList");
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.CallList = Agent.CallCollection.ToList();
            model.HoldList = Agent.HeldCallCollection.ToList();
        }
        private static void Agent_OnTeleStateChanged(object sender, client.TeleStateChangedEventArgs e)
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.TeleState = e.NewState;
            ReloadCallList();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        private static void Agent_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            ResetSkillGroup();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        private static void Agent_OnGroupCollectionReceived(object sender, EventArgs e)
        {
            ResetSkillGroup();
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        private static void Agent_OnAgentStateChanged(object sender, client.AgentStateChangedEventArgs e)
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.AgentStateWorkType = new AgentStateWorkType(e.NewState.AgentState, e.NewState.WorkType);
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();

            //// 刚刚连接上来的
            //{
            //    client.AgentState[] states = { client.AgentState.NotExist, client.AgentState.OffLine };
            //    if (e.OldState.AgentState == client.AgentState.Idle)
            //    {

            //    }
            //}

            //{
            //    client.AgentState[] agentStates = { client.AgentState.Idle, client.AgentState.Pause };
            //    client.WorkType[] workTypes = {
            //        client.WorkType.PauseBusy, client.WorkType.PauseLeave, client.WorkType.PauseTyping,
            //        client.WorkType.PauseDinner, client.WorkType.PauseSnooze, client.WorkType.PauseTrain
            //    };
            //    if (agentStates.Any(x => x == e.NewState.AgentState) && workTypes.Any(x => x == e.NewState.WorkType))
            //    {
            //        modifiedAgentStateWorkType = e.NewState;
            //    }
            //}
        }

        private static void Agent_OnAgentDisplayNameReceived(object sender, client.AgentDisplayNameReceivedEventArgs e)
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.WorkerNumber = Agent.WorkerNumber;
            model.DisplayName = e.Value;
            ViewModels.MainViewModel.Instance.RefreshAgentExecutables();
        }

        internal static void DisposeAgent()
        {
            if (Agent != null)
            {
                Agent.Dispose();
                Agent = null;
            }
        }

        static void ResetSkillGroup()
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.SkillGroups = Agent.GroupCollection.ToList();
        }

        internal static bool AgentStartupFlag = false;

        public static async Task StartupAgentAsync(string workerNumber, string password)
        {
            //Models.Cti.AgentBasicInfo.Instance.WorkerNumber = workerNumber;
            try
            {
                await Agent.StartUpAsync(workerNumber, password);
                AgentStartupFlag = true;
                logger.Info("主服务节点连接成功");
            }
            catch (client.ConnectionException err)
            {
                if (Agent.GetConnectionState(Agent.MainConnectionIndex) == client.ConnectionState.Ok)
                {
                    logger.WarnFormat("主服务节点连接成功, 从服务节点连接失败: {0}", err);
                }
                else
                {
                    logger.ErrorFormat("主服务节点连接失败. agent.Dispose: {0}", err);
                    throw;
                }
            }
        }
    }
}
