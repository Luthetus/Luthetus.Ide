using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Outlines.Models;

public class OutlineService : IOutlineService
{
    private readonly object _stateModificationLock = new();

    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

	public OutlineService(CommonBackgroundTaskApi commonBackgroundTaskApi)
	{
		_commonBackgroundTaskApi = commonBackgroundTaskApi;
	}
	
	private OutlineState _outlineState = new();
		
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
                var elementDimensions = await _commonBackgroundTaskApi.JsRuntimeCommonApi
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
