using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.CSharpProjects.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewCSharpProjectToProjectReference"/>
/// </summary>
public class TreeViewCSharpProjectToProjectReferenceTests : IdeTestBase
{
    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReference(CSharpProjectToProjectReference, ILuthetusIdeComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCSharpProjectToProjectReference.IdeComponentRenderers"/>
    /// <see cref="TreeViewCSharpProjectToProjectReference.FileSystemProvider"/>
    /// <see cref="TreeViewCSharpProjectToProjectReference.EnvironmentProvider"/>
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

        var cSharpProjectToProjectReference = new CSharpProjectToProjectReference(
            new NamespacePath(
                "ProjectOne",
                environmentProvider.AbsolutePathFactory("/ProjectOne/ProjectOne.csproj", false)),
            environmentProvider.AbsolutePathFactory("/ProjectTwo/ProjectTwo.csproj", false));

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewCSharpProjectToProjectReference(
            cSharpProjectToProjectReference,
            ideComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, cSharpProjectToProjectReference);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReference.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReference.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReference.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReference.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectToProjectReference.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}