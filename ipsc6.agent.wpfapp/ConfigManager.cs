using System;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ipsc6.agent.wpfapp
{
    internal static class ConfigManager
    {
        public static IConfigurationRoot ConfigurationRoot { get; private set; }

        public static void Initialize()
        {
            GetAllSettings();
        }

        public static string UserSettingsPath
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    versionInfo.ProductName,
                                    "settings.json");
            }
        }

        public static IConfigurationRoot GetUserSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(UserSettingsPath, optional: true);
            ConfigurationRoot = builder.Build();
            return ConfigurationRoot;
        }

        public static IConfigurationRoot GetAllSettings()
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
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
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
