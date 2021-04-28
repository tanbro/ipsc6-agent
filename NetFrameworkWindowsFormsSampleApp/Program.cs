using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ipsc6.agent.network;

namespace NetFrameworkWindowsFormsSampleApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Connector.Initial();
            try
            {
                SipClient.endpoint.libCreate();
                try
                {
                    var epCfg = new org.pjsip.pjsua2.EpConfig();
                    epCfg.logConfig.level = 6;
                    epCfg.logConfig.writer = new SipLogWriter();
                    SipClient.endpoint.libInit(epCfg);

                    var sipTpConfig = new org.pjsip.pjsua2.TransportConfig
                    {
                        port = 5060
                    };
                    SipClient.endpoint.transportCreate(org.pjsip.pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, sipTpConfig);
                    SipClient.endpoint.libStart();

                    Application.Run(new Form1());
                }
                finally
                {
                    SipClient.endpoint.libDestroy();
                }
            }
            finally
            {
                Connector.Release(); 
            }
        }
    }
}
