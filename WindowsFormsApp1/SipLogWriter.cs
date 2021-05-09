using System;
using org.pjsip.pjsua2;

namespace WindowsFormsApp1
{
    public class SipLogWriter : LogWriter
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("org.pjsip.pjsua2");

        static readonly object _l = new object();
        private static SipLogWriter instance = null;
        public static SipLogWriter Instance
        {
            get
            {
                lock (_l)
                {
                    if (instance == null)
                    {
                        instance = new SipLogWriter();
                    }
                    return instance;
                }
            }
        }

        public override void write(LogEntry entry)
        {
            var message = entry.msg.Trim();
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
                default:
                    logger.Debug(message);
                    break;
            }
        }
    }
}
