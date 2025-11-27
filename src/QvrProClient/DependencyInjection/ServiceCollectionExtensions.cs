using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QvrProClient;
using QvrProClient.Http;
using QvrProClient.Options;
using QvrProClient.Services;

namespace QvrProClient.DependencyInjection;

/// <summary>
/// ServiceCollection extensions for QvrProClient.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQvrProClient(this IServiceCollection services, Action<QvrProOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<QvrProHttpClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<QvrProOptions>>().CurrentValue;
            client.BaseAddress = options.BuildBaseUri();
            client.Timeout = options.Timeout;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<QvrProOptions>>().CurrentValue;
            var handler = new HttpClientHandler();
            if (options.IgnoreCertificateErrors)
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }

            return handler;
        });

        services.AddScoped<IQvrProClient, Services.QvrProClient>();
        return services;
    }
}
