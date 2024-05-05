using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewAbsolutePath"/>
/// </summary>
public class TreeViewAbsolutePathTests : IdeTestBase
{
    /// <summary>
    /// <see cref="TreeViewAbsolutePath(IAbsolutePath, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewAbsolutePath.IdeComponentRenderers"/>
    /// <see cref="TreeViewAbsolutePath.CommonComponentRenderers"/>
    /// <see cref="TreeViewAbsolutePath.FileSystemProvider"/>
    /// <see cref="TreeViewAbsolutePath.EnvironmentProvider"/>
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

        var absolutePath = environmentProvider.AbsolutePathFactory(Test_WellKnownPaths.Files.NervousSystemTxt, false);

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewAbsolutePath(
            absolutePath,
            ideComponentRenderers,
            commonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, absolutePath);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.CommonComponentRenderers, commonComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewAbsolutePath.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewAbsolutePath.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewAbsolutePath.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewAbsolutePath.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewAbsolutePath.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}