using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/*
These IBackgroundTaskGroup "args" structs are a bit heavy at the moment.
This is better than how things were, I need to find another moment
to go through and lean these out.
*/
public struct CommonWorkArgs
{
	public string WriteToLocalStorage_Key { get; set; }
	public object WriteToLocalStorage_Value { get; set; }
    public Func<TabContextMenuEventArgs, Task> HandleTabButtonOnContextMenu { get; set; }
    public Func<MouseEventArgs?, Key<TreeViewContainer>, TreeViewNoType?, Task> HandleTreeViewOnContextMenu { get; set; }
    public Func<TreeViewCommandArgs, Task>? OnContextMenuFunc { get; set; }
    public MouseEventArgs MouseEventArgs { get; set; }
    public TabContextMenuEventArgs TabContextMenuEventArgs { get; set; }
	public TreeViewCommandArgs TreeViewContextMenuCommandArgs { get; set; }
    public Key<TreeViewContainer> ContainerKey { get; set; }
    public TreeViewContainer? TreeViewContainer { get; set; }
    public TreeViewNoType? TreeViewNoType { get; set; }
    public CommonWorkKind WorkKind { get; set; }
}
