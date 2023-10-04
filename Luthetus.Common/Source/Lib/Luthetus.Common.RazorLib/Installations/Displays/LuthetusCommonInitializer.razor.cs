using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Installations.Displays;

public partial class LuthetusCommonInitializer : ComponentBase
{
    [Inject]
    private LuthetusCommonOptions LuthetusCommonOptions { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private ContinuousBackgroundTaskWorker ContinuousBackgroundTaskWorker { get; set; } = null!;
    [Inject]
    private BlockingBackgroundTaskWorker BlockingBackgroundTaskWorker { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.ServerSide)
            {
                _ = Task.Run(async () => await ContinuousBackgroundTaskWorker
                                                   .StartAsync(CancellationToken.None)
                                                   .ConfigureAwait(false));

                _ = Task.Run(async () => await BlockingBackgroundTaskWorker
                                                   .StartAsync(CancellationToken.None)
                                                   .ConfigureAwait(false));
            }

            AppOptionsService.SetActiveThemeRecordKey(LuthetusCommonOptions.InitialThemeKey, false);
            await AppOptionsService.SetFromLocalStorageAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}