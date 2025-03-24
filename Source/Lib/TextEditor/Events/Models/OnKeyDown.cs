using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnKeyDown
{
	public OnKeyDown(
		TextEditorComponentData componentData,
	    KeymapArgs keymapArgs,
	    ResourceUri resourceUri,
	    Key<TextEditorViewModel> viewModelKey)
    {
        ComponentData = componentData;

		KeymapArgs = keymapArgs;

        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public KeymapArgs KeymapArgs { get; set; }
	public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorComponentData ComponentData { get; set; }

	/// <summary>
	/// CONFUSING: '0 == 0' used to be 'BatchLength == 0'. The batching code is being removed but is a bit of a mess at the moment.
	/// </summary>
    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	return ComponentData.Options.Keymap.HandleEvent(this);	
    }

    private bool KeyAndModifiersAreEqual(KeymapArgs x, KeymapArgs y)
    {
        return
            x.Key == y.Key &&
            x.ShiftKey == y.ShiftKey &&
            x.CtrlKey == y.CtrlKey &&
            x.AltKey == y.AltKey;
    }
}
