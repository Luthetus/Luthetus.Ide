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

    public override Task LoadChildListAsync()
    {
        var rootRoute = Item.Routes.Single(x => x.Split('/').Length == 2);

        var routeWrapperList = new List<RouteWrapper>();

        var previousChildren = ChildList;

        ChildList = new()
        {
            new RouteTreeView(
                new RouteWrapper(
                rootRoute,
                Item.WebsiteServerState,
                Item.Name),
                true,
                false)
        };

        LinkChildren(previousChildren, ChildList);

        return Task.CompletedTask;
    }
}
