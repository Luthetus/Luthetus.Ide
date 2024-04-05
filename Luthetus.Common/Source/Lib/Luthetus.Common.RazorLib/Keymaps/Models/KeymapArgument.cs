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
		return new KeyboardEventArgs()
		{
			Code = Code,
			Key = Code.Replace("Key", string.Empty),
			ShiftKey = ShiftKey,
			CtrlKey = CtrlKey,
			AltKey = AltKey,
		};
	}
}
