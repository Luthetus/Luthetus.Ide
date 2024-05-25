using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class TextEditorWorkOutput : ITextEditorWork
{
	public TextEditorWorkOutput(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		Key<TextEditorViewModel> viewModelKey,
		int outputOffset,
        string output,
        List<TextEditorTextSpan> outputTextSpanList,
        TerminalCommand terminalCommand,
        TerminalCommandBoundary terminalCommandBoundary,
		ITextEditorService textEditorService)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		ViewModelKey = viewModelKey;
		OutputOffset = outputOffset;
        Output = output;
        OutputTextSpanList = outputTextSpanList;
        TerminalCommand = terminalCommand;
        TerminalCommandBoundary = terminalCommandBoundary;
		TextEditorService = textEditorService;
	}

	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Insertion;

	/// <summary>
	/// The resource uri of the model which is to be worked upon.
	/// </summary>
	public ResourceUri ResourceUri { get; }

	/// <summary>
	/// This property is optional, and can be Key<TextEditorViewModel>.Empty,
	/// if one does not make use of it.
	///
	/// The key of the view model which is to be worked upon.
	/// </summary>
	public Key<TextEditorViewModel> ViewModelKey { get; }

	/// <summary>
	/// This property is optional, and can be Key<TextEditorCursor>.Empty,
	/// if one does not make use of it.
	///
	/// Track where the content should be inserted.
	/// </summary>
	public Key<TextEditorCursor> CursorKey { get; }

	/// <summary>
	/// If the cursor is not already registered within the ITextEditorEditContext,
	/// then invoke this Func, and then register a CursorModifier in the
	/// ITextEditorEditContext.
	/// </summary>
	public Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> GetCursorFunc { get; }
	
	public int OutputOffset { get; }
    public string Output { get; }
    public List<TextEditorTextSpan> OutputTextSpanList { get; private set; }
    public TerminalCommand TerminalCommand { get; }
    public TerminalCommandBoundary TerminalCommandBoundary { get; }
	public ITextEditorService TextEditorService { get; }

	public async Task Invoke(IEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (modelModifier is null || viewModelModifier is null)
            return;

		var (primaryCursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);

		var entryPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        TerminalCommandBoundary.StartPositionIndexInclusive ??= entryPositionIndex;

        await TextEditorService.ModelApi.InsertTextFactory(
                ResourceUri,
                ViewModelKey,
                Output,
                CancellationToken.None)
            .Invoke(editContext)
            .ConfigureAwait(false);

        var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
        if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
            return;

        OutputTextSpanList = OutputTextSpanList.Select(x => x with
        {
            StartingIndexInclusive = entryPositionIndex + x.StartingIndexInclusive,
            EndingIndexExclusive = entryPositionIndex + x.EndingIndexExclusive,
            ResourceUri = ResourceUri,
            SourceText = modelModifier.GetAllText(),
        }).ToList();

        terminalResource.ManualDecorationTextSpanList.AddRange(OutputTextSpanList);
        terminalResource.ManualSymbolList.AddRange(OutputTextSpanList.Select(x => new SourceFileSymbol(x)));

        await editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                modelModifier.ResourceUri,
                terminalResource.GetTokenTextSpans())
            .Invoke(editContext)
            .ConfigureAwait(false);

        TerminalCommandBoundary.EndPositionIndexExclusive = modelModifier.GetPositionIndex(primaryCursorModifier);

		TerminalCommand.TextSpan = new TextEditorTextSpan(
		    TerminalCommandBoundary.StartPositionIndexInclusive ?? 0,
		    TerminalCommandBoundary.EndPositionIndexExclusive ?? 0,
		    0,
		    ResourceUri,
			modelModifier.GetAllText() ?? string.Empty);

        await TerminalCommand.InvokeStateChangedCallbackFunc();
	}
}
