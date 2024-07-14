using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

/// <inheritdoc cref="IStateScheduler"/>
public partial class TestExplorerScheduler : IStateScheduler
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IState<TestExplorerState> _testExplorerStateWrap;
    private readonly IDispatcher _dispatcher;

    public TestExplorerScheduler(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        DotNetCliOutputParser dotNetCliOutputParser,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IState<TerminalState> terminalStateWrap,
        IState<TestExplorerState> testExplorerStateWrap,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
		_textEditorService = textEditorService;
		_backgroundTaskService = backgroundTaskService;
		_dotNetCliOutputParser = dotNetCliOutputParser;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		_terminalStateWrap = terminalStateWrap;
		_testExplorerStateWrap = testExplorerStateWrap;
        _dispatcher = dispatcher;
    }
}

