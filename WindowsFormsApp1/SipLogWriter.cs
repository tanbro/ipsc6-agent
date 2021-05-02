using System;
using org.pjsip.pjsua2;

namespace WindowsFormsApp1
{
    public class SipLogWriter: LogWriter
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
