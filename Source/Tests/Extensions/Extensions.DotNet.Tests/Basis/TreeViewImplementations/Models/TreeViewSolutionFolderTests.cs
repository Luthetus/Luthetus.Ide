using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewSolutionFolder"/>
/// </summary>
public class TreeViewSolutionFolderTests : ExtensionsDotNetTestBase
{
    /// <summary>
    /// <see cref="TreeViewSolutionFolder(SolutionFolder, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, IFileSystemProvider, IEnvironmentProvider, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewSolutionFolder.IdeComponentRenderers"/>
    /// <see cref="TreeViewSolutionFolder.CommonComponentRenderers"/>
    /// <see cref="TreeViewSolutionFolder.FileSystemProvider"/>
    /// <see cref="TreeViewSolutionFolder.EnvironmentProvider"/>
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

        var dotNetSolutionFolder = new SolutionFolder(
            "Tests",
            Guid.NewGuid(),
            string.Empty,
            Guid.NewGuid(),
            new OpenAssociatedGroupToken(TextEditorTextSpan.FabricateTextSpan(string.Empty)),
            new CloseAssociatedGroupToken(TextEditorTextSpan.FabricateTextSpan(string.Empty)),
            environmentProvider.AbsolutePathFactory(string.Empty, false));

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewSolutionFolder(
            dotNetSolutionFolder,
            ideComponentRenderers,
            commonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, dotNetSolutionFolder);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.CommonComponentRenderers, commonComponentRenderers);
        Assert.Equal(treeView.FileSystemProvider, fileSystemProvider);
        Assert.Equal(treeView.EnvironmentProvider, environmentProvider);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewSolutionFolder.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewSolutionFolder.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewSolutionFolder.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewSolutionFolder.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewSolutionFolder.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        throw new NotImplementedException();
    }
}