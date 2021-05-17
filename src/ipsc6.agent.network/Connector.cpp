/* clang-format off */
#include "pch.h"
/* clang-format on */

#include "Connector.h"
#include <RakNet/BitStream.h>
#include <RakNet/GetTime.h>
#include <RakNet/MessageIdentifiers.h>
#include <msclr/lock.h>
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include <stdexcept>

using namespace msclr;
using namespace msclr::interop;

namespace ipsc6 {
namespace agent {
namespace network {

Connector::Connector(unsigned short localPort, String ^ address) {
    _address = address;
    _localPort = localPort;
    _peer = RakNet::RakPeerInterface::GetInstance();
    _sendStream = RakNet::BitStream::GetInstance();
    _remoteAddrIndex = -1;
    _msecConnectionRequestAccepted = 0;
}

Connector::~Connector() {
    RakNet::RakPeerInterface::DestroyInstance(_peer);
    RakNet::BitStream::DestroyInstance(_sendStream);
}

void Connector::Initial() {
    connectors = gcnew HashSet<Connector ^>();
    receiveThreadStarted =
        gcnew EventWaitHandle(false, EventResetMode::AutoReset);
    receiveThread = gcnew Thread(gcnew ThreadStart(ReceiveThreadProc));
    do {
        lock l(connectors);
        receiveThreadStopping = false;
        receiveThread->Start();
        receiveThreadStarted->WaitOne();
    } while (0);
}

void Connector::Release() {
    if (Thread::CurrentThread == receiveThread) {
        throw gcnew InvalidOperationException(
            "Can not Release Connectors from inside receive thread");
    }
    do {
        lock l(connectors);
        for each (auto connector in connectors) {
            connector->Shutdown();
        }
        receiveThreadStopping = true;
    } while (0);
    if (receiveThread->IsAlive) {
        receiveThread->Join();
    }
    connectors->Clear();
}

Connector ^
    Connector::CreateInstance(unsigned short localPort, String ^ address) {
    auto connector = gcnew Connector(localPort, address);
    connector->StartUp();
    do {
        lock l(connectors);
        connectors->Add(connector);
    } while (0);
    return connector;
}

Connector ^ Connector::CreateInstance(unsigned short localPort) {
    return CreateInstance(localPort, "");
}

Connector ^ Connector::CreateInstance() {
    return CreateInstance(0, "");
}

void Connector::DeallocateInstance(Connector ^ connector) {
    if (Thread::CurrentThread == receiveThread) {
        throw gcnew InvalidOperationException(
            "Can not deallocate Connector from inside receive thread");
    }
    do {
        lock l(connectors);
        connectors->Remove(connector);
    } while (0);
    connector->Shutdown();
}

void Connector::Connect(String ^ host,
                        unsigned short remotePort,
                        unsigned int timeoutMs) {
    static const char CONNECT_PASSWORD[] = "Rumpelstiltskin";
    marshal_context ^ context = gcnew marshal_context();
    try {
        const char* host_cstr = context->marshal_as<const char*>(host);
        RakNet::ConnectionAttemptResult result = _peer->Connect(
            host_cstr, remotePort, CONNECT_PASSWORD, strlen(CONNECT_PASSWORD),
            0, 0, 12U, 500U, timeoutMs);
        if (RakNet::CONNECTION_ATTEMPT_STARTED != result) {
            throw gcnew InvalidOperationException(
                String::Format("RakPeer Connect failed ({0})", (int)result));
        }
    } finally {
        delete context;
    }
}

void Connector::Connect(String ^ host, unsigned short remotePort) {
    Connect(host, remotePort, 0);
}

void Connector::Connect(String ^ host) {
    Connect(host, DEFAULT_REMOTE_PORT);
}

void Connector::Disconnect() {
    Disconnect(false);
}

void Connector::Disconnect(bool force) {
    auto sysAddr = _peer->GetSystemAddressFromIndex(_remoteAddrIndex);
    _peer->CloseConnection(sysAddr, force);
    _remoteAddrIndex = -1;
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
        Thread::Sleep(receiveCount > 0 ? 0 : 1000);
    }
}

void Connector::StartUp() {
    marshal_context ^ context = gcnew marshal_context();
    try {
        lock l(this);
        const char* address_cstr = context->marshal_as<const char*>(_address);
        RakNet::SocketDescriptor socketDescriptor(_localPort, address_cstr);
        RakNet::StartupResult startupResult =
            _peer->Startup(1, &socketDescriptor, 1);
        if (startupResult != RakNet::RAKNET_STARTED) {
            throw gcnew InvalidOperationException(String::Format(
                "RakNetPeer Startup failed(code={0})", (int)startupResult));
        }
        _boundAddress =
            marshal_as<String ^>((char*)_peer->GetMyBoundAddress().ToString());
    } finally {
        delete context;
    }
}

void Connector::Shutdown() {
    if (_peer->IsActive()) {
        _peer->Shutdown(1000);
    }
    _boundAddress = "";
    _remoteAddrIndex = -1;
    _msecConnectionRequestAccepted = 0;
}

int Connector::Receive() {
    RakNet::Packet* packet = _peer->Receive();

    if (nullptr == packet) {
        return 0;
    }

    RakNet::MessageID msgId = getPacketIdentifier(packet);
    switch (msgId) {
        case ID_DISCONNECTION_NOTIFICATION: {
            _remoteAddrIndex = -1;
            try {
                OnDisconnected(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_CONNECTION_ATTEMPT_FAILED: {
            _remoteAddrIndex = -1;
            try {
                OnConnectAttemptFailed(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_INVALID_PASSWORD: {
            _remoteAddrIndex = -1;
            try {
                OnConnectAttemptFailed(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_NO_FREE_INCOMING_CONNECTIONS: {
            _remoteAddrIndex = -1;
            try {
                OnConnectAttemptFailed(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_CONNECTION_BANNED: {
            _remoteAddrIndex = -1;
            try {
                OnConnectAttemptFailed(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_INCOMPATIBLE_PROTOCOL_VERSION: {
            _remoteAddrIndex = -1;
            try {
                OnConnectAttemptFailed(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_IP_RECENTLY_CONNECTED: {
            _remoteAddrIndex = -1;
            try {
                OnConnectAttemptFailed(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_CONNECTION_LOST: {
            _remoteAddrIndex = -1;
            try {
                OnConnectionLost(this, gcnew EventArgs());
            } catch (NullReferenceException ^) {
            }
        } break;

        case ID_CONNECTION_REQUEST_ACCEPTED: {
            _remoteAddrIndex =
                _peer->GetIndexFromSystemAddress(packet->systemAddress);
            DoOnConnectionRequestAccepted(packet);
        } break;

        case ID_USER_PACKET_ENUM: {
            DoOnUserPacketReceived(packet);
        } break;

        default:
            break;
    }

    return 1;
}

void Connector::SendRawData(const BYTE* data, size_t length) {
    if (length > 1024) {
        throw gcnew InvalidOperationException("The data is too big.");
    }
    do {
        lock l(this);
        _sendStream->Reset();
        _sendStream->Write((RakNet::MessageID)ID_TIMESTAMP);
        _sendStream->Write(RakNet::GetTime());
        _sendStream->Write((RakNet::MessageID)ID_USER_PACKET_ENUM);
        _sendStream->Write((char)0);  //压缩标志——不压缩
        _sendStream->Write((const char*)data, length);  // user data
        if (0 == _peer->Send(_sendStream, MEDIUM_PRIORITY, RELIABLE_ORDERED, 0,
                             _peer->GetSystemAddressFromIndex(_remoteAddrIndex),
                             false)) {
            throw gcnew InvalidOperationException("RakNetPeer Send() failed");
        }
    } while (0);
}

void Connector::SendRawData(array<Byte> ^ data) {
    // see:
    // 1)
    // https://docs.microsoft.com/en-us/cpp/dotnet/how-to-marshal-arrays-using-cpp-interop
    // 2)
    // https://docs.microsoft.com/en-us/cpp/dotnet/how-to-obtain-a-pointer-to-byte-array
    pin_ptr<System::Byte> p = &data[0];
    BYTE* pby = p;
    BYTE* result = reinterpret_cast<BYTE*>(pby);
    SendRawData(result, data->Length);
}

void Connector::SendAgentMessage(int commandType, int n, String ^ s) {
    size_t data_sz = sizeof(MsgType) + sizeof(int) + sizeof(int) + sizeof(int) +
                     s->Length + 1;
    BYTE* data_buf = (BYTE*)calloc(data_sz, sizeof(BYTE));
    if (data_buf) {
        try {
            BYTE* ptr = data_buf;
            //
            *(MsgType*)ptr = mtAppData;
            ptr += sizeof(MsgType);
            // 不再需要指定AgentID了
            *(int*)ptr = 0;
            ptr += sizeof(int);
            //
            *(int*)ptr = commandType;
            ptr += sizeof(int);
            //
            *(int*)ptr = n;
            ptr += sizeof(int);
            //
            marshal_context ^ context = gcnew marshal_context();
            try {
                const char* pch = context->marshal_as<const char*>(s);
                strcpy((char*)ptr, pch);
            } finally {
                delete context;
            }
            //
            SendRawData(data_buf, data_sz);
        } finally {
            free(data_buf);
        }
    } else {
        throw new std::runtime_error("calloc failed");
    }
}

static size_t SZ_ASK_DYNAMIC_SIP_AGENT_ID =
    sizeof(MsgType) + sizeof(BYTE) + sizeof(int32_t);

void Connector::DoOnConnectionRequestAccepted(RakNet::Packet* packet) {
    /// 连接成功事件
    auto e = gcnew ConnectedEventArgs();
    try {
        OnConnected(this, e);
    } catch (NullReferenceException ^) {
    }
}

static size_t SZ_USER_PACKET_HEADER =
    sizeof(RakNet::MessageID)   /* ID_TIMESTAMP */
    + sizeof(RakNet::Time)      /* 时间戳 */
    + sizeof(RakNet::MessageID) /* ID_USER_PACKET_ENUM */
    + sizeof(BYTE) /* 压缩标志 */;

void Connector::DoOnUserPacketReceived(RakNet::Packet* packet) {
    if (packet->length <= SZ_USER_PACKET_HEADER) {
        throw std::overflow_error("UserPacket too small.");
    }
    BYTE* user_data = packet->data + SZ_USER_PACKET_HEADER;
    size_t user_length = packet->length - SZ_USER_PACKET_HEADER;
    BYTE compressed =
        *(BYTE*)(packet->data + sizeof(RakNet::MessageID) +
                 sizeof(RakNet::Time) + sizeof(RakNet::MessageID));
    if (compressed) {
        throw gcnew NotImplementedException(
            "Uncompressing is not supported yet.");
    }
    ///////////
    // 用户部分：
    //求包类型
    MsgType msgType = *(MsgType*)(user_data);
    BYTE* ptr = user_data + sizeof(MsgType);
    switch (msgType) {
        case mtAppData: {
            /*
            内存块数据如下：MsgType msgType, long command_type, long param1,
            long param2, char str[strLen] long command_type =
            REMOTE_MSG_SENDDATA; long param1; long param2; char * pcparam;
            */
            int command_type = *(int*)ptr;
            ptr += sizeof(int);
            //
            int n1 = *(int*)ptr;
            ptr += sizeof(int);
            //
            int n2 = *(int*)ptr;
            ptr += sizeof(int);
            //
            auto s = marshal_as<String ^>((char*)ptr);
            // auto utfBytes = System::Console::OutputEncoding->GetBytes(s);
            // auto encodedString =
            // System::Text::Encoding::Default->GetString(utfBytes, 0,
            // utfBytes->Length);
            /// 抛出事件：坐席收到来自服务器端的数据
            auto e =
                gcnew AgentMessageReceivedEventArgs(command_type, n1, n2, s);
            try {
                OnAgentMessageReceived(this, e);
            } catch (NullReferenceException ^) {
            }
        } break;

        default: {
        } break;
    }
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
