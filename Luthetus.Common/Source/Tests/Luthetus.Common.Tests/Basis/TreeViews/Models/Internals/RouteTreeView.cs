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

    public override Task LoadChildBagAsync()
    {
        var websiteServer = Item.WebsiteServerState.WebsiteServerMap[Item.WebsiteServerName];

        var childRouteBag = websiteServer.Routes.Where(x => x.StartsWith(Item.Name));
        var routeWrapperBag = new List<RouteWrapper>();

        foreach (var childRoute in childRouteBag)
        {
            routeWrapperBag.Add(new RouteWrapper(
                Item.Name,
                Item.WebsiteServerState,
                Item.WebsiteServerName));
        }

        return Task.CompletedTask;
    }
}
