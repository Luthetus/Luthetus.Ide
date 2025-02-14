using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;
using Luthetus.Extensions.DotNet.Commands;
using Luthetus.Extensions.DotNet.Installations.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Displays;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Microsoft.JSInterop;
using Luthetus.Extensions.DotNet.Outputs.Displays;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public class DotNetBackgroundTaskApi : IBackgroundTaskGroup
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
	private readonly IAppDataService _appDataService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly ITextEditorService _textEditorService;
	private readonly IFindAllService _findAllService;
	private readonly ICodeSearchService _codeSearchService;
	private readonly IStartupControlService _startupControlService;
	private readonly INotificationService _notificationService;
	private readonly ITerminalService _terminalService;
	private readonly IDotNetCommandFactory _dotNetCommandFactory;
	private readonly IPanelService _panelService;
	private readonly IDialogService _dialogService;
	private readonly IJSRuntime _jsRuntime;
	private readonly IAppOptionsService _appOptionsService;
	private readonly IIdeHeaderService _ideHeaderService;
	private readonly ITextEditorHeaderRegistry _textEditorHeaderRegistry;
	private readonly INugetPackageManagerProvider _nugetPackageManagerProvider;

    public DotNetBackgroundTaskApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IAppDataService appDataService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
		DotNetCliOutputParser dotNetCliOutputParser,
		IFileSystemProvider fileSystemProvider,
		ITextEditorService textEditorService,
		IFindAllService findAllService,
		ICodeSearchService codeSearchService,
		IStartupControlService startupControlService,
		INotificationService notificationService,
		ITerminalService terminalService,
        IDotNetCommandFactory dotNetCommandFactory,
        IPanelService panelService,
        IDialogService dialogService,
        IJSRuntime jsRuntime,
        IAppOptionsService appOptionsService,
        IIdeHeaderService ideHeaderService,
        ITextEditorHeaderRegistry textEditorHeaderRegistry,
        INugetPackageManagerProvider nugetPackageManagerProvider,
        IServiceProvider serviceProvider)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_storageService = storageService;
		_appDataService = appDataService;
        _dotNetComponentRenderers = dotNetComponentRenderers;
		_ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_fileSystemProvider = fileSystemProvider;
		_textEditorService = textEditorService;
		_findAllService = findAllService;
		_codeSearchService = codeSearchService;
		_startupControlService = startupControlService;
		_notificationService = notificationService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalService = terminalService;
        _dotNetCommandFactory = dotNetCommandFactory;
        _dialogService = dialogService;
        _panelService = panelService;
        _jsRuntime = jsRuntime;
        _appOptionsService = appOptionsService;
        _ideHeaderService = ideHeaderService;
        _textEditorHeaderRegistry = textEditorHeaderRegistry;
        _nugetPackageManagerProvider = nugetPackageManagerProvider;

        DotNetSolutionService = new DotNetSolutionService();
		
		CompilerServiceExplorerService = new CompilerServiceExplorerService();

        CompilerService = new CompilerServiceIdeApi(
			this,
            _ideBackgroundTaskApi,
            _backgroundTaskService,
			CompilerServiceExplorerService,
			_compilerServiceRegistry,
			_ideComponentRenderers,
			_commonComponentRenderers,
			_treeViewService);
			
		TestExplorerService = new TestExplorerService(
			this,
			_ideBackgroundTaskApi,
			DotNetSolutionService);

        TestExplorer = new TestExplorerScheduler(
            this,
            _ideBackgroundTaskApi,
            _commonComponentRenderers,
            _treeViewService,
            _textEditorService,
            _notificationService,
            _backgroundTaskService,
            _fileSystemProvider,
            _dotNetCliOutputParser,
            DotNetSolutionService,
            _terminalService,
            TestExplorerService);
            
        OutputService = new OutputService(this);
            
        Output = new OutputScheduler(
        	this,
			_backgroundTaskService,
			_dotNetCliOutputParser,
			_treeViewService,
			_environmentProvider,
			OutputService);

        DotNetSolution = new DotNetSolutionIdeApi(
			_ideBackgroundTaskApi,
			_backgroundTaskService,
			_storageService,
			_appDataService,
			CompilerServiceExplorerService,
            _dotNetComponentRenderers,
            _ideComponentRenderers,
			_commonComponentRenderers,
			_treeViewService,
			_notificationService,
			_environmentProvider,
			DotNetSolutionService,
			_fileSystemProvider,
			_textEditorService,
			_findAllService,
			_codeSearchService,
			_startupControlService,
			_compilerServiceRegistry,
			_terminalService,
			_dotNetCliOutputParser,
			serviceProvider);
			
			NuGetPackageManagerService = new NuGetPackageManagerService();
			
			CompilerServiceEditorService = new CompilerServiceEditorService();
	}

	public DotNetSolutionIdeApi DotNetSolution { get; }
    public CompilerServiceIdeApi CompilerService { get; }
    public TestExplorerScheduler TestExplorer { get; }
    public OutputScheduler Output { get; }
    public IOutputService OutputService { get; }
    public ITestExplorerService TestExplorerService { get; }
    public IDotNetSolutionService DotNetSolutionService { get; }
    public INuGetPackageManagerService NuGetPackageManagerService { get; }
    public ICompilerServiceEditorService CompilerServiceEditorService { get; }
    public ICompilerServiceExplorerService CompilerServiceExplorerService { get; }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(DotNetBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<DotNetBackgroundTaskApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

	private readonly Queue<TreeViewCommandArgs> _queue_SolutionExplorer_TreeView_MultiSelect_DeleteFiles = new();

    private Key<PanelGroup> _leftPanelGroupKey;
    private Key<Panel> _solutionExplorerPanelKey;

    private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();

    public void Enqueue_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(TreeViewCommandArgs commandArgs)
    {
        lock (_workLock)
		{
			_workKindQueue.Enqueue(DotNetBackgroundTaskApiWorkKind.SolutionExplorer_TreeView_MultiSelect_DeleteFiles);
			_queue_SolutionExplorer_TreeView_MultiSelect_DeleteFiles.Enqueue(commandArgs);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
	
	public async ValueTask Do_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(TreeViewCommandArgs commandArgs)
    {
        foreach (var node in commandArgs.TreeViewContainer.SelectedNodeList)
        {
            var treeViewNamespacePath = (TreeViewNamespacePath)node;

            if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
            {
                await _fileSystemProvider.Directory
                    .DeleteAsync(treeViewNamespacePath.Item.AbsolutePath.Value, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                await _fileSystemProvider.File
                    .DeleteAsync(treeViewNamespacePath.Item.AbsolutePath.Value)
                    .ConfigureAwait(false);
            }

            if (_treeViewService.TryGetTreeViewContainer(commandArgs.TreeViewContainer.Key, out var mostRecentContainer) &&
                mostRecentContainer is not null)
            {
                var localParent = node.Parent;

                if (localParent is not null)
                {
                    await localParent.LoadChildListAsync().ConfigureAwait(false);
                    _treeViewService.ReduceReRenderNodeAction(mostRecentContainer.Key, localParent);
                }
            }
        }
    }

    public void Enqueue_LuthetusExtensionsDotNetInitializerOnInit()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnInit);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public ValueTask Do_LuthetusExtensionsDotNetInitializerOnInit()
    {
        InitializePanelTabs();
        _dotNetCommandFactory.Initialize();
        return ValueTask.CompletedTask;
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

        // solutionExplorerPanel
        var solutionExplorerPanel = new Panel(
            "Solution Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.SolutionExplorerContext.ContextKey,
            typeof(SolutionExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _jsRuntime);
        _panelService.ReduceRegisterPanelAction(solutionExplorerPanel);
        _panelService.ReduceRegisterPanelTabAction(leftPanel.Key, solutionExplorerPanel, false);

        // SetActivePanelTabAction
        //
        // HACK: capture the variables and do it in OnAfterRender so it doesn't get overwritten by the IDE
        // 	  settings the active panel tab to the folder explorer.
        _leftPanelGroupKey = leftPanel.Key;
        _solutionExplorerPanelKey = solutionExplorerPanel.Key;
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetTopRightPanelGroup(_panelService.GetPanelState());
        rightPanel.PanelService = _panelService;

        // compilerServiceExplorerPanel
        var compilerServiceExplorerPanel = new Panel(
            "Compiler Service Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceExplorerContext.ContextKey,
            typeof(CompilerServiceExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _jsRuntime);
        _panelService.ReduceRegisterPanelAction(compilerServiceExplorerPanel);
        _panelService.ReduceRegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanel, false);

        // compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
            "Compiler Service Editor",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null,
            _panelService,
            _dialogService,
            _jsRuntime);
        _panelService.ReduceRegisterPanelAction(compilerServiceEditorPanel);
        _panelService.ReduceRegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanel, false);
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelGroup(_panelService.GetPanelState());
        bottomPanel.PanelService = _panelService;

        // outputPanel
        var outputPanel = new Panel(
            "Output",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.OutputContext.ContextKey,
            typeof(OutputPanelDisplay),
            null,
            _panelService,
            _dialogService,
            _jsRuntime);
        _panelService.ReduceRegisterPanelAction(outputPanel);
        _panelService.ReduceRegisterPanelTabAction(bottomPanel.Key, outputPanel, false);

        // testExplorerPanel
        var testExplorerPanel = new Panel(
            "Test Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.TestExplorerContext.ContextKey,
            typeof(TestExplorerDisplay),
            null,
            _panelService,
            _dialogService,
            _jsRuntime);
        _panelService.ReduceRegisterPanelAction(testExplorerPanel);
        _panelService.ReduceRegisterPanelTabAction(bottomPanel.Key, testExplorerPanel, false);
        // This UI has resizable parts that need to be initialized.
        TestExplorerService.ReduceInitializeResizeHandleDimensionUnitAction(
            new DimensionUnit(
                () => _appOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind.Pixels,
                DimensionOperatorKind.Subtract,
                DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));

        // nuGetPanel
        var nuGetPanel = new Panel(
            "NuGet",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.NuGetPackageManagerContext.ContextKey,
            typeof(NuGetPackageManager),
            null,
            _panelService,
            _dialogService,
            _jsRuntime);
        _panelService.ReduceRegisterPanelAction(nuGetPanel);
        _panelService.ReduceRegisterPanelTabAction(bottomPanel.Key, nuGetPanel, false);
    }

    public void Enqueue_LuthetusExtensionsDotNetInitializerOnAfterRender()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnAfterRender);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public ValueTask Do_LuthetusExtensionsDotNetInitializerOnAfterRender()
    {
        var menuOptionOpenDotNetSolution = new MenuOptionRecord(
            ".NET Solution",
            MenuOptionKind.Other,
            () =>
            {
                DotNetSolutionState.ShowInputFile(_ideBackgroundTaskApi, this);
                return Task.CompletedTask;
            });

        _ideHeaderService.ReduceModifyMenuFileAction(
            inMenu =>
            {
                var indexMenuOptionOpen = inMenu.MenuOptionList.FindIndex(x => x.DisplayName == "Open");

                if (indexMenuOptionOpen == -1)
                {
                    var copyList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
                    copyList.Add(menuOptionOpenDotNetSolution);
                    return inMenu with
                    {
                        MenuOptionList = copyList
                    };
                }

                var menuOptionOpen = inMenu.MenuOptionList[indexMenuOptionOpen];

                if (menuOptionOpen.SubMenu is null)
                    menuOptionOpen.SubMenu = new MenuRecord(new List<MenuOptionRecord>());

                // UI foreach enumeration was modified nightmare. (2025-02-07)
                var copySubMenuList = new List<MenuOptionRecord>(menuOptionOpen.SubMenu.MenuOptionList);
                copySubMenuList.Add(menuOptionOpenDotNetSolution);

                menuOptionOpen.SubMenu = menuOptionOpen.SubMenu with
                {
                    MenuOptionList = copySubMenuList
                };

                // Menu Option New
                {
                    var menuOptionNewDotNetSolution = new MenuOptionRecord(
                        ".NET Solution",
                        MenuOptionKind.Other,
                        OpenNewDotNetSolutionDialog);

                    var menuOptionNew = new MenuOptionRecord(
                        "New",
                        MenuOptionKind.Other,
                        subMenu: new MenuRecord(new() { menuOptionNewDotNetSolution }));

                    var copyMenuOptionList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
                    copyMenuOptionList.Insert(0, menuOptionNew);

                    return inMenu with
                    {
                        MenuOptionList = copyMenuOptionList
                    };
                }
            });

        InitializeMenuRun();

        _panelService.ReduceSetActivePanelTabAction(_leftPanelGroupKey, _solutionExplorerPanelKey);

        var compilerService = _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);

        /*if (compilerService is CSharpCompilerService cSharpCompilerService)
		{
			cSharpCompilerService.SetSymbolRendererType(typeof(Luthetus.Extensions.DotNet.TextEditors.Displays.CSharpSymbolDisplay));
		}*/

        _textEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.TextEditorCompilerServiceHeaderDisplay));

        return ValueTask.CompletedTask;
    }

    private void InitializeMenuRun()
    {
        var menuOptionsList = new List<MenuOptionRecord>();

        // Menu Option Build Project (startup project)
        menuOptionsList.Add(new MenuOptionRecord(
            "Build Project (startup project)",
            MenuOptionKind.Create,
            () =>
            {
                var startupControlState = _startupControlService.GetStartupControlState();
                var activeStartupControl = startupControlState.ActiveStartupControl;

                if (activeStartupControl?.StartupProjectAbsolutePath is not null)
                    BuildProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(BuildProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        // Menu Option Clean (startup project)
        menuOptionsList.Add(new MenuOptionRecord(
            "Clean Project (startup project)",
            MenuOptionKind.Create,
            () =>
            {
                var startupControlState = _startupControlService.GetStartupControlState();
                var activeStartupControl = startupControlState.ActiveStartupControl;

                if (activeStartupControl?.StartupProjectAbsolutePath is not null)
                    CleanProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(CleanProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        // Menu Option Build Solution
        menuOptionsList.Add(new MenuOptionRecord(
            "Build Solution",
            MenuOptionKind.Delete,
            () =>
            {
                var dotNetSolutionState = DotNetSolutionService.GetDotNetSolutionState();
                var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

                if (dotNetSolutionModel?.AbsolutePath is not null)
                    BuildSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(BuildSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        // Menu Option Clean Solution
        menuOptionsList.Add(new MenuOptionRecord(
            "Clean Solution",
            MenuOptionKind.Delete,
            () =>
            {
                var dotNetSolutionState = DotNetSolutionService.GetDotNetSolutionState();
                var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

                if (dotNetSolutionModel?.AbsolutePath is not null)
                    CleanSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
                else
                    NotificationHelper.DispatchError(nameof(CleanSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));
                return Task.CompletedTask;
            }));

        _ideHeaderService.ReduceModifyMenuRunAction(inMenu =>
        {
            // UI foreach enumeration was modified nightmare. (2025-02-07)
            var copyMenuOptionList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
            copyMenuOptionList.AddRange(menuOptionsList);
            return inMenu with
            {
                MenuOptionList = copyMenuOptionList
            };
        });
    }

    private void BuildProjectOnClick(string projectAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildProject(projectAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Build-Project_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Build-Project_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private void CleanProjectOnClick(string projectAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanProject(projectAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Clean-Project_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Clean-Project_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private void BuildSolutionOnClick(string solutionAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildSolution(solutionAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Build-Solution_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Build-Solution_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private void CleanSolutionOnClick(string solutionAbsolutePathString)
    {
        var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanSolution(solutionAbsolutePathString);
        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);

        var localParentDirectory = solutionAbsolutePath.ParentDirectory;
        if (localParentDirectory is null)
            return;

        var terminalCommandRequest = new TerminalCommandRequest(
            formattedCommand.Value,
            localParentDirectory)
        {
            BeginWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    string.Empty,
                    "Clean-Solution_started");
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                _dotNetCliOutputParser.ParseOutputEntireDotNetRun(
                    parsedCommand.OutputCache.ToString(),
                    "Clean-Solution_completed");
                return Task.CompletedTask;
            }
        };

        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }

    private Task OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogViewModel(
            _newDotNetSolutionDialogKey,
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null,
            null,
            true,
            null);

        _dialogService.ReduceRegisterAction(dialogRecord);
        return Task.CompletedTask;
    }

    private readonly Queue<INugetPackageManagerQuery> _queue_SubmitNuGetQuery = new();

    public void Enqueue_SubmitNuGetQuery(INugetPackageManagerQuery query)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetBackgroundTaskApiWorkKind.SubmitNuGetQuery);
            _queue_SubmitNuGetQuery.Enqueue(query);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_SubmitNuGetQuery(INugetPackageManagerQuery query)
    {
        var localNugetResult = await _nugetPackageManagerProvider
            .QueryForNugetPackagesAsync(query)
            .ConfigureAwait(false);

        NuGetPackageManagerService.ReduceSetMostRecentQueryResultAction(localNugetResult);
    }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        DotNetBackgroundTaskApiWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case DotNetBackgroundTaskApiWorkKind.SolutionExplorer_TreeView_MultiSelect_DeleteFiles:
            {
				var args = _queue_SolutionExplorer_TreeView_MultiSelect_DeleteFiles.Dequeue();
                return Do_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(args);
            }
            case DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnInit:
            {
                return Do_LuthetusExtensionsDotNetInitializerOnInit();
            }
            case DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnAfterRender:
            {
                return Do_LuthetusExtensionsDotNetInitializerOnAfterRender();
            }
            case DotNetBackgroundTaskApiWorkKind.SubmitNuGetQuery:
            {
                var args = _queue_SubmitNuGetQuery.Dequeue();
                return Do_SubmitNuGetQuery(args);
            }
            default:
            {
                Console.WriteLine($"{nameof(DotNetBackgroundTaskApi)} {nameof(HandleEvent)} default case");
                return ValueTask.CompletedTask;
            }
        }
    }
}
