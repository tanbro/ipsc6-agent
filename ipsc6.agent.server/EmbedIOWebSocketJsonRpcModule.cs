using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using EmbedIO.WebSockets;
using StreamJsonRpc;

using LocalRpcTargetFunc = System.Func<EmbedIO.WebSockets.WebSocketModule, EmbedIO.WebSockets.IWebSocketContext, object>;

namespace ipsc6.agent.server
{
    // <summary>
    /// Defines a very simple JsonRpc server.
    /// </summary>
    public class EmbedIOWebSocketJsonRpcModule : WebSocketModule
    {
        public EmbedIOWebSocketJsonRpcModule(string urlPath, IEnumerable<LocalRpcTargetFunc> localRpcTargetCreators, JsonRpcTargetOptions jsonRpcTargetOptions = null) : base(urlPath, true)
        {
            this.localRpcTargetCreators = localRpcTargetCreators ?? throw new ArgumentNullException(nameof(localRpcTargetCreators));
            this.jsonRpcTargetOptions = jsonRpcTargetOptions ?? DefaultJsonRpcTargetOptions;
        }

        ~EmbedIOWebSocketJsonRpcModule()
        {
            foreach (var kv in jsonRpcMap)
            {
                kv.Value.Dispose();
            }
            jsonRpcMap.Clear();
        }

        private readonly JsonRpcTargetOptions jsonRpcTargetOptions;

        public static readonly JsonRpcTargetOptions DefaultJsonRpcTargetOptions = new()
        {
            MethodNameTransform = CommonMethodNameTransforms.CamelCase,
            EventNameTransform = CommonMethodNameTransforms.CamelCase,
        };

        private readonly IEnumerable<LocalRpcTargetFunc> localRpcTargetCreators;

        private readonly ConcurrentDictionary<IWebSocketContext, JsonRpc> jsonRpcMap = new();

        /// <inheritdoc />
        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            EmbedIOWebSocketJsonRpcMessageHandler handler = new(context, new JsonMessageFormatter());
            handler.OnSend += (_, e) =>
            {
                e.SendTask = SendAsync(e.Context, Encoding.UTF8.GetString(e.Message));
            };
            JsonRpc jsonRpc = new(handler);
            var targets = (
                from func in localRpcTargetCreators
                select func(this, context)
            ).ToList();
            foreach (var target in targets)
            {
                jsonRpc.AddLocalRpcTarget(target, jsonRpcTargetOptions);
            }
            jsonRpcMap[context] = jsonRpc;
            jsonRpc.StartListening();
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
            if (jsonRpcMap.TryRemove(context, out JsonRpc value))
            {
                value.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}
