﻿using Fluxor;
using Luthetus.Ide.ClassLib.FileTemplates;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Nuget;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.FileSystem.HostedServiceCase;
using Luthetus.Common.RazorLib;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;

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
            .AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueue>()
            .AddSingleton<ICommonBackgroundTaskMonitor, CommonBackgroundTaskMonitor>()
            .AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueue>()
            .AddSingleton<ITextEditorBackgroundTaskMonitor, TextEditorBackgroundTaskMonitor>()
            .AddSingleton<IFileSystemBackgroundTaskQueue, FileSystemBackgroundTaskQueue>()
            .AddSingleton<IFileSystemBackgroundTaskMonitor, FileSystemBackgroundTaskMonitor>()
            .AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueue>()
            .AddSingleton<ICompilerServiceBackgroundTaskMonitor, CompilerServiceBackgroundTaskMonitor>()
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(ServiceCollectionExtensions).Assembly,
                    typeof(LuthetusCommonOptions).Assembly,
                    typeof(LuthetusTextEditorOptions).Assembly));
    }
}