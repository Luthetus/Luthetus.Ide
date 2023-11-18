namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

public class RouteWrapper
{
    public RouteWrapper(
        string name,
        WebsiteServerState websiteServerState,
        string websiteServerName)
    {
        Name = name;
        WebsiteServerState = websiteServerState;
        WebsiteServerName = websiteServerName;
    }

    public string Name { get; set; }
    public WebsiteServerState WebsiteServerState { get; set; }
    public string WebsiteServerName { get; set; }
}
