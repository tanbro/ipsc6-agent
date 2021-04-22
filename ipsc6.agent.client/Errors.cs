using System;

namespace ipsc6.agent.client
{
    public enum ServerSendErrorCode
    {
        ERR_AGENT = -300,
        // 无效座席
        ERR_AGENT_INVALID_AGENTNO = -301,
        // 未登记的工作站
        ERR_AGENT_NO_TCOMPUTERNAME = -302,
        // 座席为无工作站座席
        ERR_AGENT_NO_WORKSTATION = -303,
        // 座席未在线
        ERR_AGENT_OFFLINE = -304,
        // 座席未签人
        ERR_AGENT_NOSIGNON = -305,
        // 座席未在振铃状态
        ERR_AGENT_SRCAGENT_NOTRING = -306,
        // 座席未在工作状态
        ERR_AGENT_SRCAGENT_NOTWORK = -307,
        // 座席未在空闲状态
        ERR_AGENT_NOTIDLE = -308,
        // 座席未就绪
        ERR_AGENT_NOT_READY_OK = -309,
        // 座席不是预期的状态
        ERR_AGENT_STATE = -310,
        // 座席忙
        ERR_AGENT_BUSY = -311,
        // 座席工作中
        ERR_AGENT_WORKING = -312,
        // 座席未注销
        ERR_AGENT_NO_LOGINOFF = -313,
        // 工号检查错误
        ERR_AGENT_USER_ERR = -314,
        // 密码检查错误
        ERR_AGENT_PSW_ERR = -315,
        // 工号已登录
        ERR_AGENT_USER_EXIST = -316,
        // 登录失败
        ERR_AGENT_LOGIN_FAIL = -317,
        // 座席不是软电话模式
        ERR_AGENT_NO_SOFTMODE = -318,
        // 座席电话未摘机
        ERR_AGENT_HANGUP = -319,
        // 如果物理线路挂机，不能摘机
        ERR_AGENT_HARD_HANGUP = -320,
        // 权限不够
        ERR_AGENT_NOTPOWER = -321,
        // 错误的通知类型
        ERR_AGENT_ERRINFOTYPE = -322,
        // 动作失败
        ERR_AGENT_ACTION_FAILED = -323,
        // 此项目不能
        ERR_AGENT_NOTINPROJECT = -324,
        // 已经在录音状态
        ERR_AGENT_EXIST_RECORD = -325,
        // 调功能失败
        ERR_AGENT_CALLFUNC_FAILED = -326,
        // 不能对本站操作
        ERR_AGENT_AGENT_EQ = -327,
        // 代拨号码为空
        ERR_AGENT_DIALNO_NULL = -328,
        // 座席正在处理子项目
        ERR_AGENT_PROCSUBPROJECT = -329,
        // 无效的参数
        ERR_AGENT_PARAM = -330,
        // 座席未在拨号中
        ERR_AGENT_NOT_DIAL = -331,
        // 未发现工作会话
        ERR_AGENT_WORKSESSION = -334,
        // 未发现指定会话
        ERR_AGENT_NOTSESSION = -335,
        //座席尚未登陆
        ERR_AGENT_NOT_LOGIN = -500,
        // 系统当前还没有初始化CTI服务器的时间
        ERR_AGENT_NO_INITIALIZE_CITTIME = -501,
        // CTI服务器的时间格式不正确
        ERR_AGENT_ILLEGAL_CIT_TIME = -502,
        // CTI服务器名称为空或没有指定
        ERR_AGENT_CITNAME_NOTSET = -503,
        // 联接的CTI服务器失败
        ERR_AGENT_CONNECTED_FAILED = -504,
        // 断开服务器失败
        ERR_AGENT_DISCONNECTED_FAILED = -505,
        // 直接发送消息到CTI服务器失败
        ERR_AGENT_SENDMESSAGE = -506,
        // 等待服务器返回消息超时
        ERR_AGENT_WAITMSGTIMEOUT = -507,
        // 注销用户失败
        ERR_AGENT_LOGOUT_FAILED = -508,
        //指定的坐席组字符串太长
        ERR_AGENT_GROUPNO_TOOLONGER = -509,
        //签入座席组时失败
        ERR_AGENT_SIGNON_FAILED = -510,
        //指定的坐席组不存在
        ERR_AGENT_NOTEXSIT_GROUP = -511,
        //签出座席组失败	
        ERR_AGENT_SIGNOFF_FAILED = -512,
        //座席暂停失败
        ERR_AGENT_PAUSE = -513,
        //座席取消暂停失败
        ERR_AGENT_CANCEL_PAUSE = -514,
        //当前座席没有拦截的权限
        ERR_AGENT_INTERCEPT_POWER = -515,
        //抢接另外一个座席的电话失败
        ERR_AGENT_INTERCEPT = -516,
        //拨叫外线电话失败
        ERR_AGENT_DIAL = -517,
        //当前座席没有转移到座席的权限
        ERR_AGENT_TRANSFER_POWER = -518,
        //将通话转移到其它座席失败
        ERR_AGENT_TRANSFER = -519,
        //当前座席没有转移到外线的权限
        ERR_AGENT_TRANSFEREX_POWER = -520,
        //将通话转移到外线失败
        ERR_AGENT_TRANSFEREX = -521,
        //当前座席没有咨询到座席的权限
        ERR_AGENT_CONSULT_POWER = -522,
        //咨询到另外一个座席时失败
        ERR_AGENT_CONSULT = -523,
        //当前座席没有咨询到外线的权限
        ERR_AGENT_CONSULTEX_POWER = -524,
        //咨询到外线电话失败
        ERR_AGENT_CONSULTEX = -525,
        //通话保持失败
        ERR_AGENT_HOLDON = -526,
        //找回被保持的通话失败
        ERR_AGENT_RETRIEVE = -527,
        //切断与一个外线通道的会话失败
        ERR_AGENT_BREAKSESSION = -528,
        //挂机失败
        ERR_AGENT_HANGUP_FAILED = -529,
        //摘机失败
        ERR_AGENT_OFFHOOK_FAILED = -530,
        //当前座席没有强插的权限
        ERR_AGENT_FORCEINSERT_POWER = -531,
        //强插失败.
        ERR_AGENT_FORCEINSERT = -532,
        //强拆失败.
        ERR_AGENT_FORCEHANGUP = -533,
        //录音的权限
        ERR_AGENT_RECORD_POWER = -534,
        //对座席录音失败
        ERR_AGENT_RECORD = -535,
        //对座席停止录音失败
        ERR_AGENT_STOPRECORD = -536,
        //当前座席没有监听的权限
        ERR_AGENT_LISTEN_POWER = -537,
        //监听座席失败
        ERR_AGENT_LISTEN = -538,
        //停止监听座席失败
        ERR_AGENT_STOPLISTEN = -539,
        //获取排队失败
        ERR_AGENT_GETQEUE = -540,
        //当前座席没有闭塞其它座席的权限
        ERR_AGENT_BLOCK_POWER = -541,
        //闭塞其它座席失败
        ERR_AGENT_BLOCK = -542,
        //当前座席没有解闭其它座席的权限
        ERR_AGENT_UNBLOCK_POWER = -543,
        //解除对座席闭塞失败
        ERR_AGENT_UNBLOCK = -544,
        //当前座席没有强行注销的权限
        ERR_AGENT_KICKOUT_POWER = -545,
        //强行注销失败
        ERR_AGENT_KICKOUT = -546,
        //强行签出座席失败
        ERR_AGENT_FORCESIGNOUT = -547,
        //切换话机模式失败
        ERR_AGENT_CHANGETELEMODE = -548,
        //调用服务器上的子流程失败
        ERR_AGENT_CALLSUBFLOW = -549,
        //指定的索引号必须是大于等于0的正整数
        ERR_AGENT_ILLEGAL_INDEX = -550,
        //当前座席没有可用的座席组
        ERR_AGENT_NOGROUP = -551,
        //指定的索引号超出了数组范围
        ERR_AGENT_OUTOFARRAYRANGE = -552,
        //从服务器端获取的座席组ID集合与座席组名称集合尺寸不相等
        ERR_AGENT_GROUPSIZENOTEQUAL = -553
    }

    public class ServerSendError : BaseAgentException
    {
        public ServerSendError() { }
        public ServerSendError(string message) : base(message) { }
        public ServerSendError(string message, Exception inner) : base(message, inner) { }
    }

    public class ErrorResponseException : BaseAgentException
    {
        public ErrorResponseException() { }
        public ErrorResponseException(string message) : base(message) { }
        public ErrorResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
