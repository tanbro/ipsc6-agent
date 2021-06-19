using System;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;


namespace ipsc6.agent.server
{
    public class Server
    {
        public ushort Port { get; }
        public string Path { get; }
        public Server(Func<object> localRpcTargetCreator, ushort port = 9696, string path = "/jsonrpc")
        {
            Port = port;
            Path = "/" + path.TrimStart('/');
            this.localRpcTargetCreator = localRpcTargetCreator;
        }

        private readonly Func<object> localRpcTargetCreator;

        // Create and configure our web server.

        private WebServer CreateWebServer()
        {
            var url = $"http://localhost:{Port}/";
            return new WebServer(options => options
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO)
            )
            // First, we will configure our web server by adding Modules.
            .WithLocalSessionManager()
            .WithModule(new EmbedIOWebSocketJsonRpcModule(Path, localRpcTargetCreator));
        }

        public async Task RunAsync(CancellationToken cancellation)
        {
            using var webServer = CreateWebServer();
            await webServer.RunAsync(cancellation);
        }

        public async Task RunAsync()
        {
            using var webServer = CreateWebServer();
            await webServer.RunAsync();
        }

    }
}
