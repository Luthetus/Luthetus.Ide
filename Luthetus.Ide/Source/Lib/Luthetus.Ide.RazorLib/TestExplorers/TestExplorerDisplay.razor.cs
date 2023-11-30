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

namespace Luthetus.Ide.RazorLib.TestExplorers;

public partial class TestExplorerDisplay : FluxorComponent
{
	[Inject]
    private IState<TerminalSessionState> TerminalSessionsStateWrap { get; set; } = null!;
	[Inject]
    private InputFileSync InputFileSync { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

	private const string DOTNET_TEST_LIST_TESTS_COMMAND = "dotnet test -t";
	
	public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

	private string _directoryNameForTestDiscovery = string.Empty;
	private List<string> _dotNetTestListTestsCommandOutput = new();
	private Dictionary<string, StringFragment> RootStringFragmentMap = new();
    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;
	private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
	private bool _firstIf = false;

    public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public Key<TerminalCommand> DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public CancellationTokenSource DotNetTestListTestsCancellationTokenSource { get; set; } = new();

    private FormattedCommand FormattedCommand => DotNetCliCommandFormatter.FormatDotNetTestListTests();
	
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

        base.OnInitialized();
    }

	private async Task SetTreeViewAsync()
    {
		var rootStringFragment = new StringFragment(string.Empty);
		rootStringFragment.Map = RootStringFragmentMap;

		var adhocChildren = rootStringFragment.Map.Select(kvp => 
			(TreeViewNoType)new TreeViewStringFragment(
	            kvp.Value,
	            CommonComponentRenderers,
	            true,
	            true))
			.ToArray();

		for (var i = 0; i < adhocChildren.Length; i++)
		{
			var node = (TreeViewStringFragment)adhocChildren[i];
			await node.LoadChildBagAsync();
		}

        var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(adhocChildren);

		var firstNode = adhocChildren.FirstOrDefault();

		var activeNodes = firstNode is null
			? Array.Empty<TreeViewNoType>()
			: new TreeViewNoType[] { firstNode };

        if (!TreeViewService.TryGetTreeViewContainer(TreeViewTestExplorerKey, out _))
        {
			_firstIf = true;

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

	private async Task StartDotNetTestListTestsCommandOnClick()
    {
        var localFormattedCommand = FormattedCommand;

		if (String.IsNullOrWhiteSpace(_directoryNameForTestDiscovery))
			return;

		var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        var dotNetTestListTestsCommand = new TerminalCommand(
            DotNetTestListTestsTerminalCommandKey,
            localFormattedCommand,
            _directoryNameForTestDiscovery,
            DotNetTestListTestsCancellationTokenSource.Token,
            async () => 
			{
				var output = generalTerminalSession.ReadStandardOut(DotNetTestListTestsTerminalCommandKey);

				_dotNetTestListTestsCommandOutput = DotNetCliOutputLexer.LexDotNetTestListTestsTerminalOutput(output);
    			THINKING_ABOUT_TREE_VIEW();
				await InvokeAsync(StateHasChanged);
			});

        await generalTerminalSession.EnqueueCommandAsync(dotNetTestListTestsCommand);
    }

	private void RequestInputFileForTestDiscovery()
    {
        InputFileSync.RequestInputFileStateForm("Directory for Test Discovery",
            async afp =>
            {
                if (afp is null)
                    return;

                _directoryNameForTestDiscovery = afp.Value;

                await InvokeAsync(StateHasChanged);
            },
            afp =>
            {
                if (afp is null || !afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", afp => afp.IsDirectory)
            }.ToImmutableArray());
    }

	private async Task RunTestByFullyQualifiedName(string fullyQualifiedName)
	{
		var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);

		if (String.IsNullOrWhiteSpace(_directoryNameForTestDiscovery) ||
			String.IsNullOrWhiteSpace(fullyQualifiedName))
		{
			return;
		}

		var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        var dotNetTestByFullyQualifiedNameTerminalCommand = new TerminalCommand(
            DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey,
            dotNetTestByFullyQualifiedNameFormattedCommand,
            _directoryNameForTestDiscovery,
            CancellationToken.None,
            () => Task.CompletedTask);

        await generalTerminalSession.EnqueueCommandAsync(dotNetTestByFullyQualifiedNameTerminalCommand);
	}

	private void THINKING_ABOUT_TREE_VIEW()
	{
		var splitOutputBag = _dotNetTestListTestsCommandOutput
			.Select(x => x.Split('.'));

		foreach (var splitOutput in splitOutputBag)
		{
			var targetMap = RootStringFragmentMap;
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

	private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            TestExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

	
}
 
    