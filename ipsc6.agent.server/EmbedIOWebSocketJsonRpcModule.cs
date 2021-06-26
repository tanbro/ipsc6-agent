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
            this.localRpcTargetCreator = localRpcTargetCreator;
            this.jsonRpcTargetOptions = jsonRpcTargetOptions ?? DefaultJsonRpcTargetOptions;
        }

        private readonly Func<object> localRpcTargetCreator;
        private readonly JsonRpcTargetOptions jsonRpcTargetOptions;

        public static readonly JsonRpcTargetOptions DefaultJsonRpcTargetOptions = new()
        {
            MethodNameTransform = CommonMethodNameTransforms.CamelCase,
            EventNameTransform = CommonMethodNameTransforms.CamelCase,
        };

        private static readonly ConcurrentDictionary<IWebSocketContext, JsonRpc> mapWsRpc = new();

        /// <inheritdoc />
        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            EmbedIOWebSocketJsonRpcMessageHandler handler = new(context, new JsonMessageFormatter());
            handler.OnSend += (_, e) =>
            {
                e.SendTask = SendAsync(e.Context, Encoding.UTF8.GetString(e.Message));
            };
            JsonRpc rpc = new(handler);
            mapWsRpc[context] = rpc;
            var target = localRpcTargetCreator();
            if (target is IEnumerable<object>)
            {
                foreach (var obj in target as IEnumerable<object>)
                {
                    rpc.AddLocalRpcTarget(obj, jsonRpcTargetOptions);
                }
            }
            else
            {
                rpc.AddLocalRpcTarget(target, jsonRpcTargetOptions);
            }
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
            if (mapWsRpc.TryRemove(context, out JsonRpc rpc))
            {
                rpc.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}
