using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

namespace Luthetus.Ide.RazorLib.TestExplorers;

public partial class TestExplorerDisplay : ComponentBase, IDisposable
{
	[Inject]
    private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;	
	[Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
	[Inject]
    private InputFileSync InputFileSync { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

	private const string DOTNET_TEST_LIST_TESTS_COMMAND = "dotnet test -t";
	
	public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

	private readonly List<ProjectTestModel> _projectTestModelBag = new();

    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;
	private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

    public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public CancellationTokenSource DotNetTestListTestsCancellationTokenSource { get; set; } = new();
	
	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

	protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new TreeViewKeyboardEventHandler(
            TreeViewService,
			BackgroundTaskService);

        _treeViewMouseEventHandler = new TreeViewMouseEventHandler(
            TreeViewService,
			BackgroundTaskService);

		DotNetSolutionStateWrap.StateChanged += DotNetSolutionStateWrap_StateChanged;
		AppOptionsStateWrap.StateChanged += AppOptionsStateWrap_StateChanged;

        base.OnInitialized();
    }

	private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            TestExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

	private async void DotNetSolutionStateWrap_StateChanged(object? sender, EventArgs e)
	{
		await InvokeAsync(StateHasChanged);

		var dotNetSolutionState = DotNetSolutionStateWrap.Value;
		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

		_projectTestModelBag.Clear();
		
		if (dotNetSolutionModel is null)
			return;

		var localFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestListTests();

		foreach (var project in dotNetSolutionModel.DotNetProjectBag
				 	.Where(x => x.DotNetProjectKind == DotNetProjectKind.CSharpProject))
		{
			var projectTestModel = new ProjectTestModel
			{
				ProjectIdGuid = project.ProjectIdGuid,
				AbsolutePath = project.AbsolutePath
			};

			_projectTestModelBag.Add(projectTestModel);

			if (String.IsNullOrWhiteSpace(projectTestModel.DirectoryNameForTestDiscovery))
				return;
	
			var executionTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[
	            TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];
	
	        var dotNetTestListTestsCommand = new TerminalCommand(
	            projectTestModel.DotNetTestListTestsTerminalCommandKey,
	            localFormattedCommand,
	            projectTestModel.DirectoryNameForTestDiscovery,
	            DotNetTestListTestsCancellationTokenSource.Token,
	            async () => 
				{
					var output = executionTerminalSession.ReadStandardOut(DotNetTestListTestsTerminalCommandKey);
	
					projectTestModel.DotNetTestListTestsCommandOutput = DotNetCliOutputLexer.LexDotNetTestListTestsTerminalOutput(output);
	
	    			// THINKING_ABOUT_TREE_VIEW();
					{
						var splitOutputBag = projectTestModel.DotNetTestListTestsCommandOutput
							.Select(x => x.Split('.'));
				
						foreach (var splitOutput in splitOutputBag)
						{
							var targetMap = projectTestModel.RootStringFragmentMap;
							var lastSeenStringFragment = (StringFragment?)null;
				
							foreach (var fragment in splitOutput)
							{
								if (!targetMap.ContainsKey(fragment))
									targetMap.Add(fragment, new(fragment));
				
								lastSeenStringFragment = targetMap[fragment];
								targetMap = lastSeenStringFragment.Map;
							}
							
							lastSeenStringFragment.IsEndpoint = true;
						}
					}
					
					await InvokeAsync(StateHasChanged);
				});
	
	        await executionTerminalSession.EnqueueCommandAsync(dotNetTestListTestsCommand);
		}

		var adhocChildren = _projectTestModelBag.Select(x =>
		{
			var treeViewProjectTestModel = (TreeViewNoType)new TreeViewProjectTestModel(
				x,
				CommonComponentRenderers,
				true,
				false);

			return treeViewProjectTestModel;
		}).ToArray();

		var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(adhocChildren);

		var firstNode = adhocChildren.FirstOrDefault();

		var activeNodes = firstNode is null
			? Array.Empty<TreeViewNoType>()
			: new []{ firstNode };
			
		if (!TreeViewService.TryGetTreeViewContainer(TreeViewTestExplorerKey, out _))
        {
            TreeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                TreeViewTestExplorerKey,
                adhocRoot,
                activeNodes.ToImmutableList()));
        }
        else
        {
            TreeViewService.SetRoot(TreeViewTestExplorerKey, adhocRoot);
            TreeViewService.SetActiveNode(TreeViewTestExplorerKey, firstNode);
        }		
	}

	private async void AppOptionsStateWrap_StateChanged(object? sender, EventArgs e)
	{
		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		DotNetSolutionStateWrap.StateChanged -= DotNetSolutionStateWrap_StateChanged;
		AppOptionsStateWrap.StateChanged -= AppOptionsStateWrap_StateChanged;
	}
}
 
    