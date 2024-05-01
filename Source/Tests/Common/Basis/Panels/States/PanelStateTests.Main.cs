using Luthetus.Common.RazorLib.Panels.States;

namespace Luthetus.Common.Tests.Basis.Panels.States;

/// <summary>
/// <see cref="PanelState"/>
/// </summary>
public class PanelStateMainTests
{
    /// <summary>
    /// <see cref="PanelState()"/>
    /// <br/>----<br/>
    /// <see cref="PanelState.DragEventArgs"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var panelState = new PanelState();

        Assert.Equal(3, panelState.PanelGroupList.Length);
        Assert.Null(panelState.DragEventArgs);
    }
}