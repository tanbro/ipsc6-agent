namespace ipsc6.agent.client
{
    public enum ServerSentMessageSubType
    {
        /// <summary>
        /// 本座席ID
        /// </summary>
        AgentId = 1,

        /// <summary>
        /// 本座席通道号
        /// </summary>
        Channel = 2,

        /// <summary>
        /// 本座席话机模式
        /// </summary>
        TeleMode = 3,

        /// <summary>
        /// 本座席所有座席组ID集合
        /// </summary>
        GroupIdList = 4,

        /// <summary>
        /// 本座席基础权限
        /// </summary>
        PrivilegeList = 5,

        /// <summary>
        /// Server时钟
        /// </summary>
        Time = 6,

        /// <summary>
        /// 自定义权限
        /// </summary>
        PrivilegeExternList = 7,

        /// <summary>
        /// ?
        /// </summary>
        Udl = 8,

        /// <summary>
        /// 座席心跳同步超时值
        /// </summary>
        SyncTimeout = 9,

        /// <summary>
        /// 座席自定义串
        /// </summary>
        CustomString = 10,

        /// <summary>
        /// 本座席签人座席组ID集合
        /// </summary>
        SignedGroupIdList = 11,

        /// <summary>
        /// 本座席所有座席组名称集合
        /// </summary>
        GroupNameList = 12,

        /// <summary>
        /// 
        /// </summary>
        SipRegistrarList = 13,

        /// <summary>
        /// 各种座席客户端相关的数据
        /// </summary>
        /// N = {{ AgentID }}
        /// S = JSON
        /// JSON example:
        /// {"powerex": "", "agentid": 1021275263, "username": "USER1001", "power": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17], "svrtime": "2021-07-14 10:46:43", "agentgroup": ["AGENTGROUP1"], "custom": "", "telemode": 2, "synch_interval": 120, "agentch": -1, "agentgroupname": ["GROUP_1"], "udl": "", "auto_checkin_skillgroup": true}
        AgentInfo = 15,

        /// <summary>
        /// 当天工作信息
        /// </summary>
        /// nParam=当天通话次数 pcParam=当天通话时长（秒，字符串）
        TodayWorkInfo = 16,

        /// <summary>
        /// 录音状态,=0为停止录音,产生停止录音事件;=1录音事件,产生开始录音事件
        /// </summary>
        RecordState = 22,

        /// <summary>
        /// 振铃的时候获得工作信息 nParam= 外线通道 pcParam=自定义信息 "custom string..."
        /// </summary>
        Ring = 20,

        /// <summary>
        /// 工作通道（与内线接通的外线）变化
        /// </summary>
        WorkingChannel = 21,

        /// <summary>
        /// 流程中向座席发数据nParam = 数据1pcParam=数据2 
        /// </summary>
        IvrData = 23,

        /// <summary>
        /// CTI服务器发来的客户端配置，如IVR菜单定义（JSON格式）
        /// </summary>
        AppConfig = 24,
    }
}
