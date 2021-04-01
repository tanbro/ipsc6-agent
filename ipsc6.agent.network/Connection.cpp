#include "Connection.h"

#include <RakNet/BitStream.h>
#include <RakNet/GetTime.h>
#include <RakNet/MessageIdentifiers.h>

namespace ipsc6 {
namespace agent {
namespace network {

Connection::Connection(String ^ address, UInt16 ^ localPort) {
    _address = address;
    _localPort = localPort;
    _initial();
}

Connection::Connection(UInt16 ^ localPort) {
    _address = "";
    _localPort = localPort;
    _initial();
}

Connection::Connection() {
    _address = "";
    _localPort = Convert::ToUInt16(0);
    _initial();
}

Connection::~Connection() {
    RakNet::RakPeerInterface::DestroyInstance(_peer);
}

void Connection::_initial() {
    _peer = RakNet::RakPeerInterface::GetInstance();
}

void Connection::Initial() {
    throw gcnew NotImplementedException();
}

void Connection::Release() {
    throw gcnew NotImplementedException();
}

void ^ Connection::StartUp() {
    RakNet::SocketDescriptor socketDescriptor(0, "");
    RakNet::StartupResult startupResult =
        _peer->Startup(1, &socketDescriptor, 1);
    if (startupResult != RakNet::RAKNET_STARTED) {
        /// TODO: error msg!
    }
}

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
