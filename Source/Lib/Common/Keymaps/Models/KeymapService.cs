using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class KeymapService : IKeymapService
{
    private KeymapState _keymapState = new();
    
    public event Action? KeymapStateChanged;
    
    public KeymapState GetKeymapState() => _keymapState;
    
    public void ReduceRegisterKeymapLayerAction(KeymapLayer keymapLayer)
    {
    	var inState = GetKeymapState();
    
        if (inState.KeymapLayerList.Any(x => x.Key == keymapLayer.Key))
        {
            KeymapStateChanged?.Invoke();
            return;
        }

		var outKeymapLayerList = new List<KeymapLayer>(inState.KeymapLayerList);
		outKeymapLayerList.Add(keymapLayer);

        _keymapState = inState with
        {
            KeymapLayerList = outKeymapLayerList
        };
        
        KeymapStateChanged?.Invoke();
        return;
    }
    
    public void ReduceDisposeKeymapLayerAction(Key<KeymapLayer> keymapLayerKey)
    {
    	var inState = GetKeymapState();
    
        var indexExisting = inState.KeymapLayerList.FindIndex(x => x.Key == keymapLayerKey);

        if (indexExisting == -1)
        {
            KeymapStateChanged?.Invoke();
       	 return;
        }

		var outKeymapLayerList = new List<KeymapLayer>(inState.KeymapLayerList);
		outKeymapLayerList.RemoveAt(indexExisting);

        _keymapState = inState with
        {
            KeymapLayerList = outKeymapLayerList
        };
        
        KeymapStateChanged?.Invoke();
        return;
    }
}
