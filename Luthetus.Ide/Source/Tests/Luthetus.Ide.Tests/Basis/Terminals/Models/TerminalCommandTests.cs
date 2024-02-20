using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public record TerminalCommandTests
{
    [Fact]
    public void TerminalCommandKey()
    {
        //Key<TerminalCommand> ,
    }

    [Fact]
    public void FormattedCommand()
    {
        //FormattedCommand ,
    }

    [Fact]
    public void ChangeWorkingDirectoryTo()
    {
        //string?  = null,
    }

    [Fact]
    public void CancellationToken()
    {
        //CancellationToken  = default,
    }

    [Fact]
    public void ContinueWith()
    {
        //Func<Task>?  = null,
    }

    [Fact]
    public void BeginWith()
    {
        //Func<Task>?  = null);
    }
}