using Luthetus.Common.RazorLib.Drags.Displays;

namespace Luthetus.Common.Tests.Basis.Drags.States;

/// <summary>
/// <see cref="DragState"/>
/// </summary>
public class DragStateActionsTests
{
    /// <summary>
    /// <see cref="DragState.WithAction"/>
    /// </summary>
    [Fact]
    public void WithAction()
    {
        var withAction = new DragState.WithAction(x => x with
        {
            ShouldDisplay = true,
            MouseEventArgs = new(),
        });

        Assert.NotNull(withAction.WithFunc);
    }
}