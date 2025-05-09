using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandBars.Models;

namespace Luthetus.Ide.RazorLib.CommandBars.Displays;

public partial class CommandBarDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ICommandBarService CommandBarService { get; set; } = null!;
	[Inject]
	private IWidgetService WidgetService { get; set; } = null!;
	[Inject]
	private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;
	
	public const string INPUT_HTML_ELEMENT_ID = "luth_ide_command-bar-input-id";
		
	protected override void OnInitialized()
	{
		CommandBarService.CommandBarStateChanged += OnCommandBarStateChanged;
		base.OnInitialized();
	}
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await CommonBackgroundTaskApi.JsRuntimeCommonApi
				.FocusHtmlElementById(CommandBarDisplay.INPUT_HTML_ELEMENT_ID)
	            .ConfigureAwait(false);
		}
        			
		await base.OnAfterRenderAsync(firstRender);
	}
	
	private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Key == "Enter")
			WidgetService.SetWidget(null);
	}
	
	private async void OnCommandBarStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		CommandBarService.CommandBarStateChanged -= OnCommandBarStateChanged;
	}
}