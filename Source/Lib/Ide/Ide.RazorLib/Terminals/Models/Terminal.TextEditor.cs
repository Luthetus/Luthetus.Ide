using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Ide.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public partial class Terminal
{
	private readonly ITextEditorService _textEditorService;
	private readonly Dictionary<Key<TerminalCommand>, TextEditorTextSpan> _terminalCommandTextSpanMap = new();
	private readonly Dictionary<Key<TerminalCommand>, Key<TextEditorViewModel>> _terminalCommandViewModelKeyMap = new();
	private readonly object _terminalCommandMapLock = new();

	public ResourceUri ResourceUri { get; init; }
	public Key<TextEditorViewModel> TextEditorViewModelKey { get; init; } = Key<TextEditorViewModel>.NewKey();

	/// <summary>
	/// When using the <see cref="Terminal"/>, one can run many <see cref="TerminalCommand"/>.
	/// Therefore, a way to filter the <see cref="Terminal"/>, such that only the output of a single
	/// <see cref="TerminalCommand"/> is returned, is needed.
	/// </summary>
	public bool TryGetTerminalCommandTextSpan(Key<TerminalCommand> terminalCommandKey, out TextEditorTextSpan? textSpan)
    {
		lock (_terminalCommandMapLock)
		{
        	return _terminalCommandTextSpanMap.TryGetValue(terminalCommandKey, out textSpan);
		}
    }

	public Key<TextEditorViewModel> GetTerminalCommandViewModelKey(
        Key<TerminalCommand> terminalCommandKey)
	{
		Console.WriteLine(nameof(GetTerminalCommandViewModelKey));
		var needsToInitializeTheTextEditor = false;
		var textEditorViewModelKey = Key<TextEditorViewModel>.Empty;

		lock (_terminalCommandMapLock)
		{
			var success = _terminalCommandViewModelKeyMap.TryGetValue(
				terminalCommandKey,
				out textEditorViewModelKey);

			if (!success)
			{
				needsToInitializeTheTextEditor = true;
				textEditorViewModelKey = Key<TextEditorViewModel>.NewKey();
				_terminalCommandViewModelKeyMap.Add(terminalCommandKey, textEditorViewModelKey);
			}
		}

		if (needsToInitializeTheTextEditor)
		{
			_ = Task.Run(() => CreateTextEditorForCommandOutput(
				terminalCommandKey,
				textEditorViewModelKey));
		}

		return textEditorViewModelKey;
	}

	public async Task CreateTextEditorForCommandOutput(
		Key<TerminalCommand> terminalCommandKey,
		Key<TextEditorViewModel> commandOutputViewModelKey)
	{
		Console.WriteLine(nameof(CreateTextEditorForCommandOutput));

		var success = _terminalCommandTextSpanMap.TryGetValue(
			terminalCommandKey,
			out var textSpan);

		if (!success)
			textSpan = TextEditorTextSpan.FabricateTextSpan(string.Empty);

        var commandOutputResourceUri = new ResourceUri("terminalCommand" + '_' + terminalCommandKey);

		var model = new TextEditorModel(
			commandOutputResourceUri,
            DateTime.UtcNow,
            "terminal",
            textSpan.GetText(),
            null, //new TerminalDecorationMapper(),
            null); //_compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

		_textEditorService.ModelApi.RegisterCustom(model);

        await _textEditorService.PostSimpleBatch(
			nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
            string.Empty,
            async editContext =>
			{
				await _textEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						TerminalPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext)
                    .ConfigureAwait(false);

				await _textEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext)
                    .ConfigureAwait(false);

				await _textEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						FindOverlayPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext)
                    .ConfigureAwait(false);

				model.CompilerService.RegisterResource(model.ResourceUri);

				_textEditorService.ViewModelApi.Register(
					commandOutputViewModelKey,
					commandOutputResourceUri,
					new Category("terminal"));

                var viewModelModifier = editContext.GetViewModelModifier(commandOutputViewModelKey);

                if (viewModelModifier is null)
                    throw new NullReferenceException();

				var layerFirstPresentationKeys = new[]
				{
			        TerminalPresentationFacts.PresentationKey,
			        CompilerServiceDiagnosticPresentationFacts.PresentationKey,
			        FindOverlayPresentationFacts.PresentationKey,
		        }.ToImmutableArray();

                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
					FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
				};
			});
	}

    private async Task CreateTextEditor()
    {
        var line1 = "Integrated-Terminal";
        var line2 = "Try: cmd /c \"dir\"";

        var longestLineLength = Math.Max(line1.Length, line2.Length);

        var model = new TextEditorModel(
            ResourceUri,
            DateTime.UtcNow,
            "terminal",
            $"{line1}\n" +
                $"{line2}\n" +
                new string('=', longestLineLength) +
                "\n\n",
            new TerminalDecorationMapper(),
            _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

        _textEditorService.ModelApi.RegisterCustom(model);

        await _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
            string.Empty,
            async editContext =>
            {
                await _textEditorService.ModelApi.AddPresentationModelFactory(
                        model.ResourceUri,
                        TerminalPresentationFacts.EmptyPresentationModel)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                await _textEditorService.ModelApi.AddPresentationModelFactory(
                        model.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                await _textEditorService.ModelApi.AddPresentationModelFactory(
                        model.ResourceUri,
                        FindOverlayPresentationFacts.EmptyPresentationModel)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                model.CompilerService.RegisterResource(model.ResourceUri);
            });

        _textEditorService.ViewModelApi.Register(
            TextEditorViewModelKey,
            ResourceUri,
            new Category("terminal"));

        var layerFirstPresentationKeys = new[]
        {
            TerminalPresentationFacts.PresentationKey,
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        }.ToImmutableArray();

        await _textEditorService.PostSimpleBatch(
            nameof(Terminal),
            string.Empty,
            _textEditorService.ViewModelApi.WithValueFactory(
                TextEditorViewModelKey,
                textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
                    }));

        await _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                await _textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Code = KeyboardKeyFacts.MovementKeys.END,
                            Key = KeyboardKeyFacts.MovementKeys.END,
                            CtrlKey = true,
                        },
                        ResourceUri,
                        TextEditorViewModelKey)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    0,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Comment,
                    ResourceUri,
                    modelModifier.GetAllText()));

                await editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                        modelModifier.ResourceUri,
                        terminalResource.GetTokenTextSpans())
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            });
    }

    public async Task WriteWorkingDirectory(bool prependNewLine = false)
    {
        await _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var startingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

				var content = (WorkingDirectoryAbsolutePathString ?? "null") + '>';
				if (prependNewLine)
					content = '\n' + content;

                await _textEditorService.ModelApi.InsertTextFactory(
                        ResourceUri,
                        TextEditorViewModelKey,
                        content,
                        CancellationToken.None)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    startingPositionIndex,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Keyword,
                    ResourceUri,
                    modelModifier.GetAllText()));

                await editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                        modelModifier.ResourceUri,
                        terminalResource.GetTokenTextSpans())
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            });
    }
    
    public async Task MoveCursorToEnd()
    {
        await _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                await _textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Code = KeyboardKeyFacts.MovementKeys.END,
                            Key = KeyboardKeyFacts.MovementKeys.END,
                            CtrlKey = true,
                        },
                        ResourceUri,
                        TextEditorViewModelKey)
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            });
    }

    public async Task ClearTerminal()
    {
        await _textEditorService.PostSimpleBatch(
            nameof(ClearTerminal),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                await TextEditorCommandDefaultFunctions.SelectAllFactory(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        null)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                await _textEditorService.ModelApi.DeleteTextByMotionFactory(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        MotionKind.Delete,
                        CancellationToken.None)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return;

                terminalResource.ManualDecorationTextSpanList.Clear();
                terminalResource.SyntaxTokenList.Clear();
            });
    }

    /// <summary>
    /// This method refers to a substring of the terminal output.
    /// For example, the unit test explorer, will show terminal output,
    /// which is the result of taking a substring from the entire terminal output.
    /// </summary>
    private async Task ClearOutputView(TerminalCommand terminalCommand)
    {
        await _textEditorService.PostSimpleBatch(
            "clear-content_" + terminalCommand.TerminalCommandKey.Guid,
            string.Empty,
            editContext =>
            {
                var commandOutputResourceUri = new ResourceUri("terminalCommand" + '_' + terminalCommand.TerminalCommandKey);
                var modelModifier = editContext.GetModelModifier(commandOutputResourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                var textSpan = TextEditorTextSpan.FabricateTextSpan($"> {terminalCommand.FormattedCommand.Value}\n");

                _terminalCommandTextSpanMap[terminalCommand.TerminalCommandKey] = textSpan;

                modelModifier.SetContent(textSpan.GetText());
                return Task.CompletedTask;
            });
    }

    private Task AddTerminalCommandTextSpanMap(
        Key<TerminalCommand> terminalCommandKey,
        TerminalCommandBoundary terminalCommandBoundary)
    {
		return _textEditorService.PostSimpleBatch(
	        "_terminalCommandTextSpanMap.Add(...)",
	        "_terminalCommandTextSpanMap.Add(...)",
	        editContext =>
	        {
		        _terminalCommandTextSpanMap.Add(
			        terminalCommandKey,
			        new TextEditorTextSpan(
				        terminalCommandBoundary.StartPositionIndexInclusive ?? 0,
				        terminalCommandBoundary.EndPositionIndexExclusive ?? 0,
				        0,
				        ResourceUri,
				        _textEditorService.ModelApi.GetAllText(ResourceUri) ?? string.Empty));

		        return Task.CompletedTask;
	        });
	}

    private Task SetTerminalCommandContent(
        Key<TerminalCommand> terminalCommandKey,
        TerminalCommandBoundary terminalCommandBoundary)
    {
		return _textEditorService.PostSimpleBatch(
	        "set-content_" + terminalCommandKey.Guid,
	        string.Empty,
	        editContext =>
	        {
		        var commandOutputResourceUri = new ResourceUri("terminalCommand" + '_' + terminalCommandKey);
		        var modelModifier = editContext.GetModelModifier(commandOutputResourceUri);

		        if (modelModifier is null)
			        return Task.CompletedTask;

		        var textSpan = new TextEditorTextSpan(
			        terminalCommandBoundary.StartPositionIndexInclusive ?? 0,
			        terminalCommandBoundary.EndPositionIndexExclusive ?? 0,
			        0,
			        ResourceUri,
			        _textEditorService.ModelApi.GetAllText(ResourceUri) ?? string.Empty);

		        _terminalCommandTextSpanMap[terminalCommandKey] = textSpan;

		        modelModifier.SetContent(textSpan.GetText());
		        return Task.CompletedTask;
	        });
	}
    
    private Task TerminalOnOutput(
		int outputOffset,
		string output,
		List<TextEditorTextSpan> outputTextSpanList,
		TerminalCommandBoundary terminalCommandBoundary)
    {
		return _textEditorService.Post(new OnOutput(
		    outputOffset,
		    output,
		    outputTextSpanList,
		    ResourceUri,
		    _textEditorService,
		    terminalCommandBoundary,
		    TextEditorViewModelKey));
	}
}