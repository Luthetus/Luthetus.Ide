using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewSolution"/>
/// </summary>
public class TreeViewSolutionTests : ExtensionsDotNetTestBase
{
    /// <summary>
    /// <see cref="TreeViewSolution(DotNetSolutionModel, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewSolution.IdeComponentRenderers"/>
    /// <see cref="TreeViewSolution.CommonComponentRenderers"/>
    /// <see cref="TreeViewSolution.FileSystemProvider"/>
    /// <see cref="TreeViewSolution.EnvironmentProvider"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        Test_RegisterServices(out var serviceProvider);
        Test_CreateFileSystem(serviceProvider);

        var dotNetComponentRenderers = serviceProvider.GetRequiredService<IDotNetComponentRenderers>();
        var ideComponentRenderers = serviceProvider.GetRequiredService<IIdeComponentRenderers>();
        var commonComponentRenderers = serviceProvider.GetRequiredService<ICommonComponentRenderers>();
        var fileSystemProvider = serviceProvider.GetRequiredService<IFileSystemProvider>();
        var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();

        var dotNetSolutionModel = new DotNetSolutionModel(
            environmentProvider.AbsolutePathFactory("/unitTesting.sln", false),
            new DotNetSolutionHeader(),
            ImmutableArray<IDotNetProject>.Empty,
            ImmutableArray<SolutionFolder>.Empty,
            ImmutableArray<NestedProjectEntry>.Empty,
            new DotNetSolutionGlobal(),
            string.Empty);

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewSolution(
            dotNetSolutionModel,
            dotNetComponentRenderers,
            ideComponentRenderers,
            commonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, dotNetSolutionModel);
        Assert.Equal(treeView.DotNetComponentRenderers, dotNetComponentRenderers);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.CommonComponentRenderers, commonComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewSolution.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewSolution.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewSolution.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// <see cref="TreeViewSolution.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// <see cref="TreeViewSolution.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}