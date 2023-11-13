using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Tabs.States;

/// <summary>
/// <see cref="TabState"/>
/// </summary>
public class TabStateMainTests
{
    /// <summary>
    /// <see cref="TabState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var tabState = new TabState();

        Assert.Equal(ImmutableList<TabGroup>.Empty, tabState.TabGroupBag);
    }
}
