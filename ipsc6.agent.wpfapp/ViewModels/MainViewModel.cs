using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.wpfapp.ViewModels
{
    public class MainViewModel : Utils.SingletonModelBase<MainViewModel>
    {
        static readonly Models.Cti.AgentBasicInfo agentBasicInfo = Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.AgentBasicInfo AgentBasicInfo => agentBasicInfo;

        static readonly Models.Cti.RingInfo ringInfo = Models.Cti.RingInfo.Instance;
        public Models.Cti.RingInfo RingInfo => ringInfo;
    }
}
