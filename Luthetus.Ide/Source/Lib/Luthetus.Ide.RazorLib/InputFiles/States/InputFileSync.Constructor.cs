using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileSync
{
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;

    public InputFileSync(
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _ideComponentRenderers = ideComponentRenderers;
        
        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}