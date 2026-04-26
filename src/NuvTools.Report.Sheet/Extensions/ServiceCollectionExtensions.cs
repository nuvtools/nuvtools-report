using Microsoft.Extensions.DependencyInjection;
using NuvTools.Report.Sheet.Csv;
using NuvTools.Report.Sheet.Excel;

namespace NuvTools.Report.Sheet.Extensions;

/// <summary>
/// Provides extension methods for registering report services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers sheet report readers and exporters (<see cref="ICsvReader"/>, <see cref="ICsvExporter"/>,
    /// and <see cref="IExcelExporter"/>) as singleton services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance, enabling method chaining.</returns>
    public static IServiceCollection AddSheetReportServices(this IServiceCollection services)
    {
        services.AddSingleton<ICsvReader, CsvReader>();
        services.AddSingleton<ICsvExporter, CsvExporter>();
        services.AddSingleton<IExcelExporter, ExcelExporter>();
        return services;
    }
}
