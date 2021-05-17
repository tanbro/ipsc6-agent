namespace ipsc6.agent.client
{
    /// <summary>
    /// 连接状态枚举
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// 未进行任何连接
        /// </summary>
        Init,
        /// <summary>
        /// 正在连接
        /// </summary>
        Opening,
        /// <summary>
        /// 连接失败
        /// </summary>
        Failed,
        /// <summary>
        /// 连接成功
        /// </summary>
        /// <remarks>
        /// 登录完毕后方认为连接成功
        /// </remarks>
        Ok,
        /// <summary>
        /// 正在关闭连接
        /// </summary>
        Closing,
        /// <summary>
        /// 连接已关闭
        /// </summary>
        Closed,
        /// <summary>
        /// 连接意外丢失
        /// </summary>
        Lost,
    }
}
