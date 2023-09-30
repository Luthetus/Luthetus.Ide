using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);
}