using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.wpfapp.Enties.Cti
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    public class AgentController
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AgentController));

        internal static client.Agent Agent = null;

        internal static client.Agent CreateAgent(IReadOnlyCollection<string> adresses)
        {
            if (Agent != null) throw new InvalidOperationException("Data member `agent` is not null");
            Agent = new client.Agent(adresses);
            Agent.OnAgentDisplayNameReceived += Agent_OnAgentDisplayNameReceived;
            Agent.OnAgentStateChanged += Agent_OnAgentStateChanged;
            Agent.OnGroupCollectionReceived += Agent_OnGroupCollectionReceived;
            Agent.OnSignedGroupsChanged += Agent_OnSignedGroupsChanged;
            return Agent;
        }

        private static void Agent_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            ResetSkillGroup();
        }

        private static void Agent_OnGroupCollectionReceived(object sender, EventArgs e)
        {
            ResetSkillGroup();
        }

        private static void Agent_OnAgentStateChanged(object sender, client.AgentStateChangedEventArgs e)
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.AgentStateWorkType = new AgentStateWorkType(e.NewState.AgentState, e.NewState.WorkType);
        }

        private static void Agent_OnAgentDisplayNameReceived(object sender, client.AgentDisplayNameReceivedEventArgs e)
        {
            var model = Models.Cti.AgentBasicInfo.Instance;
            model.WorkerNumber = Agent.WorkerNumber;
            model.DisplayName = e.Value;
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
            model.SkillGroups = (
                from obj in Agent.GroupCollection
                orderby obj.Id
                select obj.Clone() as client.AgentGroup
            ).ToList();
        }

        public static async Task StartupAgent(string workerNumber, string password)
        {
            //Models.Cti.AgentBasicInfo.Instance.WorkerNumber = workerNumber;
            try
            {
                await Agent.StartUp(workerNumber, password);
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
