using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

public class TreeViewCSharpProjectDependenciesTests
{
    [Fact]
    public void Constructor()
    {
        //public TreeViewCSharpProjectDependencies(
        //        CSharpProjectDependencies cSharpProjectDependencies,
        //        ILuthetusIdeComponentRenderers ideComponentRenderers,
        //        IFileSystemProvider fileSystemProvider,
        //        IEnvironmentProvider environmentProvider,
        //        bool isExpandable,
        //        bool isExpanded)
        //    : base(cSharpProjectDependencies, isExpandable, isExpanded)
        //{
        //    IdeComponentRenderers = ideComponentRenderers;
        //    FileSystemProvider = fileSystemProvider;
        //    EnvironmentProvider = environmentProvider;
        //}
    }

    [Fact]
    public void IdeComponentRenderers()
    {
        //public ILuthetusIdeComponentRenderers  { get; }
    }

    [Fact]
    public void FileSystemProvider()
    {
        //public IFileSystemProvider  { get; }
    }

    [Fact]
    public void EnvironmentProvider()
    {
        //public IEnvironmentProvider  { get; }
    }

    [Fact]
    public void Equals()
    {
        //public override bool (object? obj)
    }


    [Fact]
    public void GetHashCode()
    {
        //public override int () => Item.CSharpProjectNamespacePath.AbsolutePath.Value.GetHashCode();
    }

    [Fact]
    public void GetTreeViewRenderer()
    {
        //public override TreeViewRenderer ()
    }

    [Fact]
    public void LoadChildListAsync()
    {
        //public override Task ()
    }

    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        //public override void (List<TreeViewNoType> siblingsAndSelfTreeViews)
    }
}