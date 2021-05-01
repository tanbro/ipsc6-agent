using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    static class Program
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));

            logger.Info("=============== startup ===============");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ipsc6.agent.network.Connector.Initial();
            try
            {
                logger.Info("Application.Run() >>>");
                try
                {
                    Application.Run(new Form1());
                }
                finally
                {
                    logger.Info("Application.Run() <<<");
                }
            }
            finally
            {
                logger.Info("ipsc6.agent.network.Connector.Release() >>>");
                ipsc6.agent.network.Connector.Release();
                logger.Info("ipsc6.agent.network.Connector.Release() <<<");
            }
            logger.Info("=============== shutdown ===============");
        }
    }
}
