using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using hesong.plum.client.Utils;
using ipsc6.agent.network;

namespace hesong.plum.client
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Log("ipsc6.agent.network Initial");
            Connector.Initial();
            try
            {

                Logger.Log("program starting...");
                Application.Run(new frmSoftTel());
            }
            finally
            {
                Logger.Log("ipsc6.agent.network Release");
                Connector.Release();
            }
        }
    }
}
