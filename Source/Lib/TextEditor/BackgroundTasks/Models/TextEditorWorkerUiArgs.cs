using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public struct TextEditorWorkerUiArgs
{
	// Only OnKeyDown invokes this constructor.
	public TextEditorWorkerUiArgs(
		TextEditorComponentData componentData,
		Key<TextEditorViewModel> viewModelKey,
		KeyboardEventArgs keyboardEventArgs)
	{
		ComponentData = componentData;
		ViewModelKey = viewModelKey;
		EventArgs = keyboardEventArgs;
	
		TextEditorWorkUiKind = TextEditorWorkUiKind.OnKeyDown;
	}
	
	// Can't distinguish the event kind without accepting it as argument.
	public TextEditorWorkerUiArgs(
		TextEditorComponentData componentData,
		Key<TextEditorViewModel> viewModelKey,
		MouseEventArgs mouseEventArgs,
		TextEditorWorkUiKind workUiKind)
	{
		ComponentData = componentData;
		ViewModelKey = viewModelKey;
		EventArgs = mouseEventArgs;
	
		TextEditorWorkUiKind = workUiKind;
	}
	
	// Can't distinguish the event kind without accepting it as argument.
	public TextEditorWorkerUiArgs(
		TextEditorComponentData componentData,
		Key<TextEditorViewModel> viewModelKey,
		WheelEventArgs wheelEventArgs)
	{
		ComponentData = componentData;
		ViewModelKey = viewModelKey;
		EventArgs = wheelEventArgs;
		
		TextEditorWorkUiKind = TextEditorWorkUiKind.OnWheel;
	}

	public TextEditorComponentData ComponentData { get; set; }
    public Key<TextEditorViewModel> ViewModelKey { get; set; }
    /// <summary>
    /// Hack: I want all the events in a shared queue. All events other than scrolling events...
	/// ...can be stored in the same property as an 'object' type.
	///
	/// Scrolling is a pain since it would mean copying around a double at all times
	/// that is only used for the scrolling events.
	///
	/// Thus:
	///     - MouseEventArgs.ClientX will be used to store the scrollLeft.
	///     - MouseEventArgs.ClientY will be used to store the scrollTop.
    /// </summary>
	public object EventArgs { get; set; }
    public TextEditorWorkUiKind TextEditorWorkUiKind { get; set; }
}
