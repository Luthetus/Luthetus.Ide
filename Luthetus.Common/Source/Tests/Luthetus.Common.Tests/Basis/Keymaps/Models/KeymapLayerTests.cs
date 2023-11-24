using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Keymaps.Models;

/// <summary>
/// <see cref="KeymapLayer"/>
/// </summary>
public class KeymapLayerTests
{
    /// <summary>
    /// <see cref="KeymapLayer(Key{KeymapLayer}, string, string)
    /// </summary>
    [Fact]
    public void ConstructorA()
    {
        var key = Key<KeymapLayer>.NewKey();
        var displayName = "DisplayName";
        var internalName = "InternalName";

        var keymapLayer = new KeymapLayer(
            key,
            displayName,
            internalName);

        Assert.Equal(key, keymapLayer.Key);
        Assert.Equal(displayName, keymapLayer.DisplayName);
        Assert.Equal(internalName, keymapLayer.InternalName);
    }
    
    /// <summary>
    /// <see cref="KeymapLayer()
    /// </summary>
    [Fact]
    public void ConstructorB()
    {
        var keymapLayer = new KeymapLayer();

        Assert.Equal(Key<KeymapLayer>.Empty, keymapLayer.Key);
        Assert.Equal(string.Empty, keymapLayer.DisplayName);
        Assert.Equal(string.Empty, keymapLayer.InternalName);
    }
}