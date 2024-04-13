using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

public class RouteTreeView : TreeViewWithType<RouteWrapper>
{
    public RouteTreeView(
            RouteWrapper routeWrapper,
            bool isExpandable,
            bool isExpanded)
        : base(routeWrapper, isExpandable, isExpanded)
    {
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    public override Task LoadChildListAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildList);

        var websiteServer = Item.WebsiteServerState.WebsiteServerMap[Item.WebsiteServerName];

        var childRouteList = websiteServer.Routes.Where(x => x.StartsWith(Item.Name));
        var routeWrapperList = new List<RouteWrapper>();

        foreach (var childRoute in childRouteList)
        {
            routeWrapperList.Add(new RouteWrapper(
                Item.Name,
                Item.WebsiteServerState,
                Item.WebsiteServerName));
        }

        LinkChildren(previousChildren, ChildList);

        return Task.CompletedTask;
    }
}
