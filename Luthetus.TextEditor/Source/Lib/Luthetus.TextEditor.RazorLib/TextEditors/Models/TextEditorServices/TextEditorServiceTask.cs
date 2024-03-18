using Fluxor;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Edits.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public class TextEditorServiceTask : ITextEditorTask
{
	private readonly ITextEditorEditContext _editContext;
	private readonly IDispatcher _dispatcher;

	public TextEditorServiceTask(
		ITextEditorTask innerTask,
		ITextEditorEditContext editContext,
		IDispatcher dispatcher)
    {
		_innerTask = innerTask;
        _editContext = editContext;
		_dispatcher = dispatcher;
    }

	private ITextEditorTask _innerTask;

    public Key<BackgroundTask> BackgroundTaskKey => _innerTask.BackgroundTaskKey;
    public Key<BackgroundTaskQueue> QueueKey => _innerTask.QueueKey;
    public string Name => _innerTask.Name;
    public Task? WorkProgress => _innerTask.WorkProgress;

    public TimeSpan ThrottleTimeSpan => _innerTask.ThrottleTimeSpan;

    public Task InvokeWithEditContext(ITextEditorEditContext editContext)
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

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
        await _innerTask.InvokeWithEditContext(_editContext);

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
