using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QvrProClient;

namespace QvrProClient.WpfTester.ViewModels;

public class CamerasViewModel : ViewModelBase
{
    private readonly Action<string> _log;
    private IQvrProClient? _client;

    public CamerasViewModel(Action<string> log, IQvrProClient? client = null)
    {
        _log = log;
        _client = client;
        Cameras = new ObservableCollection<CameraViewModel>();

        LoadCamerasCommand = new RelayCommand(LoadCamerasAsync, CanLoadCameras);
    }

    public ObservableCollection<CameraViewModel> Cameras { get; }

    public RelayCommand LoadCamerasCommand { get; }

    public IQvrProClient? Client
    {
        get => _client;
        set
        {
            if (SetProperty(ref _client, value))
            {
                LoadCamerasCommand.RaiseCanExecuteChanged();
                if (_client is null)
                {
                    Cameras.Clear();
                }
            }
        }
    }

    private bool CanLoadCameras() => Client is not null;

    private async Task LoadCamerasAsync()
    {
        var client = Client;
        if (client is null)
        {
            _log("No client configured. Please login first.");
            return;
        }

        try
        {
            Cameras.Clear();
            var items = await client.GetCamerasAsync();
            foreach (var camera in items)
            {
                Cameras.Add(new CameraViewModel(camera));
            }

            _log($"Loaded {Cameras.Count} camera(s).");
        }
        catch (Exception ex)
        {
            _log($"Failed to load cameras: {ex.Message}");
        }
    }
}
