using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays.Internals;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionVisualizationDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ICompilerServiceRegistry InterfaceCompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IState<AppDimensionState> AppDimensionState { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	[CascadingParameter]
	public IDialog Dialog { get; set; }

	private readonly Throttle _stateChangedThrottle = new(Throttle.Thirty_Frames_Per_Second);

	private SolutionVisualizationModel _solutionVisualizationModel;
	private DotNetSolutionCompilerService _dotNetSolutionCompilerService;
	private CSharpProjectCompilerService _cSharpProjectCompilerService;
	private CSharpCompilerService _cSharpCompilerService;
	private LuthetusCommonJavaScriptInteropApi? _commonJavaScriptInteropApi;
    private MouseEventArgs? _mostRecentMouseEventArgs;

	public Guid IdSalt { get; } = Guid.NewGuid();

	public string DivHtmlElementId => _encompassingDivHtmlElementId ??= $"luth_ide_solution-visualization-div_{IdSalt}";
	public string SvgHtmlElementId => _encompassingDivHtmlElementId ??= $"luth_ide_solution-visualization-svg_{IdSalt}";
	private LuthetusCommonJavaScriptInteropApi CommonJavaScriptInteropApi => _commonJavaScriptInteropApi ??= JsRuntime.GetLuthetusCommonApi();

	protected override void OnInitialized()
	{
		AppDimensionState.StateChanged += OnAppDimensionStateChanged;

		SubscribeTo_DotNetSolutionCompilerService();
		SubscribeTo_CSharpProjectCompilerService();
		SubscribeTo_CSharpCompilerService();

		_solutionVisualizationModel = new(OnCompilerServiceChanged);

		OnCompilerServiceChanged();

		base.OnInitialized();
	}

	private async void OnAppDimensionStateChanged()
	{
		var divBoundingClientRect = await CommonJavaScriptInteropApi
			.MeasureElementById(DivHtmlElementId)
			.ConfigureAwait(false);

		var svgBoundingClientRect = await CommonJavaScriptInteropApi
			.MeasureElementById(SvgHtmlElementId)
			.ConfigureAwait(false);
	}

	private void OnCompilerServiceChanged()
	{
		_stateChangedThrottle.Run(_ =>
		{
			MakeDrawing();
			return InvokeAsync(StateHasChanged);
		});

		void MakeDrawing()
		{
			var localSolutionVisualizationModel = _solutionVisualizationModel.ShallowClone();
			localSolutionVisualizationModel.SolutionVisualizationDrawingList.Clear();
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Clear();

			var dotNetSolutionResourceList = _dotNetSolutionCompilerService.CompilerServiceResources;
			var cSharpProjectResourceList = _cSharpProjectCompilerService.CompilerServiceResources;
			var cSharpResourceList = _cSharpCompilerService.CompilerServiceResources;

			var renderCycleIndex = 0;
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());

			foreach (var dotNetSolutionResource in dotNetSolutionResourceList)
			{
				var dotNetSolutionDrawing = new SolutionVisualizationDrawing<DotNetSolutionResource>
				{
					Item = (DotNetSolutionResource)dotNetSolutionResource,
					SolutionVisualizationDrawingKind = SolutionVisualizationDrawingKind.Solution,
					CenterX = 25,
					CenterY = 25,
					Radius = 25,
					Fill = "var(--luth_icon-solution-font-color)",
					RenderCycle = renderCycleIndex,
				};

				localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(dotNetSolutionDrawing);
				localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(dotNetSolutionDrawing);
			}

			renderCycleIndex++;
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());
			var cSharpProjectIndex = 0;

			foreach (var cSharpProjectResource in cSharpProjectResourceList)
			{
				var cSharpProjectDrawing = new SolutionVisualizationDrawing<CSharpProjectResource>
				{
					Item = (CSharpProjectResource)cSharpProjectResource,
					SolutionVisualizationDrawingKind = SolutionVisualizationDrawingKind.Project,
					CenterX = (++cSharpProjectIndex) * 25,
					CenterY = 50,
					Radius = 25,
					Fill = "var(--luth_icon-project-font-color)",
					RenderCycle = renderCycleIndex,
				};

				localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(cSharpProjectDrawing);
				localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(cSharpProjectDrawing);
			}

			_solutionVisualizationModel = localSolutionVisualizationModel;
		}
	}

	private async Task HandleOnContextMenu(MouseEventArgs mouseEventArgs)
    {
        _mostRecentMouseEventArgs = mouseEventArgs;

		// The order of 'StateHasChanged(...)' and 'AddActiveDropdownKey(...)' is important.
		// The ChildContent renders nothing, unless the provider of the child content
		// re-renders now that there is a given '_mostRecentTreeViewContextMenuCommandArgs'
		await InvokeAsync(StateHasChanged);

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            SolutionVisualizationContextMenu.ContextMenuEventDropdownKey));
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

		AppDimensionState.StateChanged -= OnAppDimensionStateChanged;
	}
}