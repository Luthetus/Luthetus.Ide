using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Dropdowns.States;

/// <summary>
/// <see cref="DropdownState"/>
/// </summary>
public class DropdownStateActionsTests
{
    /// <summary>
    /// <see cref="DropdownState.AddActiveAction"/>
    /// </summary>
    [Fact]
    public void AddActiveAction()
    {
        var key = Key<DropdownRecord>.NewKey();
        var addActiveAction = new DropdownState.AddActiveAction(key);
        
        Assert.Equal(key, addActiveAction.Key);
    }

    /// <summary>
    /// <see cref="DropdownState.RemoveActiveAction"/>
    /// </summary>
    [Fact]
    public void RemoveActiveAction()
    {
        var key = Key<DropdownRecord>.NewKey();
        var removeActiveAction = new DropdownState.RemoveActiveAction(key);

        Assert.Equal(key, removeActiveAction.Key);
    }

    /// <summary>
    /// <see cref="DropdownState.ClearActivesAction"/>
    /// </summary>
    [Fact]
    public void ClearActivesAction()
    {
        var clearActivesAction = new DropdownState.ClearActivesAction();
        Assert.NotNull(clearActivesAction);
    }
}