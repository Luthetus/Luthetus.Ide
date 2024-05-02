using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.States;

[FeatureState]
public partial record GitState(
    IAbsolutePath? GitFolderAbsolutePath,
    ImmutableList<GitFile> GitFileList,
    ImmutableDictionary<string, GitFile> StagedGitFileMap,
    ImmutableList<GitTask> ActiveGitTasks)
{
    public static readonly Key<TreeViewContainer> TreeViewGitChangesKey = Key<TreeViewContainer>.NewKey();

    public GitState()
        : this(null, ImmutableList<GitFile>.Empty, ImmutableDictionary<string, GitFile>.Empty, ImmutableList<GitTask>.Empty)
    {

    }
}