using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QvrProClient;
using QvrProClient.Models;

namespace QvrProClient.WpfTester.ViewModels;

public class PlaybackViewModel : ViewModelBase
{
    private readonly Action<string> _log;
    private IQvrProClient? _client;
    private CameraViewModel? _selectedCamera;
    private string _startTimeText = DateTimeOffset.UtcNow.AddMinutes(-5).ToString("o");
    private string _endTimeText = DateTimeOffset.UtcNow.ToString("o");
    private string _resultText = string.Empty;

    public PlaybackViewModel(Action<string> log, IQvrProClient? client = null)
    {
        _log = log;
        _client = client;
        Cameras = new ObservableCollection<CameraViewModel>();

        OpenPlaybackCommand = new RelayCommand(OpenPlaybackAsync, CanOpenPlayback);
    }

    public ObservableCollection<CameraViewModel> Cameras { get; }

    public CameraViewModel? SelectedCamera
    {
        get => _selectedCamera;
        set => SetProperty(ref _selectedCamera, value);
    }

    public string StartTimeText
    {
        get => _startTimeText;
        set => SetProperty(ref _startTimeText, value);
    }

    public string EndTimeText
    {
        get => _endTimeText;
        set => SetProperty(ref _endTimeText, value);
    }

    public string ResultText
    {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }

    public RelayCommand OpenPlaybackCommand { get; }

    public IQvrProClient? Client
    {
        get => _client;
        set
        {
            if (SetProperty(ref _client, value))
            {
                OpenPlaybackCommand.RaiseCanExecuteChanged();
                if (_client is null)
                {
                    Cameras.Clear();
                    SelectedCamera = null;
                }
                else
                {
                    _ = LoadCamerasAsync();
                }
            }
        }
    }

    private bool CanOpenPlayback() => Client is not null;

    private async Task LoadCamerasAsync()
    {
        var client = Client;
        if (client is null)
        {
            return;
        }

        try
        {
            var items = await client.GetCamerasAsync().ConfigureAwait(false);
            Cameras.Clear();
            foreach (var camera in items)
            {
                Cameras.Add(new CameraViewModel(camera));
            }

            if (SelectedCamera is null && Cameras.Count > 0)
            {
                SelectedCamera = Cameras[0];
            }
        }
        catch (Exception ex)
        {
            _log($"Unable to load cameras: {ex.Message}");
        }
    }

    private async Task OpenPlaybackAsync()
    {
        var client = Client;
        if (client is null)
        {
            ResultText = "No client configured. Please login first.";
            _log(ResultText);
            return;
        }

        if (!DateTimeOffset.TryParse(StartTimeText, out var start))
        {
            ResultText = "Invalid start time format.";
            return;
        }

        if (!DateTimeOffset.TryParse(EndTimeText, out var end))
        {
            ResultText = "Invalid end time format.";
            return;
        }

        if (SelectedCamera is null)
        {
            if (Cameras.Count == 0)
            {
                await LoadCamerasAsync().ConfigureAwait(false);
            }

            if (SelectedCamera is null)
            {
                ResultText = "Please select a camera.";
                return;
            }
        }

        try
        {
            var session = await client.OpenPlaybackAsync(SelectedCamera.CameraId, start, end).ConfigureAwait(false);
            var status = session.StatusCode?.ToString() ?? "N/A";
            var stream = session.StreamUri?.ToString() ?? "(none)";
            ResultText = $"Status: {status}\nSession: {session.SessionId}\nStream: {stream}";
            _log($"Opened playback for camera {SelectedCamera.Name} [{SelectedCamera.CameraId}]");
        }
        catch (Exception ex)
        {
            ResultText = $"Open playback failed: {ex.Message}";
            _log(ResultText);
        }
    }
}
