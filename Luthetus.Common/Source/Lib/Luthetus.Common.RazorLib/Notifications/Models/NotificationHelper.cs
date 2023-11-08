using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.States;

namespace Luthetus.Common.RazorLib.Notifications.Models;

public static class NotificationHelper
{
    public static void DispatchInformative(
        string title,
        string message,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IDispatcher dispatcher,
        TimeSpan? notificationOverlayLifespan)
    {
        var notificationInformative = new NotificationRecord(
            Key<NotificationRecord>.NewKey(),
            title,
            luthetusCommonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message),
                    message
                },
            },
            notificationOverlayLifespan,
            true,
            null);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationInformative));
    }

    public static void DispatchError(
        string title,
        string message,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IDispatcher dispatcher,
        TimeSpan? notificationOverlayLifespan)
    {
        var notificationError = new NotificationRecord(Key<NotificationRecord>.NewKey(),
            title,
            luthetusCommonComponentRenderers.ErrorNotificationRendererType,
            new Dictionary<string, object?>
            {
                { nameof(IErrorNotificationRendererType.Message), $"ERROR: {message}" },
            },
            notificationOverlayLifespan,
            true,
            IErrorNotificationRendererType.CSS_CLASS_STRING);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationError));
    }
}