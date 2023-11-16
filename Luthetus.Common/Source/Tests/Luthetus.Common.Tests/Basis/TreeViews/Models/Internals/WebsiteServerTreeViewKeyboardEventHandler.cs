using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

public class WebsiteServerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    public WebsiteServerTreeViewKeyboardEventHandler(ITreeViewService treeViewService)
        : base(treeViewService)
    {
    }

    public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {

        return base.OnKeyDownAsync(commandArgs);
    }
}
