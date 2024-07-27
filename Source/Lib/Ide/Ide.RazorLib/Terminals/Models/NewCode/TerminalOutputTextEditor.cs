using System.Collections.Immutable;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Web;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Events.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputTextEditor : ITerminalOutput
{
	private readonly ITerminal _terminal;

	public TerminalOutputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
		ResourceUri = new(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + _terminal.Key.Guid.ToString());
		CreateTextEditor();
		
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
	}
	
	private readonly ITextEditorService _textEditorService;
	private readonly Dictionary<Key<TerminalCommand>, TextEditorTextSpan> _terminalCommandTextSpanMap = new();
	private readonly Dictionary<Key<TerminalCommand>, Key<TextEditorViewModel>> _terminalCommandViewModelKeyMap = new();
	private readonly object _terminalCommandMapLock = new();

	public ResourceUri ResourceUri { get; init; }
	public Key<TextEditorViewModel> TextEditorViewModelKey { get; init; } = Key<TextEditorViewModel>.NewKey();

	public string OutputRaw { get; private set; }

	public event Action? OnWriteOutput;

	public void OnWorkingDirectoryChanged()
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		OnWriteOutput?.Invoke();
		
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
				// TODO: If the source of the terminal command is a user having...
				//       ...typed themselves, then hitting enter, do not write this out.
				//       |
				//       This is here for when the command was started programmatically
				//       without a user typing into the terminal.
				///////output = $"{terminalCommand.FormattedCommand.Value}\n";
				break;
			case StandardOutputCommandEvent stdOut:
				output = $"{stdOut.Text}\n";
				break;
			case StandardErrorCommandEvent stdErr:
				output = $"{stdErr.Text}\n";
				break;
			case ExitedCommandEvent exited:
				output = $"Process exited; Code: {exited.ExitCode}\n";
				break;
		}

		/*
		if (output is not null)
		{
			var outputTextSpanList = new List<TextEditorTextSpan>();

			if (terminalCommand.OutputParser is not null)
			{
				outputTextSpanList = terminalCommand.OutputParser.OnAfterOutputLine(
					terminalCommand,
					output);
			}
			
			if (terminalCommand.OutputBuilder is null)
			{
				TerminalOnOutput(
					outputOffset,
					output,
					outputTextSpanList,
					terminalCommand,
					terminalCommandBoundary);

				outputOffset += output.Length;
			}
			else
			{
				terminalCommand.OutputBuilder.Append(output);
				terminalCommand.TextSpanList = outputTextSpanList;
			}
		}
		*/
	}

    private void CreateTextEditor()
    {
    	/*
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

        _textEditorService.PostUnique(
            nameof(_textEditorService.ModelApi.AddPresentationModel),
            editContext =>
            {
	            var modelModifier = editContext.GetModelModifier(model.ResourceUri);
				
				if (modelModifier is null)
					return Task.CompletedTask;
            
                _textEditorService.ModelApi.AddPresentationModel(
                    editContext,
                    modelModifier,
                    TerminalPresentationFacts.EmptyPresentationModel);

                _textEditorService.ModelApi.AddPresentationModel(
                    editContext,
                    modelModifier,
                    CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

                _textEditorService.ModelApi.AddPresentationModel(
                    editContext,
                    modelModifier,
                    FindOverlayPresentationFacts.EmptyPresentationModel);

                model.CompilerService.RegisterResource(model.ResourceUri);
                return Task.CompletedTask;
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

        _textEditorService.PostUnique(
            nameof(Terminal),
            editContext =>
            {
            	var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
            	if (viewModelModifier is null)
            		return Task.CompletedTask;

                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
                    FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
                };
                return Task.CompletedTask;
            });

        _textEditorService.PostUnique(
            nameof(_textEditorService.ViewModelApi.MoveCursor),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                _textEditorService.ViewModelApi.MoveCursor(
                	new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MovementKeys.END,
                        Key = KeyboardKeyFacts.MovementKeys.END,
                        CtrlKey = true,
                    },
			        editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return Task.CompletedTask;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    0,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Comment,
                    ResourceUri,
                    modelModifier.GetAllText()));

                editContext.TextEditorService.ModelApi.ApplyDecorationRange(
                	editContext,
                    modelModifier,
                    terminalResource.GetTokenTextSpans());
                return Task.CompletedTask;
            });
    }

    public void WriteWorkingDirectory(bool prependNewLine = false)
    {
        _textEditorService.PostUnique(
            nameof(_textEditorService.ViewModelApi.MoveCursor),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                var startingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

				var content = (WorkingDirectoryAbsolutePathString ?? "null") + '>';
				if (prependNewLine)
					content = '\n' + content;

                _textEditorService.ModelApi.InsertText(
                	editContext,
                    modelModifier,
                    cursorModifierBag,
                    content,
                    CancellationToken.None);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return Task.CompletedTask;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    startingPositionIndex,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Keyword,
                    ResourceUri,
                    modelModifier.GetAllText()));

                editContext.TextEditorService.ModelApi.ApplyDecorationRange(
                	editContext,
                    modelModifier,
                    terminalResource.GetTokenTextSpans());
                return Task.CompletedTask;
            });
		*/
    }
    
    /*
    public void MoveCursorToEnd()
    {
        _textEditorService.PostUnique(
            nameof(_textEditorService.ViewModelApi.MoveCursor),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                _textEditorService.ViewModelApi.MoveCursor(
                	new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MovementKeys.END,
                        Key = KeyboardKeyFacts.MovementKeys.END,
                        CtrlKey = true,
                    },
			        editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag);
                return Task.CompletedTask;
            });
    }
    */

	/*
    public void ClearTerminal()
    {
        _textEditorService.PostUnique(
            nameof(ClearTerminal),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is TerminalResource firstTerminalResource)
                {
	                firstTerminalResource.ManualDecorationTextSpanList.Clear();
	                firstTerminalResource.SyntaxTokenList.Clear();
                }

				modelModifier.SetContent(string.Empty);
				
				_textEditorService.ViewModelApi.MoveCursor(
					new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MovementKeys.END,
                        Key = KeyboardKeyFacts.MovementKeys.END,
                        CtrlKey = true,
                    },
			        editContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag);
	                
	            var startingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

				var content = (WorkingDirectoryAbsolutePathString ?? "null") + '>';

                _textEditorService.ModelApi.InsertText(
                	editContext,
                    modelModifier,
                    cursorModifierBag,
                    content,
                    CancellationToken.None);

                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is TerminalResource secondTerminalResource)
                {
	                secondTerminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
	                    startingPositionIndex,
	                    modelModifier.GetPositionIndex(primaryCursorModifier),
	                    (byte)TerminalDecorationKind.Keyword,
	                    ResourceUri,
	                    modelModifier.GetAllText()));
	
	                editContext.TextEditorService.ModelApi.ApplyDecorationRange(
	                	editContext,
                        modelModifier,
                        secondTerminalResource.GetTokenTextSpans());
                }

                return Task.CompletedTask;
            });
    }
    */
    
    /*
    private void TerminalOnOutput(
		int outputOffset,
		string output,
		List<TextEditorTextSpan> outputTextSpanList,
        TerminalCommand terminalCommand,
		TerminalCommandBoundary terminalCommandBoundary)
    {
		_textEditorService.Post(new OnOutput(
		    outputOffset,
		    output,
		    outputTextSpanList,
		    ResourceUri,
		    _textEditorService,
			terminalCommand,
			terminalCommandBoundary,
		    TextEditorViewModelKey));
	}
	*/
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}