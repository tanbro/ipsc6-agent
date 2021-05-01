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
        public Form1()
        {
            InitializeComponent();
        }

        static Agent agent = null;

        static void ReallocAgent(string workerNum, IEnumerable<string> serverAddreses)
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
            agent = new Agent(workerNum, addrList);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button_Init_Click(object sender, EventArgs e)
        {
            var s = textBox_ServerAddressList.Text.Trim();
            var addresses = s.Split(new char[] { ',' });
            var workNum = textBox_workerNum.Text.Trim();
            var _ = textBox_password.Text.Trim();

            ReallocAgent(workNum, addresses);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (agent!=null)
            {
                agent.Dispose();
            }
        }
    }
}
