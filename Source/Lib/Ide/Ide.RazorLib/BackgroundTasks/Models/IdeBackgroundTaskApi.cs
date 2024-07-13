using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.TestExplorers.States;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class IdeBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly CompilerServiceRegistry _compilerServiceRegistry;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _interfaceCompilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IDecorationMapperRegistry _decorationMapperRegistry;
	private readonly IState<TestExplorerState> _testExplorerStateWrap;

    public IdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        ICompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
		DotNetCliOutputParser dotNetCliOutputParser,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
        GitCliOutputParser gitCliOutputParser,
        IDecorationMapperRegistry decorationMapperRegistry,
        IState<TestExplorerState> testExplorerStateWrap,
        IServiceProvider serviceProvider)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _compilerServiceRegistry = (CompilerServiceRegistry)compilerServiceRegistry;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
        _environmentProvider = environmentProvider;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _interfaceCompilerServiceRegistry = interfaceCompilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;
        _gitStateWrap = gitStateWrap;
		_gitCliOutputParser = gitCliOutputParser;
		_decorationMapperRegistry = decorationMapperRegistry;
		_testExplorerStateWrap = testExplorerStateWrap;

        CompilerService = new CompilerServiceIdeApi(
            this,
            _backgroundTaskService,
            _compilerServiceExplorerStateWrap,
            _compilerServiceRegistry,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _dispatcher);

        DotNetSolution = new DotNetSolutionIdeApi(
            this,
            _backgroundTaskService,
            _storageService,
            _compilerServiceExplorerStateWrap,
            _compilerServiceRegistry,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _dispatcher,
            _environmentProvider,
            _dotNetSolutionStateWrap,
            _fileSystemProvider,
            _textEditorService,
            _interfaceCompilerServiceRegistry,
            _terminalStateWrap,
			serviceProvider);

        Editor = new EditorIdeApi(
            this,
            _backgroundTaskService,
            _textEditorService,
            _ideComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _decorationMapperRegistry,
            _compilerServiceRegistry,
            _dispatcher,
            serviceProvider);

        FileSystem = new FileSystemIdeApi(
            this,
            _fileSystemProvider,
            _commonComponentRenderers,
            _backgroundTaskService,
            _dispatcher);

        FolderExplorer = new FolderExplorerIdeApi(
            this,
            _fileSystemProvider,
            _environmentProvider,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _backgroundTaskService,
            _dispatcher);

        InputFile = new InputFileIdeApi(
            this,
            _ideComponentRenderers,
            _backgroundTaskService,
            _dispatcher);

        TestExplorer = new TestExplorerIdeApi(
            this,
            _commonComponentRenderers,
            _treeViewService,
			_textEditorService,
            _backgroundTaskService,
			_dotNetCliOutputParser,
            _dotNetSolutionStateWrap,
			_terminalStateWrap,
			_testExplorerStateWrap,
            _dispatcher);

		Git = new GitIdeApi(
			this,
			_terminalStateWrap,
			_gitStateWrap,
			_gitCliOutputParser,
			_environmentProvider,
			_backgroundTaskService,
            _commonComponentRenderers,
			_dispatcher);
    }
    
    public CompilerServiceIdeApi CompilerService { get; }
    public DotNetSolutionIdeApi DotNetSolution { get; }
    public EditorIdeApi Editor { get; }
    public FileSystemIdeApi FileSystem { get; }
    public FolderExplorerIdeApi FolderExplorer { get; }
    public InputFileIdeApi InputFile { get; }
    public TestExplorerIdeApi TestExplorer { get; }
    public GitIdeApi Git { get; }
}
