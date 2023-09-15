using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileSync
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;

    public InputFileSync(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        
        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}