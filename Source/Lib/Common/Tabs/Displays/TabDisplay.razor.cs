using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Resizes.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabDisplay : ComponentBase, IDisposable
{
	[Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

	[CascadingParameter(Name=nameof(HandleTabButtonOnContextMenu)), EditorRequired]
	public Func<TabContextMenuEventArgs, Task>? HandleTabButtonOnContextMenu { get; set; }

	[Parameter, EditorRequired]
	public ITab Tab { get; set; } = null!;

	[Parameter]
	public string CssClassString { get; set; }
	[Parameter]
	public bool ShouldDisplayCloseButton { get; set; } = true;
	[Parameter]
	public string CssStyleString { get; set; }
	[Parameter]
	public bool IsBeingDragged { get; set; }

    private bool _thinksLeftMouseButtonIsDown;
	private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

	private string _htmlIdDragged = null;
	private string _htmlId = null;

	private ElementReference? _tabButtonElementReference;

	private string HtmlId => IsBeingDragged
		? _htmlId ??= $"luth_polymorphic-tab_{Tab.DynamicViewModelKey}"
		: _htmlIdDragged ??= $"luth_polymorphic-tab-drag_{Tab.DynamicViewModelKey}";

	private string IsActiveCssClass => (Tab.TabGroup?.GetIsActive(Tab) ?? false)
		? "luth_active"
	    : string.Empty;

	protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
		if (IsBeingDragged)
			return;

        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
            _thinksLeftMouseButtonIsDown = false;
        }
        else
        {
            var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null && mouseEventArgs is not null)
                {
                    await _dragEventHandler
                        .Invoke((_previousDragMouseEventArgs, mouseEventArgs));
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

	private async Task CloseTabOnClickAsync()
	{
		if (IsBeingDragged)
			return;

        await Tab.TabGroup.CloseAsync(Tab);
	}

	private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
	{
		if (IsBeingDragged)
			return;

		if (mouseEventArgs.Button == 0)
	        _thinksLeftMouseButtonIsDown = true;
		if (mouseEventArgs.Button == 1)
            await CloseTabOnClickAsync();
		else if (mouseEventArgs.Button == 2)
			ManuallyPropagateOnContextMenu(mouseEventArgs, Tab);
	}

    private void ManuallyPropagateOnContextMenu(
        MouseEventArgs mouseEventArgs,
        ITab tab)
    {
		var localHandleTabButtonOnContextMenu = HandleTabButtonOnContextMenu;

		if (localHandleTabButtonOnContextMenu is null)
			return;

		BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Tab.ManuallyPropagateOnContextMenu",
            async () => await localHandleTabButtonOnContextMenu.Invoke(
				new TabContextMenuEventArgs(mouseEventArgs, tab, FocusAsync)));
    }

	private async Task FocusAsync()
	{
		try
		{
			var localTabButtonElementReference = _tabButtonElementReference;

			if (localTabButtonElementReference is not null)
				await localTabButtonElementReference.Value.FocusAsync();
		}
		catch (Exception)
		{
			// 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
			//             This bug is seemingly happening randomly. I have a suspicion
			//             that there are race-condition exceptions occurring with "FocusAsync"
			//             on an ElementReference.
		}
	}

	private void HandleOnMouseUp()
    {
		if (IsBeingDragged)
			return;

        _thinksLeftMouseButtonIsDown = false;
    }

	private async Task HandleOnMouseOutAsync(MouseEventArgs mouseEventArgs)
    {
		if (IsBeingDragged)
			return;

        if (_thinksLeftMouseButtonIsDown && Tab is IDrag draggable)
        {
			var measuredHtmlElementDimensions = await JsRuntime.GetLuthetusCommonApi()
                .MeasureElementById(
                    HtmlId);

			await draggable.OnDragStartAsync();

			// Width
			{
				var widthDimensionAttribute = draggable.DragElementDimensions.DimensionAttributeList.First(
	                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);
	
				widthDimensionAttribute.DimensionUnitList.Clear();
	            widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
	            {
	                Value = measuredHtmlElementDimensions.WidthInPixels,
	                DimensionUnitKind = DimensionUnitKind.Pixels
	            });
			}

			// Height
			{
				var heightDimensionAttribute = draggable.DragElementDimensions.DimensionAttributeList.First(
	                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);
	
				heightDimensionAttribute.DimensionUnitList.Clear();
	            heightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
	            {
	                Value = measuredHtmlElementDimensions.HeightInPixels,
	                DimensionUnitKind = DimensionUnitKind.Pixels
	            });
			}

			// Left
			{
				var leftDimensionAttribute = draggable.DragElementDimensions.DimensionAttributeList.First(
	                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);
	
	            leftDimensionAttribute.DimensionUnitList.Clear();
	            leftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
	            {
	                Value = mouseEventArgs.ClientX,
	                DimensionUnitKind = DimensionUnitKind.Pixels
	            });
			}

			// Top
			{
				var topDimensionAttribute = draggable.DragElementDimensions.DimensionAttributeList.First(
	                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);
	
	            topDimensionAttribute.DimensionUnitList.Clear();
	            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
	            {
	                Value = mouseEventArgs.ClientY,
	                DimensionUnitKind = DimensionUnitKind.Pixels
	            });
			}

            draggable.DragElementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            SubscribeToDragEventForScrolling(draggable);
        }
    }

	public void SubscribeToDragEventForScrolling(IDrag draggable)
    {
		if (IsBeingDragged)
			return;

        _dragEventHandler = DragEventHandlerAsync;

        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = true,
            MouseEventArgs = null,
			Drag = draggable,
        }));
    }

	private Task DragEventHandlerAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
		if (IsBeingDragged)
			return Task.CompletedTask;

        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;

        if (!localThinksLeftMouseButtonIsDown)
            return Task.CompletedTask;

        // Buttons is a bit flag '& 1' gets if left mouse button is held
        if (Tab is IDrag draggable &&
			localThinksLeftMouseButtonIsDown &&
            (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
        {
            ResizeHelper.Move(
                draggable.DragElementDimensions,
                mouseEventArgsTuple.firstMouseEventArgs,
                mouseEventArgsTuple.secondMouseEventArgs);
        }
        else
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
            _thinksLeftMouseButtonIsDown = false;
        }

        return Task.CompletedTask;
    }

	private string GetDraggableCssStyleString()
	{
		if (IsBeingDragged && Tab is IDrag draggable)
			return draggable.DragElementDimensions.StyleString;

		return string.Empty;
	}

	private string GetIsBeingDraggedCssClassString()
	{
		return IsBeingDragged
			? "luth_drag"
			: string.Empty;
	}

	public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}