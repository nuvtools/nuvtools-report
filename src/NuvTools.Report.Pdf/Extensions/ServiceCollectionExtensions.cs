using Microsoft.Extensions.DependencyInjection;
using NuvTools.Report.Pdf.Table;
using NuvTools.Report.Pdf.Util;

namespace NuvTools.Report.Pdf.Extensions;

/// <summary>
/// Provides extension methods for registering PDF report services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers PDF report services (<see cref="IPdfExporter"/> and <see cref="IPdfMerger"/>) as singleton services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance, enabling method chaining.</returns>
    public static IServiceCollection AddPdfReportServices(this IServiceCollection services)
    {
        services.AddSingleton<IPdfExporter, PdfExporter>();
        services.AddSingleton<IPdfMerger, PdfMerger>();
        return services;
    }
}
