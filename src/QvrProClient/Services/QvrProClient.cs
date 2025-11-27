using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QvrProClient.Api;
using QvrProClient.Exceptions;
using QvrProClient.Http;
using QvrProClient.Models;
using QvrProClient.Options;

namespace QvrProClient.Services;

/// <summary>
/// Default implementation of <see cref="IQvrProClient"/> built on top of <see cref="HttpClient"/>.
/// </summary>
public class QvrProClient : IQvrProClient
{
    private readonly QvrProHttpClient _httpClient;
    private readonly ILogger<QvrProClient> _logger;
    private readonly IOptionsMonitor<QvrProOptions> _options;
    private string? _sessionId;

    public QvrProClient(QvrProHttpClient httpClient, IOptionsMonitor<QvrProOptions> options, ILogger<QvrProClient> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        var loginPath = $"{QvrProRoutes.Login}?user={Uri.EscapeDataString(username)}&serviceKey=1&pwd={Uri.EscapeDataString(encodedPassword)}";

        var document = await _httpClient.GetXmlAsync(loginPath, cancellationToken).ConfigureAwait(false);
        var authPassed = document.Root?.Element("authPassed")?.Value?.Trim();
        if (authPassed != "1")
        {
            throw new QvrProException("Authentication failed.", HttpStatusCode.Unauthorized);
        }

        _sessionId = document.Root?.Element("authSid")?.Value?.Trim();
        if (string.IsNullOrEmpty(_sessionId))
        {
            throw new QvrProException("Authentication failed: SID was not returned.", HttpStatusCode.Unauthorized);
        }

        _logger.LogInformation("Authenticated to QVR Pro at {Host}:{Port}", _options.CurrentValue.Host, _options.CurrentValue.Port);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_sessionId))
        {
            _logger.LogDebug("Logout requested but no active session present");
            return;
        }

        var logoutPath = $"{QvrProRoutes.Logout}?sid={Uri.EscapeDataString(_sessionId)}&logout=1";
        await _httpClient.GetXmlAsync(logoutPath, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Logged out of QVR Pro");
        _sessionId = null;
    }

    public Task<IReadOnlyList<QvrCameraInfo>> GetCamerasAsync(CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();
        // TODO: call cameras endpoint defined in swagger.
        IReadOnlyList<QvrCameraInfo> placeholder = Array.Empty<QvrCameraInfo>();
        return Task.FromResult(placeholder);
    }

    public Task<QvrRecordingInfo?> GetRecordingInfoAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();
        // TODO: call recording info endpoint.
        return Task.FromResult<QvrRecordingInfo?>(null);
    }

    public Task<QplaySession> OpenPlaybackAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();
        // TODO: call /qvrpro/apis/qplay.cgi and parse session id/stream info.
        var session = new QplaySession { SessionId = Guid.NewGuid().ToString("N"), StreamUri = null };
        return Task.FromResult(session);
    }

    public Task ExportRecordingAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        EnsureAuthenticated();
        // TODO: initiate export job via API.
        _logger.LogInformation("Export requested for camera {Camera} between {From} and {To}", cameraId, from, to);
        return Task.CompletedTask;
    }

    private void EnsureAuthenticated()
    {
        if (string.IsNullOrEmpty(_sessionId))
        {
            throw new InvalidOperationException("Client is not authenticated. Call LoginAsync first.");
        }
    }
}
