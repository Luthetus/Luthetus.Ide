using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Widgets.States;

public partial record WidgetState
{
	public class Effector
	{
		private IJSRuntime _jsRuntime;
		private LuthetusCommonJavaScriptInteropApi _jsRuntimeCommonApi;
				
		public Effector(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}
		
		private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi => _jsRuntimeCommonApi
			??= _jsRuntime.GetLuthetusCommonApi();
		
		[EffectMethod]
        public async Task HandleSetWidgetAction(
            SetWidgetAction setWidgetAction,
            IDispatcher dispatcher)
        {
        	if (setWidgetAction.Widget is null && setWidgetAction.ResultedInChange)
        	{
        		await JsRuntimeCommonApi
	                .FocusHtmlElementById(IDynamicViewModel.DefaultSetFocusOnCloseElementId)
	                .ConfigureAwait(false);
        	}
        }
	}
}
