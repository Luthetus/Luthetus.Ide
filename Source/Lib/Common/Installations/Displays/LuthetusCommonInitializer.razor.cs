using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Installations.Displays;

public partial class LuthetusCommonInitializer : ComponentBase
{
    [Inject]
    private LuthetusCommonConfig CommonConfig { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BackgroundTaskService.EnqueueAsync(
                Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(LuthetusCommonInitializer),
                async () =>
                {
                    await AppOptionsService
                        .SetActiveThemeRecordKey(CommonConfig.InitialThemeKey, false)
                        .ConfigureAwait(false);

                    await AppOptionsService
                        .SetFromLocalStorageAsync()
                        .ConfigureAwait(false);
                }).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}