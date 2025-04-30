using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.TextEditor.RazorLib.Diffs.Displays.Internals;

public partial class TextEditorDiffHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Parameter, EditorRequired]
	public Key<TextEditorComponentData> ComponentDataKey { get; set; }

	private static readonly Throttle _throttleRender = new(TimeSpan.FromMilliseconds(1_000));

	protected override void OnInitialized()
    {
        // TextEditorViewModelSlimDisplay.RenderBatchChanged += OnRenderBatchChanged;
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
    	// TextEditorViewModelSlimDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    }
}