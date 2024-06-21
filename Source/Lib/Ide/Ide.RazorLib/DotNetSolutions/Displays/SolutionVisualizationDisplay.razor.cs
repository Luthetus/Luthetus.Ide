using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays.Internals;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionVisualizationDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ICompilerServiceRegistry InterfaceCompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	[CascadingParameter]
	public IDialog Dialog { get; set; }

	private readonly SolutionVisualizationModel _solutionVisualizationModel = new();

	private DotNetSolutionCompilerService _dotNetSolutionCompilerService;
	private CSharpProjectCompilerService _cSharpProjectCompilerService;
	private CSharpCompilerService _cSharpCompilerService;

    private MouseEventArgs? _mostRecentMouseEventArgs;

	protected override void OnInitialized()
	{
		SubscribeTo_DotNetSolutionCompilerService();
		SubscribeTo_CSharpProjectCompilerService();
		SubscribeTo_CSharpCompilerService();

		base.OnInitialized();
	}

	public async void OnCompilerServiceChanged()
	{
		await InvokeAsync(StateHasChanged);
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
	}
}