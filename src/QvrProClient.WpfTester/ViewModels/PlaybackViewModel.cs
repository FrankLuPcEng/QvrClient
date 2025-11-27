using QvrProClient.Models;
using QvrProClient.WpfTester.Services;
using System.Collections.ObjectModel;

namespace QvrProClient.WpfTester.ViewModels;

public class PlaybackViewModel : ViewModelBase
{
    private readonly QvrClientFactory _clientFactory;
    private readonly Action<string> _log;
    private QvrCameraInfo? _selectedCamera;
    private string _startTimeText = DateTimeOffset.UtcNow.AddMinutes(-5).ToString("o");
    private string _endTimeText = DateTimeOffset.UtcNow.ToString("o");
    private string _playbackInfo = string.Empty;

    public PlaybackViewModel(QvrClientFactory clientFactory, Action<string> log)
    {
        _clientFactory = clientFactory;
        _log = log;
        Cameras = new ObservableCollection<QvrCameraInfo>();

        OpenPlaybackCommand = new RelayCommand(OpenPlaybackAsync);
    }

    public ObservableCollection<QvrCameraInfo> Cameras { get; }

    public QvrCameraInfo? SelectedCamera
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

    public string PlaybackInfo
    {
        get => _playbackInfo;
        set => SetProperty(ref _playbackInfo, value);
    }

    public RelayCommand OpenPlaybackCommand { get; }

    private async Task OpenPlaybackAsync()
    {
        var client = _clientFactory.Client;
        if (client is null)
        {
            _log("No client configured. Please login first.");
            return;
        }

        if (!DateTimeOffset.TryParse(StartTimeText, out var start))
        {
            PlaybackInfo = "Invalid start time";
            return;
        }

        if (!DateTimeOffset.TryParse(EndTimeText, out var end))
        {
            PlaybackInfo = "Invalid end time";
            return;
        }

        if (SelectedCamera is null)
        {
            // refresh list if empty
            if (Cameras.Count == 0)
            {
                await LoadCamerasAsync(client).ConfigureAwait(false);
            }

            PlaybackInfo = "Please select a camera.";
            return;
        }

        try
        {
            var session = await client.OpenPlaybackAsync(SelectedCamera.Id, start, end).ConfigureAwait(false);
            PlaybackInfo = $"Session: {session.SessionId}\nStream: {session.StreamUri.ToString() ?? "(none)"}";
            _log($"Opened playback for camera {SelectedCamera.Name ?? SelectedCamera.Id}");
        }
        catch (Exception ex)
        {
            PlaybackInfo = ex.Message;
            _log($"Open playback failed: {ex.Message}");
        }
    }

    private async Task LoadCamerasAsync(IQvrProClient client)
    {
        try
        {
            Cameras.Clear();
            var items = await client.GetCamerasAsync().ConfigureAwait(false);
            foreach (var camera in items)
            {
                Cameras.Add(camera);
            }
        }
        catch (Exception ex)
        {
            _log($"Unable to load cameras: {ex.Message}");
        }
    }
}
