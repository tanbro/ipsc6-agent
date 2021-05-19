using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentWpfApp.ViewModels
{
    public class MainViewModel
    {
        private static readonly Models.RingInfo ringInfo;
        public Models.RingInfo RingInfo => ringInfo;

        static MainViewModel()
        {
            ringInfo = new Models.RingInfo();
        }

    }
}
