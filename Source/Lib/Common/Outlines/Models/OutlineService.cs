using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Outlines.Models;

public class OutlineService : IOutlineService
{
    private readonly object _stateModificationLock = new();

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

	public void SetOutline(
		string? elementId,
		MeasuredHtmlElementDimensions? measuredHtmlElementDimensions,
		bool needsMeasured)
	{
		lock (_stateModificationLock)
		{
			var inState = GetOutlineState();

			_outlineState = inState with
			{
				ElementId = elementId,
				MeasuredHtmlElementDimensions = measuredHtmlElementDimensions,
				NeedsMeasured = needsMeasured,
			};
		}

        if (needsMeasured && elementId is not null)
        {
            _ = Task.Run(async () =>
            {
                var elementDimensions = await JsRuntimeCommonApi
                    .MeasureElementById(elementId)
                    .ConfigureAwait(false);

                SetMeasurements(
                    elementId,
                    elementDimensions);
            });

            return; // The state has changed will occur in 'ReduceSetMeasurementsAction'
        }
        else
        {
            goto finalize;
        }

        finalize:
        OutlineStateChanged?.Invoke();
    }
	
	public void SetMeasurements(
		string? elementId,
		MeasuredHtmlElementDimensions? measuredHtmlElementDimensions)
	{
		lock (_stateModificationLock)
		{
			var inState = GetOutlineState();
	
			if (inState.ElementId != elementId)
			{
				goto finalize;
			}
			
			_outlineState = inState with
			{
				MeasuredHtmlElementDimensions = measuredHtmlElementDimensions,
				NeedsMeasured = false,
			};
		
			goto finalize;
        }

        finalize:
        OutlineStateChanged?.Invoke();
    }
}
