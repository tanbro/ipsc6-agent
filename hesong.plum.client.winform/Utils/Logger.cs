using System;
using hesong.Lib.Log;


namespace hesong.plum.client.Utils
{
    public class Logger
    {
        static Logger()
        {
            try
            {
                // 日志保存器对象
                _logSaverInfo = new LogSaver(Log4NetConfigFile, Log4NetLaggerName);
                _loggerDebug = new LogSaver(Log4NetConfigFile, Log4NetLaggerNameDebug);
                _loggerWarn = new LogSaver(Log4NetConfigFile, Log4NetLaggerNameWarn);
                _loggerError = new LogSaver(Log4NetConfigFile, Log4NetLaggerNameError);
                _loggerFatal = new LogSaver(Log4NetConfigFile, Log4NetLaggerNameFatal);
                LogSaver = new SaveLog();
            }
            catch
            {
                Console.Out.WriteLine("error for init LogSaver from file 'hesong.log4net'.");
            }
        }


        #region 日志相关
        #region 常量
        /// <summary>log4netConfig文件路径</summary>
        private const string Log4NetConfigFile = "hesong.log4net";
        /// <summary>log4netLogger日志保存器名称(Info)</summary>
        private const string Log4NetLaggerName = "hesong.Info";
        /// <summary>log4netLogger日志保存器名称(Debug)</summary>
        private const string Log4NetLaggerNameDebug = "hesong.Debug";
        /// <summary>log4netLogger日志保存器名称(Warn)</summary>
        private const string Log4NetLaggerNameWarn = "hesong.Warn";
        /// <summary>log4netLogger日志保存器名称(Error)</summary>
        private const string Log4NetLaggerNameError = "hesong.Error";
        /// <summary>log4netLogger日志保存器名称(Fatal)</summary>
        private const string Log4NetLaggerNameFatal = "hesong.Fatal";

        #endregion

        #region 成员及属性
        public static ISaveLog LogSaver { get; set; }
        /// <summary>日志保存器对象</summary>
        private static readonly LogSaver _logSaverInfo;
        /// <summary>日志保存器对象</summary>
        public static LogSaver LogSaverInfo
        {
            get { return _logSaverInfo; }
        }
        /// <summary>日志保存器对象(Debug)</summary>
        private static readonly LogSaver _loggerDebug;
        /// <summary>日志保存器对象(Debug)</summary>
        public static LogSaver LoggerDebug
        {
            get { return _loggerDebug; }
        }
        /// <summary>日志保存器对象(Warn)</summary>
        private static readonly LogSaver _loggerWarn;
        /// <summary>日志保存器对象(Warn)</summary>
        public static LogSaver LoggerWarn
        {
            get { return _loggerWarn; }
        }
        /// <summary>日志保存器对象(Error)</summary>
        private static readonly LogSaver _loggerError;
        /// <summary>日志保存器对象(Error)</summary>
        public static LogSaver LoggerError
        {
            get { return _loggerError; }
        }
        /// <summary>日志保存器对象(Fatal)</summary>
        private static readonly LogSaver _loggerFatal;
        /// <summary>日志保存器对象(Fatal)</summary>
        public static LogSaver LoggerFatal
        {
            get { return _loggerFatal; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void Log(string logContent)
        {
            LogSaverInfo.Log(LogType.INFO, logContent);
        }
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logContent">日志内容</param>
        public static void Log(string logType, string logContent)
        {
            LogSaver ls;
            switch (logType.ToUpper().Trim())
            {
                case "WARN":
                    ls = LoggerWarn;
                    break;
                case "ERROR":
                    ls = LoggerError;
                    break;
                case "FATAL":
                    ls = LoggerFatal;
                    break;
                case "DEBUG":
                    ls = LoggerDebug;
                    break;
                case "Info":
                    ls = LogSaverInfo;
                    break;
                default:
                    ls = LogSaverInfo;
                    break;
            }
            ls.Log(logType, logContent);
        }
        #endregion
        #endregion
    }
}
