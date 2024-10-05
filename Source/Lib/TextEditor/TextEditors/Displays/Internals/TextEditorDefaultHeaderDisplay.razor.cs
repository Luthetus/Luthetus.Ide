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

	private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMilliseconds(500);

	/// <summary>byte is used as TArgs just as a "throwaway" type. It isn't used.</summary>
	private ThrottleOptimized<byte> _throttleRender;

	private HeaderDriver _headerDriver;

	protected override void OnInitialized()
    {
    	_headerDriver = new HeaderDriver(TextEditorViewModelDisplay);
    	
    	_throttleRender = new(ThrottleTimeSpan, async (_, _) =>
    	{
    		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    	});
    
        TextEditorViewModelDisplay.RenderBatchChanged += OnRenderBatchChanged;
        OnRenderBatchChanged();
        
        base.OnInitialized();
    }

	private void OnRenderBatchChanged()
    {
    	_throttleRender.Run(0);
    }

	public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    }
}