using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.FileSystems.Displays;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Editors.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.Shareds.Displays;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;
using Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class IdeBackgroundTaskApi : IBackgroundTaskGroup
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly ITreeViewService _treeViewService;
    private readonly IEnvironmentProvider _environmentProvider;
	private readonly IFileSystemProvider _fileSystemProvider;
    private readonly TextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly ITerminalService _terminalService;
	private readonly IDecorationMapperRegistry _decorationMapperRegistry;
	private readonly IDialogService _dialogService;
	private readonly IDropdownService _dropdownService;
	private readonly IPanelService _panelService;
	private readonly INotificationService _notificationService;
	private readonly IInputFileService _inputFileService;
	private readonly IFolderExplorerService _folderExplorerService;
	private readonly ICodeSearchService _codeSearchService;
	private readonly LuthetusTextEditorConfig _textEditorConfig;
	private readonly IThemeService _themeService;
	private readonly IAppOptionsService _appOptionsService;
	private readonly ICommandFactory _commandFactory;
	private readonly ITerminalGroupService _terminalGroupService;
	private readonly LuthetusHostingInformation _luthetusHostingInformation;
	private readonly IIdeHeaderService _ideHeaderService;
	private readonly IServiceProvider _serviceProvider;

    public IdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        ICompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        ITreeViewService treeViewService,
        IEnvironmentProvider environmentProvider,
        IFileSystemProvider fileSystemProvider,
        TextEditorService textEditorService,
        ITerminalService terminalService,
        IDecorationMapperRegistry decorationMapperRegistry,
        IDialogService dialogService,
        IDropdownService dropdownService,
        IPanelService panelService,
        INotificationService notificationService,
        IInputFileService inputFileService,
        IFolderExplorerService folderExplorerService,
        ICodeSearchService codeSearchService,
        LuthetusTextEditorConfig textEditorConfig,
        IThemeService themeService,
        IAppOptionsService appOptionsService,
        ICommandFactory commandFactory,
        ITerminalGroupService terminalGroupService,
        LuthetusHostingInformation luthetusHostingInformation,
        IIdeHeaderService ideHeaderService,
        IServiceProvider serviceProvider)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _treeViewService = treeViewService;
        _environmentProvider = environmentProvider;
		_fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _compilerServiceRegistry = compilerServiceRegistry;
        _terminalService = terminalService;
		_decorationMapperRegistry = decorationMapperRegistry;
		_dialogService = dialogService;
        _dropdownService = dropdownService;
		_panelService = panelService;
		_notificationService = notificationService;
		_inputFileService = inputFileService;
		_folderExplorerService = folderExplorerService;
        _codeSearchService = codeSearchService;
        _textEditorConfig = textEditorConfig;
        _themeService = themeService;
        _appOptionsService = appOptionsService;
        _commandFactory = commandFactory;
        _terminalGroupService = terminalGroupService;
        _luthetusHostingInformation = luthetusHostingInformation;
        _ideHeaderService = ideHeaderService;
        _serviceProvider = serviceProvider;

        Editor = new EditorIdeApi(
            this,
            _backgroundTaskService,
            _textEditorService,
            _commonComponentRenderers,
            _commonBackgroundTaskApi,
            _ideComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _decorationMapperRegistry,
            _compilerServiceRegistry,
            _dialogService,
            _panelService,
            _notificationService,
            _serviceProvider);

        FileSystem = new FileSystemIdeApi(
            this,
            _fileSystemProvider,
            _commonComponentRenderers,
            _backgroundTaskService,
            _notificationService);

        FolderExplorer = new FolderExplorerIdeApi(
            this,
            _fileSystemProvider,
            _environmentProvider,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _treeViewService,
            _backgroundTaskService,
            _folderExplorerService);

        InputFile = new InputFileIdeApi(
            this,
            _ideComponentRenderers,
            _backgroundTaskService,
            _dialogService,
            _inputFileService);
    }
    
    public EditorIdeApi Editor { get; }
    public FileSystemIdeApi FileSystem { get; }
    public FolderExplorerIdeApi FolderExplorer { get; }
    public InputFileIdeApi InputFile { get; }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(IdeBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<IdeBackgroundTaskApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private static readonly Key<IDynamicViewModel> _permissionsDialogKey = Key<IDynamicViewModel>.NewKey();
    private static readonly Key<IDynamicViewModel> _backgroundTaskDialogKey = Key<IDynamicViewModel>.NewKey();
    private static readonly Key<IDynamicViewModel> _solutionVisualizationDialogKey = Key<IDynamicViewModel>.NewKey();

    public void Enqueue_LuthetusIdeInitializerOnInit()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(IdeBackgroundTaskApiWorkKind.LuthetusIdeInitializerOnInit);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_LuthetusIdeInitializerOnInit()
    {
        if (_textEditorConfig.CustomThemeRecordList is not null)
        {
            foreach (var themeRecord in _textEditorConfig.CustomThemeRecordList)
            {
                _themeService.ReduceRegisterAction(themeRecord);
            }
        }

        foreach (var terminalKey in TerminalFacts.WELL_KNOWN_KEYS)
        {
            if (terminalKey == TerminalFacts.GENERAL_KEY)
                AddGeneralTerminal();
            else if (terminalKey == TerminalFacts.EXECUTION_KEY)
                AddExecutionTerminal();
        }

        _codeSearchService.InitializeResizeHandleDimensionUnit(
            new DimensionUnit(
                () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleHeightInPixels / 2,
                DimensionUnitKind.Pixels,
                DimensionOperatorKind.Subtract,
                DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW));

        InitializePanelResizeHandleDimensionUnit();
        InitializePanelTabs();
        _commandFactory.Initialize();

        return ValueTask.CompletedTask;
    }

    private void InitializePanelResizeHandleDimensionUnit()
    {
        // Left
        {
            var leftPanel = PanelFacts.GetTopLeftPanelGroup(_panelService.GetPanelState());
            leftPanel.PanelService = _panelService;

            _panelService.InitializeResizeHandleDimensionUnit(
                leftPanel.Key,
                new DimensionUnit(
                    () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
                    DimensionUnitKind.Pixels,
                    DimensionOperatorKind.Subtract,
                    DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));
        }

        // Right
        {
            var rightPanel = PanelFacts.GetTopRightPanelGroup(_panelService.GetPanelState());
            rightPanel.PanelService = _panelService;

            _panelService.InitializeResizeHandleDimensionUnit(
                rightPanel.Key,
                new DimensionUnit(
                    () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
                    DimensionUnitKind.Pixels,
                    DimensionOperatorKind.Subtract,
                    DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));
        }

        // Bottom
        {
            var bottomPanel = PanelFacts.GetBottomPanelGroup(_panelService.GetPanelState());
            bottomPanel.PanelService = _panelService;

            _panelService.InitializeResizeHandleDimensionUnit(
                bottomPanel.Key,
                new DimensionUnit(
                    () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleHeightInPixels / 2,
                    DimensionUnitKind.Pixels,
                    DimensionOperatorKind.Subtract,
                    DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW));
        }
    }

    private void InitializePanelTabs()
    {
        InitializeLeftPanelTabs();
        InitializeRightPanelTabs();
        InitializeBottomPanelTabs();
    }

    private void InitializeLeftPanelTabs()
    {
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(_panelService.GetPanelState());
        leftPanel.PanelService = _panelService;

        // folderExplorerPanel
        var folderExplorerPanel = new Panel(
            "Folder Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.FolderExplorerContext.ContextKey,
            typeof(FolderExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(folderExplorerPanel);
        _panelService.RegisterPanelTab(leftPanel.Key, folderExplorerPanel, false);

        // SetActivePanelTabAction
        _panelService.SetActivePanelTab(leftPanel.Key, folderExplorerPanel.Key);
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetTopRightPanelGroup(_panelService.GetPanelState());
        rightPanel.PanelService = _panelService;
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelGroup(_panelService.GetPanelState());
        bottomPanel.PanelService = _panelService;

        // terminalGroupPanel
        var terminalGroupPanel = new Panel(
            "Terminal",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.TerminalContext.ContextKey,
            typeof(TerminalGroupDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(terminalGroupPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, terminalGroupPanel, false);
        // This UI has resizable parts that need to be initialized.
        _terminalGroupService.InitializeResizeHandleDimensionUnit(
            new DimensionUnit(
                () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind.Pixels,
                DimensionOperatorKind.Subtract,
                DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));

        // activeContextsPanel
        var activeContextsPanel = new Panel(
            "Active Contexts",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.ActiveContextsContext.ContextKey,
            typeof(ContextsPanelDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(activeContextsPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, activeContextsPanel, false);

        // SetActivePanelTabAction
        //_panelService.SetActivePanelTab(bottomPanel.Key, terminalGroupPanel.Key);
    }

    private void AddGeneralTerminal()
    {
        if (_luthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Wasm ||
            _luthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
        {
            _terminalService.Register(
                new TerminalWebsite(
                    "General",
                    terminal => new TerminalInteractive(terminal),
                    terminal => new TerminalInputStringBuilder(terminal),
                    terminal => new TerminalOutput(
                        terminal,
                        new TerminalOutputFormatterExpand(
                            terminal,
                            _textEditorService,
                            _compilerServiceRegistry,
                            _dialogService,
                            _panelService,
                            _commonBackgroundTaskApi)),
                    _backgroundTaskService,
                    _commonComponentRenderers,
                    _notificationService)
                {
                    Key = TerminalFacts.GENERAL_KEY
                });
        }
        else
        {
            _terminalService.Register(
                new Terminal(
                    "General",
                    terminal => new TerminalInteractive(terminal),
                    terminal => new TerminalInputStringBuilder(terminal),
                    terminal => new TerminalOutput(
                        terminal,
                        new TerminalOutputFormatterExpand(
                            terminal,
                            _textEditorService,
                            _compilerServiceRegistry,
                            _dialogService,
                            _panelService,
                            _commonBackgroundTaskApi)),
                    _backgroundTaskService,
                    _commonComponentRenderers,
                    _notificationService,
                    _terminalService)
                {
                    Key = TerminalFacts.GENERAL_KEY
                });
        }
    }

    private void AddExecutionTerminal()
    {
        if (_luthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Wasm ||
            _luthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
        {
            _terminalService.Register(
                new TerminalWebsite(
                    "Execution",
                    terminal => new TerminalInteractive(terminal),
                    terminal => new TerminalInputStringBuilder(terminal),
                    terminal => new TerminalOutput(
                        terminal,
                        new TerminalOutputFormatterExpand(
                            terminal,
                            _textEditorService,
                            _compilerServiceRegistry,
                            _dialogService,
                            _panelService,
                            _commonBackgroundTaskApi)),
                    _backgroundTaskService,
                    _commonComponentRenderers,
                    _notificationService)
                {
                    Key = TerminalFacts.EXECUTION_KEY
                });
        }
        else
        {
            _terminalService.Register(
                new Terminal(
                    "Execution",
                    terminal => new TerminalInteractive(terminal),
                    terminal => new TerminalInputStringBuilder(terminal),
                    terminal => new TerminalOutput(
                        terminal,
                        new TerminalOutputFormatterExpand(
                            terminal,
                            _textEditorService,
                            _compilerServiceRegistry,
                            _dialogService,
                            _panelService,
                            _commonBackgroundTaskApi)),
                    _backgroundTaskService,
                    _commonComponentRenderers,
                    _notificationService,
                    _terminalService)
                {
                    Key = TerminalFacts.EXECUTION_KEY
                });
        }
    }

    private readonly Queue<IdeHeader> _queue_IdeHeaderOnInit = new();

    public void Enqueue_IdeHeaderOnInit(IdeHeader ideHeader)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(IdeBackgroundTaskApiWorkKind.IdeHeaderOnInit);
            _queue_IdeHeaderOnInit.Enqueue(ideHeader);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public ValueTask Do_IdeHeaderOnInit(IdeHeader ideHeader)
    {
        InitializeMenuFile();
        InitializeMenuTools();
        ideHeader.InitializeMenuView();

        AddAltKeymap(ideHeader);
        return ValueTask.CompletedTask;
    }

    private void InitializeMenuFile()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

        // Menu Option Open
        {
            var menuOptionOpenFile = new MenuOptionRecord(
                "File",
                MenuOptionKind.Other,
                () =>
                {
                    Editor.ShowInputFile();
                    return Task.CompletedTask;
                });

            var menuOptionOpenDirectory = new MenuOptionRecord(
                "Directory",
                MenuOptionKind.Other,
                () =>
                {
                    FolderExplorer.ShowInputFile();
                    return Task.CompletedTask;
                });

            var menuOptionOpenCSharpProject = new MenuOptionRecord(
                "C# Project - TODO: Adhoc Sln",
                MenuOptionKind.Other,
                () =>
                {
                    Editor.ShowInputFile();
                    return Task.CompletedTask;
                });

            var menuOptionOpen = new MenuOptionRecord(
                "Open",
                MenuOptionKind.Other,
                subMenu: new MenuRecord(new List<MenuOptionRecord>()
                {
                    menuOptionOpenFile,
                    menuOptionOpenDirectory,
                    menuOptionOpenCSharpProject,
                }));

            menuOptionsList.Add(menuOptionOpen);
        }

        // Menu Option Permissions
        {
            var menuOptionPermissions = new MenuOptionRecord(
                "Permissions",
                MenuOptionKind.Delete,
                ShowPermissionsDialog);

            menuOptionsList.Add(menuOptionPermissions);
        }

        _ideHeaderService.SetMenuFile(new MenuRecord(menuOptionsList));
    }

    private void InitializeMenuTools()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

        // Menu Option Find All
        {
            var menuOptionFindAll = new MenuOptionRecord(
                "Find All (Ctrl Shift f)",
                MenuOptionKind.Delete,
                () =>
                {
                    _textEditorService.OptionsApi.ShowFindAllDialog();
                    return Task.CompletedTask;
                });

            menuOptionsList.Add(menuOptionFindAll);
        }

        // Menu Option Code Search
        {
            var menuOptionCodeSearch = new MenuOptionRecord(
                "Code Search (Ctrl ,)",
                MenuOptionKind.Delete,
                () =>
                {
                    _commandFactory.CodeSearchDialog ??= new DialogViewModel(
                        Key<IDynamicViewModel>.NewKey(),
                        "Code Search",
                        typeof(CodeSearchDisplay),
                        null,
                        null,
                        true,
                        null);

                    _dialogService.ReduceRegisterAction(_commandFactory.CodeSearchDialog);
                    return Task.CompletedTask;
                });

            menuOptionsList.Add(menuOptionCodeSearch);
        }

        // Menu Option BackgroundTasks
        {
            var menuOptionBackgroundTasks = new MenuOptionRecord(
                "BackgroundTasks",
                MenuOptionKind.Delete,
                () =>
                {
                    var dialogRecord = new DialogViewModel(
                        _backgroundTaskDialogKey,
                        "Background Tasks",
                        typeof(BackgroundTaskDialogDisplay),
                        null,
                        null,
                        true,
                        null);

                    _dialogService.ReduceRegisterAction(dialogRecord);
                    return Task.CompletedTask;
                });

            menuOptionsList.Add(menuOptionBackgroundTasks);
        }

        //// Menu Option Solution Visualization
        //
        // NOTE: This UI element isn't useful yet, and its very unoptimized.
        //       Therefore, it is being commented out. Because given a large enough
        //       solution, clicking this by accident is a bit annoying.
        //
        //{
        //    var menuOptionSolutionVisualization = new MenuOptionRecord(
        //		"Solution Visualization",
        //        MenuOptionKind.Delete,
        //        () => 
        //        {
        //			var dialogRecord = new DialogViewModel(
        //	            _solutionVisualizationDialogKey,
        //	            "Solution Visualization",
        //	            typeof(SolutionVisualizationDisplay),
        //	            null,
        //	            null,
        //				true);
        //	
        //	        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
        //	        return Task.CompletedTask;
        //        });
        //
        //    menuOptionsList.Add(menuOptionSolutionVisualization);
        //}

        _ideHeaderService.SetMenuTools(new MenuRecord(menuOptionsList));
    }

    private Task ShowPermissionsDialog()
    {
        var dialogRecord = new DialogViewModel(
            _permissionsDialogKey,
            "Permissions",
            typeof(PermissionsDisplay),
            null,
            null,
            true,
            null);

        _dialogService.ReduceRegisterAction(dialogRecord);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Add option to allow a user to disable the alt keymap to access to the header button dropdowns.
    /// </summary>
    private void AddAltKeymap(IdeHeader ideHeader)
    {
        _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs()
                {
                    Key = "f",
                    Code = "KeyF",
                    ShiftKey = false,
                    CtrlKey = false,
                    AltKey = true,
                    MetaKey = false,
                    LayerKey = Key<KeymapLayer>.Empty,
                },
                new CommonCommand("Open File Dropdown", "open-file-dropdown", false, async _ => await ideHeader.RenderFileDropdownOnClick()));

        _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs
                {
                    Key = "t",
                    Code = "KeyT",
                    ShiftKey = false,
                    CtrlKey = false,
                    AltKey = true,
                    MetaKey = false,
                    LayerKey = Key<KeymapLayer>.Empty,
                },
                new CommonCommand("Open Tools Dropdown", "open-tools-dropdown", false, async _ => await ideHeader.RenderToolsDropdownOnClick()));

        _ = ContextFacts.GlobalContext.Keymap.TryRegister(
                new KeymapArgs
                {
                    Key = "v",
                    Code = "KeyV",
                    ShiftKey = false,
                    CtrlKey = false,
                    AltKey = true,
                    MetaKey = false,
                    LayerKey = Key<KeymapLayer>.Empty,
                },
                new CommonCommand("Open View Dropdown", "open-view-dropdown", false, async _ => await ideHeader.RenderViewDropdownOnClick()));

        _ = ContextFacts.GlobalContext.Keymap.TryRegister(
            new KeymapArgs
            {
                Key = "r",
                Code = "KeyR",
                ShiftKey = false,
                CtrlKey = false,
                AltKey = true,
                MetaKey = false,
                LayerKey = Key<KeymapLayer>.Empty,
            },
            new CommonCommand("Open Run Dropdown", "open-run-dropdown", false, async _ => await ideHeader.RenderRunDropdownOnClick()));
    }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        IdeBackgroundTaskApiWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case IdeBackgroundTaskApiWorkKind.LuthetusIdeInitializerOnInit:
            {
                return Do_LuthetusIdeInitializerOnInit();
            }
            case IdeBackgroundTaskApiWorkKind.IdeHeaderOnInit:
            {
                var args = _queue_IdeHeaderOnInit.Dequeue();
                return Do_IdeHeaderOnInit(args);
            }
            default:
            {
                Console.WriteLine($"{nameof(IdeBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
