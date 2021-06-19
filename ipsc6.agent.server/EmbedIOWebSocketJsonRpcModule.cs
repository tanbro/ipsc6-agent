using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using EmbedIO.WebSockets;
using StreamJsonRpc;

namespace ipsc6.agent.server
{

    // <summary>
    /// Defines a very simple JsonRpc server.
    /// </summary>
    public class EmbedIOWebSocketJsonRpcModule : WebSocketModule
    {
        public EmbedIOWebSocketJsonRpcModule(string urlPath, Func<object> localRpcTargetCreator, JsonRpcTargetOptions jsonRpcTargetOptions = null) : base(urlPath, true)
        {
            localRpcTargetCreator = localRpcTargetCreator ?? throw new ArgumentNullException(nameof(localRpcTargetCreator));
            this.localRpcTargetCreator = () => new object[] { localRpcTargetCreator() };
            this.jsonRpcTargetOptions = jsonRpcTargetOptions ?? DefaultJsonRpcTargetOptions;
        }

        public EmbedIOWebSocketJsonRpcModule(string urlPath, Func<IEnumerable<object>> localRpcTargetCreator, JsonRpcTargetOptions jsonRpcTargetOptions = null) : base(urlPath, true)
        {
            this.localRpcTargetCreator = localRpcTargetCreator ?? throw new ArgumentNullException(nameof(localRpcTargetCreator));
            this.jsonRpcTargetOptions = jsonRpcTargetOptions ?? DefaultJsonRpcTargetOptions;
        }

        readonly Func<IEnumerable<object>> localRpcTargetCreator;
        readonly JsonRpcTargetOptions jsonRpcTargetOptions;

        public static readonly JsonRpcTargetOptions DefaultJsonRpcTargetOptions = new()
        {
            MethodNameTransform = CommonMethodNameTransforms.CamelCase,
            EventNameTransform = CommonMethodNameTransforms.CamelCase,
        };

        static readonly ConcurrentDictionary<IWebSocketContext, JsonRpc> rpcDict = new();
        static readonly ConcurrentDictionary<IWebSocketContext, IEnumerable<object>> targetsDict = new();

        /// <inheritdoc />
        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            var handler = new EmbedIOWebSocketJsonRpcMessageHandler(context, new JsonMessageFormatter());
            handler.OnSend += (_, e) =>
            {
                e.SendTask = SendAsync(e.Context, Encoding.UTF8.GetString(e.Message));
            };

            var rpc = new JsonRpc(handler);
            rpcDict[context] = rpc;
            var targets = localRpcTargetCreator();
            foreach (var target in targets)
            {
                rpc.AddLocalRpcTarget(target, jsonRpcTargetOptions);
            }
            targetsDict[context] = targets;
            rpc.StartListening();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            EmbedIOWebSocketJsonRpcMessageHandler.PushReceivedMessage(context, rxBuffer);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            if (rpcDict.TryRemove(context, out JsonRpc rpc))
                rpc.Dispose();
            targetsDict.TryRemove(context, out _);
            return Task.CompletedTask;
        }
    }
}
