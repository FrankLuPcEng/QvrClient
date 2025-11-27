namespace QvrProClient.Models;

/// <summary>
/// Basic camera metadata returned by QVR Pro.
/// </summary>
public record QvrCameraInfo
{
    public required string Id { get; init; }

    public string? Name { get; init; }

    public string? Channel { get; init; }

    public string? Ip { get; init; }

    public string? Url { get; init; }
}
