using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

public class WebsiteServerTreeView : TreeViewWithType<WebsiteServer>
{
    public WebsiteServerTreeView(
            WebsiteServer item,
            bool isExpandable,
            bool isExpanded)
        : base(item, isExpandable, isExpanded)
    {
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    public override Task LoadChildBagAsync()
    {
        var rootRoute = Item.Routes.Single(x => x.Split('/').Length == 2);

        var routeWrapperBag = new List<RouteWrapper>();

        var previousChildren = ChildBag;

        ChildBag = new()
        {
            new RouteTreeView(
                new RouteWrapper(
                rootRoute,
                Item.WebsiteServerState,
                Item.Name),
                true,
                false)
        };

        LinkChildren(previousChildren, ChildBag);

        return Task.CompletedTask;
    }
}
