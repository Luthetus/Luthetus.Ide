using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Fluxor;
using Luthetus.Ide.ClassLib.FileTemplates;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Nuget;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Ide.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeClassLibServices(
        this IServiceCollection services)
    {
        return services
            .AddScoped<ICommonMenuOptionsFactory, CommonMenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>()
            .AddSingleton<IBackgroundTaskMonitor, BackgroundTaskMonitor>()
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(Luthetus.Common.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(Luthetus.TextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                    typeof(ServiceCollectionExtensions).Assembly));
    }
}