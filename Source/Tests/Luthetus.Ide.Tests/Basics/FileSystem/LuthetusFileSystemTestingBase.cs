namespace Luthetus.Ide.Tests.Basics.FileSystem;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class LuthetusFileSystemTestingBase
{
    protected readonly ServiceProvider ServiceProvider;

    protected IEnvironmentProvider EnvironmentProvider =>
        ServiceProvider.GetRequiredService<IEnvironmentProvider>();

    protected IFileSystemProvider FileSystemProvider =>
        ServiceProvider.GetRequiredService<IFileSystemProvider>();

    protected IDispatcher Dispatcher =>
        ServiceProvider.GetRequiredService<IDispatcher>();

    public LuthetusFileSystemTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        services.AddScoped<ILuthetusCommonComponentRenderers>(_ => new LuthetusCommonComponentRenderers(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null));

        services.AddLuthetusTextEditor(inTextEditorOptions =>
        {
            var luthetusCommonOptions = inTextEditorOptions.LuthetusCommonOptions ?? new();

            var luthetusCommonFactories = luthetusCommonOptions.LuthetusCommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                StorageServiceFactory = _ => new DoNothingStorageService(true)
            };

            luthetusCommonOptions = luthetusCommonOptions with
            {
                LuthetusCommonFactories = luthetusCommonFactories
            };

            return inTextEditorOptions with
            {
                CustomThemeRecords = LuthetusTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = LuthetusTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
                LuthetusCommonOptions = luthetusCommonOptions
            };
        });

        services.AddScoped<IEnvironmentProvider>(_ => new LocalEnvironmentProvider());
        services.AddScoped<IFileSystemProvider>(_ => new LocalFileSystemProvider());

        services.AddFluxor(options => options
            .ScanAssemblies(
                typeof(LuthetusCommonOptions).Assembly,
                typeof(LuthetusTextEditorOptions).Assembly,
                typeof(ClassLib.ServiceCollectionExtensions).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();
    }
}