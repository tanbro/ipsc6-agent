#pragma once

/* clang-format off */
#include "framework.h"
/* clang-format on */

#include <iostream>
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
    Connector(unsigned short localPort, String ^ address);
    ~Connector();

   private:
    static ICollection<Connector ^> ^ connectors;

    static EventWaitHandle ^ receiveThreadStarted;
    static Thread ^ receiveThread;
    static void ReceiveThreadStarter();
    static bool ReceiveSpinFunc();
    static CancellationTokenSource ^ receiveCancelTokenSource;

    String ^ _address;
    unsigned short _localPort;
    String ^ _boundAddress;
    int _remoteAddrIndex;
    long long _msecConnectionRequestAccepted;
    RakNet::RakPeerInterface* _peer;
    RakNet::BitStream* _sendStream;

    void StartUp();
    void Shutdown();
    int Receive();
    void SendRawData(const BYTE* data, size_t length);

    void DoOnConnectionRequestAccepted(RakNet::Packet* packet);
    void DoOnUserPacketReceived(RakNet::Packet* packet);

   public:
    static const unsigned short DEFAULT_REMOTE_PORT = 13920;

    static void Initial();
    static void Release();

    static Connector ^
        CreateInstance(unsigned short localPort, String ^ address);
    static Connector ^ CreateInstance(unsigned short localPort);
    static Connector ^ CreateInstance();
    static void DeallocateInstance(Connector ^ connector);

    void Connect(String ^ host,
                 unsigned short remotePort,
                 unsigned int timeoutMs);
    void Connect(String ^ host, unsigned short remotePort);
    void Connect(String ^ host);
    void Disconnect();
    void Disconnect(bool force);
    void SendRawData(array<Byte> ^ data);
    void SendAgentMessage(int commandType, int n, String ^ s);

    event EventHandler ^ OnConnectAttemptFailed;
    event EventHandler ^ OnDisconnected;
    event EventHandler ^ OnConnectionLost;
    event EventHandler<ConnectedEventArgs ^> ^ OnConnected;
    event EventHandler<AgentMessageReceivedEventArgs ^> ^
        OnAgentMessageReceived;

    /* clang-format off */
    property String ^ BoundAddress { String ^ get() { return _boundAddress; } }
        /* clang-format on */

        /* clang-format off */
    property bool Connected { bool get() { return _remoteAddrIndex >= 0; } }
    /* clang-format on */

};  // ref class Connector

enum MsgType {
    mtAskAgentId,  //客户端连接之后发送自己的计算机名给服务器请求服务器告知座席ID
    /*
    这个时候，包的格式是：mtAskAgentId,"机器名\0"
    */
    mtTellAgentId,  //服务器收到客户端连接之后发送的计算机之后，告知座席ID
    mtAppData,             //应用数据，需要如实调用回调进行通知
    mtSendLicenseAgentId,  //客户端向服务器传递许可证中的AgentId信息，许可证从文本文件读。格式：unsinged
                           // long m_AgentId
    mtAskDynamicSIPAgentId,  //客户端连接之后发送自己的计算机名给服务器请求服务器告知座席ID，该ID是用于SIP座席与远程座席的动态ID
    mtAskDynamicRemoteAgentId,  //客户端连接之后发送自己的计算机名给服务器请求服务器告知座席ID，该ID是用于SIP座席与远程座席的动态ID
    mtComputerNameNotExists,    //没有找到对应该计算机名的AgentId
    mtAlreadyLogged,            //该AgentId已经登录
    mtCountOfMsgType
};

//连接失败原因
enum ConnectFailReason {
    cfrConnectionAttemptFailed,    //无法连接到主机
    cfrAlreadyConnected,           //已经连接了
    cfrConnectionBanned,           //主机禁止连接
    cfrNoFreeIncomingConnections,  //主机无资源
    cfrInvalidPassword,            //连接密码错误
    cfrComputerNameNotExists,      //没有找到对应该计算机名的AgentId
    cfrAlreadyLogged,              //该AgentId已经登录
    cftCountOfConnectFailReason
};

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
