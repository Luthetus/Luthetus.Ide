using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;

public class FileSystemBackgroundTaskMonitor : IFileSystemBackgroundTaskMonitor
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;

    public FileSystemBackgroundTaskMonitor(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
    }

    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }

    public event Action? ExecutingBackgroundTaskChanged;

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
    {
        ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();

        if (backgroundTask is not null &&
            backgroundTask.ShouldNotifyWhenStartingWorkItem &&
            backgroundTask.Dispatcher is not null &&
            _luthetusIdeComponentRenderers.CompilerServiceBackgroundTaskDisplayRendererType is not null)
        {
            var notificationRecord = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                $"Starting: {backgroundTask.Name}",
                _luthetusIdeComponentRenderers.CompilerServiceBackgroundTaskDisplayRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IFileSystemBackgroundTaskDisplayRendererType.BackgroundTask),
                        backgroundTask
                    }
                },
                null,
                true,
                null);

            backgroundTask.Dispatcher.Dispatch(new NotificationRecordsCollection.RegisterAction(
                notificationRecord));
        }
    }
}