using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.CSharpProject.CompilerServiceCase;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models.Internals;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays;

public partial class SolutionVisualizationDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IAppDimensionService AppDimensionService { get; set; } = null!;
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;

	[CascadingParameter]
	public IDialog Dialog { get; set; }

	private readonly Throttle _stateChangedThrottle = new(ThrottleFacts.TwentyFour_Frames_Per_Second);

	private SolutionVisualizationModel _solutionVisualizationModel;
	private DotNetSolutionCompilerService _dotNetSolutionCompilerService;
	private CSharpProjectCompilerService _cSharpProjectCompilerService;
	private CSharpCompilerService _cSharpCompilerService;
	private LuthetusCommonJavaScriptInteropApi? _commonJavaScriptInteropApi;
	private string _divHtmlElementId;
	private string _svgHtmlElementId;

	public Guid IdSalt { get; } = Guid.NewGuid();

	public string DivHtmlElementId => _divHtmlElementId ??= $"luth_ide_solution-visualization-div_{IdSalt}";
	public string SvgHtmlElementId => _svgHtmlElementId ??= $"luth_ide_solution-visualization-svg_{IdSalt}";
	private LuthetusCommonJavaScriptInteropApi CommonJavaScriptInteropApi => _commonJavaScriptInteropApi ??= JsRuntime.GetLuthetusCommonApi();

	protected override void OnInitialized()
	{
		_solutionVisualizationModel = new(null, OnCompilerServiceChanged);

		AppDimensionService.AppDimensionStateChanged += OnAppDimensionStateWrapChanged;
		DotNetSolutionStateWrap.StateChanged += OnDotNetSolutionStateWrapChanged;

		SubscribeTo_DotNetSolutionCompilerService();
		SubscribeTo_CSharpProjectCompilerService();
		SubscribeTo_CSharpCompilerService();

		base.OnInitialized();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			OnAppDimensionStateWrapChanged();
			OnDotNetSolutionStateWrapChanged(null, EventArgs.Empty);
			OnCompilerServiceChanged();
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private async void OnAppDimensionStateWrapChanged()
	{
		_solutionVisualizationModel.Dimensions.DivBoundingClientRect = await CommonJavaScriptInteropApi
			.MeasureElementById(DivHtmlElementId)
			.ConfigureAwait(false);

		_solutionVisualizationModel.Dimensions.SvgBoundingClientRect = await CommonJavaScriptInteropApi
			.MeasureElementById(SvgHtmlElementId)
			.ConfigureAwait(false);

		OnCompilerServiceChanged();
	}

	private void OnDotNetSolutionStateWrapChanged(object sender, EventArgs e)
	{
		_solutionVisualizationModel = new(DotNetSolutionStateWrap.Value.DotNetSolutionModel?.AbsolutePath, OnCompilerServiceChanged);
		OnAppDimensionStateWrapChanged();
	}

	private void OnCompilerServiceChanged()
	{
		_stateChangedThrottle.Run(_ =>
		{
			_solutionVisualizationModel = _solutionVisualizationModel.MakeDrawing(
				_dotNetSolutionCompilerService,
				_cSharpProjectCompilerService,
				_cSharpCompilerService);

			return InvokeAsync(StateHasChanged);
		});
	}

	private void HandleOnContextMenu(MouseEventArgs mouseEventArgs)
	{
		var dropdownRecord = new DropdownRecord(
			SolutionVisualizationContextMenu.ContextMenuEventDropdownKey,
			mouseEventArgs.ClientX,
			mouseEventArgs.ClientY,
			typeof(SolutionVisualizationContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(SolutionVisualizationContextMenu.MouseEventArgs),
					mouseEventArgs
				},
				{
					nameof(SolutionVisualizationContextMenu.SolutionVisualizationModel),
					_solutionVisualizationModel
				}
			},
			restoreFocusOnClose: null);

		DropdownService.ReduceRegisterAction(dropdownRecord);
	}

	private void SubscribeTo_DotNetSolutionCompilerService()
	{
        _dotNetSolutionCompilerService = (DotNetSolutionCompilerService)CompilerServiceRegistry
			.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

        _dotNetSolutionCompilerService.ResourceRegistered += OnCompilerServiceChanged;
		_dotNetSolutionCompilerService.ResourceParsed += OnCompilerServiceChanged;
		_dotNetSolutionCompilerService.ResourceDisposed += OnCompilerServiceChanged;
	}

	private void DisposeFrom_DotNetSolutionCompilerService()
	{
		_dotNetSolutionCompilerService.ResourceRegistered -= OnCompilerServiceChanged;
		_dotNetSolutionCompilerService.ResourceParsed -= OnCompilerServiceChanged;
		_dotNetSolutionCompilerService.ResourceDisposed -= OnCompilerServiceChanged;
	}

	private void SubscribeTo_CSharpProjectCompilerService()
	{
        _cSharpProjectCompilerService = (CSharpProjectCompilerService)CompilerServiceRegistry
			.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

		_cSharpProjectCompilerService.ResourceRegistered += OnCompilerServiceChanged;
		_cSharpProjectCompilerService.ResourceParsed += OnCompilerServiceChanged;
		_cSharpProjectCompilerService.ResourceDisposed += OnCompilerServiceChanged;
	}

	private void DisposeFrom_CSharpProjectCompilerService()
	{
		_cSharpProjectCompilerService.ResourceRegistered -= OnCompilerServiceChanged;
		_cSharpProjectCompilerService.ResourceParsed -= OnCompilerServiceChanged;
		_cSharpProjectCompilerService.ResourceDisposed -= OnCompilerServiceChanged;
	}

	private void SubscribeTo_CSharpCompilerService()
	{
        _cSharpCompilerService = (CSharpCompilerService)CompilerServiceRegistry
			.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);

        _cSharpCompilerService.ResourceRegistered += OnCompilerServiceChanged;
		_cSharpCompilerService.ResourceParsed += OnCompilerServiceChanged;
		_cSharpCompilerService.ResourceDisposed += OnCompilerServiceChanged;
	}

	private void DisposeFrom_CSharpCompilerService()
	{
		_cSharpCompilerService.ResourceRegistered -= OnCompilerServiceChanged;
		_cSharpCompilerService.ResourceParsed -= OnCompilerServiceChanged;
		_cSharpCompilerService.ResourceDisposed -= OnCompilerServiceChanged;
	}

	public void Dispose()
	{
		DisposeFrom_DotNetSolutionCompilerService();
		DisposeFrom_CSharpProjectCompilerService();
		DisposeFrom_CSharpCompilerService();

		AppDimensionService.AppDimensionStateChanged -= OnAppDimensionStateWrapChanged;
		DotNetSolutionStateWrap.StateChanged -= OnDotNetSolutionStateWrapChanged;
	}
}