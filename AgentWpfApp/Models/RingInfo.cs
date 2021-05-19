using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AgentWpfApp.Models
{
    public class RingInfo : INotifyPropertyChanged
    {
        private string teleNum;
        public string TeleNum
        {
            get => teleNum;
            set => SetField(ref teleNum, value);
        }

        private string location;
        public string Location
        {
            get => location;
            set => SetField(ref location, value);
        }

        private string ivrPath = "[]";
        public string IvrPath
        {
            get => ivrPath;
            set => SetField(ref ivrPath, value);
        }

        private string bizName;
        public string BizName
        {
            get => bizName;
            set => SetField(ref bizName, value);
        }

        #region SetField
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region INotifyPropertyChanged Members  
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
