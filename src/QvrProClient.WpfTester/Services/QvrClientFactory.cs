using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QvrProClient;
using QvrProClient.DependencyInjection;
using QvrProClient.Options;

namespace QvrProClient.WpfTester.Services;

/// <summary>
/// Lightweight service locator used by the tester to materialize <see cref="IQvrProClient"/> instances
/// based on the latest connection settings entered in the UI.
/// </summary>
public class QvrClientFactory : IDisposable
{
    private IServiceProvider? _provider;
    private IServiceScope? _scope;

    public IQvrProClient? Client => _scope?.ServiceProvider.GetService<IQvrProClient>();

    public void Configure(QvrProOptions options)
    {
        _scope?.Dispose();
        if (_provider is IDisposable disposableProvider)
        {
            disposableProvider.Dispose();
        }

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddQvrProClient(opt =>
        {
            opt.Host = options.Host;
            opt.Port = options.Port;
            opt.UseHttps = options.UseHttps;
            opt.IgnoreCertificateErrors = options.IgnoreCertificateErrors;
            opt.DefaultUsername = options.DefaultUsername;
            opt.DefaultPassword = options.DefaultPassword;
            opt.Timeout = options.Timeout;
        });

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        if (_provider is IDisposable disposableProvider)
        {
            disposableProvider.Dispose();
        }
    }
}
