using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Outlines.Models;

public class OutlineService : IOutlineService
{
	private readonly IJSRuntime _jsRuntime;

	public OutlineService(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}
	
	private LuthetusCommonJavaScriptInteropApi _jsRuntimeCommonApi;

	private OutlineState _outlineState = new();
	
	private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi => _jsRuntimeCommonApi
		??= _jsRuntime.GetLuthetusCommonApi();
		
	public event Action? OutlineStateChanged;
	
	public OutlineState GetOutlineState() => _outlineState;

	public void ReduceSetOutlineAction(
		string? elementId,
		MeasuredHtmlElementDimensions? measuredHtmlElementDimensions,
		bool needsMeasured)
	{
		var inState = GetOutlineState();
	
		_outlineState = inState with
		{
			ElementId = elementId,
			MeasuredHtmlElementDimensions = measuredHtmlElementDimensions,
			NeedsMeasured = needsMeasured,
		};
	
		if (needsMeasured && elementId is not null)
	    {
			_ = Task.Run(async () =>
			{
				var elementDimensions = await JsRuntimeCommonApi
					.MeasureElementById(elementId)
					.ConfigureAwait(false);
					
				ReduceSetMeasurementsAction(
					elementId,
					elementDimensions);
			});
			
			// The state has changed will occur in 'ReduceSetMeasurementsAction'
			return;
		}
		else
		{
			OutlineStateChanged?.Invoke();
			return;
		}
	}
	
	public void ReduceSetMeasurementsAction(
		string? elementId,
		MeasuredHtmlElementDimensions? measuredHtmlElementDimensions)
	{
		var inState = GetOutlineState();
	
		if (inState.ElementId != elementId)
		{
			OutlineStateChanged?.Invoke();
			return;
		}
			
		_outlineState = inState with
		{
			MeasuredHtmlElementDimensions = measuredHtmlElementDimensions,
			NeedsMeasured = false,
		};
		
		OutlineStateChanged?.Invoke();
		return;
	}
}
