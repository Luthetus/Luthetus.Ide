using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

/// <inheritdoc cref="IStateScheduler"/>
public partial class TestExplorerScheduler : IStateScheduler
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IState<TestExplorerState> _testExplorerStateWrap;
    private readonly IDispatcher _dispatcher;

    public TestExplorerScheduler(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IState<TerminalState> terminalStateWrap,
        IState<TestExplorerState> testExplorerStateWrap,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
		_textEditorService = textEditorService;
		_backgroundTaskService = backgroundTaskService;
		_terminalStateWrap = terminalStateWrap;
		_testExplorerStateWrap = testExplorerStateWrap;
        _dispatcher = dispatcher;
    }
}

