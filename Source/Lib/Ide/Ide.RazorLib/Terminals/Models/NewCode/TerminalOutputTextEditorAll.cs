using System.Collections.Immutable;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputTextEditorAll : ITerminalOutput
{
    public static ResourceUri TextEditorModelResourceUri { get; } = new(
        ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + nameof(TerminalOutputTextEditorAll));

    public static Key<TextEditorViewModel> TextEditorViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();

    private readonly ITerminal _terminal;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;

	public TerminalOutputTextEditorAll(
		ITerminal terminal,
		ITextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry)
	{
		_terminal = terminal;
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
		
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		
		CreateTextEditor();
	}
	
	private string _output = string.Empty;
	
	public string OutputRaw
	{
		get => _output;
		private set
		{
			_output = value;
			OnWriteOutput?.Invoke();
		}
	}
	
	public StringBuilder OutputBuilder { get; } = new();
	
	public event Action? OnWriteOutput;

	public void OnWorkingDirectoryChanged()
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
				OutputBuilder.Append($"{terminalCommandParsed.SourceTerminalCommandRequest.CommandText}\n");
				break;
			case StandardOutputCommandEvent stdOut:
				OutputBuilder.Append($"{stdOut.Text}\n");
				break;
			case StandardErrorCommandEvent stdErr:
				OutputBuilder.Append($"{stdErr.Text}\n");
				break;
			case ExitedCommandEvent exited:
				OutputBuilder.Append($"Process exited; Code: {exited.ExitCode}\n");
				break;
		}
		
		OutputRaw = OutputBuilder.ToString();
	}
	
	private void CreateTextEditor()
    {
        _textEditorService.PostUnique(
            nameof(TerminalOutputTextEditorAll),
            editContext =>
            {

                var line1 = "Integrated-Terminal";
                var line2 = "Try: cmd /c \"dir\"";

                var longestLineLength = Math.Max(line1.Length, line2.Length);

                var model = new TextEditorModel(
                    TextEditorModelResourceUri,
                    DateTime.UtcNow,
                    "terminal",
                    $"{line1}\n" +
                        $"{line2}\n" +
                        new string('=', longestLineLength) +
                        "\n\n",
                    new TerminalDecorationMapper(),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

                _textEditorService.ModelApi.RegisterCustom(model);

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

                _textEditorService.ViewModelApi.Register(
                    TextEditorViewModelKey,
                    TextEditorModelResourceUri,
                    new Category("terminal"));

                var layerFirstPresentationKeys = new[]
                {
                    TerminalPresentationFacts.PresentationKey,
                    CompilerServiceDiagnosticPresentationFacts.PresentationKey,
                    FindOverlayPresentationFacts.PresentationKey,
                }.ToImmutableArray();

                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                if (viewModelModifier is null)
                    return Task.CompletedTask;

                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
                    FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
                };

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
                    TextEditorModelResourceUri,
                    modelModifier.GetAllText()));

                editContext.TextEditorService.ModelApi.ApplyDecorationRange(
                    editContext,
                    modelModifier,
                    terminalResource.GetTokenTextSpans());

                return Task.CompletedTask;
            });
    }
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
