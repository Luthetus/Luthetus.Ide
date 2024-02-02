using Xunit;
using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

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

        await TextEditorCommandVimFacts.Motions.Word.CommandFunc.Invoke(textEditorCommandArgs);

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
        var services = new ServiceCollection()
            .AddSingleton<LuthetusCommonConfig>()
            .AddSingleton<LuthetusTextEditorConfig>()
            .AddScoped<IStorageService, DoNothingStorageService>()
            .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
            .AddScoped<StorageSync>()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
            .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
            .AddScoped<ITextEditorService, TextEditorService>()
            .AddScoped<IClipboardService, InMemoryClipboardService>()
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

        model = new TextEditorModel(
            new ResourceUri($"/{nameof(InitializeTextEditorCommandVimFactsMotionsTests)}.txt"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.TXT,
            TestConstants.SOURCE_TEXT,
            null,
            null);

        textEditorService.ModelApi.RegisterCustom(model);

        model = textEditorService.ModelApi.GetOrDefault(model.ResourceUri)
           ?? throw new ArgumentNullException();

        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            viewModelKey,
            model.ResourceUri);

        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
           ?? throw new ArgumentNullException();

        textEditorCommandArgs = new TextEditorCommandArgs(
            model.ResourceUri,
            viewModel.ViewModelKey,
            false,
            serviceProvider.GetRequiredService<IClipboardService>(),
            textEditorService,
            (MouseEventArgs m) => Task.CompletedTask,
            serviceProvider.GetRequiredService<IJSRuntime>(),
            serviceProvider.GetRequiredService<IDispatcher>(),
            (ResourceUri resourceUri) => { },
            (ResourceUri resourceUri) => { },
            (Key<TextEditorViewModel> viewModelKey) => { });
    }
}