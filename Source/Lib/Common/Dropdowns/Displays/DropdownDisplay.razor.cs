using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Common.RazorLib.Contexts.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownDisplay : ComponentBase, IDisposable
{
	[Inject]
	public IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	public IState<AppDimensionState> AppDimensionStateWrap { get; set; } = null!;
	[Inject]
	public IDispatcher Dispatcher { get; set; } = null!;

	[Parameter, EditorRequired]
	public DropdownRecord Dropdown { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<DropdownRecord, Task> OnFocusInFunc { get; set; } = null!;
	[Parameter, EditorRequired]
    public Func<DropdownRecord, Task> OnFocusOutFunc { get; set; } = null!;

	private readonly object _hasPendingEventLock = new();

	private LuthetusCommonJavaScriptInteropApi _jsRuntimeCommonApi;

	private Guid _htmlElementIdSalt = Guid.NewGuid();
	private string _htmlElementId => $"luth_dropdown_{_htmlElementIdSalt}";
	private MeasuredHtmlElementDimensions _htmlElementDimensions;
	private MeasuredHtmlElementDimensions _globalHtmlElementDimensions;
	private bool _isOffScreenHorizontally;
	private bool _isOffScreenVertically;
	private int _renderCount = 1;

	/// <summary>
	/// After repositioning the dropdown, its possible that the dropdown
	/// is offscreen, as it was just too big to fit.
	///
	/// So, only re-position a dropdown after someone fired the event,
	/// and ignore any 'infinite loop' scenarios.
	/// </summary>
	private bool _hasPendingEvent;

	protected override void OnInitialized()
	{
		_jsRuntimeCommonApi = JsRuntime.GetLuthetusCommonApi();

		// TODO: Does this line work in relating to the <see cref="Dropdown"/> parameter...
		//       ...having been changed?
		//	   |
		//       The presumption is that it will work because of the '@key' attribute
		//       which is being used when rendering the component. But this needs proven.
		Dropdown.HtmlElementDimensionsChanged += OnHtmlElementDimensionsChanged;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
#if DEBUG
		AssertInfiniteRenderLoop();
#endif
		
		if (firstRender)
		{
			// Force the initial invocation (as opposed to waiting for the event)
			await RemeasureAndRerender();
		}
		else if (_hasPendingEvent)
		{
			if (_isOffScreenHorizontally || _isOffScreenVertically)
			{
				lock (_hasPendingEventLock)
				{
					_hasPendingEvent = false;
				}

				var inDropdown = Dropdown;

				var outWidth = inDropdown.Width;
				var outHeight = inDropdown.Height;
				var outLeft = inDropdown.Left;
				var outTop = inDropdown.Top;

				if (_isOffScreenHorizontally) // These 'if'(s) are not mutually exclusive
				{
					var horizontalPadding = 5;

					outLeft = Math.Max(
						0,
						(_globalHtmlElementDimensions?.WidthInPixels ?? 0) -
							(_htmlElementDimensions?.WidthInPixels ?? 0) -
							horizontalPadding);
				}

				if (_isOffScreenVertically) // These 'if'(s) are not mutually exclusive
				{
					var verticalPadding = 5;

					outTop = Math.Max(
						0,
						(_globalHtmlElementDimensions?.HeightInPixels ?? 0) -
							(_htmlElementDimensions?.HeightInPixels ?? 0) -
							verticalPadding);
				}

				var outDropdown = inDropdown with
				{
					Width = outWidth,
					Height = outHeight,
					Left = outLeft,
					Top = outTop
				};

				Dispatcher.Dispatch(new DropdownState.FitOnScreenAction(outDropdown));
			}
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private async void OnHtmlElementDimensionsChanged()
	{
		await RemeasureAndRerender();
	}

	private async Task RemeasureAndRerender()
	{
		lock (_hasPendingEventLock)
		{
			_hasPendingEvent = true;
		}

		_htmlElementDimensions = await _jsRuntimeCommonApi.MeasureElementById(_htmlElementId);
		_globalHtmlElementDimensions = await _jsRuntimeCommonApi.MeasureElementById(ContextFacts.GlobalContext.ContextElementId);
		await InvokeAsync(StateHasChanged);
	}

	public string GetStyleCssString(DropdownRecord localDropdown)
	{
		var styleBuilder = new StringBuilder();

		//if (Dropdown.Width is not null)
		//	styleBuilder.Append($"width: {Dropdown.Width.Value.ToCssValue()}px; ");

		//if (Dropdown.Height is not null)
		//	styleBuilder.Append($"height: {Dropdown.Height.Value.ToCssValue()}px; ");

		styleBuilder.Append($"left: {Dropdown.Left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {Dropdown.Top.ToCssValue()}px; ");

		return styleBuilder.ToString();
	}
	
	private Task HandleOnFocusIn()
    {
       return OnFocusInFunc.Invoke(Dropdown);
    }
    
	private Task HandleOnFocusOut()
    {
    	return OnFocusOutFunc.Invoke(Dropdown);
    }

#if DEBUG
	/// <summary>
	/// This method is here because its incredibly simple to cause an infinite loop
	/// when making use of this component.
	///
	/// That is to say, if one renders a <see cref="Menus.Displays.MenuDisplay"/>,
	/// and does not re-use the same instance of the <see cref="Menus.Displays.MenuDisplay.MenuRecord"/> parameter,
	/// then in the blazor lifecycle method 'OnParametersSet()', the code
	/// '!Object.ReferenceEquals(_previousMenuRecord, MenuRecord)' will re-render the
	/// dropdown over and over infinitely.
	///
	/// The check for a different MenuRecord is being done because if the menu displayed changes,
	/// then one would need to re-measure the dropdown to determine if it still fit on-screen.
	/// 
	/// TODO: Change the interactions between this component, and <see cref="Menus.Displays.MenuDisplay.MenuRecord"/>...
	///       ...such that it isn't easy to accidentally create an infinite render loop.
	/// </summary>
	private void AssertInfiniteRenderLoop()
	{
		if (_renderCount++ % 1_000 == 0)
			Console.WriteLine($"The {nameof(DropdownDisplay)} is rendering suspiciously many times. _renderCount: {_renderCount}");
	}
#endif

	public void Dispose()
	{
		Dropdown.HtmlElementDimensionsChanged -= OnHtmlElementDimensionsChanged;
	}
}