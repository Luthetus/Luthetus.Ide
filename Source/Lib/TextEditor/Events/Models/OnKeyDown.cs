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
	    Key<TextEditorViewModel> viewModelKey)
    {
        ComponentData = componentData;
		KeymapArgs = keymapArgs;
        ViewModelKey = viewModelKey;
    }

	public TextEditorComponentData ComponentData { get; set; }
    public KeymapArgs KeymapArgs { get; set; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	// return ComponentData.Options.Keymap.HandleEvent(this);
    	return ValueTask.CompletedTask;
    }
}
