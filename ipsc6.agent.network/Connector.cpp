#include "Connector.h"
#include <RakNet/BitStream.h>
#include <RakNet/GetTime.h>
#include <RakNet/MessageIdentifiers.h>
#include <msclr\lock.h>
#include <msclr\marshal.h>
#include <msclr\marshal_cppstd.h>

using namespace msclr;
using namespace msclr::interop;

namespace ipsc6 {
namespace agent {
namespace network {

Connector::Connector(UInt16 localPort, String ^ address) {
    _address = address;
    _localPort = localPort;
    _peer = RakNet::RakPeerInterface::GetInstance();
}

Connector::~Connector() {
    RakNet::RakPeerInterface::DestroyInstance(_peer);
}

void Connector::Initial() {
    connectors = gcnew HashSet<Connector ^>();
    receiveThreadStarted =
        gcnew EventWaitHandle(false, EventResetMode::AutoReset);
    receiveThread = gcnew Thread(gcnew ThreadStart(ReceiveThreadProc));
    receiveThreadStopping = false;
}

void Connector::Release() {
    do {
        lock l(connectors);
        for each (auto connector in connectors) {
            connector->Shutdown();
        }
        receiveThreadStopping = true;
    } while (0);
    receiveThread->Join();
    connectors->Clear();
}

Connector ^ Connector::CreateInstance(UInt16 localPort, String ^ address) {
    auto connector = gcnew Connector(localPort, address);
    connector->StartUp();
    do {
        lock l(connectors);
        connectors->Add(connector);
        if (!receiveThread->IsAlive) {
            receiveThreadStopping = false;
            receiveThread->Start();
            receiveThreadStarted->WaitOne();
        }
    } while (0);
    return connector;
}

Connector ^ Connector::CreateInstance(UInt16 localPort) {
    return CreateInstance(localPort, "");
}

Connector ^ Connector::CreateInstance() {
    return CreateInstance(0, "");
}

void Connector::DeallocateInstance(Connector ^ connector) {
    do {
        lock l(connectors);
        connectors->Remove(connector);
        if (connectors->Count == 0) {
            receiveThreadStopping = true;
        }
    } while (0);
    if (receiveThreadStopping) {
        receiveThread->Join();
    }
    connector->Shutdown();
}

void Connector::Connect(String ^ host, UInt16 remotePort) {
    static const char CONNECT_PASSWORD[] = "Rumpelstiltskin";
    marshal_context ^ context = gcnew marshal_context();
    try {
        const char* host_cstr = context->marshal_as<const char*>(host);
        RakNet::ConnectionAttemptResult result = _peer->Connect(
            host_cstr, remotePort, CONNECT_PASSWORD, strlen(CONNECT_PASSWORD));
        if (RakNet::CONNECTION_ATTEMPT_STARTED != result) {
            throw gcnew InvalidOperationException(
                String::Format("RakPeer Connect failed ({0})", (int)result));
        }
    } finally {
        delete context;
    }
}

void Connector::ReceiveThreadProc() {
    bool stopping = false;
    receiveThreadStarted->Set();
    while (true) {
        int receiveCount = 0;
        do {
            lock l(connectors);
            // 检查是否可以退出了?
            stopping = receiveThreadStopping;
            if (stopping) {
                break;
            }
            //
            for each (auto connector in connectors) {
                receiveCount += connector->Receive();
            }
        } while (0);
        if (stopping) {
            break;
        }
        // 睡眠，死循环
        Thread::Sleep(receiveCount > 0 ? 0 : 25);
    }
}

void Connector::StartUp() {
    RakNet::SocketDescriptor socketDescriptor(0, "");
    RakNet::StartupResult startupResult =
        _peer->Startup(1, &socketDescriptor, 1);
    if (startupResult != RakNet::RAKNET_STARTED) {
        throw gcnew InvalidOperationException(String::Format(
            "RakNetPeer Startup failed(code={0})", (int)startupResult));
    }
}

void Connector::Shutdown() {
    if (!_peer->IsActive()) {
        throw gcnew InvalidOperationException(
            "Can not close a RakPeer instance who is not active");
    }
    _peer->Shutdown(1000);
}

int Connector::Receive() {
    RakNet::Packet* packet = _peer->Receive();
    if (nullptr == packet) {
        return 0;
    }

    RakNet::MessageID msgId = getPacketIdentifier(packet);
    switch (msgId) {
        case ID_DISCONNECTION_NOTIFICATION: {
            _connectionId = 0;
        } break;

        case ID_CONNECTION_ATTEMPT_FAILED: {
            _connectionId = 0;
            try {
                OnConnectAttemptFailed();
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_INVALID_PASSWORD: {
            _connectionId = 0;
        } break;

        case ID_CONNECTION_LOST: {
            _connectionId = 0;
        } break;

        case ID_CONNECTION_REQUEST_ACCEPTED: {
        } break;

        case ID_USER_PACKET_ENUM: {
        } break;

        default: {
        } break;
    }
    return 1;
}

RakNet::MessageID getPacketIdentifier(const RakNet::Packet* packet) {
    if (NULL == packet) {
        return 255;
    }
    if ((RakNet::MessageID)packet->data[0] == ID_TIMESTAMP) {
        if (packet->length > sizeof(RakNet::MessageID) + sizeof(RakNet::Time)) {
            return (RakNet::MessageID)(
                packet->data[sizeof(RakNet::MessageID) + sizeof(RakNet::Time)]);
        } else {
            return 255;
        }
    } else {
        return (RakNet::MessageID)(packet->data[0]);
    }
}

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
