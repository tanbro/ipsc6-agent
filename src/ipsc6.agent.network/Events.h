#pragma once

using namespace System;

namespace ipsc6 {
namespace agent {
namespace network {

public
ref class ConnectedEventArgs : EventArgs {
   public:
    ConnectedEventArgs() : EventArgs(){};
};

public
ref class AgentMessageReceivedEventArgs : EventArgs {
   public:
    property int CommandType;
    property int N1;
    property int N2;
    property String ^ S;
    AgentMessageReceivedEventArgs() : EventArgs(){};
    AgentMessageReceivedEventArgs(int commandType, int n1, int n2, String ^ s)
        : EventArgs() {
        CommandType = commandType;
        N1 = n1;
        N2 = n2;
        S = s;
    };
    virtual String ^ ToString() override;
};

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
