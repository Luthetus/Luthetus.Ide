using Luthetus.Common.RazorLib.Keys.Models;

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
}
