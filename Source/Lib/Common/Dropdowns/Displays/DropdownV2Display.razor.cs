using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Common.RazorLib.Contexts.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownV2Display : ComponentBase
{
	[Inject]
	public IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	public IState<AppDimensionState> AppDimensionStateWrap { get; set; } = null!;

	[Parameter, EditorRequired]
	public DropdownRecord Dropdown { get; set; } = null!;

	private LuthetusCommonJavaScriptInteropApi _jsRuntimeCommonApi;

	private Guid _htmlElementIdSalt = Guid.NewGuid();
	private string _htmlElementId => $"luth_dropdown_{_htmlElementIdSalt}";
	private MeasuredHtmlElementDimensions _htmlElementDimensions;
	private MeasuredHtmlElementDimensions _globalHtmlElementDimensions;

	protected override void OnInitialized()
	{
		_jsRuntimeCommonApi = JsRuntime.GetLuthetusCommonApi();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_htmlElementDimensions = await _jsRuntimeCommonApi.MeasureElementById(_htmlElementId);
			_globalHtmlElementDimensions = await _jsRuntimeCommonApi.MeasureElementById(ContextFacts.GlobalContext.ContextElementId);
			await InvokeAsync(StateHasChanged);
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	public string GetStyleCssString(DropdownRecord localDropdown)
	{
		var styleBuilder = new StringBuilder();

		if (Dropdown.Width is not null)
			styleBuilder.Append($"width: {Dropdown.Width.Value.ToCssValue()}px; ");

		if (Dropdown.Height is not null)
			styleBuilder.Append($"height: {Dropdown.Height.Value.ToCssValue()}px; ");

		styleBuilder.Append($"left: {Dropdown.Left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {Dropdown.Top.ToCssValue()}px; ");

		return styleBuilder.ToString();
	}
}