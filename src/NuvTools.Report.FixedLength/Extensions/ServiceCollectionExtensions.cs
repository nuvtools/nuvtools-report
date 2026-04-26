using Microsoft.Extensions.DependencyInjection;

namespace NuvTools.Report.FixedLength.Extensions;

/// <summary>
/// Provides extension methods for registering fixed-length report services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers fixed-length report services (<see cref="IFixedLengthReader"/>) as singleton services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance, enabling method chaining.</returns>
    public static IServiceCollection AddFixedLengthReportServices(this IServiceCollection services)
    {
        services.AddSingleton<IFixedLengthReader, FixedLengthReader>();
        return services;
    }
}
