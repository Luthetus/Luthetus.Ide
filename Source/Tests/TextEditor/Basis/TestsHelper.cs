using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models;
using Luthetus.TextEditor.Tests.JsRuntimes;

namespace Luthetus.TextEditor.Tests.Basis;

public class TestsHelper
{
    public static void InitializeTextEditorServicesTestsHelper(
        out ITextEditorService textEditorService,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out IServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddSingleton<LuthetusCommonConfig>()
            .AddSingleton<LuthetusTextEditorConfig>()
            .AddScoped<IStorageService, DoNothingStorageService>()
            .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
            .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
            .AddScoped<ITextEditorService, TextEditorService>()
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();

        var continuousQueue = new BackgroundTaskQueue(
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(continuousQueue);

        var blockingQueue = new BackgroundTaskQueue(
            BlockingBackgroundTaskWorker.GetQueueKey(),
            BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(blockingQueue);

        var textEditorRegistryWrap = serviceProvider.GetRequiredService<ITextEditorRegistryWrap>();

        textEditorRegistryWrap.DecorationMapperRegistry = serviceProvider
            .GetRequiredService<IDecorationMapperRegistry>();

        textEditorRegistryWrap.CompilerServiceRegistry = serviceProvider
            .GetRequiredService<ICompilerServiceRegistry>();

        textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        model = textEditorService.ModelApi.GetOrDefault(resourceUri)
           ?? throw new ArgumentNullException();

        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            viewModelKey,
            resourceUri,
            new Category("UnitTesting"));

        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
           ?? throw new ArgumentNullException();
    }

    public static void ConstructTestTextEditorModel(out TextEditorModel model)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(ConstructTestTextEditorModel)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);
    }

    /// <summary>
    /// Start of a row &amp;&amp; !start_of_document
    /// </summary>
    public static void InBounds_StartOfRow(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(InBounds_StartOfRow)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(1, 0, true);
    }

    public static void InBounds_NOT_StartOfRow_AND_NOT_EndOfRow(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(InBounds_NOT_StartOfRow_AND_NOT_EndOfRow)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(1, 3, true);
    }

    public static void InBounds_EndOfRow(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(InBounds_EndOfRow)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(1, 9, true);
    }

    public static void InBounds_StartOfDocument(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(InBounds_StartOfDocument)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(0, 0, true);
    }

    public static void InBounds_EndOfDocument(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(InBounds_EndOfDocument)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(3, 7, true);
    }

    public static void OutOfBounds_PositionIndex_LESS_THAN_Zero(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(OutOfBounds_PositionIndex_LESS_THAN_Zero)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(-3, -5, true);
    }

    public static void OutOfBounds_PositionIndex_GREATER_THAN_DocumentLength_PLUS_One(
        out TextEditorModel model,
        out TextEditorCursor cursor)
    {
        model = new TextEditorModel(
            new ResourceUri($"/{nameof(OutOfBounds_PositionIndex_GREATER_THAN_DocumentLength_PLUS_One)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        cursor = new TextEditorCursor(7, 2, true);
    }
}
