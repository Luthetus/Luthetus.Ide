using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public record struct FolderExplorerState(
    AbsolutePath? AbsolutePath,
    bool IsLoadingFolderExplorer)
{
    public static readonly Key<TreeViewContainer> TreeViewContentStateKey = Key<TreeViewContainer>.NewKey();

    public FolderExplorerState() : this(
        default,
        false)
    {

    }
}