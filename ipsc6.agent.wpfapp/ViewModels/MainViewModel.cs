using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.wpfapp.ViewModels
{
    public class MainViewModel
    {
        private static readonly Models.RingInfo ringInfo;
        public Models.RingInfo RingInfo => ringInfo;

        static MainViewModel()
        {
            ringInfo = new Models.RingInfo
            {
                TeleNum = "未知归号码",
                Location = "未知归属地",
                BizName = "未知业务",
            };
        }

    }
}
