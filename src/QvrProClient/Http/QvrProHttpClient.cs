using System.Net.Http.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QvrProClient.Exceptions;
using QvrProClient.Options;

namespace QvrProClient.Http;

/// <summary>
/// Lightweight wrapper around HttpClient with QVR Pro defaults.
/// </summary>
public class QvrProHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<QvrProHttpClient> _logger;
    private readonly IOptionsMonitor<QvrProOptions> _options;

    public QvrProHttpClient(HttpClient httpClient, IOptionsMonitor<QvrProOptions> options, ILogger<QvrProHttpClient> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Execute a GET request and deserialize JSON content.
    /// </summary>
    public async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        EnsureBaseAddress();
        var response = await _httpClient.GetAsync(path, cancellationToken).ConfigureAwait(false);
        return await HandleResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Execute a POST request with JSON content and deserialize JSON response.
    /// </summary>
    public async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest payload, CancellationToken cancellationToken = default)
    {
        EnsureBaseAddress();
        var response = await _httpClient.PostAsJsonAsync(path, payload, cancellationToken).ConfigureAwait(false);
        return await HandleResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Execute a GET request expecting an XML payload.
    /// </summary>
    public async Task<XDocument> GetXmlAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureBaseAddress();
        using var response = await _httpClient.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var message = $"QVR Pro request failed with status {(int)response.StatusCode} ({response.StatusCode})";
            throw new QvrProException(message, response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return XDocument.Load(contentStream);
    }

    private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (content is null)
            {
                throw new QvrProException("Empty response when content was expected.", response.StatusCode);
            }

            return content;
        }

        string? errorCode = null;
        try
        {
            var errorEnvelope = await response.Content.ReadFromJsonAsync<QvrProErrorEnvelope>(cancellationToken: cancellationToken).ConfigureAwait(false);
            errorCode = errorEnvelope?.Code;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse QVR Pro error payload");
        }

        var message = $"QVR Pro request failed with status {(int)response.StatusCode} ({response.StatusCode})";
        throw new QvrProException(message, response.StatusCode, errorCode);
    }

    private void EnsureBaseAddress()
    {
        var desiredBase = _options.CurrentValue.BuildBaseUri();
        if (_httpClient.BaseAddress != desiredBase)
        {
            _logger.LogDebug("Updating QVR Pro base address to {BaseUri}", desiredBase);
            _httpClient.BaseAddress = desiredBase;
        }
    }

    private record QvrProErrorEnvelope(string? Code, string? Message);
}
