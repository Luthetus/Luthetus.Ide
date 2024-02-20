using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.Gits.States;

public class GitStateMainTests(
    IAbsolutePath? GitFolderAbsolutePath,
    ImmutableList<GitFile> GitFilesList,
    ImmutableList<GitTask> ActiveGitTasks)
{
    public GitState() : this(null, ImmutableList<GitFile>.Empty, ImmutableList<GitTask>.Empty)
    {

    }
}