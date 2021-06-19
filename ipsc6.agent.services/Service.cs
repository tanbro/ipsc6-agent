using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ipsc6.agent.wpfapp")]
namespace ipsc6.agent.services
{
#pragma warning disable VSTHRD200
    public class Service
    {
        #region Common
        public string Echo(string s) => s;
        public void Throw() => throw new Exception();


        public async Task<string> DelayEcho(string s, int milliseconds)
        {
            await Task.Delay(milliseconds);
            return s;
        }
        #endregion

        #region GUI 无关的 Agent 操作

        internal static client.Agent agent;

        internal static void Initial()
        {
            client.Agent.Initial();
        }

        internal static void Release()
        {
            client.Agent.Release();
        }

        internal static void CreateAgent(IEnumerable<string> addresses, ushort localPort, string localAddress)
        {
            if (agent != null) throw new InvalidOperationException();
            agent = new client.Agent(addresses, localPort, localAddress);
            agent.OnAgentDisplayNameReceived += Agent_OnAgentDisplayNameReceived;
        }

        private static void Agent_OnAgentDisplayNameReceived(object sender, client.AgentDisplayNameReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static void DestroyAgent()
        {
            if (agent == null) return;
            agent.Dispose();
            agent = null;
        }

        internal static async Task LogInAsync(string workerNumber, string password)
        {
            await agent.StartUpAsync(workerNumber, password);
        }

        public static async Task SignIn(string skillId)
        {
            await agent.SignInAsync(skillId);
        }

        #endregion
    }
#pragma warning restore VSTHRD200
}
