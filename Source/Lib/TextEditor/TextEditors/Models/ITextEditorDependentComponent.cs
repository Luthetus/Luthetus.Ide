using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorDependentComponent : IDisposable
{
	/// <summary>
	/// When implementing this type, add the Blazor attribute '[Parameter]',
	/// and the text editor will provide its instance to the dependent component via this Blazor parameter.
	///
	/// The only way for a dependent component to re-render
	/// is via the event: <see cref="TextEditorViewModelSlimDisplay.RenderBatchChanged"/>
	/// (i.e.: the 'StateHasChanged()' for the text editor will not cascade to the dependent component).
	///
	/// One can subscribe to this event and throttle re-rendering as desired.
	/// This allows the accessory UI that surrounds the text editor to be rendered
	/// at a "lower priority" than the text editor itself.
	/// </summary>
	public Key<TextEditorComponentData> ComponentDataKey { get; set; }
}
