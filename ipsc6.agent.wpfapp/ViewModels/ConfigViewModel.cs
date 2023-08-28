using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace ipsc6.agent.wpfapp.ViewModels
{

    public class ConfigViewModel : Utils.SingletonObservableObject<ConfigViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        private static Views.ConfigWindow window;

        private static IConfigurationRoot userSettings;

        private static config.Ipsc cfgIpsc;
        private static config.LocalWebServer cfgLocalWebServer;
        private static config.Phone cfgPhone;
        private static config.Startup cfgStartup;

        private static ObservableCollection<ObservableContent<string>> ipscServerList = new();
        public ObservableCollection<ObservableContent<string>> IpscServerList
        {
            get => ipscServerList;
            set => SetProperty(ref ipscServerList, value);
        }

        internal void Load(object sender)
        {
            window = sender as Views.ConfigWindow;

            // 重新加载!
            userSettings = ConfigManager.GetUserSettings();

            cfgIpsc = new();
            userSettings.GetSection(nameof(config.Ipsc)).Bind(cfgIpsc);
            cfgIpsc.ServerList ??= new List<string> { };
            ipscServerList.Clear();
            foreach (var addr in cfgIpsc.ServerList)
            {
                ipscServerList.Add(new ObservableContent<string>(addr));
            }

            cfgLocalWebServer = new();
            userSettings.GetSection(nameof(config.LocalWebServer)).Bind(cfgLocalWebServer);

            cfgPhone = new();
            userSettings.GetSection(nameof(config.Phone)).Bind(cfgPhone);

            cfgStartup = new();
            userSettings.GetSection(nameof(config.Startup)).Bind(cfgStartup);
        }

        private static readonly IRelayCommand newIpscServerCommand = new RelayCommand(DoNewIpscServer);
        public IRelayCommand NewIpscServerCommand => newIpscServerCommand;

        private static void DoNewIpscServer()
        {
            ipscServerList.Add(new ObservableContent<string>());
            saveCommand.NotifyCanExecuteChanged();
        }

        private static readonly IRelayCommand delIpscServerCommand = new RelayCommand<object>(DoDelIpscServer);
        public IRelayCommand DelIpscServerCommand => delIpscServerCommand;

        private static void DoDelIpscServer(object item)
        {
            var val = item as ObservableContent<string>;
            ipscServerList.Remove(val);
            saveCommand.NotifyCanExecuteChanged();
        }

        private static readonly IRelayCommand saveCommand = new RelayCommand(DoSave);
        public IRelayCommand SaveCommand => saveCommand;
        private static void DoSave()
        {
            cfgIpsc.ServerList = (
                from x in ipscServerList
                where !string.IsNullOrWhiteSpace(x.Content)
                select x.Content.Trim()
            ).ToList();
            if (cfgIpsc.ServerList.Count == 0)
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    @"未设置 CTI 服务器地址",
                    Application.Current.MainWindow.Title,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                return;
            }
            foreach (var s0 in cfgIpsc.ServerList)
            {
                if (cfgIpsc.ServerList.Count(s1 => s0 == s1) > 1)
                {
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        @"设置了重复的 CTI 服务器地址",
                        Application.Current.MainWindow.Title,
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                    return;
                }
            }

            var data = JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object>()
            {
                { nameof(config.Ipsc), cfgIpsc },
                { nameof(config.LocalWebServer), cfgLocalWebServer },
                { nameof(config.Phone), cfgPhone },
                { nameof(config.Startup), cfgStartup }
            }, new JsonSerializerOptions { WriteIndented = true });

            Directory.CreateDirectory(Path.GetDirectoryName(ConfigManager.UserSettingsPath));
            using (var fileStream = File.Open(ConfigManager.UserSettingsPath, FileMode.Create))
            {
                fileStream.Write(data, 0, data.Length);
            }

            userSettings.Reload();

            window.Close();
        }

    }
}
