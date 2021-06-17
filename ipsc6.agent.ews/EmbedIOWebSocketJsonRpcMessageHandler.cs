using System;
using System.Linq;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using System.Net.WebSockets;

using Microsoft.VisualStudio.Threading;

using StreamJsonRpc;
using StreamJsonRpc.Protocol;

using Nerdbank.Streams;

using EmbedIO.WebSockets;


namespace ipsc6.agent.ews
{
    public class EmbedIOWebSocketJsonRpcMessageHandlerSendEventArgs : EventArgs
    {
        public Task SendTask { get; internal set; }
        public IWebSocketContext Context { get; }
        public byte[] Message { get; }
        public EmbedIOWebSocketJsonRpcMessageHandlerSendEventArgs(IWebSocketContext context, byte[] message) : base()
        {
            Context = context;
            Message = message;
        }
    }

    public delegate void EmbedIOWebSocketJsonRpcMessageHandlerSendEventHandler(object sender, EmbedIOWebSocketJsonRpcMessageHandlerSendEventArgs e);

    public class EmbedIOWebSocketJsonRpcMessageHandler : MessageHandlerBase, IJsonRpcMessageHandler
    {
        static readonly ConcurrentDictionary<IWebSocketContext, EmbedIOWebSocketJsonRpcMessageHandler> handlerDict = new();

        internal static void PushReceivedMessage(IWebSocketContext context, byte[] message)
        {
            var inst = handlerDict[context];
            inst.receiveQueue.Enqueue(message);
        }

        private readonly AsyncQueue<byte[]> receiveQueue = new();

        public event EmbedIOWebSocketJsonRpcMessageHandlerSendEventHandler OnSend;

        public IWebSocketContext Context { get; }
        public EmbedIOWebSocketJsonRpcMessageHandler(IWebSocketContext context, IJsonRpcMessageFormatter formatter) : base(formatter)
        {
            Context = context;
            handlerDict[context] = this;
        }

        /// <inheritdoc />
        public override bool CanRead => Context.WebSocket.State == WebSocketState.Open;

        /// <inheritdoc />
        public override bool CanWrite => Context.WebSocket.State == WebSocketState.Open;

        /// <inheritdoc />
        protected override ValueTask FlushAsync(CancellationToken cancellationToken) => default;

        protected override async ValueTask<JsonRpcMessage> ReadCoreAsync(CancellationToken cancellationToken)
        {
            var data = await receiveQueue.DequeueAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return Formatter.Deserialize(new ReadOnlySequence<byte>(data));
        }

        /// <inheritdoc />
        protected override async ValueTask WriteCoreAsync(JsonRpcMessage content, CancellationToken cancellationToken)
        {
            byte[] data;
            using (var contentSequenceBuilder = new Sequence<byte>())
            {
                Formatter.Serialize(contentSequenceBuilder, content);
                data = contentSequenceBuilder.AsReadOnlySequence.ToArray();
            }
            var e = new EmbedIOWebSocketJsonRpcMessageHandlerSendEventArgs(Context, data);
            OnSend?.Invoke(this, e);
            cancellationToken.ThrowIfCancellationRequested();
            if (e.SendTask != null)
            {
                await e.SendTask.WithCancellation(cancellationToken);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                EmbedIOWebSocketJsonRpcMessageHandler _;
                handlerDict.TryRemove(Context, out _);
            }
            base.Dispose(disposing);
        }
    }
}
