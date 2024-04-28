using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Keymaps.Models;

/// <summary>
/// <see cref="KeymapArgument"/>
/// </summary>
public class KeymapArgumentTests
{
    /// <summary>
    /// <see cref="KeymapArgument(string, bool, bool, bool, Key{KeymapLayer})"/>
    /// </summary>
    [Fact]
    public void ConstructorA()
    {
        // No modifiers (shift key, etc...)
        {
            var code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE;
            var shiftKey = false;
            var ctrlKey = false;
            var altKey = false;
            var keymapLayerKey = Key<KeymapLayer>.Empty;

            var keymapArgument = new KeymapArgument(
                code,
                shiftKey,
                ctrlKey,
                altKey,
                keymapLayerKey);

            Assert.Equal(code, keymapArgument.Code);
            Assert.Equal(shiftKey, keymapArgument.ShiftKey);
            Assert.Equal(ctrlKey, keymapArgument.CtrlKey);
            Assert.Equal(altKey, keymapArgument.AltKey);
            Assert.Equal(keymapLayerKey, keymapArgument.LayerKey);
        }

        // With modifiers (shift key, etc...)
        {
            var code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE;
            var shiftKey = true;
            var ctrlKey = true;
            var altKey = true;
            var keymapLayerKey = Key<KeymapLayer>.Empty;

            var keymapArgument = new KeymapArgument(
                code,
                shiftKey,
                ctrlKey,
                altKey,
                keymapLayerKey);

            Assert.Equal(code, keymapArgument.Code);
            Assert.Equal(shiftKey, keymapArgument.ShiftKey);
            Assert.Equal(ctrlKey, keymapArgument.CtrlKey);
            Assert.Equal(altKey, keymapArgument.AltKey);
            Assert.Equal(keymapLayerKey, keymapArgument.LayerKey);
        }
    }
    
    /// <summary>
    /// <see cref="KeymapArgument(string)"/>
    /// </summary>
    [Fact]
    public void ConstructorB()
    {
        var code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE;

        var keymapArgument = new KeymapArgument(code);

        Assert.Equal(code, keymapArgument.Code);
        Assert.False(keymapArgument.ShiftKey);
        Assert.False(keymapArgument.CtrlKey);
        Assert.False(keymapArgument.AltKey);
        Assert.Equal(Key<KeymapLayer>.Empty, keymapArgument.LayerKey);
    }
}
