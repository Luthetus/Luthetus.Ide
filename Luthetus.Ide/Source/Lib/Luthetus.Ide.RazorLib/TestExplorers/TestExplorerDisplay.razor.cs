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

	private List<ProjectTestModel> _projectTestModelBag = new();
	private readonly SemaphoreSlim _projectTestModelBagSemaphoreSlim = new(1, 1);

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
		var dotNetSolutionState = DotNetSolutionStateWrap.Value;
		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

		try
		{
			await _projectTestModelBagSemaphoreSlim.WaitAsync();

			if (dotNetSolutionModel is null)
				return;

			var localDotNetProjectBag = dotNetSolutionModel.DotNetProjectBag
				.Where(x => x.DotNetProjectKind == DotNetProjectKind.CSharpProject);

			var localProjectTestModelBag = localDotNetProjectBag.Select(x => new ProjectTestModel
			{
				ProjectIdGuid = x.ProjectIdGuid,
				AbsolutePath = x.AbsolutePath,
				EnqueueDiscoverTestsFunc = callback => Task.CompletedTask,
				ReRenderNodeAction = node => TreeViewService.ReRenderNode(
					TreeViewTestExplorerKey,
					node)
			});

			var localFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestListTests();
	
			var localTreeViewProjectTestModelBag = localProjectTestModelBag.Select(x =>
					(TreeViewNoType)new TreeViewProjectTestModel(
						x,
						CommonComponentRenderers,
						true,
						false))
				.ToArray();

			foreach (var entry in localTreeViewProjectTestModelBag)
			{
				var treeViewProjectTestModel = (TreeViewProjectTestModel)entry;
				
				if (String.IsNullOrWhiteSpace(treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery))
					return;

				treeViewProjectTestModel.Item.EnqueueDiscoverTestsFunc = async callback =>
				{
					var executionTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[
		            	TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];
		
			        var dotNetTestListTestsCommand = new TerminalCommand(
			            treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey,
			            localFormattedCommand,
			            treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery,
			            DotNetTestListTestsCancellationTokenSource.Token,
			            async () => 
						{
							try
							{
								var output = executionTerminalSession.ReadStandardOut(treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey);
			
								treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput = DotNetCliOutputLexer.LexDotNetTestListTestsTerminalOutput(output);
				
				    			// THINKING_ABOUT_TREE_VIEW();
								{
									var splitOutputBag = treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput
										.Select(x => x.Split('.'));
							
									var rootMap = new Dictionary<string, StringFragment>();
	
									foreach (var splitOutput in splitOutputBag)
									{
										var targetMap = rootMap;
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
	
									treeViewProjectTestModel.Item.RootStringFragmentMap = rootMap;
									await callback.Invoke(rootMap);
								}
							}
							catch (Exception)
							{
								await callback.Invoke(new());
								throw;
							}
						});

			        await executionTerminalSession.EnqueueCommandAsync(dotNetTestListTestsCommand);
				};				
			}
	
			var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(localTreeViewProjectTestModelBag);
			var firstNode = localTreeViewProjectTestModelBag.FirstOrDefault();
	
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

			_projectTestModelBag = localProjectTestModelBag.ToList();
		}
		finally
		{
			_projectTestModelBagSemaphoreSlim.Release();
		}

		await InvokeAsync(StateHasChanged);
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
 
    