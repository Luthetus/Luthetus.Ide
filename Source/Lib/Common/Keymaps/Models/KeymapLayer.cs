using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public record KeymapLayer(
    Key<KeymapLayer> Key,
    string DisplayName,
    string InternalName)
{
    public KeymapLayer()
        : this(Key<KeymapLayer>.Empty, string.Empty, string.Empty)
    {

    }
}