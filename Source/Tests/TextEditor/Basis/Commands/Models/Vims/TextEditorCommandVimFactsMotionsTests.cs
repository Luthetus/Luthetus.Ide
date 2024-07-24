using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models.Vims;

/// <summary>
/// <see cref="TextEditorCommandVimFacts.Motions"/>
/// </summary>
public class TextEditorCommandVimFactsMotionsTests
{
	/// <summary>
	/// <see cref="TextEditorCommandVimFacts.Motions.Word"/>
	/// </summary>
	[Fact]
	public async Task Word()
	{
        InitializeTextEditorCommandVimFactsMotionsTests(
            out var textEditorService, out var inModel, out var inViewModel,
            out var textEditorCommandArgs, out var serviceProvider);

        await TextEditorCommandVimFacts.Motions.WordCommand.CommandFunc.Invoke(textEditorCommandArgs);

        throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandVimFacts.Motions.End"/>
	/// </summary>
	[Fact]
    public void End()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandVimFacts.Motions.PerformEnd(TextEditorCommandArgs, bool)"/>
	/// </summary>
	[Fact]
    public void PerformEnd()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandVimFacts.Motions.Back"/>
	/// </summary>
	[Fact]
    public void Back()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandVimFacts.Motions.GetVisual(TextEditorCommand, string)"/>
	/// </summary>
	[Fact]
    public void GetVisual()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandVimFacts.Motions.GetVisualLineFactory(TextEditorCommand, string)"/>
	/// </summary>
	[Fact]
    public void GetVisualLine()
    {
		throw new NotImplementedException();
	}

    private static void InitializeTextEditorCommandVimFactsMotionsTests(
        out ITextEditorService textEditorService,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out TextEditorCommandArgs textEditorCommandArgs,
        out IServiceProvider serviceProvider)
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //var services = new ServiceCollection()
        //    .AddSingleton<LuthetusCommonConfig>()
        //    .AddSingleton<LuthetusTextEditorConfig>()
        //    .AddScoped<IStorageService, DoNothingStorageService>()
        //    .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
        //    .AddScoped<StorageSync>()
        //    .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
        //    .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>()
        //    .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>()
        //    .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
        //    .AddScoped<ITextEditorService, TextEditorService>()
        //    .AddScoped<IClipboardService, InMemoryClipboardService>()
        //    .AddFluxor(options => options.ScanAssemblies(
        //        typeof(LuthetusCommonConfig).Assembly,
        //        typeof(LuthetusTextEditorConfig).Assembly));

        //serviceProvider = services.BuildServiceProvider();

        //var store = serviceProvider.GetRequiredService<IStore>();
        //store.InitializeAsync().Wait();

        //var backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();

        //var continuousQueue = new BackgroundTaskQueue(
        //    ContinuousBackgroundTaskWorker.GetQueueKey(),
        //    ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        //backgroundTaskService.RegisterQueue(continuousQueue);

        //var blockingQueue = new BackgroundTaskQueue(
        //    BlockingBackgroundTaskWorker.GetQueueKey(),
        //    BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        //backgroundTaskService.RegisterQueue(blockingQueue);

        //var textEditorRegistryWrap = serviceProvider.GetRequiredService<ITextEditorRegistryWrap>();

        //textEditorRegistryWrap.DecorationMapperRegistry = serviceProvider
        //    .GetRequiredService<IDecorationMapperRegistry>();

        //textEditorRegistryWrap.CompilerServiceRegistry = serviceProvider
        //    .GetRequiredService<ICompilerServiceRegistry>();

        //textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();

        //model = new TextEditorModel(
        //    new ResourceUri($"/{nameof(InitializeTextEditorCommandVimFactsMotionsTests)}.txt"),
        //    DateTime.UtcNow,
        //    ExtensionNoPeriodFacts.TXT,
        //    TestConstants.SOURCE_TEXT,
        //    null,
        //    null);

        //textEditorService.ModelApi.RegisterCustom(model);

        //model = textEditorService.ModelApi.GetOrDefault(model.ResourceUri)
        //   ?? throw new ArgumentNullException();

        //var viewModelKey = Key<TextEditorViewModel>.NewKey();

        //textEditorService.ViewModelApi.Register(
        //    viewModelKey,
        //    model.ResourceUri,
        //    new TextEditorCategory("UnitTesting"));

        //viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
        //   ?? throw new ArgumentNullException();

        //textEditorCommandArgs = new TextEditorCommandArgs(
        //    model.ResourceUri,
        //    viewModel.ViewModelKey,
        //    false,
        //    serviceProvider.GetRequiredService<IClipboardService>(),
        //    textEditorService,
        //    (MouseEventArgs m) => Task.CompletedTask,
        //    serviceProvider.GetRequiredService<IJSRuntime>(),
        //    serviceProvider.GetRequiredService<IDispatcher>(),
        //    serviceProvider.GetRequiredService<IServiceProvider>(),
        //    serviceProvider.GetRequiredService<LuthetusTextEditorConfig>());
    }
}