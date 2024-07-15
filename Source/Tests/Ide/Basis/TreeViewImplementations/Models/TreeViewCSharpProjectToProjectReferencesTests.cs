using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.CSharpProjects.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewCSharpProjectToProjectReferences"/>
/// </summary>
public class TreeViewCSharpProjectToProjectReferencesTests : IdeTestBase
{
    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReferences(CSharpProjectToProjectReferences, ILuthetusIdeComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.IdeComponentRenderers"/>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.FileSystemProvider"/>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.EnvironmentProvider"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        Test_RegisterServices(out var serviceProvider);
        Test_CreateFileSystem(serviceProvider);

        var ideComponentRenderers = serviceProvider.GetRequiredService<IIdeComponentRenderers>();
        var commonComponentRenderers = serviceProvider.GetRequiredService<ICommonComponentRenderers>();
        var fileSystemProvider = serviceProvider.GetRequiredService<IFileSystemProvider>();
        var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();

        var cSharpProjectToProjectReferences = new CSharpProjectToProjectReferences(
            new NamespacePath(
                "ProjectOne",
                environmentProvider.AbsolutePathFactory("/ProjectOne/ProjectOne.csproj", false)));

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewCSharpProjectToProjectReferences(
            cSharpProjectToProjectReferences,
            ideComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, cSharpProjectToProjectReferences);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReferences.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}