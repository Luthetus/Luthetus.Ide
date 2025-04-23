using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Groups.Displays;

public partial class TextEditorGroupDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorGroupKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorGroup"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorGroupKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorGroupKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public Key<TextEditorGroup> TextEditorGroupKey { get; set; } = Key<TextEditorGroup>.Empty;

    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    /// <summary><see cref="HeaderButtonKindList"/> contains the enum value that represents a button displayed in the optional component: <see cref="TextEditorHeader"/>.</summary>
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();
    [Parameter]
    public bool UseTextEditorViewModelSlimDisplay { get; set; }

	private TabListDisplay? _tabListDisplay;

	private string? _htmlId = null;
	private string HtmlId => _htmlId ??= $"luth_te_group_{TextEditorGroupKey.Guid}";

    protected override void OnInitialized()
    {
        TextEditorService.GroupApi.TextEditorGroupStateChanged += TextEditorGroupWrapOnStateChanged;
        TextEditorService.TextEditorStateChanged += TextEditorViewModelStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void TextEditorGroupWrapOnStateChanged() =>
        await InvokeAsync(StateHasChanged);

	private async void TextEditorViewModelStateWrapOnStateChanged()
	{
		var localTabListDisplay = _tabListDisplay;

		if (localTabListDisplay is not null)
        {
			await InvokeAsync(async () => await localTabListDisplay.NotifyStateChangedAsync())
                .ConfigureAwait(false);
        }
	}

	private List<ITab> GetTabList(TextEditorGroup textEditorGroup)
	{
        var textEditorState = TextEditorService.TextEditorState;
		var tabList = new List<ITab>();

		foreach (var viewModelKey in textEditorGroup.ViewModelKeyList)
		{
            var viewModel = textEditorState.ViewModelGetOrDefault(viewModelKey);
            
            if (viewModel is not null)
            {
                viewModel.DynamicViewModelAdapter.TabGroup = textEditorGroup;
				tabList.Add(viewModel.DynamicViewModelAdapter);
            }
		}

		return tabList;
	}

    public void Dispose()
    {
        TextEditorService.GroupApi.TextEditorGroupStateChanged -= TextEditorGroupWrapOnStateChanged;
		TextEditorService.TextEditorStateChanged -= TextEditorViewModelStateWrapOnStateChanged;
    }
}