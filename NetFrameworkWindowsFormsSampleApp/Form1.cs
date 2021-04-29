using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ipsc6.agent.network;
using ipsc6.agent.client;

namespace NetFrameworkWindowsFormsSampleApp
{
    public partial class Form1 : Form
    {
        public class SipAccount : org.pjsip.pjsua2.Account
        {
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SipAccount));

            public readonly int connId;
            public readonly Form1 form;
            public SipAccount(Form1 form, int connId) : base()
            {
                this.form = form;
                this.connId = connId;
            }

            ~SipAccount()
            {
                shutdown();
            }

            public override void onRegState(org.pjsip.pjsua2.OnRegStateParam prm)
            {
                var msg = string.Format("{0} Register {1} {2}", getInfo().uri, (int)prm.code, prm.reason);
                logger.InfoFormat(msg);
                switch (connId)
                {
                    case 1:
                        form.Invoke(new Action(() =>
                        {
                            form.textBox_Log1.AppendText(string.Format("{0}\r\n", msg));
                            form.label_SipReg1.Text = msg;
                        }));
                        break;
                    case 2:
                        form.Invoke(new Action(() =>
                        {
                            form.textBox_Log2.AppendText(string.Format("{0}\r\n", msg));
                            form.label_SipReg2.Text = msg;
                        }));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override void onIncomingCall(org.pjsip.pjsua2.OnIncomingCallParam prm)
            {
                if (currentSipCall != null)
                {
                    form.Invoke(new Action(() => throw new Exception("IncomingCall: 已经有胡椒了")));
                }
                else
                {
                    var call = currentSipCall = new SipCall(this, prm.callId);
                    var ci = call.getInfo();
                    var msg = string.Format("IncomingCall {0} {1} \t\n", ci.remoteUri, ci.state);
                    switch (connId)
                    {
                        case 1:
                            form.Invoke(new Action(() =>
                            {
                                form.textBox_Log1.AppendText(string.Format("{0}\r\n", msg));
                                form.label_SipReg1.Text = msg;
                            }));
                            break;
                        case 2:
                            form.Invoke(new Action(() =>
                            {
                                form.textBox_Log2.AppendText(string.Format("{0}\r\n", msg));
                                form.label_SipReg2.Text = msg;
                            }));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public static SipCall currentSipCall;

        public class SipCall : org.pjsip.pjsua2.Call
        {
            private readonly org.pjsip.pjsua2.Account account;
            public SipCall(org.pjsip.pjsua2.Account acc, int callId = (int)org.pjsip.pjsua2.pjsua_invalid_id_const_.PJSUA_INVALID_ID)
                : base(acc, callId)
            {
                account = acc;
            }

            public override void onCallState(org.pjsip.pjsua2.OnCallStateParam prm)
            {
                var ci = getInfo();
                var acc = account as SipAccount;
                if (acc.getId() != ci.accId)
                {
                    throw new Exception(string.Format("AccID={0} in call differs with {1}", ci.accId, acc.getId()));
                }

                var msg = string.Format("Call {0} {1} \t\n", ci.remoteUri, ci.state);
                switch (acc.connId)
                {
                    case 1:
                        acc.form.Invoke(new Action(() =>
                        {
                            acc.form.textBox_Log1.AppendText(string.Format("{0}\r\n", msg));
                            acc.form.label_SipReg1.Text = msg;
                        }));
                        break;
                    case 2:
                        acc.form.Invoke(new Action(() =>
                        {
                            acc.form.textBox_Log2.AppendText(string.Format("{0}\r\n", msg));
                            acc.form.label_SipReg2.Text = msg;
                        }));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (ci.state)
                {
                    case org.pjsip.pjsua2.pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED:
                        /* Schedule/Dispatch call deletion to another thread here */
                        acc.form.Invoke(new Action(() =>
                        {
                            currentSipCall.Dispose();
                            currentSipCall = null;
                        }));
                        break;
                    default:
                        break;
                }

            }

            public override void onCallMediaState(org.pjsip.pjsua2.OnCallMediaStateParam prm)
            {
                var ci = getInfo();
                // Iterate all the call medias
                for (int i = 0; i < ci.media.Count(); i++)
                {
                    if (ci.media[i].type == org.pjsip.pjsua2.pjmedia_type.PJMEDIA_TYPE_AUDIO)
                    {
                        var audMed = getAudioMedia(i);
                        // Connect the call audio media to sound device
                        var mgr = SipClient.endpoint.audDevManager();
                        audMed.startTransmit(mgr.getPlaybackDevMedia());
                        mgr.getCaptureDevMedia().startTransmit(audMed);
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();

            conn1 = new Connection();
            conn2 = new Connection();

            conn1.OnServerSend += Conn_OnServerSendEventReceived;
            conn1.OnClosed += Conn_OnDisconnected;
            conn1.OnLost += Conn_OnConnectionLost;
            conn1.OnConnectionStateChanged += Conn_OnConnectionStateChanged;

            conn2.OnServerSend += Conn_OnServerSendEventReceived;
            conn2.OnClosed += Conn_OnDisconnected;
            conn2.OnLost += Conn_OnConnectionLost;
            conn2.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
        }

        private void Conn_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            Connection conn = (Connection)sender;
            var text = string.Format("{0}", e.NewState);
            if (conn == conn1)
            {
                label_ConnectStatus1.Text = text;
            }
            else if (conn == conn2)
            {
                label_ConnectStatus2.Text = text;

            }
            else
            {
                throw new Exception("Connection 1 or 2?");
            }
        }

        ~Form1()
        {

        }

        private static Connection conn1;
        private static Connection conn2;

        private SipAccount sipAcc1;
        private SipAccount sipAcc2;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        private void Form1_Load(object sender, EventArgs e)
        {
            EnumAudioDevices();
            label_ConnectStatus1.Text = string.Format("{0}", conn1.State);
            label_ConnectStatus2.Text = string.Format("{0}", conn2.State);
        }

        public class PjAudioDeviceInfoAndIndex
        {
            public readonly org.pjsip.pjsua2.AudioDevInfo Di;
            public readonly int Index;

            private readonly string name;
            public string Name
            {
                get { return name; }
            }

            public PjAudioDeviceInfoAndIndex(org.pjsip.pjsua2.AudioDevInfo di, int index)
            {
                Di = di;
                Index = index;
                name = di.name;
            }
        }

        private void EnumAudioDevices()
        {
            checkedListBox_AudCapture.DisplayMember = "Name";
            checkedListBox_AudPlayback.DisplayMember = "Name";
            var audDevManager = SipClient.endpoint.audDevManager();
            var captureDevId = audDevManager.getCaptureDev();
            var playbackDevId = audDevManager.getPlaybackDev();
            for (var i = 0; i < audDevManager.getDevCount(); i++)
            {
                var di = audDevManager.getDevInfo(i);
                if (di.inputCount > 0)
                {
                    checkedListBox_AudCapture.Items.Add(
                        new PjAudioDeviceInfoAndIndex(di, i),
                        i == captureDevId
                    );
                }
                if (di.outputCount > 0)
                {
                    checkedListBox_AudPlayback.Items.Add(
                        new PjAudioDeviceInfoAndIndex(di, i),
                        i == playbackDevId
                    );
                }
            }
        }

        private void Conn_OnConnectionLost(object sender)
        {
            Invoke(new Action(() =>
            {
                if (sender == conn1)
                {
                    sipAcc1?.shutdown();
                }
                else if (sender == conn2)
                {
                    sipAcc2?.shutdown();
                }
            }));
        }

        private void Conn_OnDisconnected(object sender)
        {
            Invoke(new Action(() =>
            {
                if (sender == conn1)
                {
                    sipAcc1?.shutdown();
                }
                else if (sender == conn2)
                {
                    sipAcc2?.shutdown();
                }
            }));
        }

        private void Conn_OnServerSendEventReceived(object sender, AgentMessageReceivedEventArgs e)
        {

            Invoke(new Action(() =>
            {
                var msg = string.Format("event: {0} - [{1}] [{2}] \"{3}\"\r\n", e.CommandType, e.N1, e.N2, e.S);
                int id = 0;
                if (sender == conn1)
                {
                    id = 1;
                    textBox_Log1.AppendText(msg);
                }
                else if (sender == conn2)
                {
                    id = 2;
                    textBox_Log2.AppendText(msg);
                }
                // server-send data
                if (e.CommandType == AgentMessage.REMOTE_MSG_SENDDATA)
                {
                    // SIP 注册地址
                    if (e.N1 == 13)
                    {
                        var sipRegistrar = e.S.Trim();
                        if (!string.IsNullOrWhiteSpace(sipRegistrar))
                        {
                            CreateSipAccount(id, sipRegistrar);
                        }
                    }
                }
            }));
        }

        private void CreateSipAccount(int id, string registrar)
        {
            string user;
            switch (id)
            {
                case 1:
                    user = textBox_User1.Text.Trim();
                    break;
                case 2:
                    user = textBox_User2.Text.Trim();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            using (
                var sipAccountCfg = new org.pjsip.pjsua2.AccountConfig
                {
                    idUri = string.Format("sip:{0}@{1}", user, registrar)
                }
            )
            {
                sipAccountCfg.regConfig.registrarUri = string.Format("sip:{0}", registrar);
                using (var sipAuthCred = new org.pjsip.pjsua2.AuthCredInfo("digest", "*", user, 0, "hesong"))
                {
                    sipAccountCfg.sipConfig.authCreds.Add(sipAuthCred);
                }
                switch (id)
                {
                    case 1:
                        sipAcc1?.Dispose();
                        sipAcc1 = new SipAccount(this, 1);
                        sipAcc1.create(sipAccountCfg);
                        break;
                    case 2:
                        sipAcc2?.Dispose();
                        sipAcc2 = new SipAccount(this, 2);
                        sipAcc2.create(sipAccountCfg);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private async void button_Connect1_Click(object sender, EventArgs e)
        {
            await conn1.Open(textBox_Server1.Text);
        }

        private async void button_Connect2_Click(object sender, EventArgs e)
        {
            label_ConnectStatus2.Text = "Connecting ...";
            try
            {
                await conn2.Open(textBox_Server2.Text);
            }
            catch (ConnectionException err)
            {
                label_ConnectStatus2.Text = string.Format("Connect failed: {0}", err.Message);
                return;
            }
            label_ConnectStatus2.Text = "Connected.";
        }

        private async void button_LogIn1_Click(object sender, EventArgs e)
        {
            var s = string.Format("{0}|{1}|1|0|{0}", textBox_User1.Text, textBox_Psw1.Text);
            var r = await conn1.Request(new AgentRequestArgs(
                AgentMessage.REMOTE_MSG_LOGIN, s
            ));
            textBox_Log1.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private async void button_LogOut_Click(object sender, EventArgs e)
        {
            try
            {
                await conn1.Request(new AgentRequestArgs(AgentMessage.REMOTE_MSG_RELEASE), 0);
            }
            catch (RequestTimeoutError) { }
        }

        private async void button_Req1_Click(object sender, EventArgs e)
        {
            var t = (AgentMessage)numericUpDown_ReqType1.Value;
            var n = (int)numericUpDown_ReqNum1.Value;
            var s = textBox_ReqContent1.Text.Trim();
            var r = await conn1.Request(new AgentRequestArgs(t, n, s));
            textBox_Log1.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private void button_Disconnect1_Click(object sender, EventArgs e)
        {
            conn1.Close();
        }

        private void button_Disconnect2_Click(object sender, EventArgs e)
        {
            conn2.Close();
        }

        private async void button_LogIn2_Click(object sender, EventArgs e)
        {
            var s = string.Format("{0}|{1}|1|0|{0}", textBox_User2.Text, textBox_Psw2.Text);
            var r = await conn2.Request(new AgentRequestArgs(
                AgentMessage.REMOTE_MSG_LOGIN, s
            ));
            textBox_Log2.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private async void button_Req2_Click(object sender, EventArgs e)
        {
            var t = (AgentMessage)numericUpDown_ReqType2.Value;
            var n = (int)numericUpDown_ReqNum2.Value;
            var s = textBox_ReqContent2.Text.Trim();
            var r = await conn2.Request(new AgentRequestArgs(t, n, s));
            textBox_Log2.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox_Log1.Clear();
            textBox_Log2.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sipAcc1?.Dispose();
            sipAcc2?.Dispose();
            currentSipCall?.Dispose();
            conn1.Dispose();
            conn2.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentSipCall == null)
            {
                throw new Exception("你不能这样做，因为 currentSipCall is NULL!");
            }
            using (
                var prm = new org.pjsip.pjsua2.CallOpParam
                {
                    statusCode = org.pjsip.pjsua2.pjsip_status_code.PJSIP_SC_OK
                }
            )
            {
                currentSipCall.answer(prm);
            }
        }

        private void checkedListBox_AudCapture_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkListBox = sender as CheckedListBox;
            if (checkListBox.IndexFromPoint(checkListBox.PointToClient(Cursor.Position).X, checkListBox.PointToClient(Cursor.Position).Y) <= -1)
            {
                e.NewValue = e.CurrentValue;
                return;
            }
            if (e.NewValue == CheckState.Checked)
            {
                for (var i = 0; i < checkListBox.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkListBox.SetItemChecked(i, false);
                    }
                }
                var dii = checkListBox.Items[e.Index] as PjAudioDeviceInfoAndIndex;
                var mgr = SipClient.endpoint.audDevManager();
                mgr.setCaptureDev(dii.Index);
            }
        }

        private void checkedListBox_AudPlayback_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkListBox = sender as CheckedListBox;
            if (checkListBox.IndexFromPoint(checkListBox.PointToClient(Cursor.Position).X, checkListBox.PointToClient(Cursor.Position).Y) <= -1)
            {
                e.NewValue = e.CurrentValue;
                return;
            }
            if (e.NewValue == CheckState.Checked)
            {
                for (var i = 0; i < checkListBox.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkListBox.SetItemChecked(i, false);
                    }
                }
                var dii = checkListBox.Items[e.Index] as PjAudioDeviceInfoAndIndex;
                var mgr = SipClient.endpoint.audDevManager();
                mgr.setPlaybackDev(dii.Index);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (currentSipCall == null)
            {
                throw new Exception("你不能这样做，因为 currentSipCall is NULL!");
            }
            using (
                var prm = new org.pjsip.pjsua2.CallOpParam
                {
                    statusCode = org.pjsip.pjsua2.pjsip_status_code.PJSIP_SC_DECLINE
                }
            )
            {
                currentSipCall.hangup(prm);
            }
        }
    }
}
