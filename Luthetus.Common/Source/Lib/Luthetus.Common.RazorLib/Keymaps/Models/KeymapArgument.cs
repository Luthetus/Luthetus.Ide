using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public record KeymapArgument(
    string Code,
    bool ShiftKey,
    bool CtrlKey,
    bool AltKey,
    Key<KeymapLayer> LayerKey)
{
    public KeymapArgument(string code)
        : this(code, false, false, false, Key<KeymapLayer>.Empty)
    {

    }

	public KeyboardEventArgs ToKeyboardEventArgs()
	{
		var key = Code.Replace("Key", string.Empty);

		// The event-code looks like: 'KeyA', 'KeyB', ... 'KeyZ'
		if (!ShiftKey)
			key = key.ToLower();

		return new KeyboardEventArgs()
		{
			Code = Code,
			Key = key,
			ShiftKey = ShiftKey,
			CtrlKey = CtrlKey,
			AltKey = AltKey,
		};
	}
}
