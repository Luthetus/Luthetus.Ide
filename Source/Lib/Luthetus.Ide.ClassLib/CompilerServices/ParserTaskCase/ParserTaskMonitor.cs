using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;

namespace Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

public class ParserTaskMonitor : IParserTaskMonitor
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;

    public ParserTaskMonitor(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
    }

    public IParserTask? ExecutingParserTask { get; private set; }

    public event Action? ExecutingParserTaskChanged;

    public void SetExecutingParserTask(IParserTask? backgroundTask)
    {
        ExecutingParserTask = backgroundTask;
        ExecutingParserTaskChanged?.Invoke();

        if (backgroundTask is not null &&
            backgroundTask.ShouldNotifyWhenStartingWorkItem &&
            backgroundTask.Dispatcher is not null &&
            _luthetusIdeComponentRenderers.ParserTaskDisplayRendererType is not null)
        {
            var notificationRecord = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                $"Starting: {backgroundTask.Name}",
                _luthetusIdeComponentRenderers.ParserTaskDisplayRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IParserTaskDisplayRendererType.ParserTask),
                        backgroundTask
                    }
                },
                null,
                null);

            backgroundTask.Dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationRecord));
        }
    }
}