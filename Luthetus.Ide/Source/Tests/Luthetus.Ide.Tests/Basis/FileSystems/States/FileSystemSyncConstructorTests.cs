using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.FileSystems.States;

public class FileSystemSyncConstructorTests
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public FileSystemSync(
        IFileSystemProvider fileSystemProvider,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _fileSystemProvider = fileSystemProvider;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}