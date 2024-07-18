using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class IdeBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
	private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IDecorationMapperRegistry _decorationMapperRegistry;

    public IdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        ICompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
        GitCliOutputParser gitCliOutputParser,
        IDecorationMapperRegistry decorationMapperRegistry,
        IServiceProvider serviceProvider)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
        _environmentProvider = environmentProvider;
		_fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _compilerServiceRegistry = compilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;
        _gitStateWrap = gitStateWrap;
		_gitCliOutputParser = gitCliOutputParser;
		_decorationMapperRegistry = decorationMapperRegistry;

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
    
    public EditorIdeApi Editor { get; }
    public FileSystemIdeApi FileSystem { get; }
    public FolderExplorerIdeApi FolderExplorer { get; }
    public InputFileIdeApi InputFile { get; }
    public GitIdeApi Git { get; }
}
