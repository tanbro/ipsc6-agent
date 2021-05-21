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

        //static MainViewModel()
        //{

        //    var agentBasicInfo = Models.Cti.AgentBasicInfo.Instance;
        //    agentBasicInfo.WorkerNumber = "1234";
        //    agentBasicInfo.DisplayName = "阿猫阿狗";

        //    var ringInfo = Models.Cti.RingInfo.Instance;
        //    ringInfo.TeleNum = "未知归号码";
        //    ringInfo.Location = "未知归属地";
        //    ringInfo.BizName = "未知业务";
        //}
    }
}
