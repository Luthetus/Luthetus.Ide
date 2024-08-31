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

            var keymapArgs = new KeymapArgs()
            {
                Code = code,
                ShiftKey = shiftKey,
                CtrlKey = ctrlKey,
                AltKey = altKey,
                LayerKey = keymapLayerKey
            };

            Assert.Equal(code, keymapArgs.Code);
            Assert.Equal(shiftKey, keymapArgs.ShiftKey);
            Assert.Equal(ctrlKey, keymapArgs.CtrlKey);
            Assert.Equal(altKey, keymapArgs.AltKey);
            Assert.Equal(keymapLayerKey, keymapArgs.LayerKey);
        }

        // With modifiers (shift key, etc...)
        {
            var code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE;
            var shiftKey = true;
            var ctrlKey = true;
            var altKey = true;
            var keymapLayerKey = Key<KeymapLayer>.Empty;

            var keymapArgs = new KeymapArgs()
            {
                Code = code,
                ShiftKey = shiftKey,
                CtrlKey = ctrlKey,
                AltKey = altKey,
                LayerKey = keymapLayerKey
            };

            Assert.Equal(code, keymapArgs.Code);
            Assert.Equal(shiftKey, keymapArgs.ShiftKey);
            Assert.Equal(ctrlKey, keymapArgs.CtrlKey);
            Assert.Equal(altKey, keymapArgs.AltKey);
            Assert.Equal(keymapLayerKey, keymapArgs.LayerKey);
        }
    }
    
    /// <summary>
    /// <see cref="KeymapArgument(string)"/>
    /// </summary>
    [Fact]
    public void ConstructorB()
    {
        var code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE;

        var keymapArgs = new KeymapArgs()
        {
            Code = code
        };

        Assert.Equal(code, keymapArgs.Code);
        Assert.False(keymapArgs.ShiftKey);
        Assert.False(keymapArgs.CtrlKey);
        Assert.False(keymapArgs.AltKey);
        Assert.Equal(Key<KeymapLayer>.Empty, keymapArgs.LayerKey);
    }
}
