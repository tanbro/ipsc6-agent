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
            agent.OnHoldInfo += Agent_OnHoldInfo;
        }

        private void Agent_OnHoldInfo(object sender, HoldInfoEventArgs e)
        {
            Invoke(new Action(() =>
            {
                lock (listView_hold)
                {
                    listView_hold.Items.Clear();
                    foreach (var hi in agent.HoldInfoCollection)
                    {
                        string[] row = { hi.ConnectionInfo.Host, hi.SessionId, string.Format("{0}", hi.Channel) };
                        var item = new ListViewItem(row)
                        {
                            Tag = hi
                        };
                        listView_hold.Items.Add(item);
                    }
                }
            }));
        }

        private void Agent_OnTeleStateChanged(object sender, TeleStateChangedEventArgs<TeleState> e)
        {
            Invoke(new Action(() =>
            {
                label_teleState.Text = string.Format("{0}", agent.TeleState);
            }));
        }

        private void Agent_OnWorkingChannelInfoReceived(object sender, WorkingChannelInfoReceivedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                label_workChannel.Text = e.Value.Channel.ToString();
            }));
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

        readonly List<MySipAccount> sipAccounts = new List<MySipAccount>();

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
                        // 如果是的，重新注册一次！
                        var existedAcc = (
                            from x in sipAccounts
                            where x.isValid() && x.getInfo()?.uri == uri
                            select x
                        ).FirstOrDefault();
                        if (existedAcc != null)
                        {
                            // Re-Register
                            logger.DebugFormat("重新注册 SipAccount {0}", uri);
                            existedAcc.setRegistration(true);
                        }
                        else
                        {
                            using (var sipAuthCred = new AuthCredInfo("digest", "*", user, 0, "hesong"))
                            using (var cfg = new AccountConfig { idUri = uri })
                            {
                                cfg.regConfig.registrarUri = string.Format("sip:{0}", addr);
                                cfg.sipConfig.authCreds.Add(sipAuthCred);

                                logger.DebugFormat("新建 SipAccount {0}", uri);

                                var acc = new MySipAccount(sipAccounts.Count, this);

                                lock (listView_sipAccounts)
                                {
                                    string[] row = { uri, "" };
                                    ListViewItem item = new ListViewItem(row);
                                    listView_sipAccounts.Items.Add(item);
                                }

                                acc.create(cfg);
                                sipAccounts.Add(acc);
                            }
                        }
                    }
                }
                if (listView_sipAccounts.Items.Count != sipAccounts.Count)
                {
                    throw new IndexOutOfRangeException("SIP 帐户数据和显示列表数量不一致");
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
                var listView = listView_connections;
                var index = agent.GetConnetionIndex(e.ConnectionInfo);
                var isMain = (index == agent.MainConnectionIndex);
                var item = listView.Items[index];
                var s = string.Format("{0}->{1}", e.OldState, e.NewState);
                item.SubItems[1].Text = s;
                item.SubItems[2].Text = isMain ? "*" : "";
            }));
        }

        public Endpoint Endpoint;

        private void Form1_Load(object sender, EventArgs e)
        {

            Endpoint = new Endpoint();
            Endpoint.libCreate();
            using (var epCfg = new EpConfig())
            using (var sipTpConfig = new TransportConfig { port = 5060 })
            {
                epCfg.logConfig.level = 3;
                epCfg.logConfig.writer = SipLogWriter.Instance;
                Endpoint.libInit(epCfg);
                Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, sipTpConfig);
                Endpoint.libStart();
            }

            listView_sipAccounts.Items.Clear();

            ///// 固定4个 Sip Account
            //for (var i = 0; i < 4; i++)
            //{
            //    var acc = new MySipAccount(i, this);
            //    sipAccounts.Add(acc);
            //    string[] row = { string.Format("sip acc[{0}]", i), "" };
            //    var item = new ListViewItem(row);
            //    listView_sipAccounts.Items.Add(item);
            //}

            ///
            ipsc6.agent.network.Connector.Initial();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var acc in sipAccounts)
            {
                acc.Dispose();
            }
            Endpoint.libDestroy();
            Endpoint.Dispose();
            //
            ipsc6.agent.network.Connector.Release();
        }

        private async void button_open_Click(object sender, EventArgs e)
        {
            if (agent != null)
            {
                throw new InvalidOperationException();
            }

            listView_Groups.Items.Clear();
            listView_connections.Items.Clear();

            using (WaitingCursor.Instance)
            {
                var addresses =
                    from s
                    in textBox_ServerAddressList.Text.Trim().Split(new char[] { ',' })
                    where !string.IsNullOrWhiteSpace(s)
                    select s.Trim();
                CreateAgent(addresses);

                listView_connections.Items.Clear();
                foreach (var connInfo in agent.ConnectionList)
                {
                    string[] row = { string.Format("{0}:{1}", connInfo.Host, connInfo.Port), "", "" };
                    var item = new ListViewItem(row);
                    listView_connections.Items.Add(item);
                }

                var workerNumber = textBox_workerNum.Text;
                var password = textBox_password.Text;

                logger.InfoFormat("agent.StartUp ... ");
                try
                {
                    await agent.StartUp(workerNumber, password);
                }
                catch (ConnectionException)
                {
                    if (agent.RunningState != AgentRunningState.Started)
                    {
                        agent.Dispose();
                        agent = null;
                        throw;
                    }
                }
                logger.InfoFormat("agent.StartUp Ok. ");
                label_agentId.Text = string.Format("{0}", agent.AgentId);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (WaitingCursor.Instance)
            {
                if (agent == null) return;
                
                await agent.ShutDown(checkBox_forceClose.Checked);

                agent.Dispose();
                agent = null;

                listView_Groups.Items.Clear();
                listView_hold.Items.Clear();

                foreach (var acc in sipAccounts)
                {
                    acc.Dispose();
                }
                sipAccounts.Clear();
                listView_sipAccounts.Items.Clear();

                listView_connections.Items.Clear();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using (WaitingCursor.Instance)
            {
                await agent.SignIn();
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            using (WaitingCursor.Instance)
            {
                await agent.SignOut();
            }
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView_Groups.SelectedItems.Count > 0)
            {
                var groupId = listView_Groups.SelectedItems[0].Tag as string;
                using (WaitingCursor.Instance)
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
                using (WaitingCursor.Instance)
                {
                    await agent.SignOut(groupId);
                }
            }
        }

        class WaitingCursor : IDisposable
        {
            static readonly object _l = new object();
            static WaitingCursor instance = null;

            public static WaitingCursor Instance
            {
                get
                {
                    lock (_l)
                    {
                        if (instance == null)
                        {
                            instance = new WaitingCursor();
                        }
                        return instance;
                    }
                }
            }

            WaitingCursor()
            {
                Application.UseWaitCursor = true;
            }

            ~WaitingCursor()
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
                        Application.UseWaitCursor = false;
                        lock (_l)
                        {
                            instance = null;
                        }
                        // Note disposing has been done.
                        disposed = true;
                    }
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            using (WaitingCursor.Instance)
            {
                await agent.SetIdle();
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            using (WaitingCursor.Instance)
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
            if (agent.HoldInfoCollection.Count == 1)
            {
                var chan = agent.HoldInfoCollection.First().Channel;
                await agent.UnHold(chan);
            }
            else
            {
                MessageBox.Show("保持队列的数量大于1, 应从列表中选择.");
            }
        }

        private async void button_offhook_Click(object sender, EventArgs e)
        {
            await agent.OffHook();
        }

        private async void button_hangup_Click(object sender, EventArgs e)
        {
            await agent.HangUp();
        }

        private async void unHoldToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var holdInfo = listView_hold.SelectedItems[0].Tag as HoldInfo;
            using (WaitingCursor.Instance)
            {
                var chan = holdInfo.Channel;
                await agent.UnHold(chan);
            }

        }

        private async void button6_Click(object sender, EventArgs e)
        {
            await agent.TakeOver((int)numericUpDown_MainIndex.Value);
        }
    }
}
