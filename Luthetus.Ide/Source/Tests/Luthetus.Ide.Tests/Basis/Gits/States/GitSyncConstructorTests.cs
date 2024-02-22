using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.Tests.Basis.Gits.States;

/// <summary>
/// <see cref="GitSync"/>
/// </summary>
public class GitSyncConstructorTests
{
    /// <summary>
    /// <see cref="GitSync(IState{TerminalSessionState}, IState{GitState}, IFileSystemProvider, IEnvironmentProvider, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="GitSync.BackgroundTaskService"/>
    /// <see cref="GitSync.Dispatcher"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}
