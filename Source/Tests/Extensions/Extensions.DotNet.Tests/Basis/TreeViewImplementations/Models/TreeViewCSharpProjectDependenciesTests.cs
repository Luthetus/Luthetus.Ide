using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewCSharpProjectDependencies"/>
/// </summary>
public class TreeViewCSharpProjectDependenciesTests : ExtensionsDotNetTestBase
{
    /// <summary>
    /// <see cref="TreeViewCSharpProjectDependencies(CSharpProjectDependencies, ILuthetusIdeComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCSharpProjectDependencies.IdeComponentRenderers"/>
    /// <see cref="TreeViewCSharpProjectDependencies.FileSystemProvider"/>
    /// <see cref="TreeViewCSharpProjectDependencies.EnvironmentProvider"/>
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

        var cSharpProjectDependencies = new CSharpProjectDependencies(
            new NamespacePath(
                "ProjectOne",
                environmentProvider.AbsolutePathFactory("/ProjectOne/ProjectOne.csproj", false)));

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewCSharpProjectDependencies(
            cSharpProjectDependencies,
            dotNetComponentRenderers,
            ideComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, cSharpProjectDependencies);
        Assert.Equal(treeView.DotNetComponentRenderers, dotNetComponentRenderers);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectDependencies.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectDependencies.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectDependencies.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectDependencies.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectDependencies.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}