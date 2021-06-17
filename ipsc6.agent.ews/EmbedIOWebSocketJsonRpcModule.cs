using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using EmbedIO.WebSockets;
using StreamJsonRpc;

namespace ipsc6.agent.ews
{
    // <summary>
    /// Defines a very simple JsonRpc server.
    /// </summary>
    public class EmbedIOWebSocketJsonRpcModule : WebSocketModule
    {
        public EmbedIOWebSocketJsonRpcModule(string urlPath) : base(urlPath, true) { }

        static readonly JsonRpcTargetOptions jsonRpcTargetOptions = new()
        {
            MethodNameTransform = CommonMethodNameTransforms.CamelCase,
            EventNameTransform = CommonMethodNameTransforms.CamelCase,
        };

        static readonly ConcurrentDictionary<IWebSocketContext, JsonRpc> jsonRpcDict = new();
        static readonly ConcurrentDictionary<IWebSocketContext, Service> serviceDict = new();

        /// <inheritdoc />
        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            var handler = new EmbedIOWebSocketJsonRpcMessageHandler(context, new JsonMessageFormatter());
            handler.OnSend += (_, e) =>
            {
                e.SendTask = SendAsync(e.Context, Encoding.UTF8.GetString(e.Message));
            };

            var rpc = new JsonRpc(handler);
            jsonRpcDict[context] = rpc;

            var service = new Service();
            serviceDict[context] = service;

            rpc.AddLocalRpcTarget(service, jsonRpcTargetOptions);
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
            if (jsonRpcDict.TryRemove(context, out JsonRpc rpc))
                rpc.Dispose();
            serviceDict.TryRemove(context, out _);
            return Task.CompletedTask;
        }
    }
}
