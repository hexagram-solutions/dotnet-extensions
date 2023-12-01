using FakeItEasy.Sdk;

namespace Hexagrams.Extensions.Testing;

internal sealed class AutoFakingServiceProvider(IServiceProvider serviceProvider) : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        return serviceProvider.GetService(serviceType) ?? Create.Fake(serviceType);
    }
}
