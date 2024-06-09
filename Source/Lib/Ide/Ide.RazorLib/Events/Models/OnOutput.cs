using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Events.Models;

/// <summary>
/// This class is what one would use. Then, if batching is applicable,
/// an instance of <see cref="OnOutputBatch"/> will be made in place
/// of the events which are being batched.<br/><br/>
/// 
/// This class is for use with command line text output.
/// For example, every time a line of output is written to stdout
/// one could furthermore invoke this event to write to the text editor
/// in a batched manner.
/// </summary>
public class OnOutput : ITextEditorTask
{
    private readonly TerminalCommandBoundary _terminalCommandBoundary;

    public OnOutput(
        int outputOffset,
        string output,
        List<TextEditorTextSpan> outputTextSpanList,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        TerminalCommand terminalCommand,
        TerminalCommandBoundary terminalCommandBoundary,
        Key<TextEditorViewModel> viewModelKey)
    {
        OutputOffset = outputOffset;
        Output = output;
        _outputTextSpanList = outputTextSpanList;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        TerminalCommand = terminalCommand;
		_terminalCommandBoundary = terminalCommandBoundary;
        ViewModelKey = viewModelKey;
    }

    private List<TextEditorTextSpan> _outputTextSpanList;

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnOutput);
    public Task? WorkProgress { get; }
    public int OutputOffset { get; }
    public string Output { get; }
    public ResourceUri ResourceUri { get; }
    public ITextEditorService TextEditorService { get; }
	public TerminalCommand TerminalCommand { get; }
	public Key<TextEditorViewModel> ViewModelKey { get; }

	public IEditContext EditContext { get; set; }

    public TimeSpan ThrottleTimeSpan => TextEditorComponentData.ThrottleDelayDefault;

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is OnOutput oldEventOnOutput)
        {
            if (TerminalCommand.TerminalCommandKey != oldEventOnOutput.TerminalCommand.TerminalCommandKey)
                return null;

            var localOutputList = new List<string>
            {
                oldEventOnOutput.Output,
                Output
            };

            var localOutputTextSpanAndOffsetTupleList = new List<(int OutputOffset, List<TextEditorTextSpan> OutputTextSpan)>
            {
                (oldEventOnOutput.OutputOffset, oldEventOnOutput._outputTextSpanList),
                (OutputOffset, _outputTextSpanList)
            };

            return new OnOutputBatch(
                oldEventOnOutput.OutputOffset,
                localOutputList,
                localOutputTextSpanAndOffsetTupleList,
                ResourceUri,
                TextEditorService,
                TerminalCommand,
                _terminalCommandBoundary,
                ViewModelKey)
				{
					EditContext = EditContext
				};
        }

        if (oldEvent is OnOutputBatch oldOnOutputBatch)
        {
			if (TerminalCommand.TerminalCommandKey != oldOnOutputBatch.TerminalCommand.TerminalCommandKey)
				return null;

			oldOnOutputBatch.OutputList.Add(Output);
            oldOnOutputBatch.OutputTextSpanAndOffsetTupleList.Add((OutputOffset, _outputTextSpanList));
            return oldOnOutputBatch;
        }

        return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
            var modelModifier = EditContext.GetModelModifier(ResourceUri);
            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
            var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            var entryPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
            _terminalCommandBoundary.StartPositionIndexInclusive ??= entryPositionIndex;

            await TextEditorService.ModelApi.InsertTextFactory(
                    ResourceUri,
                    ViewModelKey,
                    Output,
                    CancellationToken.None)
                .Invoke(EditContext)
                .ConfigureAwait(false);

            var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
            if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                return;

            _outputTextSpanList = _outputTextSpanList.Select(x => x with
            {
                StartingIndexInclusive = entryPositionIndex + x.StartingIndexInclusive,
                EndingIndexExclusive = entryPositionIndex + x.EndingIndexExclusive,
                ResourceUri = ResourceUri,
                SourceText = modelModifier.GetAllText(),
            }).ToList();

            terminalResource.ManualDecorationTextSpanList.AddRange(_outputTextSpanList);
            terminalResource.ManualSymbolList.AddRange(_outputTextSpanList.Select(x => new SourceFileSymbol(x)));

            await EditContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                    modelModifier.ResourceUri,
                    terminalResource.GetTokenTextSpans())
                .Invoke(EditContext)
                .ConfigureAwait(false);

            _terminalCommandBoundary.EndPositionIndexExclusive = modelModifier.GetPositionIndex(primaryCursorModifier);

            TerminalCommand.TextSpan = new TextEditorTextSpan(
                _terminalCommandBoundary.StartPositionIndexInclusive ?? 0,
                _terminalCommandBoundary.EndPositionIndexExclusive ?? 0,
                0,
                ResourceUri,
                modelModifier.GetAllText() ?? string.Empty);

            await TerminalCommand.InvokeStateChangedCallbackFunc();
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
