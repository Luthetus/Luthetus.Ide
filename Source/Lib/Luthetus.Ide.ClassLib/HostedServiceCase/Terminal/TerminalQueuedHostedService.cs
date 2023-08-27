using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;

public class TerminalQueuedHostedService : BackgroundService
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly ILogger _logger;

    public TerminalQueuedHostedService(
        ITerminalBackgroundTaskQueue taskQueue,
        ITerminalBackgroundTaskMonitor taskMonitor,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        ILoggerFactory loggerFactory)
    {
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        TaskQueue = taskQueue;
        TaskMonitor = taskMonitor;
        _logger = loggerFactory.CreateLogger<TerminalQueuedHostedService>();
    }

    public IBackgroundTaskQueue TaskQueue { get; }
    public IBackgroundTaskMonitor TaskMonitor { get; }

    protected async override Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Queued Hosted Service is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var backgroundTask = await TaskQueue
                .DequeueAsync(cancellationToken);

            if (backgroundTask is not null)
            {
                try
                {
                    TaskMonitor.SetExecutingBackgroundTask(backgroundTask);

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

                        backgroundTask.Dispatcher.Dispatch(new NotificationRecordsCollection.RegisterAction(
                            notificationRecord));
                    }
                }
                finally
                {
                    TaskMonitor.SetExecutingBackgroundTask(null);
                }
            }
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }
}