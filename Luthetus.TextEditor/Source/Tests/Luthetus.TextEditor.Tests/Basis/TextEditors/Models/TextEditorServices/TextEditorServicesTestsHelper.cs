using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

public class TextEditorServicesTestsHelper
{
    /// <summary>
    /// This sample text is used for testing.
    /// <br/><br/>
    /// It contains separate lines that start with letter, digit, whitespace, and punctuation
    /// Similarly, separate lines that end with punctuation, letter, whitespace, digit.
    /// <br/><br/>
    /// Note: there is a line which contains only a space character, this note is here to try to
    /// avoid confusion if one does not see it.
    /// <br/><br/>
    /// Do not use a verbatim string here, it will use operating system dependent line endings,
    /// which then cannot be asserted in the unit tests.
    /// </summary>
    private const string _sourceText = "Hello World!\n7 Pillows\n \n,abc123";

    public static void InitializeTextEditorServicesTestsHelper(
        out ITextEditorService textEditorService,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out IServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddSingleton<LuthetusCommonOptions>()
            .AddSingleton<LuthetusTextEditorOptions>()
            .AddScoped<IStorageService, DoNothingStorageService>()
            .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
            .AddScoped<StorageSync>()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
            .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
            .AddScoped<ITextEditorService, TextEditorService>()
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonOptions).Assembly,
                typeof(LuthetusTextEditorOptions).Assembly));

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
            resourceUri);

        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
           ?? throw new ArgumentNullException();
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
            _sourceText,
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
            _sourceText,
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
            _sourceText,
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
            _sourceText,
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
            _sourceText,
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
            _sourceText,
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
            _sourceText,
            null,
            null);

        cursor = new TextEditorCursor(7, 2, true);
    }
}
