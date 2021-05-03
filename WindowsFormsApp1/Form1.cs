using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using org.pjsip.pjsua2;
using ipsc6.agent.client;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Form1));
        public Form1()
        {
            InitializeComponent();
        }

        Agent agent = null;

        void ReallocAgent(IEnumerable<string> serverAddreses)
        {
            if (agent != null)
            {
                agent.Dispose();
                agent = null;
            }
            var addrList = new List<ConnectionInfo>();
            foreach (var s in serverAddreses)
            {
                addrList.Add(new ConnectionInfo(s));
            }
            agent = new Agent(addrList);
            numericUpDown_mainServerIndex.Value = agent.MainConnectionIndex;
            agent.OnConnectionStateChanged += Agent_OnConnectionStateChanged;
            agent.OnAgentDisplayNameReceived += Agent_OnAgentDisplayNameReceived;
            agent.OnAgentStateChanged += Agent_OnAgentStateChanged;
            agent.OnGroupCollectionReceived += Agent_OnGroupCollectionReceived;
            agent.OnSignedGroupsChanged += Agent_OnSignedGroupsChanged;
            agent.OnSipRegistrarListReceived += Agent_OnSipRegistrarListReceived;
            agent.OnRingInfoReceived += Agent_OnRingInfoReceived;
        }

        private void Agent_OnRingInfoReceived(object sender, RingInfoReceivedEventArgs e)
        {
            var info = e.Value;
            var s = string.Format(
                "{0}: [{1}] {2}",
                e.ConnectionInfo.Host,
                info.WorkingChannel,
                info.CustomString
            );
            Invoke(new Action(() =>
            {
                textBox_ringInfo.Text = s;
            }));
        }

        List<MySipAccount> sipAccounts = new List<MySipAccount>();
        private void Agent_OnSipRegistrarListReceived(object sender, SipRegistrarListReceivedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                var listView = listView_sipAccounts;
                var user = textBox_workerNum.Text;
                lock (sipAccounts)
                {
                    foreach (var addr in e.Value)
                    {
                        var uri = string.Format("sip:{0}@{1}", user, addr);
                        if (sipAccounts.Any(x => x.getInfo().uri == uri)) continue;

                        var acc = new MySipAccount(this);
                        sipAccounts.Add(acc);

                        string[] row = { uri, "" };
                        var item = new ListViewItem(row)
                        {
                            Tag = uri
                        };
                        lock (listView)
                        {
                            listView.Items.Add(item);
                        }

                        using (var cfg = new AccountConfig()
                        {
                            idUri = uri
                        })
                        {
                            cfg.regConfig.registrarUri = string.Format("sip:{0}", addr);
                            using (var sipAuthCred = new org.pjsip.pjsua2.AuthCredInfo("digest", "*", user, 0, "hesong"))
                            {
                                cfg.sipConfig.authCreds.Add(sipAuthCred);
                            }
                            acc.create(cfg);
                        }
                    }
                }
            }));
        }

        void RefreshGroupListView()
        {
            lock (listView_Groups)
            {
                listView_Groups.Items.Clear();
                foreach (var m in agent.GroupCollection)
                {
                    string[] row = { m.Id, m.Name, string.Format("{0}", m.Signed) };
                    var item = new ListViewItem(row)
                    {
                        Tag = m.Id
                    };
                    listView_Groups.Items.Add(item);
                }
            }
        }

        public static MySipCall currSipCall = null;

        public void SetSipAccountMessage(string uri, string msg)
        {
            lock (listView_sipAccounts)
            {
                foreach (ListViewItem item in listView_sipAccounts.Items)
                {
                    if ((string)item.Tag == uri)
                    {
                        item.SubItems[1].Text = msg;
                        break;
                    }
                }
            }
        }

        private void Agent_OnSignedGroupsChanged(object sender, EventArgs e)
        {
            Invoke(new Action(() => RefreshGroupListView()));
        }

        private void Agent_OnGroupCollectionReceived(object sender, EventArgs e)
        {
            Invoke(new Action(() => RefreshGroupListView()));
        }

        private void Agent_OnAgentStateChanged(object sender, AgentStateChangedEventArgs<AgentStateWorkType> e)
        {
            Invoke(new Action(() =>
            {
                var s = string.Format("{0}({1}) ==> {2}({3})", e.OldState.AgentState, e.OldState.WorkType, e.NewState.AgentState, e.NewState.WorkType);
                label_agentState.Text = s;
            }));
        }

        private void Agent_OnAgentDisplayNameReceived(object sender, AgentDisplayNameReceivedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                var s = string.Format("{0}", e.Value);
                label_agentName.Text = s;
            }));
        }

        private void Agent_OnConnectionStateChanged(object sender, ConnectionInfoStateChangedEventArgs<ipsc6.agent.client.ConnectionState> e)
        {
            Invoke(new Action(() =>
            {
                var s = string.Format("{1}", e.OldState, e.NewState);
                var i = serverList.IndexOf(e.ConnectionInfo.Host);
                var listView = listView_connections;
                var item = listView.Items[i];
                item.SubItems[1].Text = s;
            }));
        }


        List<string> serverList;
        public static Endpoint endpoint;

        private void Form1_Load(object sender, EventArgs e)
        {
            ipsc6.agent.network.Connector.Initial();

            endpoint = new Endpoint();
            endpoint.libCreate();
            using (var epCfg = new EpConfig())
            using (var sipTpConfig = new TransportConfig { port = 5060 })
            {
                epCfg.logConfig.level = 6;
                epCfg.logConfig.writer = new SipLogWriter();
                endpoint.libInit(epCfg);
                endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, sipTpConfig);
                endpoint.libStart();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Info("FormClosed");
            button1_Click(this, new EventArgs());
            //if (agent != null)
            //{
            //    logger.Debug("agent.Dispose() ...");
            //    agent.Dispose();
            //    agent = null;
            //}
            logger.Debug("ipsc6.agent.network.Connector.Release() ...");
            ipsc6.agent.network.Connector.Release();

            logger.Debug("sipAccounts.Clear() ...");
            sipAccounts.Clear();

            logger.Debug("endpoint.libDestroy() ...");
            endpoint.libDestroy();
            endpoint.Dispose();

            logger.Info("FormClosed Ok");
        }

        private async void button_open_Click(object sender, EventArgs e)
        {
            if (agent != null)
            {
                throw new InvalidOperationException();
            }

            var s = textBox_ServerAddressList.Text.Trim();
            var addresses = s.Split(new char[] { ',' });
            ReallocAgent(addresses);
            serverList = addresses.ToList();

            listView_connections.Items.Clear();
            foreach (var ci in agent.ConnectionList)
            {
                string[] row = { ci.Host, "", "" };
                var item = new ListViewItem(row);
                listView_connections.Items.Add(item);
            }

            var workerNumber = textBox_workerNum.Text;
            var password = textBox_password.Text;
            using (new Processing(this))
            {
                await agent.StartUp(workerNumber, password);
                label_agentId.Text = string.Format("{0}", agent.AgentId);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (new Processing(this))
            {
                if (agent != null)
                {
                    await agent.ShutDown(checkBox_forceClose.Checked);
                    listView_Groups.Items.Clear();

                    foreach (var acc in sipAccounts)
                    {
                        logger.DebugFormat("shutdown SIP account {0}", acc.getId());
                        acc.shutdown();
                        logger.DebugFormat("dispose SIP account {0}", acc.getId());
                        acc.Dispose();
                    }
                    sipAccounts.Clear();
                    listView_sipAccounts.Items.Clear();
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using (new Processing(this))
            {
                await agent.SignIn();
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            using (new Processing(this))
            {
                await agent.SignOut();
            }
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView_Groups.SelectedItems.Count > 0)
            {
                var groupId = listView_Groups.SelectedItems[0].Tag as string;
                using (new Processing(this))
                {
                    await agent.SignIn(groupId);
                }
            }
        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (listView_Groups.SelectedItems.Count > 0)
            {
                var groupId = listView_Groups.SelectedItems[0].Tag as string;
                using (new Processing(this))
                {
                    await agent.SignOut(groupId);
                }
            }
        }

        class Processing : IDisposable
        {
            Cursor savedCursor;
            public readonly Form Form;
            public Processing(Form form)
            {
                Form = form;
                Form.Enabled = false;
                savedCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
            }

            private bool disposed = false;
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // Check to see if Dispose has already been called.
                    if (!disposed)
                    {
                        Form.Enabled = true;
                        Cursor.Current = savedCursor;
                        // Note disposing has been done.
                        disposed = true;
                    }
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            using (new Processing(this))
            {
                await agent.SetIdle();
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            using (new Processing(this))
            {
                await agent.SetBusy();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (currSipCall != null)
            {
                using (var cop = new CallOpParam
                {
                    statusCode = pjsip_status_code.PJSIP_SC_OK
                })
                {
                    currSipCall.answer(cop);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (currSipCall != null)
            {
                using (var cop = new CallOpParam
                {
                    statusCode = pjsip_status_code.PJSIP_SC_OK
                })
                {
                    currSipCall.hangup(cop);
                }
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            var t = (MessageType)numericUpDown_ReqType2.Value;
            var n = (int)numericUpDown_ReqNum2.Value;
            var s = textBox_ReqContent2.Text.Trim();
            var r = await agent.Request(new AgentRequestMessage(t, n, s));
            textBox_reqRes.Text = string.Format("[{0}] [{1}] [{2}] \"{3}\"", r.Type, r.N1, r.N2, r.S);
        }
    }
}
