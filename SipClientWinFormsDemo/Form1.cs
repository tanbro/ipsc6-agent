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

namespace SipClientWinFormsDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        internal static Endpoint SipEndpoint = new();

        static List<MySipAccount> Accounts = new();

        private void Form1_Load(object sender, EventArgs e)
        {
            SipEndpoint.libCreate();
            using var epCfg = new EpConfig();
            SipEndpoint.libInit(epCfg);
            using TransportConfig tansCfg = new() { port = 0 };
            SipEndpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, tansCfg);
            SipEndpoint.libStart();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SipEndpoint.hangupAllCalls();

            foreach (var acc in Accounts)
            {
                acc.shutdown();
                acc.Dispose();
            }
            Accounts.Clear();
            SipEndpoint.libDestroy();
            SipEndpoint.Dispose();
        }

        private void button_Register_Click(object sender, EventArgs e)
        {
            var user = textBox_SIpUser.Text;
            var password = textBox_SipPassword.Text;
            var registrar = textBox_SipRgistrar.Text;
            var idUri = $"sip:{user}@{registrar}";

            using var sipAuthCred = new AuthCredInfo("digest", "*", user, 0, password);
            using var cfg = new AccountConfig { idUri = idUri };
            cfg.regConfig.timeoutSec = 60;
            cfg.regConfig.retryIntervalSec = 30;
            cfg.regConfig.randomRetryIntervalSec = 10;
            cfg.regConfig.firstRetryIntervalSec = 15;
            cfg.regConfig.registrarUri = $"sip:{registrar}";
            cfg.sipConfig.authCreds.Add(sipAuthCred);

            var acc = new MySipAccount();
            acc.OnIncomingCall2 += Acc_OnIncomingCall2;
            acc.OnRegisterStateChanged += Acc_OnRegisterStateChanged;
            acc.create(cfg);

            Accounts.Add(acc);
            var index = Accounts.IndexOf(acc);

            var msg = $"[注册]: 账户 [{index}] {idUri} Registering ...";
            textBox_Log.AppendText(msg + "\r\n");
        }

        private void Acc_OnIncomingCall2(object sender, MySipCallEventArgs e)
        {
            var acc = sender as MySipAccount;
            var accInfo = acc.getInfo();
            var accIndex = Accounts.IndexOf(acc);
            var call = e.MySipCall;
            var callInfo = call.getInfo();
            var msg =
                $"[呼叫]: 账户 [{accIndex}] {accInfo.uri} 新呼叫 --  {callInfo.remoteUri}";

            Invoke((Action)delegate
            {
                textBox_Log.AppendText(msg + "\r\n");
            });
        }

        private void Acc_OnRegisterStateChanged(object sender, EventArgs e)
        {
            var acc = sender as MySipAccount;
            var info = acc.getInfo();
            var index = Accounts.IndexOf(acc);
            var msg =
                $"[注册]: 账户 [{index}] {info.uri}: {info.regStatusText}";
            Invoke((Action)delegate
            {
                textBox_Log.AppendText(msg + "\r\n");
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var destUri = textBox_calleeUri.Text;
            var acc = Accounts[(int)numericUpDown_callerIndex.Value];
            var call = new MySipCall(acc);

            call.OnStateChanged += Call_OnStateChanged;
            call.OnDisconnected += Call_OnDisconnected;

            var callOpParam = new CallOpParam();
            call.makeCall(destUri, callOpParam);
        }

        private void Call_OnDisconnected(object sender, EventArgs e)
        {

        }

        private void Call_OnStateChanged(object sender, EventArgs e)
        {
            var call = sender as MySipCall;
            var callInfo = call.getInfo();
            var accId = call.getInfo().accId;
            var acc = Accounts.Find(x => x.getId() == accId);
            var accInfo = acc.getInfo();
            var accIndex = Accounts.IndexOf(acc);
            var callDirStr = call.IsIncoming ? "<--" : "-->";

            var msg =
                $"[呼叫]: 账户 [{accIndex}] {accInfo.uri} {callDirStr} {callInfo.remoteUri} 状态={callInfo.stateText}";

            Invoke((Action)delegate
            {
                textBox_Log.AppendText(msg + "\r\n");
            });
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SipEndpoint.hangupAllCalls();
        }
    }
}
