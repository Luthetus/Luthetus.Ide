using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public partial class TextEditorViewModelAutoDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    
    [Parameter]
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; } = new();

	private TextEditorViewModelDisplay? _viewModelDisplay;
	private TextEditorViewModelDisplay? _viewModelDisplayPrevious;
	private bool _isLoaded;

	public Dictionary<string, object?> DependentComponentParameters { get; set; } = new Dictionary<string, object?>
	{
		{
			nameof(TextEditorViewModelDisplay),
			null
		}
	};
	
	protected override void OnAfterRender(bool firstRender)
    {
    	Console.WriteLine("OnAfterRender TextEditorViewModelAutoDisplay");
    
    	if (firstRender)
    	{
    		_isLoaded = true;
    		
    		TextEditorService.WorkerArbitrary.PostUnique(nameof(TextEditorViewModelAutoDisplay), async editContext =>
			{
				await InvokeAsync(StateHasChanged);
			});
    	}
    	
    	base.OnAfterRender(firstRender);
    }
	
	/*protected override async Task OnAfterRenderAsync(bool firstRender)
    {
    	if (firstRender)
    	{
    		_isLoaded = true;
    		await InvokeAsync(StateHasChanged);
    	}
    	
    	base.OnAfterRenderAsync(firstRender);
    }*/
}