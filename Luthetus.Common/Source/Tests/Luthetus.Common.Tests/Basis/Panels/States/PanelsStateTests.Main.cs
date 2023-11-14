using Luthetus.Common.RazorLib.Panels.States;

namespace Luthetus.Common.Tests.Basis.Panels.States;

/// <summary>
/// <see cref="PanelsState"/>
/// </summary>
public class PanelsStateMainTests
{
    /// <summary>
    /// <see cref="PanelsState()"/>
    /// <br/>----<br/>
    /// <see cref="PanelsState.DragEventArgs"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var panelState = new PanelsState();

        Assert.Equal(3, panelState.PanelGroupBag.Length);
        Assert.Null(panelState.DragEventArgs);
    }
}