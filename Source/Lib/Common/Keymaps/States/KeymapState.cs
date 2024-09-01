using Fluxor;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Keymaps.States;

/// <summary>
/// Use this state to lookup a <see cref="KeymapLayer"> to determine the 'when' clause of the keybind.
/// If a <see cref="KeymapLayer"> is used, but isn't registered in this state, it will still function properly
/// but the 'when' clause cannot be shown when the user inspects the keybind in the keymap.
/// </summary>
[FeatureState]
public record KeymapState(ImmutableList<KeymapLayer> KeymapLayerList)
{
    public KeymapState() : this(ImmutableList<KeymapLayer>.Empty)
    {
    }

    public record struct RegisterKeymapLayerAction(KeymapLayer KeymapLayer);
    public record struct DisposeKeymapLayerAction(Key<KeymapLayer> KeymapLayerKey);

    public class Reducer
    {
        [ReducerMethod]
        public static KeymapState ReduceRegisterKeymapLayerAction(
            KeymapState inState,
            RegisterKeymapLayerAction registerKeymapLayerAction)
        {
            if (inState.KeymapLayerList.Any(x => x.Key == registerKeymapLayerAction.KeymapLayer.Key))
                return inState;

            return inState with
            {
                KeymapLayerList = inState.KeymapLayerList.Add(registerKeymapLayerAction.KeymapLayer)
            };
        }
        
        [ReducerMethod]
        public static KeymapState ReduceDisposeKeymapLayerAction(
            KeymapState inState,
            DisposeKeymapLayerAction disposeKeymapLayerAction)
        {
            var indexExisting = inState.KeymapLayerList.FindIndex(x => x.Key == disposeKeymapLayerAction.KeymapLayerKey);

            if (indexExisting == -1)
                return inState;

            return inState with
            {
                KeymapLayerList = inState.KeymapLayerList.RemoveAt(indexExisting)
            };
        }
    }
}
