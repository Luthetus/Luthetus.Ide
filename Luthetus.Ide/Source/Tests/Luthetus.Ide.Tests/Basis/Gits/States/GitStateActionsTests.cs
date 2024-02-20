namespace Luthetus.Ide.Tests.Basis.Gits.States;

public class GitStateActionsTests
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);
}