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
    }
}
