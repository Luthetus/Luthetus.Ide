using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;

public class LuthetusIdeFileSystemBackgroundTaskServiceWorker : BackgroundService
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly ILogger _logger;

    public LuthetusIdeFileSystemBackgroundTaskServiceWorker(
        ILuthetusIdeFileSystemBackgroundTaskService backgroundTaskService,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        ILoggerFactory loggerFactory)
    {
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<LuthetusIdeFileSystemBackgroundTaskServiceWorker>();
    }

    public IBackgroundTaskService BackgroundTaskService { get; }

    protected async override Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Queued Hosted Service is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var backgroundTask = await BackgroundTaskService.DequeueAsync(cancellationToken);

            if (backgroundTask is not null)
            {
                try
                {
                    BackgroundTaskService.SetExecutingBackgroundTask(backgroundTask);

                    var task = backgroundTask.InvokeWorkItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    var message = ex is OperationCanceledException
                        ? "Task was cancelled {0}." // {0} => WorkItemName
                        : "Error occurred executing {0}."; // {0} => WorkItemName

                    _logger.LogError(
                        ex,
                        message,
                        backgroundTask.Name);

                    if (backgroundTask.Dispatcher is not null &&
                        _luthetusCommonComponentRenderers.ErrorNotificationRendererType is not null)
                    {
                        var notificationRecord = new NotificationRecord(
                            NotificationKey.NewNotificationKey(),
                            "ExecutingParserTaskChanged",
                            _luthetusCommonComponentRenderers.ErrorNotificationRendererType,
                            new Dictionary<string, object?>
                            {
                                {
                                    nameof(IErrorNotificationRendererType.Message),
                                    string.Format($"backgroundTask.Name => {ex.Message}")
                                }
                            },
                            null,
                            true,
                            IErrorNotificationRendererType.CSS_CLASS_STRING);

                        backgroundTask.Dispatcher.Dispatch(new NotificationRegistry.RegisterAction(
                            notificationRecord));
                    }
                }
                finally
                {
                    BackgroundTaskService.SetExecutingBackgroundTask(null);
                }
            }
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }
}