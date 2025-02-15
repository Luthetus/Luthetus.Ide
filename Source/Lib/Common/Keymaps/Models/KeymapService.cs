using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class KeymapService : IKeymapService
{
    private readonly object _stateModificationLock = new();

    private KeymapState _keymapState = new();
    
    public event Action? KeymapStateChanged;
    
    public KeymapState GetKeymapState() => _keymapState;
    
    public void RegisterKeymapLayer(KeymapLayer keymapLayer)
    {
        lock (_stateModificationLock)
        {
            var inState = GetKeymapState();

            if (inState.KeymapLayerList.Any(x => x.Key == keymapLayer.Key))
                goto finalize;

            var outKeymapLayerList = new List<KeymapLayer>(inState.KeymapLayerList);
            outKeymapLayerList.Add(keymapLayer);

            _keymapState = inState with
            {
                KeymapLayerList = outKeymapLayerList
            };

            goto finalize;
        }

        finalize:
        KeymapStateChanged?.Invoke();
    }
    
    public void DisposeKeymapLayer(Key<KeymapLayer> keymapLayerKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetKeymapState();

            var indexExisting = inState.KeymapLayerList.FindIndex(x => x.Key == keymapLayerKey);

            if (indexExisting == -1)
                goto finalize;

            var outKeymapLayerList = new List<KeymapLayer>(inState.KeymapLayerList);
            outKeymapLayerList.RemoveAt(indexExisting);

            _keymapState = inState with
            {
                KeymapLayerList = outKeymapLayerList
            };

            goto finalize;
        }

        finalize:
        KeymapStateChanged?.Invoke();
    }
}
