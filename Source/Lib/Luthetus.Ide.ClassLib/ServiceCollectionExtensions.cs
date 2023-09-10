using Luthetus.Ide.ClassLib.FileTemplates;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Nuget;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.Ide.ClassLib.CommandCase;

namespace Luthetus.Ide.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeClassLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        if (hostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
        {
            services
                .AddHostedService(sp => sp.GetRequiredService<LuthetusIdeFileSystemBackgroundTaskServiceWorker>())
                .AddHostedService(sp => sp.GetRequiredService<LuthetusIdeTerminalBackgroundTaskServiceWorker>());
        }

        if (hostingInformation.LuthetusHostingKind != LuthetusHostingKind.UnitTesting)
        {
            services
                .AddSingleton<ILuthetusIdeFileSystemBackgroundTaskService, LuthetusIdeFileSystemBackgroundTaskService>()
                .AddSingleton<LuthetusIdeFileSystemBackgroundTaskServiceWorker>()
                .AddSingleton<ILuthetusIdeTerminalBackgroundTaskService, LuthetusIdeTerminalBackgroundTaskService>()
                .AddSingleton<LuthetusIdeTerminalBackgroundTaskServiceWorker>();
        }

        services
            .AddScoped<ICommandFactory, CommandFactory>()
            .AddScoped<XmlCompilerService>()
            .AddScoped<DotNetSolutionCompilerService>()
            .AddScoped<CSharpProjectCompilerService>()
            .AddScoped<CSharpCompilerService>()
            .AddScoped<RazorCompilerService>()
            .AddScoped<CssCompilerService>()
            .AddScoped<FSharpCompilerService>()
            .AddScoped<JavaScriptCompilerService>()
            .AddScoped<TypeScriptCompilerService>()
            .AddScoped<JsonCompilerService>()
            .AddScoped<IMenuOptionsFactory, MenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();

        services
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(ServiceCollectionExtensions).Assembly,
                    typeof(LuthetusCommonOptions).Assembly,
                    typeof(LuthetusTextEditorOptions).Assembly));

        return services;
    }
}