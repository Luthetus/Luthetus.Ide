using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

/// <summary>
/// Store the state of none or many tabs, and which tab is the active one. Each tab represents a <see cref="TextEditorViewModel"/>.
/// </summary>
public record TextEditorGroup(
        Key<TextEditorGroup> GroupKey,
        Key<TextEditorViewModel> ActiveViewModelKey,
		List<Key<TextEditorViewModel>> ViewModelKeyList,
        Category Category,
        TextEditorService TextEditorService,
        IPanelService PanelService,
        IDialogService DialogService,
        CommonBackgroundTaskApi CommonBackgroundTaskApi)
     : ITabGroup
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();

    public bool GetIsActive(ITab tab)
    {
        if (tab is not TextEditorViewModelPersistentState textEditorTab)
            return false;

        return ActiveViewModelKey == textEditorTab.ViewModelKey;
    }

    public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs)
    {
        if (tab is not TextEditorViewModelPersistentState textEditorTab)
            return Task.CompletedTask;

        if (!GetIsActive(tab))
            TextEditorService.GroupApi.SetActiveViewModel(GroupKey, textEditorTab.ViewModelKey);

        return Task.CompletedTask;
    }

    public string GetDynamicCss(ITab tab)
    {
        return string.Empty;
    }

    public Task CloseAsync(ITab tab)
    {
        if (tab is not TextEditorViewModelPersistentState textEditorTab)
            return Task.CompletedTask;

		Close(textEditorTab.ViewModelKey);
        return Task.CompletedTask;
    }

    public Task CloseAllAsync()
    {
        var localViewModelKeyList = ViewModelKeyList;

        foreach (var viewModelKey in localViewModelKeyList)
        {
            Close(viewModelKey);
        }
        
        return Task.CompletedTask;
    }

	public async Task CloseOthersAsync(ITab safeTab)
    {
        var localViewModelKeyList = ViewModelKeyList;

		if (safeTab is not TextEditorViewModelPersistentState safeTextEditorTab)
			return;
		
		// Invoke 'OnClickAsync' to set the active tab to the "safe tab"
		// OnClickAsync does not currently use its mouse event args argument.
		await OnClickAsync(safeTab, null);

        foreach (var viewModelKey in localViewModelKeyList)
        {
			var shouldClose = safeTextEditorTab.ViewModelKey != viewModelKey;

			if (shouldClose)
				Close(viewModelKey);
        }
    }
    
    private void Close(Key<TextEditorViewModel> viewModelKey)
    {
    	TextEditorService.GroupApi.RemoveViewModel(GroupKey, viewModelKey);
    }
}
