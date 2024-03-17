using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
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
        var notificationInformative = new NotificationViewModel(
            Key<IDynamicViewModel>.NewKey(),
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
        var notificationError = new NotificationViewModel(Key<IDynamicViewModel>.NewKey(),
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

    /// <summary>
    /// TODO: Finish the netcoredbg implementation. For now I'm going to do...
	/// ...a good ol' "Console.WriteLine()" style of debugging with this method...
	/// ...Then, when the debugger is implemented, I can find all references of this...
	/// ...method and remove it from existence.
	/// <br/>
	/// This method takes in a 'Func<string>' as opposed to a 'string' in order to
	/// ensure I encapsulate all of my debug message logic in the invocation of
	/// this method itself, since this method is only to exist short term.
    /// </summary>
	public static void DispatchDebugMessage(
        string title,
        Func<string> messageFunc,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IDispatcher dispatcher,
        TimeSpan? notificationOverlayLifespan)
    {
        var notificationError = new NotificationViewModel(Key<IDynamicViewModel>.NewKey(),
            title,
            luthetusCommonComponentRenderers.ErrorNotificationRendererType,
            new Dictionary<string, object?>
            {
                { nameof(IErrorNotificationRendererType.Message), $"ERROR: {messageFunc.Invoke()}" },
            },
            notificationOverlayLifespan,
            true,
            IErrorNotificationRendererType.CSS_CLASS_STRING);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationError));
    }
}