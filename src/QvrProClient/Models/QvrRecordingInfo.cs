namespace QvrProClient.Models;

/// <summary>
/// Recording metadata from QVR Pro.
/// </summary>
public record QvrRecordingInfo
{
    public required string CameraId { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; init; }

    public long SizeBytes { get; init; }
}
