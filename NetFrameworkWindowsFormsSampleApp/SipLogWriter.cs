using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

/// <summary>
/// ref: https://www.pjsip.org/docs/latest/pjlib/docs/html/group__PJ__LOG.htm
/// </summary>

namespace NetFrameworkWindowsFormsSampleApp
{
    class SipLogWriter : LogWriter
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("org.pjsip.pjsua2");
        public override void write(LogEntry entry)
        {
            var message = entry.msg.TrimEnd();
            switch (entry.level)
            {
                case 1:
                    logger.Fatal(message);
                    break;
                case 2:
                    logger.Error(message);
                    break;
                case 3:
                    logger.Warn(message);
                    break;
                case 4:
                    logger.Info(message);
                    break;
                case 5:
                    logger.Debug(message);
                    break;
                case 6:
                    logger.Debug(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalide PJ logging level {0}", entry.level));
            }
        }
    }
}
