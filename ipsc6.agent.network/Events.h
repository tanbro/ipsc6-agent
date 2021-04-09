#pragma once

using namespace System;

namespace ipsc6 {
namespace agent {
namespace network {

public
delegate void ConnectAttemptFailedEventHandler();

public
delegate void DisconnectedEventHandler();

public
delegate void ConnectionLostEventHandler();

public
delegate void ConnectedEventHandler(int);

public
delegate void AgentMessageReceivedEventHandler(int commandType, int n1, int n2, String^ s);

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
