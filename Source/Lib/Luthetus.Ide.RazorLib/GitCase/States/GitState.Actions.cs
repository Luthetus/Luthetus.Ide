namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);
}