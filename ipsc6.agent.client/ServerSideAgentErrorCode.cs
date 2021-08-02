using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
#pragma warning disable CS1529
    using Code = ServerSideAgentErrorCode;
#pragma warning restore CS1529

    public enum ServerSideAgentErrorCode
    {
        ERR_AGENT = -300, // 座席错误

        ERR_AGENT_INVALID_AGENTNO = -301, // 无效的座席ID
        ERR_AGENT_NO_TCOMPUTERNAME = -302, // 
        ERR_AGENT_NO_WORKSTATION = -303, // 

        ERR_AGENT_OFFLINE = -304, // 座席离线
        ERR_AGENT_NOSIGNON = -305, // 座席未签入
        ERR_AGENT_SRCAGENT_NOTRING = -306, // 座席不在振铃状态
        ERR_AGENT_SRCAGENT_NOTWORK = -307, // 座席不在工作状态
        ERR_AGENT_NOTIDLE = -308, // 座席不在空闲状态
        ERR_AGENT_NOT_READY_OK = -309, // 座席未就绪
        ERR_AGENT_STATE = -310, // 不是预期的状态

        ERR_AGENT_BUSY = -311, // 座席忙
        ERR_AGENT_WORKING = -312, // 座席工作中

        ERR_AGENT_NO_LOGINOFF = -313, // 座席已登录

        ERR_AGENT_USER_ERR = -314, // 工号错误
        ERR_AGENT_PSW_ERR = -315, // 密码错误
        ERR_AGENT_USER_EXIST = -316, // 工号已登录
        ERR_AGENT_LOGIN_FAIL = -317, // 登录失败

        ERR_AGENT_NO_SOFTMODE = -318, // 必须软电话模式
        ERR_AGENT_HANGUP = -319, // 不允许在挂机状态操作
        ERR_AGENT_HARD_HANGUP = -320, // 物理挂机，不能软摘机

        ERR_AGENT_NOTPOWER = -321, // 权限不足

        ERR_AGENT_ERRINFOTYPE = -322, // 
        ERR_AGENT_ACTION_FAILED = -323, // 操作失败
        ERR_AGENT_NOTINPROJECT = -324, // 
        ERR_AGENT_EXIST_RECORD = -325, // 
        ERR_AGENT_CALLFUNC_FAILED = -326, // 
        ERR_AGENT_AGENT_EQ = -327, // 不允许对本座席操作
        ERR_AGENT_DIALNO_NULL = -328, // 电话号码为空

        ERR_AGENT_PROCSUBPROJECT = -329, // 

        ERR_AGENT_PARAM = -330, // 参数错误
        ERR_AGENT_NOT_DIAL = -331, // 不能拨号
        ERR_AGENT_TELEMODE = -332, // 不是预想的话机模式

        ERR_AGENT_NOIDLE_OFFHOOK = -333, // 不允许空闲摘机
        ERR_AGENT_LOGON_NULL = -334, // 未发现登录座席

        ERR_AGENT_WORKSESSION = -335, // 未发现可用的工作会话
        ERR_AGENT_NOTSESSION = -336, // 未发现对应的会话

        ERR_AGENT_NOTCHANGETELEMODE = -337, // 不能切换话机模式

        ERR_AGENT_OFFHOOK = -338, // 座席已摘机
        ERR_AGENT_CALLAGENTTELEPHONE = -339, // 呼叫座席电话中

        ERR_AGENT_SIGNONOFF_FAIL = -340, // 签入签出组失败

        ERR_AGENT_NOT_UNHOLD_AT_CONS = -341, // 咨询转移中不允许找回
    }


    public static class ServerSideAgentErrorCodeDict
    {
        public static readonly IReadOnlyDictionary<Code, string> Value = new Dictionary<Code, string>
        {
            { Code.ERR_AGENT, "座席错误" },
            { Code.ERR_AGENT_INVALID_AGENTNO, "无效的座席ID" },
            { Code.ERR_AGENT_NO_TCOMPUTERNAME, "" },
            { Code.ERR_AGENT_NO_WORKSTATION, "" },
            { Code.ERR_AGENT_OFFLINE, "座席离线" },
            { Code.ERR_AGENT_NOSIGNON, "座席未签入" },
            { Code.ERR_AGENT_SRCAGENT_NOTRING, "座席不在振铃状态" },
            { Code.ERR_AGENT_SRCAGENT_NOTWORK, "座席不在工作状态" },
            { Code.ERR_AGENT_NOTIDLE, "座席不在空闲状态" },
            { Code.ERR_AGENT_NOT_READY_OK, "座席未就绪" },
            { Code.ERR_AGENT_STATE, "不是预期的状态" },
            { Code.ERR_AGENT_BUSY, "座席忙" },
            { Code.ERR_AGENT_WORKING, "座席工作中" },
            { Code.ERR_AGENT_NO_LOGINOFF, "座席已登录" },
            { Code.ERR_AGENT_PSW_ERR, "密码错误" },
            { Code.ERR_AGENT_USER_EXIST, "工号已登录" },
            { Code.ERR_AGENT_LOGIN_FAIL, "登录失败" },
            { Code.ERR_AGENT_NO_SOFTMODE, "必须软电话模式" },
            { Code.ERR_AGENT_HANGUP, "不允许在挂机状态操作" },
            { Code.ERR_AGENT_NOTPOWER, "权限不足" },
            { Code.ERR_AGENT_ERRINFOTYPE, "" },
            { Code.ERR_AGENT_ACTION_FAILED, "操作失败" },
            { Code.ERR_AGENT_NOTINPROJECT, "" },
            { Code.ERR_AGENT_EXIST_RECORD, "" },
            { Code.ERR_AGENT_CALLFUNC_FAILED, "" },
            { Code.ERR_AGENT_AGENT_EQ, "不允许对本座席操作" },
            { Code.ERR_AGENT_DIALNO_NULL, "电话号码为空" },
            { Code.ERR_AGENT_PROCSUBPROJECT, "" },
            { Code.ERR_AGENT_PARAM, "参数错误" },
            { Code.ERR_AGENT_NOT_DIAL, "不能拨号" },
            { Code.ERR_AGENT_TELEMODE, "不是预想的话机模式" },
            { Code.ERR_AGENT_NOIDLE_OFFHOOK, "不允许空闲摘机" },
            { Code.ERR_AGENT_LOGON_NULL, "未发现登录座席" },
            { Code.ERR_AGENT_WORKSESSION, "未发现可用的工作会话" },
            { Code.ERR_AGENT_NOTSESSION, "未发现对应的会话" },
            { Code.ERR_AGENT_NOTCHANGETELEMODE, "不能切换话机模式" },
            { Code.ERR_AGENT_OFFHOOK, "座席已摘机" },
            { Code.ERR_AGENT_CALLAGENTTELEPHONE, "呼叫座席电话中" },
            { Code.ERR_AGENT_SIGNONOFF_FAIL, "签入签出组失败" },
            {Code.ERR_AGENT_NOT_UNHOLD_AT_CONS, "咨询转移中不允许找回" },
        };
    }

}
