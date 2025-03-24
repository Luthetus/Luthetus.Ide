using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDefaultFooterDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;

	private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMilliseconds(200);

	/// <summary>byte is used as TArgs just as a "throwaway" type. It isn't used.</summary>
	// private ThrottleOptimized<byte> _throttleRender;

	private FooterDriver _footerDriver;
	
	protected override void OnInitialized()
    {
    	_footerDriver = new FooterDriver(TextEditorViewModelDisplay);
    	
    	/*_throttleRender = new(ThrottleTimeSpan, async (_, _) =>
    	{
    		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    	});*/
    
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += OnCursorShouldBlinkChanged;
        OnCursorShouldBlinkChanged();
        
        base.OnInitialized();
    }

    private void OnCursorShouldBlinkChanged()
    {
    	// Don't await this;
    	InvokeAsync(StateHasChanged);
    }

	public void Dispose()
    {
    	TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= OnCursorShouldBlinkChanged;
    }
}