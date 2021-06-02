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
    class Manager
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

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(new string[] {
                    "Config", "settings.json"
                }),
                optional: false)
                .AddJsonFile(Path.Combine(new string[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    versionInfo.ProductName, "User", "settings.json"
                }), optional: true)
                .AddEnvironmentVariables(prefix: "IPSC6AGENT_");

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                logger.DebugFormat("args = {0}", string.Join(", ", args));
                builder.AddCommandLine(args);
            }

            configurationRoot = builder.Build();

            {
                var cfg = ConfigurationRoot;
                var options = new Ipsc();
                cfg.GetSection(nameof(Ipsc)).Bind(options);
                logger.DebugFormat("ServerList: {0}", string.Join(", ", options.ServerList));
                logger.DebugFormat("Name: {0}", options.Name);
            }

            return ConfigurationRoot;
        }

    }
}
