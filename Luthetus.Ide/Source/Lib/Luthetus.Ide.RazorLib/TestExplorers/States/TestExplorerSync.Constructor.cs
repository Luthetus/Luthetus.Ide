using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial class TestExplorerSync
{
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IDispatcher _dispatcher;

    public TestExplorerSync(
		ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IBackgroundTaskService backgroundTaskService,
		IState<DotNetSolutionState> dotNetSolutionStateWrap,
		IState<TerminalState> terminalStateWrap,
        IDispatcher dispatcher)
    {
		_commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _terminalStateWrap = terminalStateWrap;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }
}

