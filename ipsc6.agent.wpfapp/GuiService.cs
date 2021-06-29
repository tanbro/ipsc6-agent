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
            await ViewModels.LoginViewModel.DoLoginAsync(password).ConfigureAwait(false);
        }
#pragma warning restore VSTHRD200
    }
}
