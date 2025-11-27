using System.Text;
using QvrProClient.WpfTester.Services;

namespace QvrProClient.WpfTester.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly StringBuilder _logBuilder = new();
    private readonly QvrClientFactory _clientFactory = new();
    private string _logText = string.Empty;

    public MainViewModel()
    {
        LoginViewModel = new LoginViewModel(_clientFactory, AppendLog);
        CamerasViewModel = new CamerasViewModel(_clientFactory, AppendLog);
        PlaybackViewModel = new PlaybackViewModel(_clientFactory, AppendLog);
    }

    public LoginViewModel LoginViewModel { get; }

    public CamerasViewModel CamerasViewModel { get; }

    public PlaybackViewModel PlaybackViewModel { get; }

    public string LogText
    {
        get => _logText;
        private set => SetProperty(ref _logText, value);
    }

    private void AppendLog(string message)
    {
        _logBuilder.AppendLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        LogText = _logBuilder.ToString();
    }
}
