using System;
using System.Threading.Tasks;
using QvrProClient.Options;
using QvrProClient.WpfTester.Services;

namespace QvrProClient.WpfTester.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly QvrClientFactory _clientFactory;
    private readonly Action<string> _log;
    private string _host = "localhost";
    private int _port = 8080;
    private bool _useHttps = true;
    private bool _ignoreCert;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _isLoggedIn;
    private string? _errorMessage;

    public LoginViewModel(QvrClientFactory clientFactory, Action<string> log)
    {
        _clientFactory = clientFactory;
        _log = log;

        LoginCommand = new RelayCommand(LoginAsync, CanLogin);
        LogoutCommand = new RelayCommand(LogoutAsync, () => IsLoggedIn);
    }

    public string Host
    {
        get => _host;
        set => SetProperty(ref _host, value);
    }

    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    public bool UseHttps
    {
        get => _useHttps;
        set => SetProperty(ref _useHttps, value);
    }

    public bool IgnoreCert
    {
        get => _ignoreCert;
        set => SetProperty(ref _ignoreCert, value);
    }

    public string Username
    {
        get => _username;
        set
        {
            if (SetProperty(ref _username, value))
            {
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        private set
        {
            if (SetProperty(ref _isLoggedIn, value))
            {
                LoginCommand.RaiseCanExecuteChanged();
                LogoutCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public RelayCommand LoginCommand { get; }

    public RelayCommand LogoutCommand { get; }

    private bool CanLogin() => !IsLoggedIn && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

    private async Task LoginAsync()
    {
        ErrorMessage = null;
        try
        {
            var options = new QvrProOptions
            {
                Host = Host,
                Port = Port,
                UseHttps = UseHttps,
                IgnoreCertificateErrors = IgnoreCert,
                DefaultUsername = Username,
                DefaultPassword = Password
            };

            _clientFactory.Configure(options);
            var client = _clientFactory.Client ?? throw new InvalidOperationException("Client not initialized.");
            await client.LoginAsync(Username, Password).ConfigureAwait(false);
            IsLoggedIn = true;
            _log($"Logged in as {Username}@{Host}:{Port} (HTTPS={(UseHttps ? "on" : "off")})");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _log($"Login failed: {ex.Message}");
        }
    }

    private async Task LogoutAsync()
    {
        try
        {
            var client = _clientFactory.Client;
            if (client is not null)
            {
                await client.LogoutAsync().ConfigureAwait(false);
            }

            IsLoggedIn = false;
            _log("Logged out");
        }
        catch (Exception ex)
        {
            _log($"Logout failed: {ex.Message}");
        }
    }
}
