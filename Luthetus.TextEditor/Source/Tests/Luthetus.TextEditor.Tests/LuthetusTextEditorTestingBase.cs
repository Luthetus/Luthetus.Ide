using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.Tests;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class LuthetusTextEditorTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ITextEditorService TextEditorService;
    protected readonly ResourceUri ResourceUri = new ResourceUri("__LUTHETUS_LuthetusTextEditorTestingBase__");
    protected readonly Key<TextEditorViewModel> TextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();

    protected TextEditorModel TextEditorModel => TextEditorService.Model.FindOrDefault(ResourceUri)
        ?? throw new ApplicationException(
            $"{nameof(TextEditorService)}.{nameof(TextEditorService.Model.FindOrDefault)} returned null.");

    protected TextEditorViewModel TextEditorViewModel => TextEditorService.ViewModel.FindOrDefault(TextEditorViewModelKey)
        ?? throw new ApplicationException(
            $"{nameof(TextEditorService)}.{nameof(TextEditorService.ViewModel.FindOrDefault)} returned null.");

    public LuthetusTextEditorTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        services.AddLuthetusCommonServices(hostingInformation, commonOptions => 
        {
            var outLuthetusCommonFactories = commonOptions.LuthetusCommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                StorageServiceFactory = _ => new DoNothingStorageService(true)
            };

            return commonOptions with
            {
                LuthetusCommonFactories = outLuthetusCommonFactories
            };
        });

        services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
        {
            AddLuthetusCommon = false,
        });

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        TextEditorService = ServiceProvider.GetRequiredService<ITextEditorService>();

        var textEditor = new TextEditorModel(
            ResourceUri,
            DateTime.UtcNow,
            "UnitTests",
            string.Empty,
            null,
            null,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditor);
        TextEditorService.ViewModel.Register(TextEditorViewModelKey, ResourceUri);
    }
}