using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDefaultHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;

	private static readonly Throttle _throttleRender = new(TimeSpan.FromMilliseconds(333));

	private HeaderDriver _headerDriver;

	protected override void OnInitialized()
    {
    	_headerDriver = new HeaderDriver(TextEditorViewModelDisplay);
    
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