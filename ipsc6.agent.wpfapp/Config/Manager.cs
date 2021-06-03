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
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Manager));

        static IConfigurationRoot configurationRoot;
        public static IConfigurationRoot ConfigurationRoot => configurationRoot;

        public static void Initialize()
        {
            Reload();
        }

        public static IConfigurationRoot Reload()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var cmdArgs = Environment.GetCommandLineArgs();

            if (cmdArgs.Length > 1)
                logger.DebugFormat("CommandLineArgs: {0}", string.Join(" ", cmdArgs));

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(new string[] {
                    "Config", "settings.json"
                }), optional: false)
                .AddJsonFile(Path.Combine(new string[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    versionInfo.ProductName, "User", "settings.json"
                }), optional: true)
                .AddEnvironmentVariables(prefix: "IPSC6AGENT_")
                .AddCommandLine(cmdArgs);

            configurationRoot = builder.Build();
            return ConfigurationRoot;
        }

    }
}
