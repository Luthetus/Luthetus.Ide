using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Widgets.Displays;

public partial class WidgetDisplay : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IWidgetService WidgetService { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public WidgetModel Widget { get; set; } = null!;
	[Parameter, EditorRequired]
    public Func<WidgetModel, Task> OnFocusInFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<WidgetModel, Task> OnFocusOutFunc { get; set; } = null!;
    
    private const string WIDGET_HTML_ELEMENT_ID = "luth_widget-id";
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.GetLuthetusCommonApi()
                .FocusHtmlElementById(WIDGET_HTML_ELEMENT_ID)
                .ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

	private Task HandleOnFocusIn()
    {
        return OnFocusInFunc.Invoke(Widget);
    }
    
	private Task HandleOnFocusOut()
    {
    	return OnFocusOutFunc.Invoke(Widget);
    }
    
    private async Task HandleOnMouseDown()
    {
        await JsRuntime.GetLuthetusCommonApi()
            .FocusHtmlElementById(WIDGET_HTML_ELEMENT_ID)
            .ConfigureAwait(false);
    }
    
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Key == "Escape")
			WidgetService.ReduceSetWidgetAction(null);
	}
}