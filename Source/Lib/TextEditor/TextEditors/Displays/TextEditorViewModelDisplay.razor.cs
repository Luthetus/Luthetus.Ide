using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public partial class TextEditorViewModelDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();

	private TextEditorViewModelSlimDisplay? _viewModelSlimDisplay;
	private TextEditorViewModelSlimDisplay? _viewModelSlimDisplayPrevious;
	private bool _isLoaded;
	
	private static string DictionaryKey => nameof(TextEditorViewModelSlimDisplay);

	public Dictionary<string, object?> DependentComponentParameters { get; set; } = new Dictionary<string, object?>
	{
		{
			DictionaryKey,
			null
		}
	};
	
	protected override void OnAfterRender(bool firstRender)
    {
    	if (firstRender)
    	{
    		_isLoaded = true;
    		
    		TextEditorService.WorkerArbitrary.PostUnique(nameof(TextEditorViewModelDisplay), async editContext =>
			{
				await InvokeAsync(StateHasChanged);
			});
    	}
    	
    	base.OnAfterRender(firstRender);
    }
}