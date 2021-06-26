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

        #region 内部方法

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
            agent.OnGroupReceived += Agent_OnGroupReceived;
            agent.OnSignedGroupsChanged += Agent_OnSignedGroupsChanged;
        }

        private static void Agent_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            ReloadGroups();
            OnSignedGroupsChanged(new EventArgs());
        }

        private static void ReloadGroups()
        {
            Model.Groups = (
                from x in agent.Groups
                select new Models.Group() { Id = x.Id, Name = x.Name, IsSigned = x.IsSigned }
            ).ToList();
        }

        private static void Agent_OnGroupReceived(object sender, EventArgs e)
        {
            ReloadGroups();
            OnSignedGroupsChanged(new EventArgs());
        }

        public event EventHandler<EventArgs> SignedGroupsChanged;
        internal void OnSignedGroupsChanged(EventArgs args) => SignedGroupsChanged?.Invoke(this, args);


        internal static void DestroyAgent()
        {
            if (agent == null) return;
            agent.Dispose();
            agent = null;
        }

        internal static Models.Model Model = new();

        internal async Task LogInAsync(string workerNumber, string password)
        {
            await agent.StartUpAsync(workerNumber, password);
        }

        #endregion

        #region AgentGroup

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

        public async Task SignGroups(string id, bool isSignIn = true)
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

        #endregion
    }
#pragma warning restore VSTHRD200
}
