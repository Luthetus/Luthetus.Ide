using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.States;

/// <summary>
/// The Folder, ".git" may be in the following locations:<br/>
/// -In the context of .NET:<br/>
///     --The folder containing the user selected .NET Solution<br/>
///     --The folder containing the user selected C# Project which is being contained in an adhoc .NET Solution<br/>
/// -In the context of using the folder explorer<br/>
///     --The folder which is user selected.<br/>
/// </summary>
[FeatureState]
public partial record GitState(
    IAbsolutePath? GitFolderAbsolutePath,
    ImmutableList<GitFile> GitFilesList,
    ImmutableList<GitTask> ActiveGitTasks)
{
    public GitState() : this(null, ImmutableList<GitFile>.Empty, ImmutableList<GitTask>.Empty)
    {

    }
}