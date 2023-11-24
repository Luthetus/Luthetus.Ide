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
    /// <see cref="Keymap()"/>
    /// <br/>----<br/>
    /// <see cref="Keymap.Key"/>
    /// <see cref="Keymap.DisplayName"/>
    /// </summary>
    [Fact]
    public void ConstructorB()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var keymap = new Keymap();
#pragma warning restore CS0618 // Type or member is obsolete

        Assert.Equal(Key<Keymap>.Empty, keymap.Key);
        Assert.Equal(string.Empty, keymap.DisplayName);
    }

    /// <summary>
    /// <see cref="Keymap.Empty"/>
    /// </summary>
    [Fact]
    public void Empty()
    {
        Assert.Equal(Key<Keymap>.Empty, Keymap.Empty.Key);
        Assert.Equal(string.Empty, Keymap.Empty.DisplayName);
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

        var keymapArgument = new KeymapArgument("KeyF", false, true, true, Key<KeymapLayer>.Empty);
        
        _ = keymap.Map.TryAdd(keymapArgument, inCommand);

        var outCommand = keymap.Map[keymapArgument];

        Assert.Equal(inCommand, outCommand);
        Assert.Equal(inCommand.DisplayName, outCommand.DisplayName);
        Assert.Equal(inCommand.InternalIdentifier, outCommand.InternalIdentifier);
        Assert.Equal(inCommand.ShouldBubble, outCommand.ShouldBubble);
        Assert.Equal(inCommand.DoAsyncFunc, outCommand.DoAsyncFunc);
    }
}
