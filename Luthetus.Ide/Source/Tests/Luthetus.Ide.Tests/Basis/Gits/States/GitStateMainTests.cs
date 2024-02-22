using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Gits.Models;

namespace Luthetus.Ide.Tests.Basis.Gits.States;

/// <summary>
/// <see cref="GitState"/>
/// </summary>
public class GitStateMainTests
{
    /// <summary>
    /// <see cref="GitState(IAbsolutePath?, ImmutableList{GitFile}, ImmutableList{GitTask})"/>
    /// <br/>----<br/>
    /// <see cref="GitState.GitFolderAbsolutePath"/>
    /// <see cref="GitState.GitFilesList"/>
    /// <see cref="GitState.ActiveGitTasks"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}