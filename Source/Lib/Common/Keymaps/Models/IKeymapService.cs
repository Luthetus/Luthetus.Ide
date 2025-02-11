using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public interface IKeymapService
{
	public event Action? KeymapStateChanged;
    
    public KeymapState GetKeymapState();
    
    public void ReduceRegisterKeymapLayerAction(KeymapLayer keymapLayer);
    public void ReduceDisposeKeymapLayerAction(Key<KeymapLayer> keymapLayerKey);
}
