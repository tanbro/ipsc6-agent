using ipsc6.agent.client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;


namespace ipsc6.agent.services.Models
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct IvrMenuItemArgs
    {
        public string IvrId { get; set; }
        public IvrInvokeType CallType { get; set; }
        public string Params {get; set;}
    }

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct IvrMenuItem
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public IvrMenuItemArgs Args { get; set; }
        public IEnumerable<IvrMenuItem> Children { get; set; }
    }
}
