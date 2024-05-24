using Fluxor;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Edits.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorServiceTask : ITextEditorTask
{
    private readonly IEditContext _editContext;
    private readonly IDispatcher _dispatcher;

    public TextEditorServiceTask(
        ITextEditorTask innerTask,
        IEditContext editContext,
        IDispatcher dispatcher)
    {
        _innerTask = innerTask;
        _editContext = editContext;
        _dispatcher = dispatcher;
    }
	
	/*

Map events and commands into 'DocumentEditArgs',

public class DocumentKeyboardEvent
{
	ToDocumentEditArgs() => new List<DocumentEditArgs>
	{
		DocumentEditArgs
	};
}

public class DocumentEditArgs
{
}

What are the "primitive edits" one can perform to a document?
-Insert
-Deletion
-


Define "primitive edit":
A given primitive edit must not be describable in terms
of a separately distinct primitive edit. Nor,
the combination of any primitive edits.

Selecting then typing "Hello World!" is made of the primitive edits:
-Deletion  (the selection)
-Insertion (the text inserted)

If we use "primitive edits" we can easily batch operations.

I'm going to add to the previous example,
Selecting then typing "Hello World!", then furthermore paste the clipboard contents.
-Deletion  (the selection)
-Insertion (the text inserted)

The example is equivalent in regards to the primitive edits.
The "complex edit" of selecting text then typing over it,
is just Deletion, then Insertion.

If we look at a queue that is paused, then it will just contain
[
	Deletion,
	Insertion,
]

Therefore when the "complex edit" of pasting the clipboard's contents
is performed. Since this "complex edit" is "Insertion" and we ended
the previous edit with an "Insertion" these can be batched.
		
Furthermore, by describing things in terms of "primitive edits",
the undo/redo logic is drastically more optimized.

Storing the entirety of the text in stages was done because
of the confusion about how one would undo a "complex edit".

But, so long as we can undo a "primitive edit" and we describe "complex edits"
with one or many "primitive edits" then there should be no reason
to store copies of the text, but instead just perform the edit / un-perform.

I don't like "TextEditorServiceTask". It results in an extra object
created, everytime I enqueue something.

That being said, "TextEditorServiceTask" does provide a single path to edit
a document. There's no way to edit without the TextEditorEditContext
and its all done through the "TextEditorServiceTask".

If prior to enqueueing from ITextEditorService to IBackgroundService
I could block the IBackgroundService.Queue and look at the last entry,
I could see if there already were a "TextEditorServiceTask" ahead of me
in the queue, then I don't need to create a "TextEditorServiceTask" for no reason.

I'm also considering the importance of using a struct?
If these UI events are occurring at a fast enough rate,
perhaps the cost of garbage collection would be noticable?
	*/
    private ITextEditorTask _innerTask;

    public Key<BackgroundTask> BackgroundTaskKey => _innerTask.BackgroundTaskKey;
    public Key<BackgroundTaskQueue> QueueKey => _innerTask.QueueKey;
    public string Name => _innerTask.Name;
    public string? Redundancy { get; } = null;
	public TextEditorEdit Edit => _innerTask.Edit;
    public Task? WorkProgress => _innerTask.WorkProgress;

    public TimeSpan ThrottleTimeSpan => _innerTask.ThrottleTimeSpan;

    public Task InvokeWithEditContext(IEditContext editContext)
    {
        return _innerTask.InvokeWithEditContext(editContext);
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is TextEditorServiceTask oldTextEditorServiceTask)
        {
            var batchResult = _innerTask.BatchOrDefault(oldTextEditorServiceTask._innerTask);

            if (batchResult is not null && batchResult is ITextEditorTask batchResultTextEditorTask)
            {
                _innerTask = batchResultTextEditorTask;
                return this;
            }
        }

        return null;
    }

	public IBackgroundTask? DequeueBatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
        await _innerTask.InvokeWithEditContext(_editContext).ConfigureAwait(false);

        foreach (var modelModifier in _editContext.ModelCache.Values)
        {
            if (modelModifier is null || !modelModifier.WasModified)
                continue;

            _dispatcher.Dispatch(new TextEditorModelState.SetAction(
                _editContext.AuthenticatedActionKey,
                _editContext,
                modelModifier));

            var viewModelBag = _editContext.TextEditorService.ModelApi.GetViewModelsOrEmpty(modelModifier.ResourceUri);

            foreach (var viewModel in viewModelBag)
            {
                // Invoking 'GetViewModelModifier' marks the view model to be updated.
                _editContext.GetViewModelModifier(viewModel.ViewModelKey);
            }

            if (modelModifier.WasDirty != modelModifier.IsDirty)
            {
                if (modelModifier.IsDirty)
                    _dispatcher.Dispatch(new DirtyResourceUriState.AddDirtyResourceUriAction(modelModifier.ResourceUri));
                else
                    _dispatcher.Dispatch(new DirtyResourceUriState.RemoveDirtyResourceUriAction(modelModifier.ResourceUri));
            }
        }

        foreach (var viewModelModifier in _editContext.ViewModelCache.Values)
        {
            if (viewModelModifier is null || !viewModelModifier.WasModified)
                return;

            var successCursorModifierBag = _editContext.CursorModifierBagCache.TryGetValue(
                viewModelModifier.ViewModel.ViewModelKey,
                out var cursorModifierBag);

            if (successCursorModifierBag && cursorModifierBag is not null)
            {
                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
                    CursorList = cursorModifierBag.List
                        .Select(x => x.ToCursor())
                        .ToImmutableArray()
                };
            }

            if (viewModelModifier.ScrollWasModified)
            {
                await ((TextEditorService)_editContext.TextEditorService)
                    .HACK_SetScrollPosition(viewModelModifier.ViewModel)
                    .ConfigureAwait(false);
            }

            // TODO: This 'CalculateVirtualizationResultFactory' invocation is horrible for performance.
            await _editContext.TextEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
                    viewModelModifier.ViewModel.ResourceUri, viewModelModifier.ViewModel.ViewModelKey, CancellationToken.None)
                .Invoke(_editContext)
                .ConfigureAwait(false);

            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                _editContext.AuthenticatedActionKey,
                _editContext,
                viewModelModifier.ViewModel.ViewModelKey,
                inState => viewModelModifier.ViewModel));
        }
    }
}
