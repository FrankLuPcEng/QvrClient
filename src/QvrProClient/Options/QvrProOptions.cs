namespace QvrProClient.Options;

/// <summary>
/// Configuration for reaching a QVR Pro instance.
/// </summary>
public class QvrProOptions
{
    /// <summary>
    /// Hostname or IP of the QVR Pro server.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// TCP port for the API service.
    /// </summary>
    public int Port { get; set; } = 8080;

    /// <summary>
    /// Whether to prefer HTTPS.
    /// </summary>
    public bool UseHttps { get; set; } = true;

    /// <summary>
    /// Whether to bypass certificate validation (only for lab/dev use).
    /// </summary>
    public bool IgnoreCertificateErrors { get; set; }

    /// <summary>
    /// Optional username used when the client is created.
    /// </summary>
    public string? DefaultUsername { get; set; }

    /// <summary>
    /// Optional password used when the client is created.
    /// </summary>
    public string? DefaultPassword { get; set; }

    /// <summary>
    /// Default timeout applied to API calls.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Build a base URI from the current options.
    /// </summary>
    public Uri BuildBaseUri() => new Uri($"{(UseHttps ? "https" : "http")}://{Host}:{Port}");
}
