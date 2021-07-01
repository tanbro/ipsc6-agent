using EmbedIO.WebSockets;
using Microsoft.VisualStudio.Threading;
using Nerdbank.Streams;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;


namespace ipsc6.agent.server
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

    public class EmbedIOWebSocketJsonRpcMessageHandler : MessageHandlerBase
    {
        private static readonly ConcurrentDictionary<IWebSocketContext, EmbedIOWebSocketJsonRpcMessageHandler> instanceMap = new();

        internal static void PushReceivedMessage(IWebSocketContext context, byte[] message)
        {
            instanceMap[context].receiveQueue.Enqueue(message);
        }

        private readonly AsyncQueue<byte[]> receiveQueue = new();

        internal event EmbedIOWebSocketJsonRpcMessageHandlerSendEventHandler OnSend;

        public IWebSocketContext Context { get; }
        public EmbedIOWebSocketJsonRpcMessageHandler(IWebSocketContext context, IJsonRpcMessageFormatter formatter) : base(formatter)
        {
            Context = context;
            instanceMap[context] = this;
        }

        /// <inheritdoc />
        public override bool CanRead => Context.WebSocket.State == WebSocketState.Open;

        /// <inheritdoc />
        public override bool CanWrite => Context.WebSocket.State == WebSocketState.Open;

        /// <inheritdoc />
        protected override ValueTask FlushAsync(CancellationToken cancellationToken) => default;

        protected override async ValueTask<JsonRpcMessage> ReadCoreAsync(CancellationToken cancellationToken)
        {
            byte[] data = await receiveQueue.DequeueAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return Formatter.Deserialize(new ReadOnlySequence<byte>(data));
        }

        /// <inheritdoc />
        protected override async ValueTask WriteCoreAsync(JsonRpcMessage content, CancellationToken cancellationToken)
        {
            byte[] data;
            using (Sequence<byte> bufferWriter = new())
            {
                Formatter.Serialize(bufferWriter, content);
                data = bufferWriter.AsReadOnlySequence.ToArray();
            }
            EmbedIOWebSocketJsonRpcMessageHandlerSendEventArgs e = new(Context, data);
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
                {
                    instanceMap.TryRemove(Context, out _);
                }
                {
                    receiveQueue.Complete();
                    while (!receiveQueue.IsEmpty)
                    {
                        receiveQueue.TryDequeue(out _);
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
