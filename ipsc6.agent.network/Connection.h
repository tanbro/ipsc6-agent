#pragma once

#include "framework.h"

#include <RakNet/RakNetTypes.h>
#include <RakNet/RakPeerInterface.h>

using namespace System;

namespace ipsc6 {
namespace agent {
namespace network {

public ref class Connection {
   public:
    Connection(String ^ address, UInt16 ^ localPort);
    Connection(UInt16 ^ localPort);
    Connection();
    ~Connection();

   private:
    void _initial();

    String ^ _address;
    UInt16 ^ _localPort;
    RakNet::RakPeerInterface* _peer;

   public:
    static void Initial();
    static void Release();

    property String ^ Address { String ^ get() { return _address; } };
    property UInt16 ^ LocalPort { UInt16 ^ get() { return LocalPort; } };

    void ^ StartUp();

};  // ref class Connection

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
