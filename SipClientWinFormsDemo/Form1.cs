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

        Endpoint SipEndpoint;

        private void Form1_Load(object sender, EventArgs e)
        {
            SipEndpoint = new();
            SipEndpoint.libCreate();
            using var epCfg = new EpConfig();
            SipEndpoint.libInit(epCfg);
            using TransportConfig tansCfg = new() { port = 0 };
            SipEndpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, tansCfg);
            SipEndpoint.libStart();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            acc.OnRegisterStateChanged += Acc_OnRegisterStateChanged;
            acc.create(cfg);

            var msg = $"[注册]: {idUri} Registering ...";
            textBox_Log.AppendText(msg + "\r\n");
        }

        private void Acc_OnRegisterStateChanged(object sender, EventArgs e)
        {
            var acc = sender as MySipAccount;
            var info = acc.getInfo();
            var msg = $"[注册]: {info.uri} {info.onlineStatus} {info.onlineStatusText}";
            Invoke((Action)delegate
            {
                textBox_Log.AppendText(msg + "\r\n");
            });
        }
    }
}
