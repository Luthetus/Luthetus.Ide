using Luthetus.Common.RazorLib.Dynamics.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public class TabContextMenuEventArgs
{
    public TabContextMenuEventArgs(MouseEventArgs mouseEventArgs, ITab tab, Func<Task> restoreFocusFunc)
    {
        MouseEventArgs = mouseEventArgs;
        Tab = tab;
        RestoreFocusFunc = restoreFocusFunc;
    }

    public MouseEventArgs MouseEventArgs { get; }
    public ITab Tab { get; }

    /// <summary>
	/// An async Func that will return focus to the tab button once the context menu is
    /// eventually closed.
	/// </summary>
    public Func<Task> RestoreFocusFunc { get; }
}
