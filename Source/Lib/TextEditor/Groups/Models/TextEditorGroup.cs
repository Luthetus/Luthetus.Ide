using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

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
        if (tab is not ITabTextEditor textEditorTab)
            return false;

        return ActiveViewModelKey == textEditorTab.ViewModelKey;
    }

    public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs)
    {
        if (tab is not ITabTextEditor textEditorTab)
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
        if (tab is not ITabTextEditor textEditorTab)
            return Task.CompletedTask;

        TextEditorService.GroupApi.RemoveViewModel(GroupKey, textEditorTab.ViewModelKey);
        return Task.CompletedTask;
    }

    public async Task CloseAllAsync()
    {
        var localViewModelKeyList = ViewModelKeyList;

        foreach (var viewModelKey in localViewModelKeyList)
        {
            await CloseAsync(new DynamicViewModelAdapterTextEditor(
                    viewModelKey,
                    TextEditorService,
                    PanelService,
                    DialogService,
                    CommonBackgroundTaskApi))
                .ConfigureAwait(false);
        }
    }

	public async Task CloseOthersAsync(ITab safeTab)
    {
        var localViewModelKeyList = ViewModelKeyList;

		if (safeTab is not ITabTextEditor safeTextEditorTab)
			return;
		
		// Invoke 'OnClickAsync' to set the active tab to the "safe tab"
		// OnClickAsync does not currently use its mouse event args argument.
		await OnClickAsync(safeTab, null);

        foreach (var viewModelKey in localViewModelKeyList)
        {
			var shouldClose = safeTextEditorTab.ViewModelKey != viewModelKey;

			if (shouldClose)
			{
				await CloseAsync(new DynamicViewModelAdapterTextEditor(
	                    viewModelKey,
	                    TextEditorService,
	                    PanelService,
	                    DialogService,
	                    CommonBackgroundTaskApi))
	                .ConfigureAwait(false);
			}
        }
    }
}
