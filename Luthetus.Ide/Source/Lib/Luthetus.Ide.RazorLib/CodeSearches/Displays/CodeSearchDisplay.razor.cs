using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CodeSearches.Displays;

public partial class CodeSearchDisplay : FluxorComponent
{
	[Inject]
	private IState<CodeSearchState> CodeSearchStateStateWrap { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;

	private string InputValue
	{
		get => CodeSearchStateStateWrap.Value.Query;
		set
		{
			if (value is null)
				value = string.Empty;

			Dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
			{
				Query = value
			}));

			Dispatcher.Dispatch(new CodeSearchState.SearchEffect());
		}
	}

	protected override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var dotNetSolutionState = DotNetSolutionStateWrap.Value;
			var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

			if (dotNetSolutionModel is not null)
			{
				var parentDirectory = dotNetSolutionModel.AbsolutePath.ParentDirectory;

				if (parentDirectory is not null)
				{
					Dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
					{
						StartingAbsolutePathForSearch = parentDirectory.Path
					}));
				}
			}
		}

		return base.OnAfterRenderAsync(firstRender);
	}

	private string GetIsActiveCssClass(CodeSearchFilterKind codeSearchFilterKind)
	{
		return CodeSearchStateStateWrap.Value.CodeSearchFilterKind == codeSearchFilterKind
			? "luth_active"
			: string.Empty;
	}

	private async Task HandleOnClick(string filePath)
	{
		Dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
		{
			PreviewFilePath = filePath
        }));
	}
	
	private async Task HandleOnDoubleClick(string filePath)
	{
		if (TextEditorConfig.OpenInEditorAsyncFunc is null)
			return;

		await TextEditorConfig.OpenInEditorAsyncFunc
			.Invoke(filePath, ServiceProvider)
			.ConfigureAwait(false);
	}
}