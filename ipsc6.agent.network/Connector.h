#pragma once

/* clang-format off */
#include "framework.h"
/* clang-format on */

#include <RakNet/RakNetTypes.h>
#include <RakNet/RakPeerInterface.h>
#include "Events.h"

using namespace System;
using namespace System::Threading;
using namespace System::Collections::Generic;

namespace ipsc6 {
namespace agent {
namespace network {

static RakNet::MessageID getPacketIdentifier(const RakNet::Packet* packet);

public
ref class Connector {
   public:
    Connector(UInt16 localPort, String ^ address);
    ~Connector();

   private:
    static HashSet<Connector ^> ^ connectors;

    static EventWaitHandle ^ receiveThreadStarted;
    static Thread ^ receiveThread;
    static Boolean receiveThreadStopping;
    static void ReceiveThreadProc();

    String ^ _address;
    UInt16 _localPort;
    UInt32 _connectionId;
    RakNet::RakPeerInterface* _peer;

    void StartUp();
    void Shutdown();
    int Receive();

   public:
    static void Initial();
    static void Release();

    static Connector ^ CreateInstance(UInt16 localPort, String ^ address);
    static Connector ^ CreateInstance(UInt16 localPort);
    static Connector ^ CreateInstance();
    static void DeallocateInstance(Connector ^ connector);

    void Connect(String ^ host, UInt16 remotePort);

    event ConnectAttemptFailedEventHandler ^ OnConnectAttemptFailed;
    event ConnectedEventHandler ^ OnConnected;

};  // ref class Connector

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
