using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            agent.OnConnectionStateChanged += Agent_OnConnectionStateChanged;
        }

        private void Agent_OnConnectionStateChanged(object sender, ConnectionInfoStateChangedEventArgs<ipsc6.agent.client.ConnectionState> e)
        {
            var s = string.Format("ConnectionState: {0}: {1}==>{2}", e.ConnectionInfo.Host, e.OldState, e.NewState);
            textBox_eventLog.AppendText(string.Format("{0}\r\n", s));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button_Init_Click(object sender, EventArgs e)
        {
            var s = textBox_ServerAddressList.Text.Trim();
            var addresses = s.Split(new char[] { ',' });

            ReallocAgent(addresses);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (agent!=null)
            {
                agent.Dispose();
            }
        }

        private async void button_open_Click(object sender, EventArgs e)
        {
            var workNum = textBox_workerNum.Text.Trim();
            var password = textBox_password.Text.Trim();
            await agent.StartUp(workNum, password);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            agent.ShutDown();
        }
    }
}
