using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Events;

/// <summary>
/// This class will take contiguous events of this same type, and combine them to
/// all to run in the same <see cref="IEditContext"/>, provided they share
/// the same <see cref="Identifier"/>.
/// The result of this is that the UI will not be notified to re-render
/// until after the batch has all done their edits.
/// </summary>
public sealed class SimpleBatchTextEditorTask : SimpleBatchBackgroundTask, ITextEditorTask
{
    private readonly List<TextEditorEdit> _textEditorEditList;

    public SimpleBatchTextEditorTask(
            string name,
            string identifier,
            TextEditorEdit textEditorEdit,
            TimeSpan? throttleTimeSpan = null)
        : this(name, identifier, new List<TextEditorEdit>() { textEditorEdit }, throttleTimeSpan)
    {
    }

    public SimpleBatchTextEditorTask(
            string name,
            string identifier,
            List<TextEditorEdit> textEditorEditList,
            TimeSpan? throttleTimeSpan = null)
        : base(name, identifier, new List<Func<CancellationToken, Task>>(), throttleTimeSpan)
    {
        _textEditorEditList = textEditorEditList;

        Name = name;
        Identifier = identifier;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;
    }

    public async Task InvokeWithEditContext(IEditContext editContext)
    {
        foreach (var edit in _textEditorEditList)
        {
            await edit
                .Invoke(editContext)
                .ConfigureAwait(false);
        }
    }

    public override IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not SimpleBatchTextEditorTask oldSimpleBatchTextEditorTask)
            return null;

        oldSimpleBatchTextEditorTask._textEditorEditList.AddRange(_textEditorEditList);
        oldSimpleBatchTextEditorTask.Name += '_' + Name;
        return oldSimpleBatchTextEditorTask;
    }

    public override Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
