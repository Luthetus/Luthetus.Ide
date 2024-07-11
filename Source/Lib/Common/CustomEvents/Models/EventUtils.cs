using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.CustomEvents.Models;

/// <summary>
/// https://learn.microsoft.com/en-us/aspnet/core/blazor/performance?view=aspnetcore-8.0
/// ====================================================================================
/// 	"In addition to preventing rerenders after event handlers fire in a component in a global fashion,
/// 	it's possible to prevent rerenders after a single event handler by employing the following utility method.
///
/// 	Add the following EventUtil class to a Blazor app. The static actions and functions at the top of the
/// 	EventUtil class provide handlers that cover several combinations of arguments and return types that
/// 	Blazor uses when handling events."
/// </summary>
public static class EventUtil
{
    public static Action AsNonRenderingEventHandler(Action callback)
        => new SyncReceiver(callback).Invoke;
    public static Action<TValue> AsNonRenderingEventHandler<TValue>(
            Action<TValue> callback)
        => new SyncReceiver<TValue>(callback).Invoke;
    public static Func<Task> AsNonRenderingEventHandler(Func<Task> callback)
        => new AsyncReceiver(callback).Invoke;
    public static Func<TValue, Task> AsNonRenderingEventHandler<TValue>(
            Func<TValue, Task> callback)
        => new AsyncReceiver<TValue>(callback).Invoke;

    private record SyncReceiver(Action callback) 
        : ReceiverBase { public void Invoke() => callback(); }
    private record SyncReceiver<T>(Action<T> callback) 
        : ReceiverBase { public void Invoke(T arg) => callback(arg); }
    private record AsyncReceiver(Func<Task> callback) 
        : ReceiverBase { public Task Invoke() => callback(); }
    private record AsyncReceiver<T>(Func<T, Task> callback) 
        : ReceiverBase { public Task Invoke(T arg) => callback(arg); }

    private record ReceiverBase : IHandleEvent
    {
        public Task HandleEventAsync(EventCallbackWorkItem item, object arg) => 
            item.InvokeAsync(arg);
    }
}