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
    
    public Task DoSaveOnClick(MouseEventArgs arg)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return Task.CompletedTask;
    	
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;

        if (model is null || viewModel is null)
            return Task.CompletedTask;

        _root.TextEditorService.TextEditorWorker.PostUnique(
            nameof(TextEditorCommandDefaultFunctions.TriggerSave),
            editContext =>
            {
            	var modelModifier = editContext.GetModelModifier(model.ResourceUri);
            	var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);
            	var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            
            	TextEditorCommandDefaultFunctions.TriggerSave(
            		editContext,
            		modelModifier,
            		viewModelModifier,
            		cursorModifierBag,
            		_root.CommonComponentRenderers,
            		_root.NotificationService);
            	return ValueTask.CompletedTask;
            });
        return Task.CompletedTask;
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

        _root.DialogService.ReduceRegisterAction(dialogRecord);
    }
}
