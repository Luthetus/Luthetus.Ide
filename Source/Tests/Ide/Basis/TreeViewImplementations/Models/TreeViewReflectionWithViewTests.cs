using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Ide.RazorLib.TreeViewUtils.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewReflectionWithView"/>
/// </summary>
public class TreeViewReflectionWithViewTests : IdeTestBase
{
    /// <summary>
    /// <see cref="TreeViewReflectionWithView(WatchWindowObject, bool, bool, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewReflectionWithView.IdeComponentRenderers"/>
    /// <see cref="TreeViewReflectionWithView.CommonComponentRenderers"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        Test_RegisterServices(out var serviceProvider);
        Test_CreateFileSystem(serviceProvider);

        var ideComponentRenderers = serviceProvider.GetRequiredService<ILuthetusIdeComponentRenderers>();
        var commonComponentRenderers = serviceProvider.GetRequiredService<ILuthetusCommonComponentRenderers>();
        var fileSystemProvider = serviceProvider.GetRequiredService<IFileSystemProvider>();
        var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();

        var item = "abc123";
        var watchWindowObject = new WatchWindowObject(item, item.GetType(), nameof(item), false);

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewReflectionWithView(
            watchWindowObject,
            isExpandable,
            isExpanded,
            ideComponentRenderers,
            commonComponentRenderers);

        Assert.Equal(treeView.Item, watchWindowObject);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.CommonComponentRenderers, commonComponentRenderers);
    }

    /// <summary>
    /// <see cref="TreeViewReflectionWithView.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }
}