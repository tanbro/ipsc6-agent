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

namespace hesong.plum.client
{
    public partial class frmLogon : Form
    {
        public frmLogon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tableLayoutPanel2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("点击了“登录”按钮！");
            if (G.agent == null)
            {
                string[] addresses = { "192.168.2.107", "192.168.2.108" };
                G.agent = new Agent(addresses);
                await G.agent.StartUp(txtUser.Text.Trim(), txtPass.Text.Trim());
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show($"{G.agent} 不为空，无法登陆");
            }

            Close();
        }
    }
}
