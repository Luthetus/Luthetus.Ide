using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

/// <summary>
/// Store the state of none or many tabs, and which tab is the active one. Each tab represents a <see cref="TextEditorViewModel"/>.
/// </summary>
public record TextEditorGroup(
	    Key<TextEditorGroup> GroupKey,
	    Key<TextEditorViewModel> ActiveViewModelKey,
	    ImmutableList<Key<TextEditorViewModel>> ViewModelKeyList)
	 : ITabGroup
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
    public ITextEditorService TextEditorService { get; set; }

    public bool GetIsActive(ITab tab)
	{
		if (tab is not ITextEditorTab textEditorTab)
			return false;

		return ActiveViewModelKey == textEditorTab.ViewModelKey;
	}

	public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs)
	{
		if (tab is not ITextEditorTab textEditorTab)
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
		if (tab is not ITextEditorTab textEditorTab)
			return Task.CompletedTask;

		TextEditorService.GroupApi.RemoveViewModel(GroupKey, textEditorTab.ViewModelKey);
		return Task.CompletedTask;
	}
}