using Luthetus.Common.RazorLib.Drags.Displays;

namespace Luthetus.Common.Tests.Basis.Drags.States;

/// <summary>
/// <see cref="DragState"/>
/// </summary>
public class DragStateMainTests
{
    /// <summary>
    /// <see cref="DragState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var dragState = new DragState();

        Assert.False(dragState.ShouldDisplay);
        Assert.Null(dragState.MouseEventArgs);
    }
}