using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

public class TreeViewReflectionWithViewTests
{
    [Fact]
    public void Constructor()
    {
        //public TreeViewReflectionWithView(
        //        WatchWindowObject watchWindowObject,
        //        bool isExpandable,
        //        bool isExpanded,
        //        ILuthetusIdeComponentRenderers ideComponentRenderers,
        //        ILuthetusCommonComponentRenderers commonComponentRenderers)
        //: base(watchWindowObject, isExpandable, isExpanded, commonComponentRenderers)
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
    public void LoadChildListAsync()
    {
        //public override async Task ()
    }
}