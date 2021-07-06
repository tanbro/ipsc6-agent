using EmbedIO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using LocalRpcTargetFunc = System.Func<EmbedIO.WebSockets.WebSocketModule, EmbedIO.WebSockets.IWebSocketContext, object>;


namespace ipsc6.agent.server
{
    public class Server
    {
        public ushort Port { get; }
        public string Path { get; }

        public Server(LocalRpcTargetFunc localRpcTargetCreator, ushort port = 9696, string path = "/jsonrpc")
        {
            localRpcTargetCreator = localRpcTargetCreator ?? throw new ArgumentNullException(nameof(localRpcTargetCreator));
            localRpcTargetCreators = new LocalRpcTargetFunc[] { localRpcTargetCreator };
            Port = port;
            Path = "/" + path.TrimStart('/');
        }

        public Server(IEnumerable<LocalRpcTargetFunc> localRpcTargetCreators, ushort port = 9696, string path = "/jsonrpc")
        {
            this.localRpcTargetCreators = localRpcTargetCreators ?? throw new ArgumentNullException(nameof(localRpcTargetCreators));
            Port = port;
            Path = "/" + path.TrimStart('/');
        }

        private readonly IEnumerable<LocalRpcTargetFunc> localRpcTargetCreators;

        // Create and configure our web server.

        private WebServer CreateWebServer()
        {
            var urls = new string[] { $"http://127.0.0.1:{Port}/", $"http://localhost:{Port}/" };
            return new WebServer(options => options
                .WithUrlPrefixes(urls)
                .WithMode(HttpListenerMode.EmbedIO)
            )
            // First, we will configure our web server by adding Modules.
            .WithLocalSessionManager()
            .WithModule(new EmbedIOWebSocketJsonRpcModule(Path, localRpcTargetCreators));
        }

        public async Task RunAsync(CancellationToken cancellation)
        {
            using var webServer = CreateWebServer();
            await webServer.RunAsync(cancellation).ConfigureAwait(false);
        }

        public async Task RunAsync()
        {
            using var webServer = CreateWebServer();
            await webServer.RunAsync().ConfigureAwait(false);
        }

    }
}
