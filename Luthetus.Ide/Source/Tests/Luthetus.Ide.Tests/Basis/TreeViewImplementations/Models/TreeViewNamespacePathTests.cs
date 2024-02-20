using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

public class TreeViewNamespacePathTests
{
    [Fact]
    public void Constructor()
    {
        //public TreeViewNamespacePath(
        //        NamespacePath namespacePath,
        //        ILuthetusIdeComponentRenderers ideComponentRenderers,
        //        ILuthetusCommonComponentRenderers commonComponentRenderers,
        //        IFileSystemProvider fileSystemProvider,
        //        IEnvironmentProvider environmentProvider,
        //        bool isExpandable,
        //        bool isExpanded)
        //    : base(namespacePath, isExpandable, isExpanded)
    }

    [Fact]
    public void IdeComponentRenderers()
    {
        //public ILuthetusIdeComponentRenderers  { get; }
    }

    [Fact]
    public void CommonComponentRenderers()
    {
        //public ILuthetusCommonComponentRenderers  { get; }
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
        //public override int () => Item.AbsolutePath.Value.GetHashCode();
    }

    [Fact]
    public void GetTreeViewRenderer()
    {
        //public override TreeViewRenderer ()
    }

    [Fact]
    public void LoadChildListAsync()
    {
        //public override async Task ()
    }

    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        //public override void (List<TreeViewNoType> siblingsAndSelfTreeViews)
    }
}