using Luthetus.Common.RazorLib.TreeViews.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

/// <summary>
/// Goal of <see cref="WebsiteServer"/>:<br/>
/// ====<br/>
/// -Create a tree view and assert that the <see cref="OnKeyDown"/>
/// and the <see cref="OnKeyDownAsync"/> work properly.<br/>
/// ====<br/>
/// -Render a <see cref="TreeViewAdhoc"/> that displays many
/// <see cref="WebsiteServer"/>s. Once again, check that
/// movement works properly.
/// </summary>
public class WebsiteServer
{
    private readonly string[] _routes = new[]
    {
        "/",
        "/index",
        "/counter",
        "/fetchdata",
    };

    public WebsiteServer(
        string name,
        string[] routes,
        WebsiteServerState websiteServerState)
    {
        _routes = routes;
        WebsiteServerState = websiteServerState;
        Name = name;
    }


    public WebsiteServerState WebsiteServerState { get; }
    public string Name { get; }
    public ImmutableArray<string> Routes => _routes.ToImmutableArray();
}
