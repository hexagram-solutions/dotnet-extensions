using FakeItEasy.Sdk;
using Microsoft.Extensions.DependencyInjection;

namespace Hexagrams.Extensions.Testing;

internal class AutoFakingServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// Creates an <see cref="IServiceProvider" /> instance that registers a fake instance of any
    /// missing dependencies for requested services.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to use.</param>
    /// <returns>An <see cref="IServiceProvider" /> with missing registrations registered as fakes.</returns>
    public IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
    {
        var implicitlyConstructedServicesDescriptors =
            serviceCollection.Where(d => d.ImplementationFactory == null);

        foreach (var descriptor in implicitlyConstructedServicesDescriptors.ToArray())
        {
            var definedServiceConstructors = descriptor.ServiceType.GetConstructors()
                .Where(c => c.GetParameters().Length != 0)
                .ToArray();

            // No defined (non-default) constructors here, move on
            if (definedServiceConstructors.Length == 0)
                continue;

            // Find the least specific non-default constructor for the service type, i.e.: the one with the fewest
            // parameters.
            var bestConstructor = definedServiceConstructors.First(ctor =>
            {
                var lowestParameterCount = definedServiceConstructors.Min(c => c.GetParameters().Length);

                return ctor.GetParameters().Length == lowestParameterCount;
            });

            var ctorParameterTypes = bestConstructor.GetParameters().Select(p => p.ParameterType);

            // Don't fake framework constructs
            var filteredCtorParameterTypes = ctorParameterTypes.Where(pt =>
                !pt.Namespace!.StartsWith("System", StringComparison.InvariantCulture) &&
                !pt.Namespace!.StartsWith("Microsoft.Extensions", StringComparison.InvariantCulture));

            foreach (var parameterType in filteredCtorParameterTypes)
            {
                // If there's already a type registered for this constructor parameter, move on, otherwise register a
                // fake of it.
                if (serviceCollection.Any(sd => sd.ServiceType == parameterType))
                    continue;

                serviceCollection.Add(new ServiceDescriptor(parameterType, Create.Fake(parameterType)));
            }
        }

        return new AutoFakingServiceProvider(serviceCollection.BuildServiceProvider());
    }
}
