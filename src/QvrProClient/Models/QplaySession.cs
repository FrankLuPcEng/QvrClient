using System.Net;

namespace QvrProClient.Models;

/// <summary>
/// Represents a Qplay playback session descriptor.
/// </summary>
public record QplaySession
{
    public required string SessionId { get; init; }

    public Uri? StreamUri { get; init; }

    public HttpStatusCode? StatusCode { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
