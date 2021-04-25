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

namespace SampleWinFormsApp
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Form1));

        public Form1()
        {
            InitializeComponent();
        }

        private static Connection conn1;
        private static Connection conn2;

        private void Form1_Load(object sender, EventArgs e)
        {
            logger.Info("Form1_Load");
            conn1 = new();
            conn2 = new();

            conn1.OnServerSendEventReceived += Conn_OnServerSendEventReceived;
        }

        private void Conn_OnServerSendEventReceived(object sender, AgentMessageReceivedEventArgs e)
        {
            if (sender == conn1)
            {
                textBox_Log1.AppendText(string.Format("\r\nServer-send event: {0} {1} {2}\r\n", e.N1, e.N2, e.S));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.Info("Form1_FormClosing");
            conn1.Dispose();
            conn2.Dispose();
        }

        private async void button_Connect1_Click(object sender, EventArgs e)
        {
            label_ConnectStatus1.Text = "Connecting ...";
            try
            {
                await conn1.Connect(textBox_Server1.Text);
            }
            catch(Exception err)
            {
                label_ConnectStatus1.Text = string.Format("Connect failed: {0}", err.Message);
                throw;
            }
            label_ConnectStatus1.Text = "Connected.";
        }

        private async void button_Connect2_Click(object sender, EventArgs e)
        {
            await conn2.Connect(textBox_Server2.Text);
            label_ConnectStatus2.Text = "Connected";
        }

        private async void button_LogIn1_Click(object sender, EventArgs e)
        {
            var s = string.Format("{0}|{1}|1|0|{0}", textBox_User1.Text, textBox_Psw1.Text);
            var res = await conn1.Request(new AgentRequestArgs(
                AgentMessageEnum.REMOTE_MSG_LOGIN, s
            ));
            MessageBox.Show(string.Format("登录成功。AgentID={0}", res.N2));
        }

        private async void button_LogOut_Click(object sender, EventArgs e)
        {
            await conn1.Request(new AgentRequestArgs(AgentMessageEnum.REMOTE_MSG_RELEASE));
        }
    }
}
