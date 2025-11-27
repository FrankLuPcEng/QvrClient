using QvrProClient.Models;

namespace QvrProClient;

/// <summary>
/// High-level entry point for interacting with QVR Pro APIs.
/// </summary>
public interface IQvrProClient
{
    /// <summary>
    /// Authenticate against QVR Pro and cache the session token.
    /// </summary>
    Task LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sign out of QVR Pro, invalidating the current session.
    /// </summary>
    Task LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve all cameras visible to the current user.
    /// </summary>
    Task<IReadOnlyList<QvrCameraInfo>> GetCamerasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get metadata for a recording segment.
    /// </summary>
    Task<QvrRecordingInfo?> GetRecordingInfoAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Open a Qplay playback session.
    /// </summary>
    Task<QplaySession> OpenPlaybackAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trigger a server-side export for a recording segment.
    /// </summary>
    Task ExportRecordingAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}
