using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public record struct KeymapArgument(
    string Key,
    string Code,
    bool ShiftKey,
    bool CtrlKey,
    bool AltKey,
    bool MetaKey,
    Key<KeymapLayer> LayerKey)
{
    public KeymapArgument(string key, string code)
        : this(
        	key,
        	code,
        	ShiftKey: false,
        	CtrlKey: false,
        	AltKey: false,
        	MetaKey: false,
        	Key<KeymapLayer>.Empty)
    {

    }
    
    public KeymapArgument(string key, string code, Key<KeymapLayer> layerKey)
        : this(
        	key,
        	code,
        	ShiftKey: false,
        	CtrlKey: false,
        	AltKey: false,
        	MetaKey: false,
        	layerKey)
    {

    }
}
