using System.Collections.Concurrent;
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
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
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
	public static readonly Key<TextEditorGroup> EditorTextEditorGroupKey = Key<TextEditorGroup>.NewKey();

    private readonly BackgroundTaskService _backgroundTaskService;
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
        BackgroundTaskService backgroundTaskService,
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
    }
    
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<IdeBackgroundTaskApiWorkArgs> _workQueue = new();

    private static readonly Key<IDynamicViewModel> _permissionsDialogKey = Key<IDynamicViewModel>.NewKey();
    private static readonly Key<IDynamicViewModel> _backgroundTaskDialogKey = Key<IDynamicViewModel>.NewKey();
    private static readonly Key<IDynamicViewModel> _solutionVisualizationDialogKey = Key<IDynamicViewModel>.NewKey();

    public void Enqueue(IdeBackgroundTaskApiWorkArgs workArgs)
    {
        _workQueue.Enqueue(workArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
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
                    Editor_ShowInputFile();
                    return Task.CompletedTask;
                });

            var menuOptionOpenDirectory = new MenuOptionRecord(
                "Directory",
                MenuOptionKind.Other,
                () =>
                {
                    FolderExplorer_ShowInputFile();
                    return Task.CompletedTask;
                });

            var menuOptionOpen = new MenuOptionRecord(
                "Open",
                MenuOptionKind.Other,
                subMenu: new MenuRecord(new List<MenuOptionRecord>()
                {
                    menuOptionOpenFile,
                    menuOptionOpenDirectory,
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

        /*// Menu Option BackgroundTasks
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
        }*/

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
    
    public void Editor_ShowInputFile()
    {
        Enqueue(new IdeBackgroundTaskApiWorkArgs
        {
        	WorkKind = IdeBackgroundTaskApiWorkKind.RequestInputFileStateForm,
            Message = "TextEditor",
            OnAfterSubmitFunc = absolutePath =>
            {
            	// TODO: Why does 'isDirectory: false' not work?
				_environmentProvider.DeletionPermittedRegister(new(absolutePath.Value, isDirectory: true));
            
            	_textEditorService.WorkerArbitrary.PostUnique(async editContext =>
				{
					await _textEditorService.OpenInEditorAsync(
						editContext,
						absolutePath.Value,
						true,
						null,
						new Category("main"),
						Key<TextEditorViewModel>.NewKey());
				});
				return Task.CompletedTask;
            },
            SelectionIsValidFunc = absolutePath =>
            {
                if (absolutePath.ExactInput is null || absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            InputFilePatterns = new()
            {
            	new InputFilePattern("File", absolutePath => !absolutePath.IsDirectory)
            }
        });
    }

    public async Task Editor_FastParseFunc(FastParseArgs fastParseArgs)
    {
        var resourceUri = fastParseArgs.ResourceUri;

        var compilerService = _compilerServiceRegistry.GetCompilerService(fastParseArgs.ExtensionNoPeriod);

		compilerService.RegisterResource(
			fastParseArgs.ResourceUri,
			shouldTriggerResourceWasModified: false);
			
		var uniqueTextEditorWork = new UniqueTextEditorWork(_textEditorService, editContext =>
			compilerService.FastParseAsync(editContext, fastParseArgs.ResourceUri, _fileSystemProvider));
		
		_textEditorService.WorkerArbitrary.EnqueueUniqueTextEditorWork(uniqueTextEditorWork);
    }
    
    public async Task Editor_RegisterModelFunc(RegisterModelArgs registerModelArgs)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(registerModelArgs.ResourceUri);
        
        if (model is not null)
        {
        	await Editor_CheckIfContentsWereModifiedAsync(
	                registerModelArgs.ResourceUri.Value,
	                model)
	            .ConfigureAwait(false);
	        return;
        }
			
    	var resourceUri = registerModelArgs.ResourceUri;

        var fileLastWriteTime = await _fileSystemProvider.File
            .GetLastWriteTimeAsync(resourceUri.Value)
            .ConfigureAwait(false);

        var content = await _fileSystemProvider.File
            .ReadAllTextAsync(resourceUri.Value)
            .ConfigureAwait(false);

        var absolutePath = _environmentProvider.AbsolutePathFactory(resourceUri.Value, false);
        var decorationMapper = _decorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
        var compilerService = _compilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

        model = new TextEditorModel(
            resourceUri,
            fileLastWriteTime,
            absolutePath.ExtensionNoPeriod,
            content,
            decorationMapper,
            compilerService,
            _textEditorService);
            
        var modelModifier = new TextEditorModel(model);
        modelModifier.PerformRegisterPresentationModelAction(CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);
        modelModifier.PerformRegisterPresentationModelAction(FindOverlayPresentationFacts.EmptyPresentationModel);
        modelModifier.PerformRegisterPresentationModelAction(DiffPresentationFacts.EmptyInPresentationModel);
        modelModifier.PerformRegisterPresentationModelAction(DiffPresentationFacts.EmptyOutPresentationModel);
        
        model = modelModifier;

        _textEditorService.ModelApi.RegisterCustom(registerModelArgs.EditContext, model);
        
		model.PersistentState.CompilerService.RegisterResource(
			model.PersistentState.ResourceUri,
			shouldTriggerResourceWasModified: false);
    	
		modelModifier = registerModelArgs.EditContext.GetModelModifier(resourceUri);

		if (modelModifier is null)
			return;

		await compilerService.ParseAsync(registerModelArgs.EditContext, modelModifier, shouldApplySyntaxHighlighting: false);
    }

    public async Task<Key<TextEditorViewModel>> Editor_TryRegisterViewModelFunc(TryRegisterViewModelArgs registerViewModelArgs)
    {
    	var viewModelKey = Key<TextEditorViewModel>.NewKey();
    	
		var model = _textEditorService.ModelApi.GetOrDefault(registerViewModelArgs.ResourceUri);

        if (model is null)
        {
        	NotificationHelper.DispatchDebugMessage(nameof(Editor_TryRegisterViewModelFunc), () => "model is null: " + registerViewModelArgs.ResourceUri.Value, _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(4));
            return Key<TextEditorViewModel>.Empty;
        }

        var viewModel = _textEditorService.ModelApi
            .GetViewModelsOrEmpty(registerViewModelArgs.ResourceUri)
            .FirstOrDefault(x => x.PersistentState.Category == registerViewModelArgs.Category);

        if (viewModel is not null)
		    return viewModel.PersistentState.ViewModelKey;

        viewModel = new TextEditorViewModel(
            viewModelKey,
            registerViewModelArgs.ResourceUri,
            _textEditorService,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi,
            VirtualizationGrid.Empty,
			new TextEditorDimensions(0, 0, 0, 0),
			scrollLeft: 0,
	    	scrollTop: 0,
		    scrollWidth: 0,
		    scrollHeight: 0,
		    marginScrollHeight: 0,
            registerViewModelArgs.Category);

        var firstPresentationLayerKeys = new List<Key<TextEditorPresentationModel>>
        {
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        };

        var absolutePath = _environmentProvider.AbsolutePathFactory(
            registerViewModelArgs.ResourceUri.Value,
            false);

        viewModel.PersistentState.OnSaveRequested = Editor_HandleOnSaveRequested;
        viewModel.PersistentState.GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension;
        viewModel.PersistentState.FirstPresentationLayerKeysList = firstPresentationLayerKeys;
        
        _textEditorService.ViewModelApi.Register(registerViewModelArgs.EditContext, viewModel);
        return viewModelKey;
    }
    
    private void Editor_HandleOnSaveRequested(TextEditorModel innerTextEditor)
    {
        var innerContent = innerTextEditor.GetAllText();
        
        var absolutePath = _environmentProvider.AbsolutePathFactory(
            innerTextEditor.PersistentState.ResourceUri.Value,
            false);

        Enqueue(new IdeBackgroundTaskApiWorkArgs
        {
        	WorkKind = IdeBackgroundTaskApiWorkKind.SaveFile,
            AbsolutePath = absolutePath,
            Content = innerContent,
            OnAfterSaveCompletedWrittenDateTimeFunc = writtenDateTime =>
            {
                if (writtenDateTime is not null)
                {
                    _textEditorService.WorkerArbitrary.PostUnique(editContext =>
                    {
                    	var modelModifier = editContext.GetModelModifier(innerTextEditor.PersistentState.ResourceUri);
                    	if (modelModifier is null)
                    		return ValueTask.CompletedTask;
                    
                    	_textEditorService.ModelApi.SetResourceData(
                    		editContext,
                            modelModifier,
                            writtenDateTime.Value);
                        return ValueTask.CompletedTask;
                    });
                }

                return Task.CompletedTask;
            },
            CancellationToken = CancellationToken.None
        });
    }

    public async Task<bool> Editor_TryShowViewModelFunc(TryShowViewModelArgs showViewModelArgs)
    {
        _textEditorService.GroupApi.Register(EditorTextEditorGroupKey);

        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(showViewModelArgs.ViewModelKey);

        if (viewModel is null)
            return false;

        if (viewModel.PersistentState.Category == new Category("main") &&
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            showViewModelArgs = new TryShowViewModelArgs(
                showViewModelArgs.ViewModelKey,
                EditorTextEditorGroupKey,
                showViewModelArgs.ShouldSetFocusToEditor,
                showViewModelArgs.ServiceProvider);
        }

        if (showViewModelArgs.ViewModelKey == Key<TextEditorViewModel>.Empty ||
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            return false;
        }

        _textEditorService.GroupApi.AddViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);

        _textEditorService.GroupApi.SetActiveViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);
            
        if (showViewModelArgs.ShouldSetFocusToEditor)
        {
        	_textEditorService.WorkerArbitrary.PostUnique(editContext =>
	        {
	        	var viewModelModifier = editContext.GetViewModelModifier(showViewModelArgs.ViewModelKey);
	        	return viewModel.FocusAsync();
	        });
        }

        return true;
    }

    private async Task Editor_CheckIfContentsWereModifiedAsync(
        string inputFileAbsolutePathString,
        TextEditorModel textEditorModel)
    {
        var fileLastWriteTime = await _fileSystemProvider.File
            .GetLastWriteTimeAsync(inputFileAbsolutePathString)
            .ConfigureAwait(false);

        if (fileLastWriteTime > textEditorModel.ResourceLastWriteTime &&
            _ideComponentRenderers.BooleanPromptOrCancelRendererType is not null)
        {
            var notificationInformativeKey = Key<IDynamicViewModel>.NewKey();

            var notificationInformative = new NotificationViewModel(
                notificationInformativeKey,
                "File contents were modified on disk",
                _ideComponentRenderers.BooleanPromptOrCancelRendererType,
                new Dictionary<string, object?>
                {
                        {
                            nameof(IBooleanPromptOrCancelRendererType.Message),
                            "File contents were modified on disk"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.AcceptOptionTextOverride),
                            "Reload"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterAcceptFunc),
                            new Func<Task>(() =>
                            {
                            	Enqueue(new IdeBackgroundTaskApiWorkArgs
                            	{
                            		WorkKind = IdeBackgroundTaskApiWorkKind.FileContentsWereModifiedOnDisk,
                            		InputFileAbsolutePathString = inputFileAbsolutePathString,
                            		TextEditorModel = textEditorModel,
                            		FileLastWriteTime = fileLastWriteTime,
                            		NotificationInformativeKey = notificationInformativeKey,
                            	});

								return Task.CompletedTask;
							})
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineFunc),
                            new Func<Task>(() =>
                            {
                                _notificationService.ReduceDisposeAction(notificationInformativeKey);
                                return Task.CompletedTask;
                            })
                        },
                },
                TimeSpan.FromSeconds(20),
                true,
                null);

            _notificationService.ReduceRegisterAction(notificationInformative);
        }
    }

    private async ValueTask Editor_Do_FileContentsWereModifiedOnDisk(string inputFileAbsolutePathString, TextEditorModel textEditorModel, DateTime fileLastWriteTime, Key<IDynamicViewModel> notificationInformativeKey)
    {
        _notificationService.ReduceDisposeAction(notificationInformativeKey);

        var content = await _fileSystemProvider.File
            .ReadAllTextAsync(inputFileAbsolutePathString)
            .ConfigureAwait(false);

        _textEditorService.WorkerArbitrary.PostUnique(editContext =>
        {
            var modelModifier = editContext.GetModelModifier(textEditorModel.PersistentState.ResourceUri);
            if (modelModifier is null)
                return ValueTask.CompletedTask;

            _textEditorService.ModelApi.Reload(
                editContext,
                modelModifier,
                content,
                fileLastWriteTime);

            editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
                editContext,
                modelModifier);
            return ValueTask.CompletedTask;
        });
    }
    
    private async ValueTask Do_SaveFile(
        AbsolutePath absolutePath,
        string content,
        Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var absolutePathString = absolutePath.Value;

        if (absolutePathString is not null &&
            await _fileSystemProvider.File.ExistsAsync(absolutePathString).ConfigureAwait(false))
        {
            await _fileSystemProvider.File.WriteAllTextAsync(absolutePathString, content).ConfigureAwait(false);
        }
        else
        {
            // TODO: Save As to make new file
            NotificationHelper.DispatchInformative("Save Action", "File not found. TODO: Save As", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(7));
        }

        DateTime? fileLastWriteTime = null;

        if (absolutePathString is not null)
        {
            fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                    absolutePathString,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }

        if (onAfterSaveCompletedWrittenDateTimeFunc is not null)
            await onAfterSaveCompletedWrittenDateTimeFunc.Invoke(fileLastWriteTime);
    }
    
    private ValueTask Do_SetFolderExplorerState(AbsolutePath folderAbsolutePath)
    {
        _folderExplorerService.With(
            inFolderExplorerState => inFolderExplorerState with
            {
                AbsolutePath = folderAbsolutePath
            });

        return Do_SetFolderExplorerTreeView(folderAbsolutePath);
    }

    private async ValueTask Do_SetFolderExplorerTreeView(AbsolutePath folderAbsolutePath)
    {
        _folderExplorerService.With(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = true
        });
        
		_environmentProvider.DeletionPermittedRegister(new(folderAbsolutePath.Value, true));

        var rootNode = new TreeViewAbsolutePath(
            folderAbsolutePath,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildListAsync().ConfigureAwait(false);

        if (!_treeViewService.TryGetTreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                out var treeViewState))
        {
            _treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                new List<TreeViewNoType>() { rootNode }));
        }
        else
        {
            _treeViewService.ReduceWithRootNodeAction(FolderExplorerState.TreeViewContentStateKey, rootNode);

            _treeViewService.ReduceSetActiveNodeAction(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                true,
                false);
        }

        _folderExplorerService.With(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = false
        });
    }

    public void FolderExplorer_ShowInputFile()
    {
        Enqueue(new IdeBackgroundTaskApiWorkArgs
        {
        	WorkKind = IdeBackgroundTaskApiWorkKind.RequestInputFileStateForm,
            Message = "Folder Explorer",
            OnAfterSubmitFunc = async absolutePath =>
            {
                if (absolutePath.ExactInput is not null)
                    await Do_SetFolderExplorerState(absolutePath).ConfigureAwait(false);
            },
            SelectionIsValidFunc = absolutePath =>
            {
                if (absolutePath.ExactInput is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            InputFilePatterns = [
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            ]
        });
    }
    
    private ValueTask Do_RequestInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        List<InputFilePattern> inputFilePatternsList)
    {
        _inputFileService.StartInputFileStateForm(
            message,
            onAfterSubmitFunc,
            selectionIsValidFunc,
            inputFilePatternsList);

        var inputFileDialog = new DialogViewModel(
            DialogFacts.InputFileDialogKey,
            "Input File",
            _ideComponentRenderers.InputFileRendererType,
            null,
            Luthetus.Ide.RazorLib.Htmls.Models.HtmlFacts.Classes.DIALOG_PADDING_0,
            true,
            null);

        _dialogService.ReduceRegisterAction(inputFileDialog);

        return ValueTask.CompletedTask;
    }
    
    public ValueTask HandleEvent()
    {
        if (!_workQueue.TryDequeue(out IdeBackgroundTaskApiWorkArgs workArgs))
            return ValueTask.CompletedTask;

        switch (workArgs.WorkKind)
        {
            case IdeBackgroundTaskApiWorkKind.LuthetusIdeInitializerOnInit:
                return Do_LuthetusIdeInitializerOnInit();
            case IdeBackgroundTaskApiWorkKind.IdeHeaderOnInit:
            	return Do_IdeHeaderOnInit(workArgs.IdeHeader);
            case IdeBackgroundTaskApiWorkKind.FileContentsWereModifiedOnDisk:
	            return Editor_Do_FileContentsWereModifiedOnDisk(
	                workArgs.InputFileAbsolutePathString, workArgs.TextEditorModel, workArgs.FileLastWriteTime, workArgs.NotificationInformativeKey);
			case IdeBackgroundTaskApiWorkKind.SaveFile:
                return Do_SaveFile(workArgs.AbsolutePath, workArgs.Content, workArgs.OnAfterSaveCompletedWrittenDateTimeFunc, workArgs.CancellationToken);
            case IdeBackgroundTaskApiWorkKind.SetFolderExplorerState:
                return Do_SetFolderExplorerState(workArgs.AbsolutePath);
            case IdeBackgroundTaskApiWorkKind.SetFolderExplorerTreeView:
                return Do_SetFolderExplorerTreeView(workArgs.AbsolutePath);
            default:
                Console.WriteLine($"{nameof(IdeBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
        }
    }
}
