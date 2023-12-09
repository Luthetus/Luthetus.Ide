using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.UnitTesting;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.Tests;

/// <summary>Setup the dependency injection necessary</summary>
public class TextEditorTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ResourceUri ResourceUri = new("__LUTHETUS_LuthetusTextEditorTestingBase__");
    protected readonly Key<TextEditorViewModel> TextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();
    protected readonly CommonUnitTestHelper CommonHelper;
    protected readonly TextEditorUnitTestHelper TextEditorHelper;

    protected TextEditorModel TextEditorModel => TextEditorHelper.TextEditorService.Model.FindOrDefault(ResourceUri)
        ?? throw new ApplicationException($"{nameof(TextEditorService)}.{nameof(TextEditorHelper.TextEditorService.Model.FindOrDefault)} returned null.");

    protected TextEditorViewModel TextEditorViewModel => TextEditorHelper.TextEditorService.ViewModel.FindOrDefault(TextEditorViewModelKey)
        ?? throw new ApplicationException($"{nameof(TextEditorService)}.{nameof(TextEditorHelper.TextEditorService.ViewModel.FindOrDefault)} returned null.");

    public TextEditorTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        CommonUnitTestHelper.AddLuthetusCommonServicesUnitTesting(services, hostingInformation);
        TextEditorUnitTestHelper.AddLuthetusTextEditorUnitTesting(services, hostingInformation);

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        CommonHelper = new CommonUnitTestHelper(ServiceProvider);
        TextEditorHelper = new TextEditorUnitTestHelper(ServiceProvider);

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        var textEditor = new TextEditorModel(
            ResourceUri,
            DateTime.UtcNow,
            "UnitTests",
            string.Empty,
            null,
            null,
            null);

        TextEditorHelper.TextEditorService.Model.RegisterCustom(textEditor);
        TextEditorHelper.TextEditorService.ViewModel.Register(TextEditorViewModelKey, ResourceUri);
    }
}