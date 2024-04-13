using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.CompilerServices.Lang.Razor.Tests;

public class RazorCompilerServiceTestsBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ITextEditorService TextEditorService;
    protected readonly RazorCompilerService RazorCompilerService;
    protected readonly XmlCompilerService XmlCompilerService;
    protected readonly CSharpCompilerService CSharpCompilerService;

    public RazorCompilerServiceTestsBase()
    {
        var services = new ServiceCollection();

        services
            .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
            .AddScoped<RazorCompilerService>()
            .AddScoped<CSharpCompilerService>()
            .AddScoped<XmlCompilerService>()
            .AddScoped<IEnvironmentProvider>(serviceProvider => new InMemoryEnvironmentProvider())
            .AddScoped<IFileSystemProvider>(serviceProvider => new InMemoryFileSystemProvider(
                serviceProvider.GetRequiredService<IEnvironmentProvider>(),
                serviceProvider.GetRequiredService<ILuthetusCommonComponentRenderers>(),
                serviceProvider.GetRequiredService<IDispatcher>()));

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        services.AddLuthetusCommonServices(hostingInformation, commonOptions =>
        {
            var outLuthetusCommonFactories = commonOptions.CommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(),
                StorageServiceFactory = _ => new DoNothingStorageService()
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
            typeof(LuthetusCommonConfig).Assembly,
            typeof(LuthetusTextEditorConfig).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        TextEditorService = ServiceProvider.GetRequiredService<ITextEditorService>();

        RazorCompilerService = ServiceProvider.GetRequiredService<RazorCompilerService>();
        XmlCompilerService = ServiceProvider.GetRequiredService<XmlCompilerService>();
        CSharpCompilerService = ServiceProvider.GetRequiredService<CSharpCompilerService>();
    }
}
