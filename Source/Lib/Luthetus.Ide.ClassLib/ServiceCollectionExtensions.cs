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
using Luthetus.Ide.ClassLib.CommandCase;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Microsoft.Extensions.Logging;

namespace Luthetus.Ide.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeClassLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        hostingInformation.BackgroundTaskService.RegisterQueue(FileSystemBackgroundTaskWorker.Queue);

        services.AddSingleton(sp => new FileSystemBackgroundTaskWorker(
            FileSystemBackgroundTaskWorker.Queue.Key,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>()));

        hostingInformation.BackgroundTaskService.RegisterQueue(TerminalBackgroundTaskWorker.Queue);

        services.AddSingleton(sp => new TerminalBackgroundTaskWorker(
            TerminalBackgroundTaskWorker.Queue.Key,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>()));

        if (hostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
        {
            services.AddHostedService(sp => sp.GetRequiredService<FileSystemBackgroundTaskWorker>());
            services.AddHostedService(sp => sp.GetRequiredService<TerminalBackgroundTaskWorker>());
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