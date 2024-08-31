using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Keymaps.Models;

/// <summary>
/// <see cref="Keymap"/>
/// </summary>
public class KeymapTests
{
    /// <summary>
    /// <see cref="Keymap(Key{Keymap}, string)"/>
    /// <br/>----<br/>
    /// <see cref="Keymap.Key"/>
    /// <see cref="Keymap.DisplayName"/>
    /// </summary>
    [Fact]
    public void ConstructorA()
    {
        var key = Key<Keymap>.NewKey();
        var displayName = "displayName";

        var keymap = new Keymap(key, displayName);

        Assert.Equal(key, keymap.Key);
        Assert.Equal(displayName, keymap.DisplayName);
    }

    /// <summary>
    /// <see cref="Keymap.Empty"/>
    /// </summary>
    [Fact]
    public void Empty()
    {
        Assert.Equal(Key<Keymap>.Empty, IKeymap.Empty.Key);
        Assert.Equal(string.Empty, IKeymap.Empty.DisplayName);
    }

    /// <summary>
    /// <see cref="Keymap.Map"/>
    /// </summary>
    [Fact]
    public void Map()
    {
        var inCommand = new CommonCommand("Test Map", "test-map", false,
            commandArgs => Task.CompletedTask);

        var keymap = new Keymap(Key<Keymap>.NewKey(), "Unit Test");

        var keymapArgs = new KeymapArgs
        {
            Code = "KeyF",
            ShiftKey = false,
            CtrlKey = true,
            AltKey = true,
            MetaKey = false,
            LayerKey = Key<KeymapLayer>.Empty
        };
        
        _ = keymap.TryRegister(keymapArgs, inCommand);

        var success = keymap.TryMap(keymapArgs, out var outCommand);

        Assert.Equal(inCommand, outCommand);
        Assert.Equal(inCommand.DisplayName, outCommand.DisplayName);
        Assert.Equal(inCommand.InternalIdentifier, outCommand.InternalIdentifier);
        Assert.Equal(inCommand.ShouldBubble, outCommand.ShouldBubble);
        Assert.Equal(inCommand.CommandFunc, outCommand.CommandFunc);
    }
}
