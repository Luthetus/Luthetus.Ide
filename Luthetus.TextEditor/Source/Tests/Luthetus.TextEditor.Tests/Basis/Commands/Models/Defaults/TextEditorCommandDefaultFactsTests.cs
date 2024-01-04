using Xunit;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
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
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models.Defaults;

/// <summary>
/// <see cref="TextEditorCommandDefaultFacts"/>
/// </summary>
public class TextEditorCommandDefaultFactsTests
{
	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.DoNothingDiscard"/>
	/// </summary>
	[Fact]
    public async Task DoNothingDiscard()
    {
        InitializeTextEditorCommandDefaultFactsTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var doNothingDiscardCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;

        var modelResourceUri = inModel.ResourceUri;
		var viewModelKey = inViewModel.ViewModelKey;
		var hasTextSelection = false;
		var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
		//var textEditorService = textEditorService;
		var handleMouseStoppedMovingEventAsyncFunc = (MouseEventArgs m) => Task.CompletedTask;
        var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var registerModelAction = (ResourceUri resourceUri) => { };
		var registerViewModelAction = (ResourceUri resourceUri) => { };
        var showViewModelAction = (Key<TextEditorViewModel> viewModel) => { };

        var textEditorCommandArgs = new TextEditorCommandArgs(
            modelResourceUri,
			viewModelKey,
			hasTextSelection,
			clipboardService,
			textEditorService,
			handleMouseStoppedMovingEventAsyncFunc,
			jsRuntime,
			dispatcher,
			registerModelAction,
			registerViewModelAction,
			showViewModelAction);

		await doNothingDiscardCommand.CommandFunc.Invoke(textEditorCommandArgs);
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Copy"/>
	/// </summary>
	[Fact]
    public async Task Copy()
    {
        InitializeTextEditorCommandDefaultFactsTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var copyCommand = TextEditorCommandDefaultFacts.Copy;

        var modelResourceUri = inModel.ResourceUri;
        var viewModelKey = inViewModel.ViewModelKey;
        var hasTextSelection = false;
        var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
        //var textEditorService = textEditorService;
        var handleMouseStoppedMovingEventAsyncFunc = (MouseEventArgs m) => Task.CompletedTask;
        var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var registerModelAction = (ResourceUri resourceUri) => { };
        var registerViewModelAction = (ResourceUri resourceUri) => { };
        var showViewModelAction = (Key<TextEditorViewModel> viewModel) => { };

        var textEditorCommandArgs = new TextEditorCommandArgs(
            modelResourceUri,
            viewModelKey,
            hasTextSelection,
            clipboardService,
            textEditorService,
            handleMouseStoppedMovingEventAsyncFunc,
            jsRuntime,
            dispatcher,
            registerModelAction,
            registerViewModelAction,
            showViewModelAction);

		await clipboardService.SetClipboard(string.Empty);

        // No selection
        {
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            await copyCommand.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Hello World!\n", outClipboard);
        }

        await clipboardService.SetClipboard(string.Empty);

        // With selection
        {
			textEditorService.Post(
				nameof(TextEditorCommandDefaultFactsTests),
				editContext =>
				{
					var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);

					if (modelModifier is null || viewModelModifier is null)
						return Task.CompletedTask;

					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (primaryCursorModifier is null)
                        return Task.CompletedTask;

					primaryCursorModifier.RowIndex = 1;
					primaryCursorModifier.SetColumnIndexAndPreferred(9);
					primaryCursorModifier.SelectionAnchorPositionIndex = 15;
					primaryCursorModifier.SelectionEndingPositionIndex = 22;

                    return Task.CompletedTask;
                });

            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            await copyCommand.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Pillows", outClipboard);
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Cut"/>
	/// </summary>
	[Fact]
	public async Task Cut()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.PasteCommand"/>
	/// </summary>
	[Fact]
    public async Task Paste()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Save"/>
	/// </summary>
	[Fact]
    public async Task Save()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.SelectAll"/>
	/// </summary>
	[Fact]
    public async Task SelectAll()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Undo"/>
	/// </summary>
	[Fact]
    public async Task Undo()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Redo"/>
	/// </summary>
	[Fact]
    public async Task Redo()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Remeasure"/>
	/// </summary>
	[Fact]
    public async Task Remeasure()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ScrollLineDown"/>
	/// </summary>
	[Fact]
    public async Task ScrollLineDown()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ScrollLineUp"/>
	/// </summary>
	[Fact]
    public async Task ScrollLineUp()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ScrollPageDown"/>
	/// </summary>
	[Fact]
    public async Task ScrollPageDown()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ScrollPageUp"/>
	/// </summary>
	[Fact]
    public async Task ScrollPageUp()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.CursorMovePageBottom"/>
	/// </summary>
	[Fact]
    public async Task CursorMovePageBottom()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.CursorMovePageTop"/>
	/// </summary>
	[Fact]
    public async Task CursorMovePageTop()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Duplicate"/>
	/// </summary>
	[Fact]
    public async Task Duplicate()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.IndentMore"/>
	/// </summary>
	[Fact]
    public async Task IndentMore()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.IndentLess"/>
	/// </summary>
	[Fact]
    public async Task IndentLess()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ClearTextSelection"/>
	/// </summary>
	[Fact]
    public async Task ClearTextSelection()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.NewLineBelow"/>
	/// </summary>
	[Fact]
    public async Task NewLineBelow()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.NewLineAbove"/>
	/// </summary>
	[Fact]
    public async Task NewLineAbove()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(bool)"/>
	/// </summary>
	[Fact]
	public async Task GoToMatchingCharacterFactory()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.GoToDefinition"/>
	/// </summary>
	[Fact]
    public async Task GoToDefinition()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ShowFindDialog"/>
	/// </summary>
	[Fact]
    public async Task ShowFindDialog()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ShowTooltipByCursorPosition"/>
	/// </summary>
	[Fact]
    public async Task ShowTooltipByCursorPosition()
    {
		throw new NotImplementedException();
	}

    private static void InitializeTextEditorCommandDefaultFactsTests(
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
            .AddScoped<IClipboardService, InMemoryClipboardService>()
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

		model = new TextEditorModel(
            new ResourceUri($"/{nameof(InitializeTextEditorCommandDefaultFactsTests)}.txt"),
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
    }
}