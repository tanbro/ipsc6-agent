using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Mvvm.Input;


namespace ipsc6.agent.wpfapp.ViewModels
{
    public class ConfigViewModel : Utils.SingletonObservableObject<ConfigViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        private static ObservableCollection<string> ipscServerList = new();
        public ObservableCollection<string> IpscServerList
        {
            get => ipscServerList;
            set => SetProperty(ref ipscServerList, value);
        }

        internal void Load()
        {
            var userSettings = ConfigManager.GetUserSettings();

            config.Ipsc cfgIpsc = new();
            userSettings.GetSection(nameof(config.Ipsc)).Bind(cfgIpsc);

            ipscServerList.Clear();
            foreach (var addr in cfgIpsc.ServerList)
            {
                ipscServerList.Add(addr);
            }

        }

        private static readonly IRelayCommand newIpscServerCommand = new RelayCommand(DoNewIpscServer);
        public IRelayCommand NewIpscServerCommand => newIpscServerCommand;

        private static void DoNewIpscServer()
        {
            ipscServerList.Add("");
        }

        private static readonly IRelayCommand delIpscServerCommand = new RelayCommand<object>(DoDelIpscServer);
        public IRelayCommand DelIpscServerCommand => delIpscServerCommand;

        private static void DoDelIpscServer(object item)
        {
            string val = item as string;
            ipscServerList.Remove(val);
        }
    }
}
