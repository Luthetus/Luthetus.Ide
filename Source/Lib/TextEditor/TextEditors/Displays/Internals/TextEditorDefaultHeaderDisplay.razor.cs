using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

// HeaderDriver.cs
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDefaultHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private INotificationService NotificationService { get; set; } = null!;
	[Inject]
	private IClipboardService ClipboardService { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
	[Inject]
	private IDirtyResourceUriService DirtyResourceUriService { get; set; } = null!;

	[Parameter, EditorRequired]
	public Key<TextEditorComponentData> ComponentDataKey { get; set; }

	public string _reloadButtonHtmlElementId = "luth_te_text-editor-header-reload-button";
	
	private Key<TextEditorComponentData> _componentDataKeyPrevious = Key<TextEditorComponentData>.Empty;
    private TextEditorComponentData? _componentData;

	protected override void OnInitialized()
    {
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += OnCursorShouldBlinkChanged;
        OnCursorShouldBlinkChanged();
        
        base.OnInitialized();
    }
    
    private TextEditorRenderBatch GetRenderBatch()
    {
    	return GetComponentData()?._renderBatch ?? default;
    }
    
    private TextEditorComponentData? GetComponentData()
    {
    	if (_componentDataKeyPrevious != ComponentDataKey)
    	{
    		if (!TextEditorService.TextEditorState._componentDataMap.TryGetValue(ComponentDataKey, out var componentData) ||
    		    componentData is null)
    		{
    			_componentData = null;
    		}
    		else
    		{
    			_componentData = componentData;
				_componentDataKeyPrevious = ComponentDataKey;
    		}
    	}
    	
		return _componentData;
    }

	private async void OnCursorShouldBlinkChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public Task DoSaveOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;

        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.TriggerSave),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	TextEditorCommandDefaultFunctions.TriggerSave(
            		editContext,
            		modelModifier,
            		viewModelModifier,
            		CommonComponentRenderers,
            		NotificationService);
            	return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }

    public void ShowWatchWindowDisplayDialogOnClick()
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
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
            $"WatchWindow: {model.PersistentState.ResourceUri}",
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

        DialogService.ReduceRegisterAction(dialogRecord);
    }

	public Task DoCopyOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.CopyAsync),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	return TextEditorCommandDefaultFunctions.CopyAsync(
                	editContext,
                	modelModifier,
                	viewModelModifier,
                	ClipboardService);
            });
        return Task.CompletedTask;
    }

    public Task DoCutOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.CutAsync),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	return TextEditorCommandDefaultFunctions.CutAsync(
            		editContext,
                	modelModifier,
                	viewModelModifier,
                	ClipboardService);
            });
        return Task.CompletedTask;
    }

    public Task DoPasteOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.PasteAsync),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	return TextEditorCommandDefaultFunctions.PasteAsync(
                	editContext,
                	modelModifier,
                	viewModelModifier,
                	ClipboardService);
            });
        return Task.CompletedTask;
    }

    public Task DoRedoOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;

        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.Redo),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	TextEditorCommandDefaultFunctions.Redo(
            		editContext,
            		modelModifier,
            		viewModelModifier);
            	
            	return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }

    public Task DoUndoOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.Undo),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	TextEditorCommandDefaultFunctions.Undo(
            		editContext,
            		modelModifier,
            		viewModelModifier);
        		return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }

    public Task DoSelectAllOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.SelectAll),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	TextEditorCommandDefaultFunctions.SelectAll(
            		editContext,
            		modelModifier,
            		viewModelModifier);
            		
        		return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }

    public Task DoRemeasureOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.TriggerRemeasure),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	TextEditorCommandDefaultFunctions.TriggerRemeasure(
            		editContext,
            		viewModelModifier);
            		
        		return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
    }

    public async Task DoReloadOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return;
        
        var dropdownKey = Key<DropdownRecord>.NewKey();
        
        var buttonDimensions = await TextEditorService.JsRuntimeCommonApi
			.MeasureElementById(_reloadButtonHtmlElementId)
			.ConfigureAwait(false);
			
		var menuOptionList = new List<MenuOptionRecord>();
		
		var absolutePath = EnvironmentProvider.AbsolutePathFactory(renderBatchLocal.Model.PersistentState.ResourceUri.Value, false);

		menuOptionList.Add(new MenuOptionRecord(
		    "Cancel",
		    MenuOptionKind.Read,
		    onClickFunc: () =>
		    {
			    DropdownService.ReduceDisposeAction(dropdownKey);
		    	return Task.CompletedTask;
		    }));
		    
		menuOptionList.Add(new MenuOptionRecord(
		    $"Reset: '{absolutePath.NameWithExtension}'",
		    MenuOptionKind.Delete,
		    onClickFunc: () =>
		    {
			    TextEditorService.WorkerArbitrary.PostUnique(
		            nameof(DoReloadOnClick),
		            editContext =>
		            {
		            	editContext.TextEditorService.ViewModelApi.Dispose(editContext, renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
		            	DirtyResourceUriService.RemoveDirtyResourceUri(renderBatchLocal.Model.PersistentState.ResourceUri);
		            	editContext.TextEditorService.ModelApi.Dispose(editContext, renderBatchLocal.Model.PersistentState.ResourceUri);
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
			async () => await TextEditorService.JsRuntimeCommonApi.FocusHtmlElementById(_reloadButtonHtmlElementId));

        DropdownService.ReduceRegisterAction(dropdownRecord);
    }

    public Task DoRefreshOnClick()
    {
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.TriggerRemeasure),
            editContext =>
            {
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.PersistentState.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
            
            	TextEditorCommandDefaultFunctions.TriggerRemeasure(
            		editContext,
            		viewModelModifier);
            	return ValueTask.CompletedTask;
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
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
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
    	var renderBatchLocal = GetRenderBatch();
    	if (!renderBatchLocal.IsValid)
    		return true;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return true;

        return !model.CanRedoEdit();
    }

	public void Dispose()
    {
    	TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= OnCursorShouldBlinkChanged;
    }
}