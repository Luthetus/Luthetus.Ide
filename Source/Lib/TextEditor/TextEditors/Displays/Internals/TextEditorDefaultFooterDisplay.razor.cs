using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDefaultFooterDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;

	private static readonly Throttle _throttleRender = new(TimeSpan.FromMilliseconds(250));

	private FooterDriver _footerDriver;
	
	protected override void OnInitialized()
    {
    	_footerDriver = new FooterDriver(TextEditorViewModelDisplay);
    
        TextEditorViewModelDisplay.RenderBatchChanged += OnRenderBatchChanged;
        OnRenderBatchChanged();
        
        base.OnInitialized();
    }

	private void OnRenderBatchChanged()
    {
    	_throttleRender.Run(async _ =>
    	{
    		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    	});
    }

	public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    }
}