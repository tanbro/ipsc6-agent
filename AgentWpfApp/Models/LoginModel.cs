using System.ComponentModel;
using System.Security;

namespace AgentWpfApp.Models
{
    class LoginModel : SingletonModelBase<LoginModel>, INotifyPropertyChanged
    {
        private string workerNum;
        public string WorkerNum
        {
            get => workerNum;
            set => SetField(ref workerNum, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => SetField(ref password, value);
        }

    }
}
