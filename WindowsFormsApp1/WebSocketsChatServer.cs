using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmbedIO.WebSockets;

namespace WindowsFormsApp1
{
    class WebSocketsChatServer : WebSocketModule
    {
        public WebSocketsChatServer(string urlPath)
            : base(urlPath, true)
        {
            // placeholder
        }

        /// <inheritdoc />
        protected override async Task OnMessageReceivedAsync(
            IWebSocketContext context,
            byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
        {
            await SendToOthersAsync(context, Encoding.GetString(rxBuffer));
        }

        /// <inheritdoc />
        protected override async Task OnClientConnectedAsync(IWebSocketContext context)
        {
            await Task.WhenAll(
                SendAsync(context, "Welcome to the chat room!"),
                SendToOthersAsync(context, "Someone joined the chat room."));
        }

        /// <inheritdoc />
        protected override async Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            await SendToOthersAsync(context, "Someone left the chat room.");
        }

        private async Task SendToOthersAsync(IWebSocketContext context, string payload)
        {
            await BroadcastAsync(payload, c => c != context);
        }

        public async Task Broadcast(byte[] payload)
        {
            await BroadcastAsync(payload);
        }

        public async Task Broadcast(string payload)
        {
            await BroadcastAsync(payload);
        }
    }
}
