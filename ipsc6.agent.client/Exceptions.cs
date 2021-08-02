using System;

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

    public class ConnecttionLostException : ConnectionException
    {
        public ConnecttionLostException() { }
        public ConnecttionLostException(string message) : base(message) { }
        public ConnecttionLostException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConnectionClosedException : ConnectionException
    {
        public ConnectionClosedException() { }
        public ConnectionClosedException(string message) : base(message) { }
        public ConnectionClosedException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConnectionTimeoutException : ConnectionException
    {
        public ConnectionTimeoutException() { }
        public ConnectionTimeoutException(string message) : base(message) { }
        public ConnectionTimeoutException(string message, Exception inner) : base(message, inner) { }
    }

    public class DisconnectionTimeoutException : ConnectionException
    {
        public DisconnectionTimeoutException() { }
        public DisconnectionTimeoutException(string message) : base(message) { }
        public DisconnectionTimeoutException(string message, Exception inner) : base(message, inner) { }
    }

    public class BaseRequestError : BaseException
    {
        public BaseRequestError() { }
        public BaseRequestError(string message) : base(message) { }
        public BaseRequestError(string message, Exception inner) : base(message, inner) { }
    }

    public class RequestNotCompleteError : BaseRequestError
    {
        public RequestNotCompleteError() { }
        public RequestNotCompleteError(string message) : base(message) { }
        public RequestNotCompleteError(string message, Exception inner) : base(message, inner) { }
    }

    public class RequestTimeoutError : BaseRequestError
    {
        public RequestTimeoutError() { }
        public RequestTimeoutError(string message) : base(message) { }
        public RequestTimeoutError(string message, Exception inner) : base(message, inner) { }
    }

    public class ErrorResponse : BaseRequestError
    {
        public readonly ServerSentMessage Arg;

        public readonly ServerSideAgentErrorCode Code;
        
        public override string Message
        {
            get
            {
                string result = ServerSideAgentErrorCodeDict.Value[Code];
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = Code.ToString();
                }
                return result;
            }
        }

        public ErrorResponse(ServerSentMessage arg) : base()
        {
            Arg = arg;
            Code = (ServerSideAgentErrorCode)arg.N1;
        }

    }

}
