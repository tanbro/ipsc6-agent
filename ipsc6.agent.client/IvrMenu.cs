using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ipsc6.agent.client
{

    public class IvrMenuItemArgs
    {
        /// <summary>
        /// 要调用的 IVR 的 ID 
        /// </summary>
        [JsonRequired]
        [JsonPropertyName("ivrId")]
        public string IvrId { get; set; }

        /// <summary>
        /// 呼叫类型整数标识
        /// </summary>
        [JsonPropertyName("callType")]
        public IvrInvokeType CallType { get; set; } = IvrInvokeType.Keep;

        /// <summary>
        /// 调用 IVR 时需要客户端传入的参数常量（固定值）
        /// </summary>
        /// TODO: params 用原始字符串!
        [JsonPropertyName("params")]
        public IEnumerable<string> Params { get; set; }
    }

    public class IvrMenuItem
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        [JsonRequired]
        [JsonPropertyName("caption")]
        public string Caption { get; set; }

        /// <summary>
        /// 菜单说明
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// 技能组ID数组，仅当坐席签入了这些数组是，方可使用 IVR 菜单
        /// </summary>
        [JsonPropertyName("groups")]
        public IEnumerable<string> Groups { get; set; }

        /// <summary>
        /// 子菜单定义
        /// </summary>
        [JsonPropertyName("args")]
        public IvrMenuItemArgs Args { get; set; }

        /// <summary>
        /// 子菜单定义
        /// </summary>
        [JsonPropertyName("children")]
        public IEnumerable<IvrMenuItem> Children { get; set; } = new List<IvrMenuItem>();
    }

}
