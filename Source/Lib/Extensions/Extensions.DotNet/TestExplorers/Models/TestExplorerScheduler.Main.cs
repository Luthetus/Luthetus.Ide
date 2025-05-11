using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
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
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly TextEditorService _textEditorService;
    private readonly INotificationService _notificationService;
    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IFileSystemProvider _fileSystemProvider;
	private readonly ITerminalService _terminalService;
	private readonly ITestExplorerService _testExplorerService;
	private readonly IDotNetSolutionService _dotNetSolutionService;
    private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    
    private readonly Throttle _throttleDiscoverTests = new(TimeSpan.FromMilliseconds(100));

    public TestExplorerScheduler(
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        TextEditorService textEditorService,
        INotificationService notificationService,
        BackgroundTaskService backgroundTaskService,
        IFileSystemProvider fileSystemProvider,
        DotNetCliOutputParser dotNetCliOutputParser,
        IDotNetSolutionService dotNetSolutionService,
        ITerminalService terminalService,
        ITestExplorerService testExplorerService)
    {
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
		_textEditorService = textEditorService;
		_notificationService = notificationService;
		_backgroundTaskService = backgroundTaskService;
		_fileSystemProvider = fileSystemProvider;
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

