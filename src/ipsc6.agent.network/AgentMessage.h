#pragma once

using namespace System;

namespace ipsc6 {
namespace agent {
namespace network {

public
enum class AgentMessage {
    UNKNOWN = 0,
    REMOTE_MSG_LOGIN = 2,    // 登录
    REMOTE_MSG_RELEASE = 3,  // 注销

    REMOTE_MSG_SIGNON = 4,         // 签人
    REMOTE_MSG_SIGNOFF = 5,        // 签出
    REMOTE_MSG_PAUSE = 6,          // 暂停
    REMOTE_MSG_CONTINUE = 7,       // 继续
    REMOTE_MSG_INTERCEPT = 8,      // 拦截
    REMOTE_MSG_DIAL = 9,           // 拨号
    REMOTE_MSG_TRANSFER = 10,      // 转移到座席
    REMOTE_MSG_TRANSFER_EX = 11,   // 转移到外线
    REMOTE_MSG_CONSULT = 12,       // 咨询到座席
    REMOTE_MSG_CONSULT_EX = 13,    // 咨询到外线
    REMOTE_MSG_HOLD = 14,          // 保持
    REMOTE_MSG_RETRIEVE = 15,      // 找回
    REMOTE_MSG_BREAK_SESS = 16,    // 挂断
    REMOTE_MSG_HANGUP = 17,        // 挂机（软电话）
    REMOTE_MSG_OFFHOOK = 18,       // 摘机（软电话）
    REMOTE_MSG_FORCEINSERT = 19,   // 强插
    REMOTE_MSG_FORCEHANGUP = 20,   // 强拆其它座席
    REMOTE_MSG_RECORD = 21,        // 录音
    REMOTE_MSG_STOPRECORD = 22,    // 停止录音
    REMOTE_MSG_LISTEN = 23,        // 监听
    REMOTE_MSG_STOPLISTEN = 24,    // 停止监听
    REMOTE_MSG_GETQUEUE = 25,      // 取排队
    REMOTE_MSG_BLOCK = 26,         // 闭塞其它座席
    REMOTE_MSG_UNBLOCK = 27,       // 解闭其它座席
    REMOTE_MSG_KICKOUT = 28,       // 强行注销
    REMOTE_MSG_FORCESIGNOFF = 29,  // 强行签出
    REMOTE_MSG_HEARTBEATSYNC = 30,  // 心跳同步.超过设定时间后，自动置为暂停状态

    REMOTE_MSG_TELEMODE = 31,     // 切换话机模式
    REMOTE_MSG_CALLSUBFLOW = 32,  // 调用子项目

    REMOTE_MSG_GREETVOICE = 33,      // 问候语管理
    REMOTE_MSG_MODIFYPOWER = 34,     // 修改权限（自己、他人）
    REMOTE_MSG_MODIFYPOWER_EX = 35,  // 修改扩展权限
    REMOTE_MSG_FORCEPAUSE = 36,      // 强制暂停
    REMOTE_MSG_FORCEIDLE = 37,       // 强制空闲

    //设置话机模式

    // Server To Client
    REMOTE_MSG_SETSTATE = 50,      // 设置状态
    REMOTE_MSG_SETTELESTATE = 51,  // 设置话机状态
    REMOTE_MSG_SETTELEMODE = 52,
    REMOTE_MSG_QUEUEINFO = 53,  // 排队信息
    REMOTE_MSG_HOLDINFO = 54,   // HOLD信息
    REMOTE_MSG_SENDDATA = 55    // 发送数据
};

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
