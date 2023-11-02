using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Misc;

public class CSharpCompilerServiceTestsBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ITextEditorService TextEditorService;
    protected readonly CSharpCompilerService CSharpCompilerService;

    public CSharpCompilerServiceTestsBase()
    {
        var services = new ServiceCollection();

        services
            .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
            .AddScoped<CSharpCompilerService>()
            .AddScoped<IEnvironmentProvider>(serviceProvider => new InMemoryEnvironmentProvider())
            .AddScoped<IFileSystemProvider>(serviceProvider => new InMemoryFileSystemProvider(serviceProvider.GetRequiredService<IEnvironmentProvider>()));

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        services.AddLuthetusCommonServices(hostingInformation, commonOptions =>
        {
            var outLuthetusCommonFactories = commonOptions.CommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                StorageServiceFactory = _ => new DoNothingStorageService(true)
            };

            return commonOptions with
            {
                CommonFactories = outLuthetusCommonFactories
            };
        });

        services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
        {
            AddLuthetusCommon = false
        });

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        TextEditorService = ServiceProvider.GetRequiredService<ITextEditorService>();

        CSharpCompilerService = ServiceProvider.GetRequiredService<CSharpCompilerService>();
    }
}
