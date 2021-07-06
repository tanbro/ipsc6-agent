using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ipsc6.agent.wpfapp.Config
{
    static class Manager
    {
        public static IConfigurationRoot ConfigurationRoot { get; private set; }

        public static void Initialize()
        {
            Reload();
        }

        public static IConfigurationRoot Reload()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var cmdArgs = Environment.GetCommandLineArgs();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(new string[] {
                    "Config", "settings.json"
                }), optional: true)
#if DEBUG
                .AddJsonFile(Path.Combine(new string[] {
                    "Config", "settings.development.json"
                }), optional: true)
#endif
                .AddJsonFile(Path.Combine(new string[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    versionInfo.ProductName, "User", "settings.json"
                }), optional: true)
#if DEBUG
                .AddJsonFile(Path.Combine(new string[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    versionInfo.ProductName, "User", "settings.development.json"
                }), optional: true)
#endif
                .AddEnvironmentVariables(prefix: "IPSC6AGENT_")
                .AddCommandLine(cmdArgs);
            ConfigurationRoot = builder.Build();
            return ConfigurationRoot;
        }

    }
}
