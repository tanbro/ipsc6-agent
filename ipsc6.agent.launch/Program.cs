using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Microsoft.Win32;

namespace ipsc6.agent.launch
{
    class Program
    {
        private const string protocolPrefix = @"ipsc6-agent-launch:";
        private const string executableFileName = @"ipsc6.agent.wpfapp.exe";

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("Length of args must be 1", nameof(args));
                }
                var arg_0 = args[0];
                var href = arg_0.StartsWith(protocolPrefix, StringComparison.OrdinalIgnoreCase)
                    ? arg_0.Substring(protocolPrefix.Length)
                    : throw new ArgumentException($"Argument must starts with \"{protocolPrefix}\"", nameof(args));

                var strArgs = Uri.UnescapeDataString(href);

                // WorkingDirectory 路径，按顺序寻找！
                string workingDir = null;
                do
                {
                    // 环境变量
                    workingDir = Environment.GetEnvironmentVariable("IPSC6_AGENT_DIR");
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            break;
                        }
                    }

                    // HKLM Software\ipsc6_agent_wpfapp-win64 (仅 64bits windows)
                    if (Environment.Is64BitOperatingSystem)
                    {
                        workingDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\ipsc6_agent_wpfapp-win64", "", "") as string;
                        if (!string.IsNullOrWhiteSpace(workingDir))
                        {
                            if (File.Exists(Path.Combine(workingDir, executableFileName)))
                            {
                                break;
                            }
                        }
                    }

                    // HKLM Software\ipsc6_agent_wpfapp-win32
                    workingDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\ipsc6_agent_wpfapp-win32", "", "") as string;
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            break;
                        }
                    }

                    // HKCU Software\ipsc6_agent_wpfapp-win64 (仅 64bits windows)
                    if (Environment.Is64BitOperatingSystem)
                    {
                        workingDir = Registry.GetValue(@"HKEY_CURRENT_USER\Software\ipsc6_agent_wpfapp-win64", "", "") as string;
                        if (!string.IsNullOrWhiteSpace(workingDir))
                        {
                            if (File.Exists(Path.Combine(workingDir, executableFileName)))
                            {
                                break;
                            }
                        }
                    }

                    // HKCU Software\ipsc6_agent_wpfapp-win32 (仅 64bits windows)
                    workingDir = Registry.GetValue(@"HKEY_CURRENT_USER\Software\ipsc6_agent_wpfapp-win32", "", "") as string;
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            break;
                        }
                    }

                    // 当前EXE所在目录
                    var assembly = Assembly.GetExecutingAssembly();
                    workingDir = Path.GetDirectoryName(assembly.Location);
                    if (File.Exists(Path.Combine(workingDir, executableFileName)))
                    {
                        break;
                    }

                    // 当前工作目录
                    workingDir = executableFileName;
                    if (File.Exists(executableFileName))
                    {
                        break;
                    }

                    // 最后也没有找到
                    throw new FileNotFoundException("ipsc6-agent 座席客户端可执行文件不存在或无法访问。", executableFileName);

                } while (false);

                using Process process = new();
                {
                    var startInfo = process.StartInfo;
                    startInfo.WorkingDirectory = workingDir;
                    startInfo.FileName = executableFileName;
                    startInfo.Arguments = strArgs;
                    process.Start();
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                Console.WriteLine();
                Console.WriteLine("Press Any Key to continue ...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }
        }
    }
}
