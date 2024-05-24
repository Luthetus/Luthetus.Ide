using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Edits.States;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.Tests.Adhoc.Rewrite;

public class TestTextEditorService : ITextEditorService
{
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IDispatcher _dispatcher;
	private readonly object _lockBackgroundTaskTryReusingSameInstance = new();

	public TestTextEditorService(
		IBackgroundTaskService backgroundTaskService,
		IState<TextEditorModelState> modelStateWrap,
		IState<TextEditorViewModelState> viewModelStateWrap,
		IDispatcher dispatcher)
	{
		_backgroundTaskService = backgroundTaskService;
		ModelStateWrap = modelStateWrap;
		ViewModelStateWrap = viewModelStateWrap;
		_dispatcher = dispatcher;
	}

	private TextEditorBackgroundTask? _backgroundTask;

    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }

    public ITextEditorModelApi ModelApi { get; }
    public ITextEditorViewModelApi ViewModelApi { get; }
    public ITextEditorGroupApi GroupApi { get; }
    public ITextEditorDiffApi DiffApi { get; }
    public ITextEditorOptionsApi OptionsApi { get; }

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindAllState> FindAllStateWrap { get; }

	public IEditContext OpenEditContext()
	{
		Console.WriteLine(nameof(OpenEditContext));

		return new TextEditorService.TextEditorEditContext(
			this,
			TextEditorService.AuthenticatedActionKey);
	}

	public async Task CloseEditContext(IEditContext editContext)
	{
		Console.WriteLine(nameof(CloseEditContext));
		
		foreach (var modelModifier in editContext.ModelCache.Values)
        {
            if (modelModifier is null || !modelModifier.WasModified)
                continue;

            _dispatcher.Dispatch(new TextEditorModelState.SetAction(
                editContext.AuthenticatedActionKey,
                editContext,
                modelModifier));

            var viewModelBag = ViewModelStateWrap.Value.ViewModelList.Where(x => x.ResourceUri == modelModifier.ResourceUri);

            foreach (var viewModel in viewModelBag)
            {
                // Invoking 'GetViewModelModifier' marks the view model to be updated.
                editContext.GetViewModelModifier(viewModel.ViewModelKey);
            }

            if (modelModifier.WasDirty != modelModifier.IsDirty)
            {
                if (modelModifier.IsDirty)
                    _dispatcher.Dispatch(new DirtyResourceUriState.AddDirtyResourceUriAction(modelModifier.ResourceUri));
                else
                    _dispatcher.Dispatch(new DirtyResourceUriState.RemoveDirtyResourceUriAction(modelModifier.ResourceUri));
            }
        }

        foreach (var viewModelModifier in editContext.ViewModelCache.Values)
        {
            if (viewModelModifier is null || !viewModelModifier.WasModified)
                return;

            var successCursorModifierBag = editContext.CursorModifierBagCache.TryGetValue(
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
                await ((TextEditorService)editContext.TextEditorService)
                    .HACK_SetScrollPosition(viewModelModifier.ViewModel)
                    .ConfigureAwait(false);
            }

            // TODO: This 'CalculateVirtualizationResultFactory' invocation is horrible for performance.
            await editContext.TextEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
                    viewModelModifier.ViewModel.ResourceUri, viewModelModifier.ViewModel.ViewModelKey, CancellationToken.None)
                .Invoke(editContext)
                .ConfigureAwait(false);

            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                editContext.AuthenticatedActionKey,
                editContext,
                viewModelModifier.ViewModel.ViewModelKey,
                inState => viewModelModifier.ViewModel));
        }
	}

	public Task Post(ITextEditorWork work)
	{
		lock (_lockBackgroundTaskTryReusingSameInstance)
		{
			if (_backgroundTask is null || !_backgroundTask.TryReusingSameInstance(work))
				_backgroundTask = new(this, work);
		}

		return _backgroundTaskService.EnqueueAsync(_backgroundTask);
	}

	public Task Post(ITextEditorTask textEditorTask)
	{
		throw new NotImplementedException();
	}

    public Task PostSimpleBatch(
        string name,
        string identifier,
		string? redundancy,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
	{
		throw new NotImplementedException();
	}
}
