using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial class GitSync
{
    private readonly IState<TerminalSessionState> _terminalSessionStateWrap;
    private readonly IState<GitState> _gitStateWrap;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;

    public GitSync(
        IState<TerminalSessionState> terminalSessionStateWrap,
        IState<GitState> gitStateWrap,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _terminalSessionStateWrap = terminalSessionStateWrap;
        _gitStateWrap = gitStateWrap;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}
