using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.wpfapp.DesignData
{
    public class MainDesignViewModel
    {
        public Models.Cti.AgentBasicInfo AgentBasicInfo => Models.Cti.AgentBasicInfo.Instance;
        public Models.Cti.RingInfo RingInfo => Models.Cti.RingInfo.Instance;

        public MainDesignViewModel()
        {
            AgentBasicInfo.WorkerNumber = "1234";
            AgentBasicInfo.DisplayName = "阿猫阿狗";
            AgentBasicInfo.AgentStateWorkType = new Tuple<client.AgentState, client.WorkType>(client.AgentState.Pause, client.WorkType.PauseSnooze);

            RingInfo.TeleNum = "未知归号码";
            RingInfo.Location = "未知归属地";
            RingInfo.BizName = "未知业务";
        }
    }
}
