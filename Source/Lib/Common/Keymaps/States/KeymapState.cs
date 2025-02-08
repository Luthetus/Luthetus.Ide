using Fluxor;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Keymaps.States;

/// <summary>
/// The list provided should not be modified after passing it as a parameter..
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
///
/// ---
///
/// Use this state to lookup a <see cref="KeymapLayer"> to determine the 'when' clause of the keybind.
/// If a <see cref="KeymapLayer"> is used, but isn't registered in this state, it will still function properly
/// but the 'when' clause cannot be shown when the user inspects the keybind in the keymap.
/// </summary>
[FeatureState]
public record KeymapState(List<KeymapLayer> KeymapLayerList)
{
    public KeymapState() : this(new List<KeymapLayer>())
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

			var outKeymapLayerList = new List<KeymapLayer>(inState.KeymapLayerList);
			outKeymapLayerList.Add(registerKeymapLayerAction.KeymapLayer);

            return inState with
            {
                KeymapLayerList = outKeymapLayerList
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

			var outKeymapLayerList = new List<KeymapLayer>(inState.KeymapLayerList);
			outKeymapLayerList.RemoveAt(indexExisting);

            return inState with
            {
                KeymapLayerList = outKeymapLayerList
            };
        }
    }
}
