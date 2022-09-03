using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVCollection
{
    public class Settings : NotifyPropertyChanged
    {
        public string RemoteProcessingHost
        {
            get => Preferences.Default.Get(nameof(RemoteProcessingHost), "http://localhost:2003");
            set
            {
                Preferences.Default.Set(nameof(RemoteProcessingHost), value);
                CVModels.Settings.RemoteProcessHost = value;
                OnPropertyChanged(nameof(RemoteProcessingHost));
            }
        }

        public string ModelDownloadHost
        {
            get => Preferences.Default.Get(nameof(ModelDownloadHost), "http://localhost:8008");
            set
            {
                Preferences.Default.Set(nameof(ModelDownloadHost), value);
                CVModels.Settings.ModelDownloadingHost = value;
                OnPropertyChanged(nameof(ModelDownloadHost));
            }
        }

        public string DownloadedModelFolder
        {
            get => Path.Combine(FileSystem.Current.AppDataDirectory, "models");
        }

        public static Settings Instance = new Settings();
    }

    internal class SettingsViewModel : BaseViewModel
    {
        public Settings Instance => Settings.Instance;
        public Command PingProcessServerCommand { get; }
        public Command TestCommand { get; }
        public Command DeleteDownloadedModelCommand { get; }
        public SettingsViewModel()
        {
            Title = "Settings";
            PingProcessServerCommand = new Command(async () =>
            {
                try
                {
                    IsBusy = true;
                    var client = new RestClient(Instance.RemoteProcessingHost);
                    var request = new RestRequest("ping");
                    var res = await client.GetAsync(request);
                    App.AlertSvc.ShowAlert("Ping Successful", res.Content);
                }
                catch (Exception ex)
                {
                    App.AlertSvc.ShowAlert("Ping Failed", ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }, () => !IsBusy);

            TestCommand = new Command(async () =>
            {
                App.AlertSvc.ShowAlert("Test", FileSystem.Current.AppDataDirectory);
            });

            DeleteDownloadedModelCommand = new Command(async () =>
            {
                try
                {
                    IsBusy = true;
                    await Task.Run(() => Directory.Delete(Instance.DownloadedModelFolder, true));
                    App.AlertSvc.ShowAlert("Done", "Downloaded models are deleted.");
                }
                catch (Exception ex)
                {
                    App.AlertSvc.ShowAlert("Oops", ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }, () => !IsBusy);

            PropertyChanged += (_, _) =>
            {
                PingProcessServerCommand.ChangeCanExecute();
                DeleteDownloadedModelCommand.ChangeCanExecute();
            };
        }
    }
}
