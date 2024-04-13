using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Keys.Models;

/// <summary>
/// <see cref="Key{T}"/>
/// </summary>
public class KeyTests
{
    /// <summary>
    /// <see cref="Key{T}.Empty"/>
    /// </summary>
    [Fact]
    public void Empty()
    {
        var keyIntEmpty = Key<int>.Empty;
        Assert.Equal(Guid.Empty, keyIntEmpty.Guid);

        var keyStringEmpty = Key<string>.Empty;
        Assert.Equal(Guid.Empty, keyStringEmpty.Guid);

        Assert.False(keyIntEmpty.Equals(keyStringEmpty));
        // Same assertion but reversed
        Assert.False(keyStringEmpty.Equals(keyIntEmpty));
    }

    /// <summary>
    /// <see cref="Key{T}.NewKey()"/>
    /// </summary>
    [Fact]
    public void NewKey()
    {
        var keyInt = Key<int>.NewKey();
        Assert.NotEqual(Guid.Empty, keyInt.Guid);

        var keyString = Key<string>.NewKey();
        Assert.NotEqual(Guid.Empty, keyString.Guid);

        Assert.False(keyInt.Equals(keyString));
        // Same assertion but reversed
        Assert.False(keyString.Equals(keyInt));
    }
}