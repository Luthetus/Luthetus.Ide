using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

/// <summary>
/// <see cref="TreeViewCompilerService"/>
/// </summary>
public class TreeViewCompilerServiceTests : IdeTestBase
{
    /// <summary>
    /// <see cref="TreeViewCompilerService(ILuthCompilerService, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCompilerService.IdeComponentRenderers"/>
    /// <see cref="TreeViewCompilerService.CommonComponentRenderers"/>
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

        var compilerService = new CSharpCompilerService(
            serviceProvider.GetRequiredService<ITextEditorService>());

        var isExpandable = true;
        var isExpanded = true;

        var treeView = new TreeViewCompilerService(
            compilerService,
            ideComponentRenderers,
            commonComponentRenderers,
            isExpandable,
            isExpanded);

        Assert.Equal(treeView.Item, compilerService);
        Assert.Equal(treeView.IdeComponentRenderers, ideComponentRenderers);
        Assert.Equal(treeView.CommonComponentRenderers, commonComponentRenderers);
        Assert.Equal(treeView.IsExpandable, isExpandable);
        Assert.Equal(treeView.IsExpanded, isExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewCompilerService.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCompilerService.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCompilerService.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCompilerService.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }
}