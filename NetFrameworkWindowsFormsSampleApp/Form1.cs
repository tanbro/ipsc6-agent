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
        public Form1()
        {
            InitializeComponent();
        }

        private static Connection conn1;
        private static Connection conn2;

        private void Form1_Load(object sender, EventArgs e)
        {
            conn1 = new Connection();
            conn2 = new Connection();

            conn1.OnServerSendEventReceived += Conn_OnServerSendEventReceived;
            conn1.OnDisconnected += Conn1_OnDisconnected;
            conn1.OnConnectionLost += Conn1_OnConnectionLost;

            conn2.OnServerSendEventReceived += Conn_OnServerSendEventReceived;
            conn2.OnDisconnected += Conn1_OnDisconnected;
            conn2.OnConnectionLost += Conn1_OnConnectionLost;
        }

        private void Conn1_OnConnectionLost(object sender)
        {
            Invoke(new Action(() =>
            {
                if (sender == conn1)
                {
                    label_ConnectStatus1.Text = "ConnectionLost";
                }
                else if (sender == conn2)
                {
                    label_ConnectStatus2.Text = "ConnectionLost";
                }
            }));
        }

        private void Conn1_OnDisconnected(object sender)
        {
            Invoke(new Action(() =>
            {
                if (sender == conn1)
                {
                    label_ConnectStatus1.Text = "Disconnected";
                }
                else if (sender == conn2)
                {
                    label_ConnectStatus2.Text = "Disconnected";
                }
            }));
        }

        private void Conn_OnServerSendEventReceived(object sender, AgentMessageReceivedEventArgs e)
        {

            Invoke(new Action(() =>
            {
                if (sender == conn1)
                {
                    textBox_Log1.AppendText(string.Format("event: {0} {1} {2}\r\n", e.N1, e.N2, e.S));
                }
                else if (sender == conn2)
                {
                    textBox_Log2.AppendText(string.Format("event: {0} {1} {2}\r\n", e.N1, e.N2, e.S));
                }
            }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn1.Dispose();
            conn2.Dispose();
        }

        private async void button_Connect1_Click(object sender, EventArgs e)
        {
            label_ConnectStatus1.Text = "Connecting ...";
            try
            {
                await conn1.Open(textBox_Server1.Text);
            }
            catch (AgentConnectException err)
            {
                label_ConnectStatus1.Text = string.Format("Connect failed: {0}", err.Message);
                return;
            }
            label_ConnectStatus1.Text = "Connected.";
        }

        private async void button_Connect2_Click(object sender, EventArgs e)
        {
            label_ConnectStatus2.Text = "Connecting ...";
            try
            {
                await conn2.Open(textBox_Server2.Text);
            }
            catch (AgentConnectException err)
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
                AgentMessageEnum.REMOTE_MSG_LOGIN, s
            ));
            textBox_Log1.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private async void button_LogOut_Click(object sender, EventArgs e)
        {
            try
            {
                await conn1.Request(new AgentRequestArgs(AgentMessageEnum.REMOTE_MSG_RELEASE), 0);
            }
            catch (AgentRequestTimeoutException) { }
        }

        private async void button_Req1_Click(object sender, EventArgs e)
        {
            var t = (AgentMessageEnum)numericUpDown_ReqType1.Value;
            var n = (int)numericUpDown_ReqNum1.Value;
            var s = textBox_ReqContent1.Text.Trim();
            var r = await conn1.Request(new AgentRequestArgs(t, n, s));
            textBox_Log1.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private void button_Disconnect1_Click(object sender, EventArgs e)
        {
            conn1.Close();
            label_ConnectStatus1.Text = "Closed";
        }

        private void button_Disconnect2_Click(object sender, EventArgs e)
        {
            conn2.Close();
            label_ConnectStatus2.Text = "Closed";
        }

        private async void button_LogIn2_Click(object sender, EventArgs e)
        {
            var s = string.Format("{0}|{1}|1|0|{0}", textBox_User2.Text, textBox_Psw2.Text);
            var r = await conn2.Request(new AgentRequestArgs(
                AgentMessageEnum.REMOTE_MSG_LOGIN, s
            ));
            textBox_Log2.AppendText(string.Format("response: {0} {1} {2} {3}\r\n", r.CommandType, r.N1, r.N2, r.S));
        }

        private async void button_Req2_Click(object sender, EventArgs e)
        {
            var t = (AgentMessageEnum)numericUpDown_ReqType2.Value;
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
    }
}
