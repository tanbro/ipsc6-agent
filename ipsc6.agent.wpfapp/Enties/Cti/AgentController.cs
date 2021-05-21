using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.wpfapp.Enties.Cti
{
    public class AgentController
    {

        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AgentController));

        internal static client.Agent Agent = null;

        internal static client.Agent CreateAgent(IReadOnlyCollection<string> adresses)
        {
            if (Agent != null) throw new InvalidOperationException("Data member `agent` is not null");
            Agent = new client.Agent(adresses);
            return Agent;
        }

        internal static void DisposeAgent()
        {
            if (Agent != null)
            {
                Agent.Dispose();
                Agent = null;
            }
        }

        public static async Task StartupAgent(string workerNumber, string password)
        {
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
