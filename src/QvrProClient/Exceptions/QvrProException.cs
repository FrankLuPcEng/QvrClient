using System.Net;

namespace QvrProClient.Exceptions;

/// <summary>
/// Represents an error returned by QVR Pro endpoints.
/// </summary>
public class QvrProException : Exception
{
    public QvrProException(string message, HttpStatusCode? statusCode = null, string? errorCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// HTTP status code returned by the API, if any.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// QVR Pro-specific error code.
    /// </summary>
    public string? ErrorCode { get; }
}
