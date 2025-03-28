using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.Ide.RazorLib.Editors.Models;

namespace Luthetus.Ide.RazorLib.Editors.Displays;

public partial class EditorDisplay : ComponentBase, IDisposable
{
	[Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;

	private TabListDisplay? _tabListDisplay;

	private string? _htmlId = null;
	private string HtmlId => _htmlId ??= $"luth_te_group_{EditorIdeApi.EditorTextEditorGroupKey.Guid}";
	
	private bool _isLoaded = false;
	
	private TextEditorViewModelDisplay _viewModelDisplay;
	
	private Key<TextEditorViewModel> _previousActiveViewModelKey = Key<TextEditorViewModel>.Empty;

    protected override void OnInitialized()
    {
    	_viewModelDisplayOptions = new()
        {
            WrapperClassCssString = "luth_te_demo-text-editor",
            TabIndex = 0,
            HeaderButtonKinds = TextEditorHeaderButtonKindsList,
            HeaderComponentType = typeof(TextEditorFileExtensionHeaderDisplay),
        };
    
        TextEditorService.GroupApi.TextEditorGroupStateChanged += TextEditorGroupWrapOnStateChanged;

        base.OnInitialized();
    }
    
    protected override void OnAfterRender(bool firstRender)
    {
    	if (firstRender)
    	{
    		_isLoaded = true;
    	}
    	
    	base.OnAfterRender(firstRender);
    }

    private async void TextEditorGroupWrapOnStateChanged()
    {
    	var textEditorGroup = TextEditorService.GroupApi.GetTextEditorGroupState().GroupList.FirstOrDefault(
	        x => x.GroupKey == EditorIdeApi.EditorTextEditorGroupKey);
	        
	    if (_previousActiveViewModelKey != textEditorGroup.ActiveViewModelKey)
	    {
	    	_previousActiveViewModelKey = textEditorGroup.ActiveViewModelKey;
	    	TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
	    }
    
        await InvokeAsync(StateHasChanged);
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

    private static readonly List<HeaderButtonKind> TextEditorHeaderButtonKindsList =
        Enum.GetValues(typeof(HeaderButtonKind))
            .Cast<HeaderButtonKind>()
            .ToList();

    private ViewModelDisplayOptions _viewModelDisplayOptions = null!;

    public void Dispose()
    {
        TextEditorService.GroupApi.TextEditorGroupStateChanged -= TextEditorGroupWrapOnStateChanged;
    }
}