using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Resizes.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Displays;

public partial class PolymorphicTabDisplay : ComponentBase, IDisposable
{
	[Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

	[Parameter, EditorRequired]
	public IPolymorphicTab Tab { get; set; } = null!;
	[Parameter, EditorRequired]
	public bool IsBeingDragged { get; set; }

    private bool _thinksLeftMouseButtonIsDown;
	private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

	private string _htmlIdDragged = null;
	private string _htmlId = null;
	private string HtmlId => IsBeingDragged
		? _htmlId ??= $"luth_polymorphic-tab_{Tab.Key}"
		: _htmlIdDragged ??= $"luth_polymorphic-tab-drag_{Tab.Key}";

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

        await Tab.TabCloseAsync();
	}

	private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
	{
		if (IsBeingDragged)
			return;

		if (mouseEventArgs.Button == 1)
            await CloseTabOnClickAsync();

        _thinksLeftMouseButtonIsDown = true;
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

        if (_thinksLeftMouseButtonIsDown && Tab is IPolymorphicDraggable draggable)
        {
			var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
                "luthetusIde.measureElementById",
                HtmlId);

			await draggable.GetDropzonesAsync();

			// Width
			{
				var widthDimensionAttribute = draggable.DraggableElementDimensions.DimensionAttributeList.First(
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
				var heightDimensionAttribute = draggable.DraggableElementDimensions.DimensionAttributeList.First(
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
				var leftDimensionAttribute = draggable.DraggableElementDimensions.DimensionAttributeList.First(
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
				var topDimensionAttribute = draggable.DraggableElementDimensions.DimensionAttributeList.First(
	                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);
	
	            topDimensionAttribute.DimensionUnitList.Clear();
	            topDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
	            {
	                Value = mouseEventArgs.ClientY,
	                DimensionUnitKind = DimensionUnitKind.Pixels
	            });
			}

            draggable.DraggableElementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            SubscribeToDragEventForScrolling(draggable);
        }
    }

	public void SubscribeToDragEventForScrolling(IPolymorphicDraggable draggable)
    {
		if (IsBeingDragged)
			return;

        _dragEventHandler = DragEventHandlerAsync;

        Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = true,
            MouseEventArgs = null,
			PolymorphicDraggable = draggable
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
        if (Tab is IPolymorphicDraggable draggable &&
			localThinksLeftMouseButtonIsDown &&
            (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
        {
            ResizeHelper.Move(
                draggable.DraggableElementDimensions,
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
		if (IsBeingDragged &&
			Tab is IPolymorphicDraggable draggable)
		{
			return draggable.DraggableElementDimensions.StyleString;
		}

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