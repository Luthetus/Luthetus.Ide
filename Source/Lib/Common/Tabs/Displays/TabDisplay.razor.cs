using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Resizes.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabDisplay : ComponentBase, IDisposable
{
	[Inject]
    private IDragService DragService { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

	[CascadingParameter(Name=nameof(HandleTabButtonOnContextMenu)), EditorRequired]
	public Func<TabContextMenuEventArgs, Task>? HandleTabButtonOnContextMenu { get; set; }

	[Parameter, EditorRequired]
	public ITab Tab { get; set; } = null!;

	[Parameter]
	public string? CssClassString { get; set; }
	[Parameter]
	public bool ShouldDisplayCloseButton { get; set; } = true;
	[Parameter]
	public string? CssStyleString { get; set; }
	[Parameter]
	public bool IsBeingDragged { get; set; }

    private bool _thinksLeftMouseButtonIsDown;
	private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

	private string _htmlIdDragged = null;
	private string _htmlId = null;

	private ElementReference? _tabButtonElementReference;

	private string HtmlId => IsBeingDragged
		? _htmlId ??= $"luth_polymorphic-tab_{Tab.DynamicViewModelKey.Guid}"
		: _htmlIdDragged ??= $"luth_polymorphic-tab-drag_{Tab.DynamicViewModelKey.Guid}";

	private string IsActiveCssClass => (Tab.TabGroup?.GetIsActive(Tab) ?? false)
		? "luth_active"
	    : string.Empty;

	protected override void OnInitialized()
    {
        DragService.DragStateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void DragStateWrapOnStateChanged()
    {
		if (IsBeingDragged)
			return;

        if (!DragService.GetDragState().ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
            _thinksLeftMouseButtonIsDown = false;
        }
        else
        {
            var mouseEventArgs = DragService.GetDragState().MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null && mouseEventArgs is not null)
                {
                    await _dragEventHandler
                        .Invoke((_previousDragMouseEventArgs, mouseEventArgs))
                        .ConfigureAwait(false);
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
    
    private async Task OnClick(ITab localTabViewModel, MouseEventArgs e)
    {
    	if (IsBeingDragged)
			return;
		
		var localTabGroup = localTabViewModel.TabGroup;
		if (localTabGroup is null)
			return;
			
		await localTabGroup.OnClickAsync(localTabViewModel, e).ConfigureAwait(false);
    }

	private async Task CloseTabOnClickAsync()
	{
		if (IsBeingDragged)
			return;
			
		var localTabViewModel = Tab;

        var localTabGroup = localTabViewModel.TabGroup;
		if (localTabGroup is null)
			return;
        
        await localTabGroup.CloseAsync(Tab).ConfigureAwait(false);
	}

	private async Task HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs)
	{
		if (IsBeingDragged)
			return;

		if (mouseEventArgs.Button == 0)
	        _thinksLeftMouseButtonIsDown = true;
		if (mouseEventArgs.Button == 1)
            await CloseTabOnClickAsync().ConfigureAwait(false);
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

		CommonBackgroundTaskApi.Enqueue(new CommonWorkArgs
		{
    		WorkKind = CommonWorkKind.Tab_ManuallyPropagateOnContextMenu,
			HandleTabButtonOnContextMenu = localHandleTabButtonOnContextMenu,
            TabContextMenuEventArgs = new TabContextMenuEventArgs(mouseEventArgs, tab, FocusAsync),
		});
    }

	private async Task FocusAsync()
	{
		try
		{
			var localTabButtonElementReference = _tabButtonElementReference;

			if (localTabButtonElementReference is not null)
			{
				await localTabButtonElementReference.Value
					.FocusAsync()
					.ConfigureAwait(false);
			}
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
			var measuredHtmlElementDimensions = await CommonBackgroundTaskApi.JsRuntimeCommonApi
                .MeasureElementById(HtmlId)
                .ConfigureAwait(false);

			await draggable.OnDragStartAsync().ConfigureAwait(false);

			// Width
			{
				draggable.DragElementDimensions.WidthDimensionAttribute.DimensionUnitList.Clear();
	            draggable.DragElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
	            	measuredHtmlElementDimensions.WidthInPixels,
	            	DimensionUnitKind.Pixels));
			}

			// Height
			{
				draggable.DragElementDimensions.HeightDimensionAttribute.DimensionUnitList.Clear();
	            draggable.DragElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
	            	measuredHtmlElementDimensions.HeightInPixels,
	            	DimensionUnitKind.Pixels));
			}

			// Left
			{
				draggable.DragElementDimensions.LeftDimensionAttribute.DimensionUnitList.Clear();
	            draggable.DragElementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
	            	mouseEventArgs.ClientX,
	            	DimensionUnitKind.Pixels));
			}

			// Top
			{
				draggable.DragElementDimensions.TopDimensionAttribute.DimensionUnitList.Clear();
	            draggable.DragElementDimensions.TopDimensionAttribute.DimensionUnitList.Add(new DimensionUnit(
	            	mouseEventArgs.ClientY,
	            	DimensionUnitKind.Pixels));
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

		DragService.ReduceShouldDisplayAndMouseEventArgsAndDragSetAction(true, null, draggable);
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
        DragService.DragStateChanged -= DragStateWrapOnStateChanged;
    }
}
