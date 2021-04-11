#pragma once

using namespace System;

namespace ipsc6 {
namespace agent {
namespace network {

public
delegate void ConnectAttemptFailedEventHandler(Object ^ sender);

public
delegate void DisconnectedEventHandler(Object ^ sender);

public
delegate void ConnectionLostEventHandler(Object ^ sender);

public
ref class ConnectedEventArgs : EventArgs {
   public:
    property int ConnectionId;
    ConnectedEventArgs() : EventArgs(){};
    ConnectedEventArgs(int connectionId) : EventArgs() {
        ConnectionId = connectionId;
    };
};

public
delegate void ConnectedEventHandler(Object ^ sender,
                                    ConnectedEventArgs^ e);

public
ref class AgentMessageReceivedEventArgs : EventArgs {
   public:
    property int CommandType;
    property int n1;
    property int n2;
    property String ^ s;
    AgentMessageReceivedEventArgs() : EventArgs(){};
    AgentMessageReceivedEventArgs(int commandType, int n1, int n2, String ^ s)
        : EventArgs() {
        CommandType = commandType;
        this->n1 = n1;
        this->n2 = n2;
        this->s = s;
    };
};

public
delegate void AgentMessageReceivedEventHandler(Object ^ sender,
                                               AgentMessageReceivedEventArgs ^
                                                   e);

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
