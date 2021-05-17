using System;
using System.IO;
using System.Xml;
using log4net;

namespace hesong.Lib.Log
{
    /// <summary>
    /// 日志保存器封装类
    /// </summary>
    public class LogSaver
    {
        /// <summary>日志保存器实例</summary>
        private ILog _saver = null;
        /// <summary>日志保存器实例</summary>
        public ILog Saver
        {
            get { return _saver; }
            set { _saver = value; }
        }

        /// <summary>log4net的配置文件名</summary>
        private string _log4netConfigFile;
        /// <summary>log4net的配置文件名</summary>
        public string Log4netConfigFile
        {
            get { return _log4netConfigFile; }
            set
            {
                _log4netConfigFile = value;
                FileInfo fi = new FileInfo(_log4netConfigFile);
                log4net.Config.XmlConfigurator.Configure(fi);
            }
        }

        /// <summary>log4net的配置文件名</summary>
        private string _logSaverName;
        /// <summary>log4net的配置文件名</summary>
        public string LogSaverName
        {
            get { return _logSaverName; }
            set
            {
                _logSaverName = value;
                _saver = LogManager.GetLogger(_logSaverName);
            }
        }


        /// <summary>
        /// 默认构造方法
        /// </summary>
        /// <param name="log4netConfigFile">log4net配置文件名称</param>
        public LogSaver(string log4netConfigFile)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netConfigFile));
        }
        /// <summary>
        /// 构造方法，初始化日志保存器
        /// </summary>
        /// <param name="log4netConfigFile">log4net配置文件名称</param>
        /// <param name="logSaverName">log4net适配器节点名称</param>
        public LogSaver(string log4netConfigFile,string logSaverName)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netConfigFile));
            _saver = LogManager.GetLogger(logSaverName);
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log4netConfigXml">log4net配置XML节点</param>
        public LogSaver(XmlElement log4netConfigXml)
        {
            log4net.Config.XmlConfigurator.Configure(log4netConfigXml);
        }
        /// <summary>
        /// 构造方法，初始化日志保存器
        /// </summary>
        /// <param name="log4netConfigXml">log4net配置XML节点</param>
        /// <param name="logSaverName">log4net适配器节点名称</param>
        public LogSaver(XmlElement log4netConfigXml, string logSaverName)
        {
            log4net.Config.XmlConfigurator.Configure(log4netConfigXml);
            _saver = LogManager.GetLogger(logSaverName);
        }

        /// <summary>
        /// 输出日志（默认日志类型为：LogType.INFO）
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void Log(string logMessage)
        {
            Log(LogType.INFO, logMessage);
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logMessage">日志消息体</param>
        public void Log(LogType logType, string logMessage)
        {
            if (_saver != null)
            {
                switch (logType)
                {
                    case LogType.INFO:
                        _saver.Info(logMessage);
                        break;
                    case LogType.DEBUG:
                        _saver.Debug(logMessage);
                        break;
                    case LogType.WARN:
                        _saver.Warn(logMessage);
                        break;
                    case LogType.ERROR:
                        _saver.Error(logMessage);
                        break;
                    case LogType.FATAL:
                        _saver.Fatal(logMessage);
                        break;
                }
            }
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logMessage">日志消息体</param>
        public void Log(string logType, string logMessage)
        {
            if (_saver != null)
            {
                switch (logType.ToUpper())
                {
                    case "INFO":
                        _saver.Info(logMessage);
                        break;
                    case "DEBUG":
                        _saver.Debug(logMessage);
                        break;
                    case "WARN":
                        _saver.Warn(logMessage);
                        break;
                    case "ERROR":
                        _saver.Error(logMessage);
                        break;
                    case "FATAL":
                        _saver.Fatal(logMessage);
                        break;
                    default:
                        _saver.Info(logMessage);
                        break;
                }
            }
        }
        /// <summary>
        /// 保存Info日志
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void Info(string logMessage)
        {
            if (_saver != null)
                _saver.Info(logMessage);
        }
        /// <summary>
        /// 保存Debug日志
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void Debug(string logMessage)
        {
            if (_saver != null)
                _saver.Debug(logMessage);
        }
        /// <summary>
        /// 保存Warn日志
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void Warn(string logMessage)
        {
            if (_saver != null)
                _saver.Warn(logMessage);
        }
        /// <summary>
        /// 保存Error日志
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void Error(string logMessage)
        {
            if (_saver != null)
                _saver.Error(logMessage);
        }
        /// <summary>
        /// 保存Fatal日志
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void Fatal(string logMessage)
        {
            if (_saver != null)
                _saver.Fatal(logMessage);
        }

        /// <summary>
        /// 在控制台界面输出日志信息(Console.Out.WriteLine)（默认日志类型为：LogType.INFO）
        /// </summary>
        /// <param name="logMessage">日志消息体</param>
        public void COW(string logMessage)
        {
            COW(LogType.INFO, logMessage);
        }
        /// <summary>
        /// 在控制台界面输出日志信息(Console.Out.WriteLine)
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logMessage">日志消息体</param>
        public void COW(LogType logType, string logMessage)
        {
            Console.Out.WriteLine("{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logType, logMessage);
        }
        /// <summary>
        /// 在控制台界面输出日志信息(Console.Out.WriteLine)
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logMessage">日志消息体</param>
        public void COW(string logType, string logMessage)
        {
            Console.Out.WriteLine("{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logType, logMessage);
        }

        /// <summary>
        /// 日志类型转换。由LogType枚举类型转换成字符串类型
        /// </summary>
        /// <param name="logType">LogType枚举类型</param>
        /// <returns>字符串日志类型</returns>
        public static string LogTypeConvert(LogType logType)
        {
            string Result;
            switch (logType)
            {
                case LogType.INFO:
                    Result = "Info";
                    break;
                case LogType.DEBUG:
                    Result = "Debug";
                    break;
                case LogType.WARN:
                    Result = "Warn";
                    break;
                case LogType.ERROR:
                    Result = "Error";
                    break;
                case LogType.FATAL:
                    Result = "Fatal";
                    break;
                default:
                    Result = "Info";
                    break;
            }
            return Result;
        }
        /// <summary>
        /// 日志类型转换。由字符串类型转换成LogType枚举类型
        /// </summary>
        /// <param name="logType">字符串日志类型</param>
        /// <returns>LogType枚举类型</returns>
        public static LogType LogTypeConvert(string logType)
        {
            LogType Result;
            logType = logType.Trim().ToUpper();
            switch (logType)
            {
                case "INFO":
                    Result = LogType.INFO;
                    break;
                case "DEBUG":
                    Result = LogType.DEBUG;
                    break;
                case "WARN":
                    Result = LogType.WARN;
                    break;
                case "ERROR":
                    Result = LogType.ERROR;
                    break;
                case "FATAL":
                    Result = LogType.FATAL;
                    break;
                default:
                    Result = LogType.INFO;
                    break;
            }
            return Result;
        }
    }

    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>调试日志输出</summary>
        DEBUG,
        /// <summary>异常日志输出</summary>
        ERROR,
        /// <summary>致命错误日志输出</summary>
        FATAL,
        /// <summary>消息日志输出</summary>
        INFO,
        /// <summary>告警日志输出</summary>
        WARN
    }

    ///<summary>
    /// 委托：保存日志
    ///</summary>
    ///<param name="logType">日志类型 (Info,Debug,Warn,Error,Fatal)</param>
    ///<param name="logMessage">日志消息体</param>
    public delegate void DelegateOnLog_Str(string logType, string logMessage);

    ///<summary>
    /// 委托：保存日志
    ///</summary>
    ///<param name="logType">日志类型 (LogType枚举)</param>
    ///<param name="logMessage">日志消息体</param>
    public delegate void DelegateOnLog(LogType logType, string logMessage);
}