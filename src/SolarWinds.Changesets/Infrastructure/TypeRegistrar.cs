using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Infrastructure;

/// <inheritdoc />
internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _serviceCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRegistrar"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceCollection"/> is null.</exception>
    public TypeRegistrar(IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);
        _serviceCollection = serviceCollection;
    }

    /// <inheritdoc />
    public void Register(Type service, Type implementation)
    {
        _serviceCollection.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterInstance(Type service, object implementation)
    {
        _serviceCollection.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.</exception>
    public void RegisterLazy(Type service, Func<object> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        _serviceCollection.AddSingleton(service, (provider) => func());
    }

    /// <inheritdoc />
    public ITypeResolver Build()
    {
        return new TypeResolver(_serviceCollection.BuildServiceProvider());
    }
}
