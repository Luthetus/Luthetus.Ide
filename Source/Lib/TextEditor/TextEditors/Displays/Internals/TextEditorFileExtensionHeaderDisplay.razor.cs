using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorFileExtensionHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private ITextEditorHeaderRegistry TextEditorHeaderRegistry { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	private Dictionary<string, object?> _componentInnerParameters = null!;
	
	private string _fileExtensionCurrent = string.Empty;
	
	protected override void OnInitialized()
	{
		_componentInnerParameters = new()
		{
			{
				nameof(ITextEditorDependentComponent.TextEditorViewModelDisplay),
				TextEditorViewModelDisplay
			}
		};
		
        TextEditorViewModelDisplay.TextEditorService.ViewModelApi.CursorShouldBlinkChanged += OnCursorShouldBlinkChanged;
        OnCursorShouldBlinkChanged();
	}
	
	private void OnCursorShouldBlinkChanged()
    {
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	
    	var fileExtensionLocal = renderBatch is null
    		? string.Empty
    		: renderBatch.Model.FileExtension;

    	if (fileExtensionLocal != _fileExtensionCurrent)
    	{
    		_fileExtensionCurrent = fileExtensionLocal;
    		// Don't await this;
    		InvokeAsync(StateHasChanged);
    	}
    }

	public void Dispose()
    {
    	TextEditorViewModelDisplay.TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= OnCursorShouldBlinkChanged;
    }
}