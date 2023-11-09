using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.Tests.Basis.Keymaps.Models;

/// <summary>
/// <see cref="KeyboardEventArgsExtensions"/>
/// </summary>
public class KeyboardEventArgsExtensionsTests
{
    /// <summary>
    /// <see cref="KeyboardEventArgsExtensions.ToKeymapArgument(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs)"/>
    /// </summary>
    [Fact]
    public void ToKeymapArgument()
    {
        // No modifiers (shift key, etc...)
        {
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Code = KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
            };

            var keymapArgument = keyboardEventArgs.ToKeymapArgument();

            Assert.Equal(keyboardEventArgs.Code, keymapArgument.Code);
            Assert.Equal(keyboardEventArgs.ShiftKey, keymapArgument.ShiftKey);
            Assert.Equal(keyboardEventArgs.CtrlKey, keymapArgument.CtrlKey);
            Assert.Equal(keyboardEventArgs.AltKey, keymapArgument.AltKey);
            Assert.Equal(Key<KeymapLayer>.Empty, keymapArgument.LayerKey);
        }

        // With modifiers (shift key, etc...)
        {
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Code = KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                ShiftKey = true,
                CtrlKey = true,
                AltKey = true,
            };

            var keymapArgument = keyboardEventArgs.ToKeymapArgument();

            Assert.Equal(keyboardEventArgs.Code, keymapArgument.Code);
            Assert.Equal(keyboardEventArgs.ShiftKey, keymapArgument.ShiftKey);
            Assert.Equal(keyboardEventArgs.CtrlKey, keymapArgument.CtrlKey);
            Assert.Equal(keyboardEventArgs.AltKey, keymapArgument.AltKey);
            Assert.Equal(Key<KeymapLayer>.Empty, keymapArgument.LayerKey);
        }
    }
}