

namespace hesong.plum.client.Utils
{
    /// <summary>
    /// 保存日志接口
    /// </summary>
    public interface ISaveLog
    {
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logMsg"></param>
        void Log(string logType, string logMsg);
        /// <summary>
        /// 保存info日志
        /// </summary>
        /// <param name="logMsg"></param>
        void Log(string logMsg);
    }
}
