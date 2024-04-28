using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial class GitSync
{
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;

    public GitSync(
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _terminalStateWrap = terminalStateWrap;
        _gitStateWrap = gitStateWrap;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}
