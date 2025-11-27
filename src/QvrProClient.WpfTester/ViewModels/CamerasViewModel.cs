using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QvrProClient.Models;
using QvrProClient.WpfTester.Services;

namespace QvrProClient.WpfTester.ViewModels;

public class CamerasViewModel : ViewModelBase
{
    private readonly QvrClientFactory _clientFactory;
    private readonly Action<string> _log;

    public CamerasViewModel(QvrClientFactory clientFactory, Action<string> log)
    {
        _clientFactory = clientFactory;
        _log = log;
        Cameras = new ObservableCollection<QvrCameraInfo>();

        LoadCamerasCommand = new RelayCommand(LoadCamerasAsync);
    }

    public ObservableCollection<QvrCameraInfo> Cameras { get; }

    public RelayCommand LoadCamerasCommand { get; }

    private async Task LoadCamerasAsync()
    {
        var client = _clientFactory.Client;
        if (client is null)
        {
            _log("No client configured. Please login first.");
            return;
        }

        try
        {
            Cameras.Clear();
            var items = await client.GetCamerasAsync().ConfigureAwait(false);
            foreach (var camera in items)
            {
                Cameras.Add(camera);
            }

            _log($"Loaded {Cameras.Count} camera(s).");
        }
        catch (Exception ex)
        {
            _log($"Failed to load cameras: {ex.Message}");
        }
    }
}
