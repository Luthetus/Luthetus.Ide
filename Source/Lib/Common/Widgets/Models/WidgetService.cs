using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Widgets.Models;

public class WidgetService : IWidgetService
{
	private readonly IJSRuntime _jsRuntime;

	public WidgetService(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}
	
	private WidgetState _widgetState = new();
	
	private LuthetusCommonJavaScriptInteropApi _jsRuntimeCommonApi;
	
	private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi => _jsRuntimeCommonApi
		??= _jsRuntime.GetLuthetusCommonApi();
	
	public event Action? WidgetStateChanged;
	
	public WidgetState GetWidgetState() => _widgetState;

	/// <summary>
	/// When this action causes the transition from a widget being rendered,
	/// to NO widget being rendered.
	///
	/// Then the user's focus will go "somewhere" so we want
	/// redirect it to the main layout at least so they can use IDE keybinds
	///
	/// As if the "somewhere" their focus moves to is outside the blazor app components
	/// they IDE keybinds won't fire.
	///
	/// TODO: Where does focus go when you delete the element which the user is focused on.
	///
	/// TODO: Prior to focusing the widget (i.e.: NO widget transitions to a widget being rendered)
	///           we should track where the user's focus is, then restore that focus once the
	///           widget is closed.
	/// </summary>
    public void ReduceSetWidgetAction(WidgetModel? widget)
    {
    	var inState = GetWidgetState();
    
    	if (widget != inState.Widget)
    	{
    		if (widget is null)
    		{
    			_ = Task.Run(async () =>
    			{
    				await JsRuntimeCommonApi
		                .FocusHtmlElementById(IDynamicViewModel.DefaultSetFocusOnCloseElementId)
		                .ConfigureAwait(false);
    			});
    		}
    	}
    
        _widgetState = inState with
        {
            Widget = widget,
        };
        
        WidgetStateChanged?.Invoke();
        return;
    }
}
