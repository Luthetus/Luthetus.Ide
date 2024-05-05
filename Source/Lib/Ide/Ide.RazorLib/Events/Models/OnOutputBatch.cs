using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.Events.Models;

/// <summary>
/// This class is for use with command line text output.
/// For example, every time a line of output is written to stdout
/// one could furthermore invoke this event to write to the text editor
/// in a batched manner.
/// </summary>
public class OnOutputBatch : ITextEditorTask
{
    private readonly TerminalCommandBoundary _terminalCommandBoundary;

    public OnOutputBatch(
        int batchOutputOffset,
        List<string> outputList,
        List<(int OutputOffset, List<TextEditorTextSpan> OutputTextSpan)> outputTextSpanAndOffsetTupleList,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        TerminalCommandBoundary terminalCommandBoundary,
        Key<TextEditorViewModel> viewModelKey)
    {
        BatchOutputOffset = batchOutputOffset;
        OutputList = outputList;
        OutputTextSpanAndOffsetTupleList = outputTextSpanAndOffsetTupleList;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        _terminalCommandBoundary = terminalCommandBoundary;
        ViewModelKey = viewModelKey;
    }


    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnOutput);
    public Task? WorkProgress { get; }
    public int BatchOutputOffset { get; }
    public List<string> OutputList { get; }
    public List<(int OutputOffset, List<TextEditorTextSpan> OutputTextSpanList)> OutputTextSpanAndOffsetTupleList { get; }
    public ResourceUri ResourceUri { get; }
    public ITextEditorService TextEditorService { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;

    public Task InvokeWithEditContext(IEditContext editContext)
    {
        // Flatten 'OutputTextSpanAndOffsetTupleList'
        var outputTextSpanList = new List<TextEditorTextSpan>();
        foreach (var tuple in OutputTextSpanAndOffsetTupleList)
        {
            outputTextSpanList.AddRange(tuple.OutputTextSpanList.Select(x => x with
            {
                StartingIndexInclusive = x.StartingIndexInclusive + tuple.OutputOffset - BatchOutputOffset,
                EndingIndexExclusive = x.EndingIndexExclusive + tuple.OutputOffset - BatchOutputOffset,
            }));
        }

        var onOutput = new OnOutput(
            BatchOutputOffset,
            string.Join(string.Empty, OutputList),
            outputTextSpanList,
            ResourceUri,
            TextEditorService,
            _terminalCommandBoundary,
            ViewModelKey);

        return onOutput.InvokeWithEditContext(editContext);
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}

