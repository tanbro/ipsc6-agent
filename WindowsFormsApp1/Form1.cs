using ipsc6.agent.client;
using org.pjsip.pjsua2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

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

        void CreateAgent(IEnumerable<string> serverAddreses)
        {
            var addrList = new List<ConnectionInfo>();
            foreach (var s in serverAddreses)
            {
                addrList.Add(new ConnectionInfo(s));
            }
            agent = new Agent(addrList);
            agent.OnConnectionStateChanged += Agent_OnConnectionStateChanged;
            agent.OnAgentDisplayNameReceived += Agent_OnAgentDisplayNameReceived;
            agent.OnAgentStateChanged += Agent_OnAgentStateChanged;
            agent.OnGroupCollectionReceived += Agent_OnGroupCollectionReceived;
            agent.OnSignedGroupsChanged += Agent_OnSignedGroupsChanged;
            agent.OnSipRegistrarListReceived += Agent_OnSipRegistrarListReceived;
            agent.OnTeleStateChanged += Agent_OnTeleStateChanged;
            agent.OnRingInfoReceived += Agent_OnRingInfoReceived;
            agent.OnMainConnectionChanged += Agent_OnMainConnectionChanged;
            agent.OnWorkingChannelInfoReceived += Agent_OnWorkingChannelInfoReceived;
        }

        private void Agent_OnTeleStateChanged(object sender, TeleStateChangedEventArgs<TeleState> e)
        {
            label_teleState.Text = string.Format("{0}", agent.TeleState);
        }

        private void Agent_OnWorkingChannelInfoReceived(object sender, WorkingChannelInfoReceivedEventArgs e)
        {
            label_workChannel.Text = e.Value.Channel.ToString();
        }

        private void Agent_OnMainConnectionChanged(object sender, EventArgs e)
        {

            Invoke(new Action(() =>
            {
                var index = agent.MainConnectionIndex;
                for (var i = 0; i < listView_connections.Items.Count; i++)
                {
                    listView_connections.Items[i].SubItems[2].Text = (i == index) ? "*" : "";
                }
            }));
        }

        private void Agent_OnRingInfoReceived(object sender, RingInfoReceivedEventArgs e)
        {

            Invoke(new Action(() =>
            {
                var info = e.Value;
                var s = string.Format(
                    "{0}: [{1}] {2}",
                    e.ConnectionInfo.Host,
                    info.WorkingChannel,
                    info.CustomString
                );
                textBox_ringInfo.Text = s;
            }));
        }

        List<MySipAccount> sipAccounts = new List<MySipAccount>();
        private void Agent_OnSipRegistrarListReceived(object sender, SipRegistrarListReceivedEventArgs e)
        {

            Invoke(new Action(() =>
            {
                var user = textBox_workerNum.Text;
                lock (sipAccounts)
                {
                    foreach (var addr in e.Value)
                    {
                        var uri = string.Format("sip:{0}@{1}", user, addr);

                        // 这个地址是不是已经注册了?
                        if (sipAccounts.Any(x => x.isValid() && x.getInfo()?.uri == uri))
                            continue;

                        int index = -1;
                        MySipAccount acc = null;
                        for (var i = 0; i < listView_sipAccounts.Items.Count; i++)
                        {
                            ListViewItem item = listView_sipAccounts.Items[i];
                            if (item.Tag == null)
                            {
                                index = i;
                                acc = sipAccounts[index];
                                item.Tag = acc;
                                item.SubItems[0].Text = uri;
                                break;
                            }
                        }

                        using (var cfg = new AccountConfig()
                        {
                            idUri = uri
                        })
                        {
                            cfg.regConfig.registrarUri = string.Format("sip:{0}", addr);
                            using (var sipAuthCred = new AuthCredInfo("digest", "*", user, 0, "hesong"))
                            {
                                cfg.sipConfig.authCreds.Add(sipAuthCred);
                            }
                            if (acc.isValid())
                            {
                                acc.modify(cfg);
                            }
                            else
                            {
                                acc.create(cfg);
                            }
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

        public void SetSipAccountMessage(int index, string msg)
        {

            lock (listView_sipAccounts)
            {
                listView_sipAccounts.Items[index].SubItems[1].Text = msg;
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
        public Endpoint Endpoint;

        private void Form1_Load(object sender, EventArgs e)
        {
            ipsc6.agent.network.Connector.Initial();

            Endpoint = new Endpoint();
            Endpoint.libCreate();
            using (var epCfg = new EpConfig())
            using (var sipTpConfig = new TransportConfig { port = 5060 })
            {
                //epCfg.logConfig.level = 6;
                //epCfg.logConfig.writer = SipLogWriter.Instance;
                Endpoint.libInit(epCfg);
                Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, sipTpConfig);
                Endpoint.libStart();
            }

            /// 固定4个 Sip Account
            /// 
            listView_sipAccounts.Items.Clear();
            for (var i = 0; i < 4; i++)
            {
                var acc = new MySipAccount(i, this);
                sipAccounts.Add(acc);
                string[] row = { string.Format("sip acc[{0}]", i), "" };
                var item = new ListViewItem(row);
                listView_sipAccounts.Items.Add(item);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Info("FormClosed");

            logger.Debug("ipsc6.agent.network.Connector.Release() ...");
            ipsc6.agent.network.Connector.Release();

            foreach (var acc in sipAccounts)
            {
                logger.DebugFormat("Dispose SipAccount {0}", acc.getId());
                acc.Dispose();
            }
            logger.Debug("endpoint.libDestroy() ...");
            Endpoint.libDestroy();
            Endpoint.Dispose();

            logger.Info("FormClosed Ok");
        }

        private async void button_open_Click(object sender, EventArgs e)
        {
            if (agent != null)
            {
                throw new InvalidOperationException();
            }

            listView_Groups.Items.Clear();
            listView_connections.Items.Clear();

            using (new Processing(this))
            {
                var s = textBox_ServerAddressList.Text.Trim();
                var addresses = s.Split(new char[] { ',' });
                CreateAgent(addresses);
                serverList = addresses.ToList();

                listView_connections.Items.Clear();
                foreach (var x in agent.ConnectionList.Select((value, index) => new { value, index }))
                {
                    bool isMaster = x.index == agent.MainConnectionIndex;
                    string[] row = { x.value.Host, "", isMaster ? "*" : "" };
                    var item = new ListViewItem(row);
                    listView_connections.Items.Add(item);
                }

                var workerNumber = textBox_workerNum.Text;
                var password = textBox_password.Text;

                logger.InfoFormat("agent.StartUp ... ");
                await agent.StartUp(workerNumber, password);
                logger.InfoFormat("agent.StartUp Ok. ");
                label_agentId.Text = string.Format("{0}", agent.AgentId);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (new Processing(this))
            {
                if (agent == null) return;
                await agent.ShutDown(checkBox_forceClose.Checked);

                foreach (var pair in sipAccounts.Select((value, index) => new { value, index }))
                {
                    var acc = pair.value;
                    var index = pair.index;
                    System.Diagnostics.Debug.Assert(acc.Index == index);
                    if (acc.isValid())
                    {
                        logger.DebugFormat("shutdown SIP account [{0}] {1}", acc.Index, acc.getId());
                        acc.shutdown();
                        lock (listView_sipAccounts)
                        {
                            listView_sipAccounts.Items[index].SubItems[0].Text = string.Format("sip acc[{0}]", index);
                            listView_sipAccounts.Items[index].SubItems[1].Text = "";
                        }
                    }
                }

                agent.Dispose();
                agent = null;
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
                Form.Cursor = Cursors.WaitCursor;
            }

            ~Processing()
            {
                Dispose(false);
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
                        Form.Cursor = savedCursor;
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

        private void button7_Click(object sender, EventArgs e)
        {
            if (currSipCall != null)
            {
                var cop = new CallOpParam
                {
                    statusCode = pjsip_status_code.PJSIP_SC_OK
                };
                currSipCall.answer(cop);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (currSipCall != null)
            {
                var cop = new CallOpParam();
                currSipCall.hangup(cop);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (agent != null)
            {
                e.Cancel = true;
                MessageBox.Show("没有下线，不可以关闭");
            }
        }

        private async void btn_hold_Click(object sender, EventArgs e)
        {
            await agent.Hold();
        }

        private async void btn_unhold_Click(object sender, EventArgs e)
        {
            var chan = (int)numericUpDown_channel.Value;
            await agent.UnHold(chan);
        }

        private async void button_offhook_Click(object sender, EventArgs e)
        {
            await agent.OffHook();
        }

        private async void button_hangup_Click(object sender, EventArgs e)
        {
            await agent.HangUp();
        }
    }
}
