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
        List<string> outputList,
        List<TextEditorTextSpan> outputTextSpanList,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        TerminalCommandBoundary terminalCommandBoundary,
        Key<TextEditorViewModel> viewModelKey)
    {
        OutputList = outputList;
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
    public List<string> OutputList { get; }
    public ResourceUri ResourceUri { get; }
    public ITextEditorService TextEditorService { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;

    public Task InvokeWithEditContext(IEditContext editContext)
    {
        var onOutput = new OnOutput(
            string.Join(string.Empty, OutputList),
            _outputTextSpanList,
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

