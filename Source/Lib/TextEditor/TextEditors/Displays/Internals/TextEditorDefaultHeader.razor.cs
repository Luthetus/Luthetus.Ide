using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDefaultHeader : ComponentBase, IDisposable
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;

	private readonly Throttle _throttleRender = new(TimeSpan.FromMilliseconds(1_000));

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