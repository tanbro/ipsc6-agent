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

    }
}
