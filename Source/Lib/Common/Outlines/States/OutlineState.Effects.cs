using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Outlines.States;

public partial record OutlineState
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
        public async Task HandleSetOutlineAction(
            SetOutlineAction setOutlineAction,
            IDispatcher dispatcher)
        {
        	if (!setOutlineAction.NeedsMeasured ||
        		setOutlineAction.ElementId is null)
        	{
	        	return;
        	}
        
    		var elementDimensions = await JsRuntimeCommonApi
				.MeasureElementById(setOutlineAction.ElementId)
				.ConfigureAwait(false);
				
			dispatcher.Dispatch(new SetMeasurementsAction(
				setOutlineAction.ElementId,
				elementDimensions));
        }
	}
}
