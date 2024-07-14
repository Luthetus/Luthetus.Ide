using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays.Internals;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionVisualizationDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ICompilerServiceRegistry InterfaceCompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IState<AppDimensionState> AppDimensionStateWrap { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;

	[CascadingParameter]
	public IDialog Dialog { get; set; }

	private readonly Throttle _stateChangedThrottle = new(Throttle.Thirty_Frames_Per_Second);

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

		AppDimensionStateWrap.StateChanged += OnAppDimensionStateWrapChanged;
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
			OnAppDimensionStateWrapChanged(null, EventArgs.Empty);
			OnDotNetSolutionStateWrapChanged(null, EventArgs.Empty);
			OnCompilerServiceChanged();
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private async void OnAppDimensionStateWrapChanged(object sender, EventArgs e)
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
		OnAppDimensionStateWrapChanged(sender, e);
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

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }

	private void SubscribeTo_DotNetSolutionCompilerService()
	{
		_dotNetSolutionCompilerService = ((CompilerServiceRegistry)InterfaceCompilerServiceRegistry)
			.DotNetSolutionCompilerService;

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
		_cSharpProjectCompilerService = ((CompilerServiceRegistry)InterfaceCompilerServiceRegistry)
			.CSharpProjectCompilerService;

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
		_cSharpCompilerService = ((CompilerServiceRegistry)InterfaceCompilerServiceRegistry)
			.CSharpCompilerService;

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

		AppDimensionStateWrap.StateChanged -= OnAppDimensionStateWrapChanged;
		DotNetSolutionStateWrap.StateChanged -= OnDotNetSolutionStateWrapChanged;
	}
}