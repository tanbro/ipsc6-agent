#pragma once

using namespace System;

namespace ipsc6 {
namespace agent {
namespace network {

public
delegate void ConnectAttemptFailedEventHandler();

public
delegate void ConnectedEventHandler(int);

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
