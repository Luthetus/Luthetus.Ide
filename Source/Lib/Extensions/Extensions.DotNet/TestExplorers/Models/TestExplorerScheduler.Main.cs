using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

/// <inheritdoc cref="IStateScheduler"/>
public partial class TestExplorerScheduler : IStateScheduler
{
	// Dependency injection
	private readonly LuthetusCommonApi _commonApi;
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ITextEditorService _textEditorService;
	private readonly ITerminalService _terminalService;
	private readonly ITestExplorerService _testExplorerService;
	private readonly IDotNetSolutionService _dotNetSolutionService;
    private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    
    private readonly Throttle _throttleDiscoverTests = new(TimeSpan.FromMilliseconds(100));

    public TestExplorerScheduler(
    	LuthetusCommonApi commonApi,
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        ITextEditorService textEditorService,
        DotNetCliOutputParser dotNetCliOutputParser,
        IDotNetSolutionService dotNetSolutionService,
        ITerminalService terminalService,
        ITestExplorerService testExplorerService)
    {
    	_commonApi = commonApi;
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
		_textEditorService = textEditorService;
		_terminalService = terminalService;
		_testExplorerService = testExplorerService;
        _dotNetSolutionService = dotNetSolutionService;
        _dotNetCliOutputParser = dotNetCliOutputParser;
    }
    
    public TreeViewGroup ContainsTestsTreeViewGroup { get; } = new("Have tests", true, true);
	public TreeViewGroup NoTestsTreeViewGroup { get; } = new("No tests (but still a test-project)", true, true);
	public TreeViewGroup ThrewAnExceptionTreeViewGroup { get; } = new("Projects that threw an exception during discovery", true, true);
	public TreeViewGroup NotValidProjectForUnitTestTreeViewGroup { get; } = new("Not a test-project", true, true);
}

