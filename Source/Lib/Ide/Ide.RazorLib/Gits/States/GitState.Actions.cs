namespace Luthetus.Ide.RazorLib.Gits.States;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);
}