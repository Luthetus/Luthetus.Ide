using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

public partial record DialogState
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
        public async Task HandleRegisterAction(
            RegisterAction registerAction,
            IDispatcher dispatcher)
        {
        	if (registerAction.WasAlreadyRegistered)
        	{
        		await JsRuntimeCommonApi
	                .FocusHtmlElementById(registerAction.Dialog.DialogFocusPointHtmlElementId)
	                .ConfigureAwait(false);
        	}
        }
	}
}
