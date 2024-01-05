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
            out var textEditorService, out var inModel, out var inViewModel,
            out var textEditorCommandArgs, out var serviceProvider);

		await TextEditorCommandDefaultFacts.DoNothingDiscard.CommandFunc.Invoke(
            textEditorCommandArgs);
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Copy"/>
	/// </summary>
	[Fact]
    public async Task Copy()
    {
        InitializeTextEditorCommandDefaultFactsTests(
            out var textEditorService, out var inModel, out var inViewModel,
            out var textEditorCommandArgs, out var serviceProvider);

        var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();

        // No selection
        {
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Hello World!\n", outClipboard);
        }


        // With selection
        {
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

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

            await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(textEditorCommandArgs);

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
        // No selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Hello World!\n", outClipboard);

            // Assert text was deleted
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var outText = outModel!.GetAllText();
                Assert.Equal("7 Pillows\n \n,abc123", outText);
            }
        }

        // With selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

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

            await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(textEditorCommandArgs);

            textEditorService.Post(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var outClipboard = await clipboardService.ReadClipboard();
                    Assert.Equal("Pillows", outClipboard);

                    // Assert text was deleted
                    {
                        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                        Assert.NotNull(outModel);

                        var outText = outModel!.GetAllText();
                        Assert.Equal("Hello World!\n7 \n \n,abc123", outText);
                    }
                });
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.PasteCommand"/>
	/// </summary>
	[Fact]
    public async Task Paste()
    {
        // No selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();

            textEditorService.Post(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);

                    if (modelModifier is null || viewModelModifier is null)
                        return;

                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (primaryCursorModifier is null)
                        return;

                    var stringToPaste = "Alphabet Soup\n";
                    await clipboardService.SetClipboard(stringToPaste);
                    Assert.Equal(stringToPaste, await clipboardService.ReadClipboard());

                    primaryCursorModifier.RowIndex = 0;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);

                    return;
                });

            await TextEditorCommandDefaultFacts.PasteCommand.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert text was pasted
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var outText = outModel!.GetAllText();
                Assert.Equal("Alphabet Soup\nHello World!\n7 Pillows\n \n,abc123", outText);
            }
        }

        // With selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();

            textEditorService.Post(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);

                    if (modelModifier is null || viewModelModifier is null)
                        return;

                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (primaryCursorModifier is null)
                        return;

                    var stringToPaste = "Alphabet Soup\n";
                    await clipboardService.SetClipboard(stringToPaste);
                    Assert.Equal(stringToPaste, await clipboardService.ReadClipboard());

                    primaryCursorModifier.RowIndex = 0;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);

                    // Select the first row in its entirety (including line ending)
                    {
                        primaryCursorModifier.SelectionAnchorPositionIndex = 0;
                        primaryCursorModifier.SelectionEndingPositionIndex = 13;
                    }

                    return;
                });

            await TextEditorCommandDefaultFacts.PasteCommand.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert text was pasted
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var outText = outModel!.GetAllText();
                Assert.Equal("Alphabet Soup\n7 Pillows\n \n,abc123", outText);
            }
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Save"/>
	/// </summary>
	[Fact]
    public async Task Save()
    {
        // ViewModel.OnSaveRequested is NOT null
        {
            try
            {
                InitializeTextEditorCommandDefaultFactsTests(
                    out var textEditorService, out var inModel, out var inViewModel,
                    out var textEditorCommandArgs, out var serviceProvider);

                var savedContent = (string?)null;
                Assert.Null(savedContent);

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

                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            OnSaveRequested = model => savedContent = model.GetAllText()
                        };

                        return Task.CompletedTask;
                    });

                await TextEditorCommandDefaultFacts.Save.CommandFunc.Invoke(textEditorCommandArgs);

                textEditorService.Post(
                    nameof(TextEditorCommandDefaultFactsTests),
                    editContext =>
                    {
                        Assert.Equal(TestConstants.SOURCE_TEXT, savedContent);
                        return Task.CompletedTask;
                    });
            }
            catch (AggregateException aggregateException)
            {
                throw new Exception("UPPER_" + string.Join('\n', aggregateException.InnerExceptions.Select(x => x.Message)));

                // throw new Exception(string.Join('\n', aggregateException.InnerExceptions.Select(x => x.StackTrace)));
            }
        }

        // ViewModel.OnSaveRequested is null
        {
            try
            {
                InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

                var savedContent = (string?)null;

                Assert.Null(savedContent);
                await TextEditorCommandDefaultFacts.Save.CommandFunc.Invoke(textEditorCommandArgs);

                textEditorService.Post(
                    nameof(TextEditorCommandDefaultFactsTests),
                    editContext =>
                    {
                        Assert.Null(savedContent);
                        return Task.CompletedTask;
                    });
            }
            catch (AggregateException aggregateException)
            {
                throw new Exception("LOWER_" + string.Join('\n', aggregateException.InnerExceptions.Select(x => x.Message)));

                // throw new Exception(string.Join('\n', aggregateException.InnerExceptions.Select(x => x.StackTrace)));
            }
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.SelectAll"/>
	/// </summary>
	[Fact]
    public async Task SelectAll()
    {
        InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

        // No already existing selection
        {
            await TextEditorCommandDefaultFacts.SelectAll.CommandFunc.Invoke(textEditorCommandArgs);
            throw new NotImplementedException();
        }

        // With an already existing selection
        {
            await TextEditorCommandDefaultFacts.SelectAll.CommandFunc.Invoke(textEditorCommandArgs);
            throw new NotImplementedException();
        }
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
        out TextEditorCommandArgs textEditorCommandArgs,
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