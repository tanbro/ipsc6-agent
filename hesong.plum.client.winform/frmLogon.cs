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


        private async void Button_LogIn_Click(object sender, EventArgs e)
        {
            string[] addresses = { "192.168.2.107", "192.168.2.108" };
            Control[] disableControls = { sender as Control, txtUser, txtPass };
            using (new Utils.WaitCursorBlock(disableControls))
            {
                if (G.agent != null)
                {
                    MessageBox.Show($"{G.agent} 不为空，无法登陆");
                    return;
                }

                G.agent = new Agent(addresses);
                try
                {
                    await G.agent.StartUp(txtUser.Text.Trim(), txtPass.Text.Trim());
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (ConnectionException err)
                {
                    if (G.agent.GetConnectionState(G.agent.MainConnectionIndex) == ipsc6.agent.client.ConnectionState.Ok)
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        G.agent.Dispose();
                        G.agent = null;
                        MessageBox.Show($"{err}", "登陆失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }
    }

}
