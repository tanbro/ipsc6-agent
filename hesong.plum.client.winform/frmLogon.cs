using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private void tableLayoutPanel2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("点击了“登录”按钮！");

            if (txtUser.Text == "1000" && txtPass.Text == "123456")
            {
                MessageBox.Show("登录成功");
                // 。。。。
            }
            
            Close();
        }
    }
}
