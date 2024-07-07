using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Dropdowns.States;

/// <summary>
/// <see cref="DropdownState"/>
/// </summary>
public class DropdownStateMainTests
{
    /// <summary>
    /// <see cref="DropdownState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var dropdownState = new DropdownState();

        Assert.Equal(ImmutableList<DropdownRecord>.Empty, dropdownState.DropdownList);
    }
}