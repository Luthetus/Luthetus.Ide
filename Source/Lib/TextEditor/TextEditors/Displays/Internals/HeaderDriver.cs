using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Edits.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

/// <summary>
/// The header has @onclick events in the UI and as such could not be made into a RenderFragment.
/// Instead, the UI was inlined into the <see cref="TextEditorViewModelDisplay"/>.
///
/// TODO: Move this back into <see cref="TextEditorDefaultHeaderDisplay"/>. This was made into the Driver idea, then made back into a component.
/// </summary>
public class HeaderDriver
{
	public readonly TextEditorViewModelDisplay _root;

	public HeaderDriver(TextEditorViewModelDisplay textEditorViewModelDisplay)
	{
		_root = textEditorViewModelDisplay;
	}

	// Odd public but am middle of thinking
	public TextEditorRenderBatch _renderBatch;

	public void GetRenderFragment(TextEditorRenderBatch renderBatch)
	{
		// Dangerous state can change mid run possible?
		_renderBatch = renderBatch;
	}

	public string _reloadButtonHtmlElementId = "luth_te_text-editor-header-reload-button";
	
    public TextEditorCommandArgs ConstructCommandArgs(TextEditorRenderBatch renderBatchLocal)
    {
        var cursorSnapshotsList = new TextEditorCursor[] { renderBatchLocal.ViewModel.PrimaryCursor }.ToImmutableArray();
        var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorSnapshotsList.First(x => x.IsPrimaryCursor).Selection);

        return new TextEditorCommandArgs(
            renderBatchLocal.Model.ResourceUri,
            renderBatchLocal.ViewModel.ViewModelKey,
            renderBatchLocal.ComponentData,
			_root.TextEditorService,
            _root.ServiceProvider,
            null);
    }

    public Task DoCopyOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Copy),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Copy.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoCutOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Cut),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Cut.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoPasteOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.PasteCommand),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.PasteCommand.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoRedoOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Redo),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Redo.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoSaveOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.TriggerSave),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.TriggerSave.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoUndoOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Undo),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Undo.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoSelectAllOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.SelectAll),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.SelectAll.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public Task DoRemeasureOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Remeasure),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Remeasure.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    public async Task DoReloadOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);
        
        var dropdownKey = Key<DropdownRecord>.NewKey();
        
        var buttonDimensions = await _root.TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(_reloadButtonHtmlElementId)
			.ConfigureAwait(false);
			
		var menuOptionList = new List<MenuOptionRecord>();
		
		var absolutePath = _root.EnvironmentProvider.AbsolutePathFactory(model.ResourceUri.Value, false);

		menuOptionList.Add(new MenuOptionRecord(
		    "Cancel",
		    MenuOptionKind.Read,
		    onClickFunc: () =>
		    {
			    _root.Dispatcher.Dispatch(new DropdownState.DisposeAction(dropdownKey));
		    	return Task.CompletedTask;
		    }));
		    
		menuOptionList.Add(new MenuOptionRecord(
		    $"Reset: '{absolutePath.NameWithExtension}'",
		    MenuOptionKind.Delete,
		    onClickFunc: () =>
		    {
			    _root.TextEditorService.TextEditorWorker.PostUnique(
		            nameof(DoReloadOnClick),
		            editContext =>
		            {
		            	editContext.TextEditorService.ViewModelApi.Dispose(viewModel.ViewModelKey);
		            	_root.Dispatcher.Dispatch(new DirtyResourceUriState.RemoveDirtyResourceUriAction(model.ResourceUri));
		            	editContext.TextEditorService.ModelApi.Dispose(model.ResourceUri);
		            	return ValueTask.CompletedTask;
		            });
		    	return Task.CompletedTask;
		    }));
		    
		var menu = new MenuRecord(menuOptionList);

		var dropdownRecord = new DropdownRecord(
			dropdownKey,
			buttonDimensions.LeftInPixels,
			buttonDimensions.TopInPixels + buttonDimensions.HeightInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					menu
				}
			},
			async () => await _root.TextEditorService.JsRuntimeCommonApi.FocusHtmlElementById(_reloadButtonHtmlElementId));

        _root.Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }

    public void ShowWatchWindowDisplayDialogOnClick()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        var model = renderBatchLocal.Model;

        if (model is null)
            return;

        var watchWindowObject = new WatchWindowObject(
            renderBatchLocal,
            typeof(TextEditorRenderBatch),
            nameof(TextEditorRenderBatch),
            true);

        var dialogRecord = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"WatchWindow: {model.ResourceUri}",
            typeof(WatchWindowDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(WatchWindowDisplay.WatchWindowObject),
                    watchWindowObject
                }
            },
            null,
			true,
			null);

        _root.DialogService.RegisterDialogRecord(dialogRecord);
    }

    public Task DoRefreshOnClick()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        var commandArgs = ConstructCommandArgs(renderBatchLocal);

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFacts.Remeasure),
            editContext =>
            {
                commandArgs.EditContext = editContext;
                return TextEditorCommandDefaultFacts.Remeasure.CommandFunc
                    .Invoke(commandArgs);
            });
        return Task.CompletedTask;
    }

    /// <summary>
    /// disabled=@GetUndoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetUndoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    public bool GetUndoDisabledAttribute()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return true;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return true;

        return !model.CanUndoEdit();
    }

    /// <summary>
    /// disabled=@GetRedoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetRedoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    public bool GetRedoDisabledAttribute()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return true;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return true;

        return !model.CanRedoEdit();
    }
}
