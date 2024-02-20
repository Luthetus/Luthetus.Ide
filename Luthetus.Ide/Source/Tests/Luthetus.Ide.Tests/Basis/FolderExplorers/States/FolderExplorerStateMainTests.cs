using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.Tests.Basis.FolderExplorers.States;

public class FolderExplorerStateMainTests(
    IAbsolutePath? AbsolutePath,
    bool IsLoadingFolderExplorer)
{
    public static readonly Key<TreeViewContainer> TreeViewContentStateKey = Key<TreeViewContainer>.NewKey();

    private FolderExplorerState() : this(
        default,
        false)
    {

    }
}