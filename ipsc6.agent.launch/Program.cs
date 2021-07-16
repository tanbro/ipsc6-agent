using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace ipsc6.agent.launch
{
    class Program
    {
        private const string protocolPrefix = @"ipsc6-agent-launch:";
        private const string executableFileName = @"ipsc6.agent.wpfapp.exe";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("Length of args must be 1", nameof(args));
                }
                string href = args[0];

                if (href.StartsWith(protocolPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    href = href.Substring(protocolPrefix.Length);
                }
                else
                {
                    throw new ArgumentException($"Argument must starts with \"{protocolPrefix}\"", nameof(args));
                }

                var strArgs = Uri.UnescapeDataString(href);

                var assembly = Assembly.GetExecutingAssembly();
                var workingDir = Path.GetDirectoryName(assembly.Location);

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
