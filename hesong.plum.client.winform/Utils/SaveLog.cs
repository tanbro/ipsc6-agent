

namespace hesong.plum.client.Utils
{
    /// <summary>
    /// 记日志类
    /// </summary>
    public class SaveLog : ISaveLog
    {
        public void Log(string logType, string logMsg)
        {
            Logger.Log(logType, logMsg);
        }

        public void Log(string logMsg)
        {
            Logger.Log(logMsg);
        }
    }
}
