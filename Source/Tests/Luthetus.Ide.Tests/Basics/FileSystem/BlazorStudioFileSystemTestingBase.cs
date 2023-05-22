using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.ComponentRenderers;
using BlazorCommon.RazorLib.Storage;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.Ide.Tests.Basics.FileSystem;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class BlazorStudioFileSystemTestingBase
{
    protected readonly ServiceProvider ServiceProvider;

    protected IEnvironmentProvider EnvironmentProvider =>
        ServiceProvider.GetRequiredService<IEnvironmentProvider>();

    protected IFileSystemProvider FileSystemProvider =>
        ServiceProvider.GetRequiredService<IFileSystemProvider>();

    protected IDispatcher Dispatcher =>
        ServiceProvider.GetRequiredService<IDispatcher>();

    public BlazorStudioFileSystemTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        services.AddScoped<IBlazorCommonComponentRenderers>(_ => new BlazorCommonComponentRenderers(
            null,
            null,
            null,
            null,
            null,
            null));

        var shouldInitializeFluxor = false;

        services.AddBlazorTextEditor(inTextEditorOptions =>
        {
            var blazorCommonOptions =
                (inTextEditorOptions.BlazorCommonOptions ?? new()) with
                {
                    InitializeFluxor = shouldInitializeFluxor
                };

            var blazorCommonFactories = blazorCommonOptions.BlazorCommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                StorageServiceFactory = _ => new DoNothingStorageService(true)
            };

            blazorCommonOptions = blazorCommonOptions with
            {
                BlazorCommonFactories = blazorCommonFactories
            };

            return inTextEditorOptions with
            {
                InitializeFluxor = shouldInitializeFluxor,
                CustomThemeRecords = BlazorTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = BlazorTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
                BlazorCommonOptions = blazorCommonOptions
            };
        });

        services.AddScoped<IEnvironmentProvider>(_ => new LocalEnvironmentProvider());
        services.AddScoped<IFileSystemProvider>(_ => new LocalFileSystemProvider());

        services.AddFluxor(options => options
            .ScanAssemblies(
                typeof(BlazorCommon.RazorLib.ServiceCollectionExtensions).Assembly,
                typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly,
                typeof(ClassLib.ServiceCollectionExtensions).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();
    }
}