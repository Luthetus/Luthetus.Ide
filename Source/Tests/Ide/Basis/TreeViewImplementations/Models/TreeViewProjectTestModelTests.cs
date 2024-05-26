using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewProjectTestModel"/>
/// </summary>
public class TreeViewProjectTestModelTests : IdeTestBase
{
    /// <summary>
    /// <see cref="TreeViewProjectTestModel(ProjectTestModel, ILuthetusCommonComponentRenderers, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewProjectTestModel.CommonComponentRenderers"/>
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

        var projectTestModel = new ProjectTestModel(
            Guid.NewGuid(),
            environmentProvider.AbsolutePathFactory("/unitTesting.txt", false),
            callback => Task.CompletedTask,
            callback => { });

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewProjectTestModel(
            projectTestModel,
            commonComponentRenderers,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, projectTestModel);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
        Assert.Equal(treeView.CommonComponentRenderers, commonComponentRenderers);
    }

    /// <summary>
    /// <see cref="TreeViewProjectTestModel.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewProjectTestModel.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewProjectTestModel.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewProjectTestModel.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewProjectTestModel.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}