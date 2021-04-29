using System;
using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    public class BaseException : Exception
    {
        public BaseException() { }
        public BaseException(string message) : base(message) { }
        public BaseException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConnectionException : BaseException
    {
        public ConnectionException() { }
        public ConnectionException(string message) : base(message) { }
        public ConnectionException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConnectionFailedException : ConnectionException
    {
        public ConnectionFailedException() { }
        public ConnectionFailedException(string message) : base(message) { }
        public ConnectionFailedException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConnectLostException : ConnectionException
    {
        public ConnectLostException() { }
        public ConnectLostException(string message) : base(message) { }
        public ConnectLostException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConnectionClosedException : ConnectionException
    {
        public ConnectionClosedException() { }
        public ConnectionClosedException(string message) : base(message) { }
        public ConnectionClosedException(string message, Exception inner) : base(message, inner) { }
    }

    public class BaseRequestError : BaseException
    {
        public BaseRequestError() { }
        public BaseRequestError(string message) : base(message) { }
        public BaseRequestError(string message, Exception inner) : base(message, inner) { }
    }

    public class RequestTimeoutError : BaseRequestError
    {
        public RequestTimeoutError() { }
        public RequestTimeoutError(string message) : base(message) { }
        public RequestTimeoutError(string message, Exception inner) : base(message, inner) { }
    }

    public class ServerSendError : BaseException
    {
        public ServerSendError() { }
        public ServerSendError(string message) : base(message) { }
        public ServerSendError(string message, Exception inner) : base(message, inner) { }
    }

    public class ErrorResponse : BaseRequestError
    {
        public readonly AgentMessageReceivedEventArgs Arg;

        static string MakeMessage(AgentMessageReceivedEventArgs arg)
        {
            return string.Format("ErrorResponse: {0}", arg);
        }

        public ErrorResponse(AgentMessageReceivedEventArgs arg) : base(MakeMessage(arg))
        {
            Arg = arg;
        }

        public ErrorResponse(AgentMessageReceivedEventArgs arg, Exception inner) : base(MakeMessage(arg), inner)
        {
            Arg = arg;
        }
    }

}
