using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Generic;

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
        TerminalCommandBoundary terminalCommandBoundary,
        Key<TextEditorViewModel> viewModelKey)
    {
        OutputOffset = outputOffset;
        Output = output;
        _outputTextSpanList = outputTextSpanList;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
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
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(IEditContext editContext)
    {
        var modelModifier = editContext.GetModelModifier(ResourceUri);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

        var entryPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
        _terminalCommandBoundary.StartPositionIndexInclusive ??= entryPositionIndex;

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

        _outputTextSpanList = _outputTextSpanList.Select(x => x with
        {
            StartingIndexInclusive = entryPositionIndex + x.StartingIndexInclusive,
            EndingIndexExclusive = entryPositionIndex + x.EndingIndexExclusive,
            ResourceUri = ResourceUri,
            SourceText = modelModifier.GetAllText(),
        }).ToList();

        terminalResource.ManualDecorationTextSpanList.AddRange(_outputTextSpanList);
        terminalResource.ManualSymbolList.AddRange(_outputTextSpanList.Select(x => new SourceFileSymbol(x)));

        await editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                modelModifier.ResourceUri,
                terminalResource.GetTokenTextSpans())
            .Invoke(editContext)
            .ConfigureAwait(false);

        _terminalCommandBoundary.EndPositionIndexExclusive = modelModifier.GetPositionIndex(primaryCursorModifier);
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is OnOutput oldEventOnOutput)
        {
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
                _terminalCommandBoundary,
                ViewModelKey);
        }

        if (oldEvent is OnOutputBatch oldOnOutputBatch)
        {
            oldOnOutputBatch.OutputList.Add(Output);
            oldOnOutputBatch.OutputTextSpanAndOffsetTupleList.Add((OutputOffset, _outputTextSpanList));
            return oldOnOutputBatch;
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
