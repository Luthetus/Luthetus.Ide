using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabListDisplay : ComponentBase
{
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	[Parameter, EditorRequired]
	public ImmutableArray<ITab> TabList { get; set; } = ImmutableArray<ITab>.Empty;
	
	[Parameter]
	public string CssClassString { get; set; } = string.Empty;

    public async Task NotifyStateChangedAsync()
	{
		// This method has a race condition when one drags a tab
		// off the text editor group tab listing.
		//
		// Exception text:
		// =============================
		// (Luthetus.Ide.Photino:28433): Gtk-WARNING **: 13:30:13.496: Failed to set text 'System.NullReferenceException: Object reference not set to an instance of an object.
		//   at Luthetus.Common.RazorLib.Tabs.Displays.TabDisplay.BuildRenderTree(RenderTreeBuilder __builder) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/Common/Tabs/Displays/TabDisplay.razor:line 1
		//   at Microsoft.AspNetCore.Components.Rendering.ComponentState.RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment, Exception& renderFragmentException)
		//--- End of stack trace from previous location ---
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.<>c__DisplayClass11_0.<NotifyUnhandledException>b__1()
		//   at Photino.Blazor.PhotinoDispatcher.InvokeAsync(Action workItem)
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.NotifyUnhandledException(Exception exception)
		//   at Microsoft.AspNetCore.Components.WebView.Services.WebViewRenderer.HandleException(Exception exception)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.HandleExceptionViaErrorBoundary(Exception error, ComponentState errorSourceOrNull)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
		//--- End of stack trace from previous location ---
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.<>c__DisplayClass11_0.<NotifyUnhandledException>b__1()
		//   at Photino.Blazor.PhotinoDispatcher.InvokeAsync(Action workItem)
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.NotifyUnhandledException(Exception exception)
		//   at Microsoft.AspNetCore.Components.WebView.Services.WebViewRenderer.HandleException(Exception exception)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
		//   at Microsoft.AspNetCore.Components.ComponentBase.StateHasChanged()
		//   at Photino.Blazor.PhotinoSynchronizationContext.<>c.<InvokeAsync>b__11_0(Object state)
		//--- End of stack trace from previous location ---
		//   at Luthetus.Ide.RazorLib.Shareds.Displays.IdeMainLayout.DragStateWrapOnStateChanged(Object sender, EventArgs e) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/Ide/Ide.RazorLib/Shareds/Displays/IdeMainLayout.razor.cs:line 111
		//   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__128_1(Object state)
		//   at System.Threading.ThreadPoolWorkQueue.Dispatch()
		//   at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()' from markup due to error parsing markup: Error on line 3: Entity did not end with a semicolon; most likely you used an ampersand character without intending to start an entity — escape ampersand as &amp;
		//
		//(Luthetus.Ide.Photino:28433): Gtk-WARNING **: 13:30:13.511: Failed to set text 'System.NullReferenceException: Object reference not set to an instance of an object.
		//   at Luthetus.Common.RazorLib.Tabs.Displays.TabDisplay.BuildRenderTree(RenderTreeBuilder __builder) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/Common/Tabs/Displays/TabDisplay.razor:line 1
		//   at Microsoft.AspNetCore.Components.Rendering.ComponentState.RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment, Exception& renderFragmentException)
		//--- End of stack trace from previous location ---
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.<>c__DisplayClass11_0.<NotifyUnhandledException>b__1()
		//   at Photino.Blazor.PhotinoDispatcher.InvokeAsync(Action workItem)
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.NotifyUnhandledException(Exception exception)
		//   at Microsoft.AspNetCore.Components.WebView.Services.WebViewRenderer.HandleException(Exception exception)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.HandleExceptionViaErrorBoundary(Exception error, ComponentState errorSourceOrNull)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
		//--- End of stack trace from previous location ---
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.<>c__DisplayClass11_0.<NotifyUnhandledException>b__1()
		//   at Photino.Blazor.PhotinoDispatcher.InvokeAsync(Action workItem)
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.NotifyUnhandledException(Exception exception)
		//   at Microsoft.AspNetCore.Components.WebView.Services.WebViewRenderer.HandleException(Exception exception)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
		//   at Microsoft.AspNetCore.Components.ComponentBase.StateHasChanged()
		//   at Photino.Blazor.PhotinoSynchronizationContext.<>c.<InvokeAsync>b__11_0(Object state)
		//--- End of stack trace from previous location ---
		//   at Luthetus.Ide.RazorLib.Shareds.Displays.IdeMainLayout.DragStateWrapOnStateChanged(Object sender, EventArgs e) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/Ide/Ide.RazorLib/Shareds/Displays/IdeMainLayout.razor.cs:line 111
		//   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__128_1(Object state)
		//   at System.Threading.ThreadPoolWorkQueue.Dispatch()
		//   at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()' from markup due to error parsing markup: Error on line 3: Entity did not end with a semicolon; most likely you used an ampersand character without intending to start an entity — escape ampersand as &amp;
		//Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
		//   at Luthetus.Common.RazorLib.Tabs.Displays.TabDisplay.BuildRenderTree(RenderTreeBuilder __builder) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/Common/Tabs/Displays/TabDisplay.razor:line 1
		//   at Microsoft.AspNetCore.Components.Rendering.ComponentState.RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment, Exception& renderFragmentException)
		//--- End of stack trace from previous location ---
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.<>c__DisplayClass11_0.<NotifyUnhandledException>b__1()
		//   at Photino.Blazor.PhotinoDispatcher.InvokeAsync(Action workItem)
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.NotifyUnhandledException(Exception exception)
		//   at Microsoft.AspNetCore.Components.WebView.Services.WebViewRenderer.HandleException(Exception exception)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.HandleExceptionViaErrorBoundary(Exception error, ComponentState errorSourceOrNull)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
		//--- End of stack trace from previous location ---
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.<>c__DisplayClass11_0.<NotifyUnhandledException>b__1()
		//   at Photino.Blazor.PhotinoDispatcher.InvokeAsync(Action workItem)
		//   at Microsoft.AspNetCore.Components.WebView.IpcSender.NotifyUnhandledException(Exception exception)
		//   at Microsoft.AspNetCore.Components.WebView.Services.WebViewRenderer.HandleException(Exception exception)
		//   at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
		//   at Microsoft.AspNetCore.Components.ComponentBase.StateHasChanged()
		//   at Photino.Blazor.PhotinoSynchronizationContext.<>c.<InvokeAsync>b__11_0(Object state)
		//--- End of stack trace from previous location ---
		//   at Luthetus.Ide.RazorLib.Shareds.Displays.IdeMainLayout.DragStateWrapOnStateChanged(Object sender, EventArgs e) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/Ide/Ide.RazorLib/Shareds/Displays/IdeMainLayout.razor.cs:line 111
		//   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__128_1(Object state)
		//   at System.Threading.ThreadPoolWorkQueue.Dispatch()
		//   at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()
		//
		// More Information
		// ================
		// This try catch didn't fix the issue. It seems the .razor is crashing.
		// Presumably something to do with .razor exceptions being fatal exceptions through way of middleware?
		//
		// I'm considering doing something with IDisposable, but it sounds like a nightmare scenario
		// to try and add concurrency safety here.
		//
		// The tab is perhaps no longer rendered as it gets told via this method to render?
		//
		// Maybe the underlying ComponentBase RenderHandle, or such, logic was disposed/null'd out
		// but this object reference was still available.
		try
		{
			await InvokeAsync(StateHasChanged);
		}
		catch (Exception e)
		{
			var title = $"{nameof(TabListDisplay)} race condition occurred.\n";
			var message = e.ToString();
		
			NotificationHelper.DispatchError(
		        title,
		        message,
		        CommonComponentRenderers,
		        Dispatcher,
		        TimeSpan.FromSeconds(6));
		
			Console.WriteLine(title + message);
		}
	}

	private Task HandleTabButtonOnContextMenu(TabContextMenuEventArgs tabContextMenuEventArgs)
    {
		var dropdownRecord = new DropdownRecord(
			TabContextMenu.ContextMenuEventDropdownKey,
			tabContextMenuEventArgs.MouseEventArgs.ClientX,
			tabContextMenuEventArgs.MouseEventArgs.ClientY,
			typeof(TabContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(TabContextMenu.TabContextMenuEventArgs),
					tabContextMenuEventArgs
				}
			},
			restoreFocusOnClose: null);

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
        return Task.CompletedTask;
    }
}