using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ipsc6.agent.wpfapp
{
    public class GuiService
    {
#pragma warning disable VSTHRD200
        public async Task LogIn(string workerNum, string password)
        {
            ViewModels.LoginViewModel.Instance.WorkerNum = workerNum;
            await ViewModels.LoginViewModel.DoLoginAsync(password);
        }
#pragma warning restore VSTHRD200
    }
}
