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

namespace ipsc6.agent.wpfapp
{
    internal static class ConfigManager
    {
        public static IConfigurationRoot ConfigurationRoot { get; private set; }

        public static void Initialize()
        {
            Reload();
        }

        public static IConfigurationRoot Reload()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var cmdArgs = Environment.GetCommandLineArgs();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile
                (
                    Path.Combine("Config", "settings.json"),
                    optional: true
                )
#if DEBUG
                .AddJsonFile
                (
                    Path.Combine("Config", "settings.development.json"),
                    optional: true
                )
#endif
                .AddJsonFile
                (
                    Path.Combine
                    (
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        versionInfo.ProductName, "settings.json"
                    ),
                    optional: true
                )
#if DEBUG
                .AddJsonFile
                (
                    Path.Combine
                    (
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        versionInfo.ProductName, "settings.development.json"
                    ),
                    optional: true
                )
#endif
                .AddEnvironmentVariables(prefix: "IPSC6AGENT_")
                .AddCommandLine(cmdArgs);
            ConfigurationRoot = builder.Build();
            return ConfigurationRoot;
        }

    }
}
