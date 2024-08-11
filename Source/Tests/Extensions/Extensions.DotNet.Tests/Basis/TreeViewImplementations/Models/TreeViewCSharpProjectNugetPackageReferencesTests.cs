using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewCSharpProjectNugetPackageReferences"/>
/// </summary>
public class TreeViewCSharpProjectNugetPackageReferencesTests : ExtensionsDotNetTestBase
{
    /// <summary>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences(CSharpProjectNugetPackageReferences, ILuthetusIdeComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.IdeComponentRenderers"/>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.FileSystemProvider"/>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.EnvironmentProvider"/>
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

        var cSharpProjectNugetPackageReferences = new CSharpProjectNugetPackageReferences(
            new NamespacePath(
                "ProjectOne",
                environmentProvider.AbsolutePathFactory("/ProjectOne/ProjectOne.csproj", false)));

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewCSharpProjectNugetPackageReferences(
            cSharpProjectNugetPackageReferences,
            dotNetComponentRenderers,
            ideComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, cSharpProjectNugetPackageReferences);
        Assert.Equal(treeView.DotNetComponentRenderers, dotNetComponentRenderers);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCSharpProjectNugetPackageReferences.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}