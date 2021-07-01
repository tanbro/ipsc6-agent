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
        public async Task LogIn(string workerNumber, string password)
        {
            ViewModels.LoginViewModel.Instance.WorkerNumber = workerNumber;
#pragma warning disable VSTHRD111
            await ViewModels.LoginViewModel.DoLoginAsync(password);
#pragma warning restore VSTHRD111
        }
#pragma warning restore VSTHRD200
    }
}
