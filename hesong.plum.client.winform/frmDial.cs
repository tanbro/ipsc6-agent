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
    public partial class frmDial : Form
    {
        private Controls.UserControl2 _ucCallOut;
        public frmDial(Controls.UserControl2 ucCallOut)
        {
            InitializeComponent();
            _ucCallOut = ucCallOut;
        }

        private void butBack_Click(object sender, EventArgs e)
        {

        }

        private void butClose_Click(object sender, EventArgs e)
        {

        }

        private void butDial_Click(object sender, EventArgs e)
        {
            _ucCallOut.CallOutNo = txtNo.Text;
        }

        private void frmDial_Load(object sender, EventArgs e)
        {
            Height = 685;
            Width = 470;
            txtNo.Width = 350;
            txtNo.Height = 50;
        }
    }
}
