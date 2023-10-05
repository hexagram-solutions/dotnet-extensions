using FakeItEasy.Sdk;

namespace Hexagrams.Extensions.Testing;

internal class AutoFakingServiceProvider : IServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    public AutoFakingServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType) ?? Create.Fake(serviceType);
    }
}
