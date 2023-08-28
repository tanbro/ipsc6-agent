using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ipsc6.agent.launch
{
    class Program
    {
        private const string protocolPrefix = @"ipsc6-agent-launch:";
        private const string executableFileName = @"ipsc6.agent.wpfapp.exe";
        private const string envWorkingDir = @"IPSC6AGENT_DIR";

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
                Console.WriteLine("\n\nArguments: {0}\n", strArgs);

                // WorkingDirectory 路径，按顺序寻找！
                string workingDir;
                do
                {
                    // 环境变量
                    workingDir = Environment.GetEnvironmentVariable(envWorkingDir);
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        Console.WriteLine("\nLookup in {0} ...", workingDir);
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            Console.WriteLine("Found!");
                            break;
                        }
                    }

                    // HKCU Software\ipsc6_agent_wpfapp-win64 (win64/user)
                    if (Environment.Is64BitOperatingSystem)
                    {
                        workingDir = Registry.GetValue(@"HKEY_CURRENT_USER\Software\ipsc6_agent_wpfapp-win64", "", "") as string;
                        if (!string.IsNullOrWhiteSpace(workingDir))
                        {
                            Console.WriteLine("\nLookup in {0} ...", workingDir);
                            if (File.Exists(Path.Combine(workingDir, executableFileName)))
                            {
                                Console.WriteLine("Found!");
                                break;
                            }
                        }
                    }

                    // HKCU Software\ipsc6_agent_wpfapp-win32 (win32/user)
                    workingDir = Registry.GetValue(@"HKEY_CURRENT_USER\Software\ipsc6_agent_wpfapp-win32", "", "") as string;
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        Console.WriteLine("\nLookup in {0} ...", workingDir);
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            Console.WriteLine("Found!");
                            break;
                        }
                    }

                    // HKLM Software\ipsc6_agent_wpfapp-win64 (win64/system)
                    if (Environment.Is64BitOperatingSystem)
                    {
                        workingDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\ipsc6_agent_wpfapp-win64", "", "") as string;
                        if (!string.IsNullOrWhiteSpace(workingDir))
                        {
                            Console.WriteLine("\nLookup in {0} ...", workingDir);
                            if (File.Exists(Path.Combine(workingDir, executableFileName)))
                            {
                                Console.WriteLine("Found!");
                                break;
                            }
                        }
                    }

                    // HKLM Software\ipsc6_agent_wpfapp-win32 (win32/system)
                    workingDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\ipsc6_agent_wpfapp-win32", "", "") as string;
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        Console.WriteLine("\nLookup in {0} ...", workingDir);
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            Console.WriteLine("Found!");
                            break;
                        }
                    }

                    // 当前EXE所在目录
                    var assembly = Assembly.GetExecutingAssembly();
                    workingDir = Path.GetDirectoryName(assembly.Location);
                    if (!string.IsNullOrWhiteSpace(workingDir))
                    {
                        Console.WriteLine("\nLookup in {0} ...", workingDir);
                        if (File.Exists(Path.Combine(workingDir, executableFileName)))
                        {
                            Console.WriteLine("Found!");
                            break;
                        }
                    }

                    // 不管了，直接在当前工作目录执行！
                    Console.WriteLine("\nNoting was found, use Default WorkingDirectory instead");
                    workingDir = "";
                } while (false);

                Console.WriteLine("\nWorkingDirectory: {0}\n", workingDir);

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
                Environment.ExitCode = 1;
                Console.Error.WriteLine(exception);
                Console.WriteLine();
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }
    }
}
