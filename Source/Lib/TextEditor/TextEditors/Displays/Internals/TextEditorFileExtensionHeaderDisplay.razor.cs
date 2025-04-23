using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorFileExtensionHeaderDisplay : ComponentBase
{
	[Inject]
	private ITextEditorHeaderRegistry TextEditorHeaderRegistry { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; }
	[Parameter, EditorRequired]
	public TextEditorViewModelSlimDisplay TextEditorViewModelSlimDisplay { get; set; } = null!;
	
	private Dictionary<string, object?> _componentInnerParameters = null!;
	
	private string _fileExtensionCurrent = string.Empty;
	
	private TextEditorViewModelSlimDisplay _previousTextEditorViewModelSlimDisplay;
	
	private string DictionaryKey => nameof(ITextEditorDependentComponent.TextEditorViewModelSlimDisplay);
	
	protected override void OnInitialized()
	{
		_componentInnerParameters = new()
		{
			{
				DictionaryKey,
				TextEditorViewModelSlimDisplay
			}
		};

        // ShouldRender does not invoke on the initial render.
        _ = ShouldRender();
	}
	
	protected override bool ShouldRender()
	{
		if (_previousTextEditorViewModelSlimDisplay != TextEditorViewModelSlimDisplay)
		{
			_previousTextEditorViewModelSlimDisplay = TextEditorViewModelSlimDisplay;
			_componentInnerParameters[DictionaryKey] = TextEditorViewModelSlimDisplay;
		}
	
		var localTextEditorState = TextEditorService.TextEditorState;
		
		var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
			TextEditorViewModelKey);
    	
    	var fileExtensionLocal = model_viewmodel_tuple.Model is null
    		? string.Empty
    		: model_viewmodel_tuple.Model.FileExtension;
    		
    	if (_fileExtensionCurrent != fileExtensionLocal)
    		_fileExtensionCurrent = fileExtensionLocal;
    		
		return true;
	}
}