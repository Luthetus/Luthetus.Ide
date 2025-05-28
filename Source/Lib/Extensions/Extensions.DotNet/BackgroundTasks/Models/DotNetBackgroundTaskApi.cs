using System.Collections.Concurrent;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ListExtensions;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;
// FindAllReferences
// using Luthetus.Ide.RazorLib.FindAllReferences.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;
using Luthetus.Extensions.DotNet.Commands;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Displays;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Extensions.DotNet.Outputs.Displays;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public class DotNetBackgroundTaskApi : IBackgroundTaskGroup
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
	private readonly IAppDataService _appDataService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
	private readonly ITreeViewService _treeViewService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly TextEditorService _textEditorService;
	private readonly IFindAllService _findAllService;
	private readonly ICodeSearchService _codeSearchService;
	// FindAllReferences
	// private readonly IFindAllReferencesService _findAllReferencesService;
	private readonly IStartupControlService _startupControlService;
	private readonly INotificationService _notificationService;
	private readonly ITerminalService _terminalService;
	private readonly IDotNetCommandFactory _dotNetCommandFactory;
	private readonly IPanelService _panelService;
	private readonly IDialogService _dialogService;
	private readonly IAppOptionsService _appOptionsService;
	private readonly IIdeHeaderService _ideHeaderService;
	private readonly ITextEditorHeaderRegistry _textEditorHeaderRegistry;
	private readonly INugetPackageManagerProvider _nugetPackageManagerProvider;

    public DotNetBackgroundTaskApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		BackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IAppDataService appDataService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		CommonBackgroundTaskApi commonBackgroundTaskApi,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
		DotNetCliOutputParser dotNetCliOutputParser,
		IFileSystemProvider fileSystemProvider,
		TextEditorService textEditorService,
		IFindAllService findAllService,
		ICodeSearchService codeSearchService,
		// FindAllReferences
		// IFindAllReferencesService findAllReferencesService,
		IStartupControlService startupControlService,
		INotificationService notificationService,
		ITerminalService terminalService,
        IDotNetCommandFactory dotNetCommandFactory,
        IPanelService panelService,
        IDialogService dialogService,
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
		_commonBackgroundTaskApi = commonBackgroundTaskApi;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_fileSystemProvider = fileSystemProvider;
		_textEditorService = textEditorService;
		_findAllService = findAllService;
		_codeSearchService = codeSearchService;
		// FindAllReferences
		// _findAllReferencesService = findAllReferencesService;
		_startupControlService = startupControlService;
		_notificationService = notificationService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalService = terminalService;
        _dotNetCommandFactory = dotNetCommandFactory;
        _dialogService = dialogService;
        _panelService = panelService;
        _appOptionsService = appOptionsService;
        _ideHeaderService = ideHeaderService;
        _textEditorHeaderRegistry = textEditorHeaderRegistry;
        _nugetPackageManagerProvider = nugetPackageManagerProvider;

        DotNetSolutionService = new DotNetSolutionService();
		
		CompilerServiceExplorerService = new CompilerServiceExplorerService();

        TestExplorerService = new TestExplorerService(
			this,
			_ideBackgroundTaskApi,
			DotNetSolutionService,
			_treeViewService,
            _commonComponentRenderers,
            _textEditorService,
            _notificationService,
            _backgroundTaskService,
            _fileSystemProvider,
            _dotNetCliOutputParser,
            _terminalService);

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
			// FindAllReferences
			// _findAllReferencesService,
			_startupControlService,
			_compilerServiceRegistry,
			_terminalService,
			_dotNetCliOutputParser,
			serviceProvider);
			
			NuGetPackageManagerService = new NuGetPackageManagerService();
			
			CompilerServiceEditorService = new CompilerServiceEditorService();
	}

	public DotNetSolutionIdeApi DotNetSolution { get; }
    public OutputScheduler Output { get; }
    public IOutputService OutputService { get; }
    public ITestExplorerService TestExplorerService { get; }
    public IDotNetSolutionService DotNetSolutionService { get; }
    public INuGetPackageManagerService NuGetPackageManagerService { get; }
    public ICompilerServiceEditorService CompilerServiceEditorService { get; }
    public ICompilerServiceExplorerService CompilerServiceExplorerService { get; }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

	private readonly ConcurrentQueue<DotNetBackgroundTaskApiWorkArgs> _workQueue = new();

    private Key<PanelGroup> _leftPanelGroupKey;
    private Key<Panel> _solutionExplorerPanelKey;

    private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();
    
    public void Enqueue(DotNetBackgroundTaskApiWorkArgs workArgs)
    {
		_workQueue.Enqueue(workArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
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
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(solutionExplorerPanel);
        _panelService.RegisterPanelTab(leftPanel.Key, solutionExplorerPanel, false);

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
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(compilerServiceExplorerPanel);
        _panelService.RegisterPanelTab(rightPanel.Key, compilerServiceExplorerPanel, false);

        /*// compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
            "Compiler Service Editor",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(compilerServiceEditorPanel);
        _panelService.RegisterPanelTab(rightPanel.Key, compilerServiceEditorPanel, false);*/
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
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(outputPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, outputPanel, false);

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
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(testExplorerPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, testExplorerPanel, false);
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
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(nuGetPanel);
        _panelService.RegisterPanelTab(bottomPanel.Key, nuGetPanel, false);
        
        // SetActivePanelTabAction
        _panelService.SetActivePanelTab(bottomPanel.Key, outputPanel.Key);
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

        _ideHeaderService.ModifyMenuFile(
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
                        subMenu: new MenuRecord(new List<MenuOptionRecord> { menuOptionNewDotNetSolution }));

                    var copyMenuOptionList = new List<MenuOptionRecord>(inMenu.MenuOptionList);
                    copyMenuOptionList.Insert(0, menuOptionNew);

                    return inMenu with
                    {
                        MenuOptionList = copyMenuOptionList
                    };
                }
            });

        InitializeMenuRun();

        _panelService.SetActivePanelTab(_leftPanelGroupKey, _solutionExplorerPanelKey);

        var compilerService = _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);

        /*if (compilerService is CSharpCompilerService cSharpCompilerService)
		{
			cSharpCompilerService.SetSymbolRendererType(typeof(Luthetus.Extensions.DotNet.TextEditors.Displays.CSharpSymbolDisplay));
		}*/

        _textEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.Extensions.CompilerServices.Displays.TextEditorCompilerServiceHeaderDisplay));

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

        _ideHeaderService.ModifyMenuRun(inMenu =>
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

    public async ValueTask Do_SubmitNuGetQuery(INugetPackageManagerQuery query)
    {
        var localNugetResult = await _nugetPackageManagerProvider
            .QueryForNugetPackagesAsync(query)
            .ConfigureAwait(false);

        NuGetPackageManagerService.ReduceSetMostRecentQueryResultAction(localNugetResult);
    }
    
    public ValueTask Do_RunTestByFullyQualifiedName(TreeViewStringFragment treeViewStringFragment, string fullyQualifiedName, TreeViewProjectTestModel treeViewProjectTestModel)
    {
        RunTestByFullyQualifiedName(
            treeViewStringFragment,
            fullyQualifiedName,
            treeViewProjectTestModel.Item.AbsolutePath.ParentDirectory);

        return ValueTask.CompletedTask;
    }

    private void RunTestByFullyQualifiedName(
        TreeViewStringFragment treeViewStringFragment,
        string fullyQualifiedName,
        string? directoryNameForTestDiscovery)
    {
        var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter
            .FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);

        if (string.IsNullOrWhiteSpace(directoryNameForTestDiscovery) ||
            string.IsNullOrWhiteSpace(fullyQualifiedName))
        {
            return;
        }

        var terminalCommandRequest = new TerminalCommandRequest(
            dotNetTestByFullyQualifiedNameFormattedCommand.Value,
            directoryNameForTestDiscovery,
            treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandRequestKey)
        {
            BeginWithFunc = parsedCommand =>
            {
                treeViewStringFragment.Item.TerminalCommandParsed = parsedCommand;
                _treeViewService.ReduceReRenderNodeAction(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
                return Task.CompletedTask;
            },
            ContinueWithFunc = parsedCommand =>
            {
                treeViewStringFragment.Item.TerminalCommandParsed = parsedCommand;
                var output = treeViewStringFragment.Item.TerminalCommandParsed?.OutputCache.ToString() ?? null;

                if (output is not null && output.Contains("Duration:"))
                {
                    if (output.Contains("Passed!"))
                    {
                        TestExplorerService.ReduceWithAction(inState =>
                        {
                            var passedTestHashSet = new HashSet<string>(inState.PassedTestHashSet);
                            passedTestHashSet.Add(fullyQualifiedName);

                            var notRanTestHashSet = new HashSet<string>(inState.NotRanTestHashSet);
                            notRanTestHashSet.Remove(fullyQualifiedName);

                            var failedTestHashSet = new HashSet<string>(inState.FailedTestHashSet);
                            failedTestHashSet.Remove(fullyQualifiedName);

                            return inState with
                            {
                                PassedTestHashSet = passedTestHashSet,
                                NotRanTestHashSet = notRanTestHashSet,
                                FailedTestHashSet = failedTestHashSet,
                            };
                        });
                    }
                    else
                    {
                        TestExplorerService.ReduceWithAction(inState =>
                        {
							var failedTestHashSet = new HashSet<string>(inState.FailedTestHashSet);
							failedTestHashSet.Add(fullyQualifiedName);

							var notRanTestHashSet = new HashSet<string>(inState.NotRanTestHashSet);
							notRanTestHashSet.Remove(fullyQualifiedName);

							var passedTestHashSet = new HashSet<string>(inState.PassedTestHashSet);
							passedTestHashSet.Remove(fullyQualifiedName);

                            return inState with
                            {
                                FailedTestHashSet = failedTestHashSet,
                                NotRanTestHashSet = notRanTestHashSet,
                                PassedTestHashSet = passedTestHashSet,
                            };
                        });
                    }
                }

                _treeViewService.ReduceReRenderNodeAction(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
                return Task.CompletedTask;
            }
        };

        treeViewStringFragment.Item.TerminalCommandRequest = terminalCommandRequest;
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
    }

    public ValueTask HandleEvent()
    {
        if (!_workQueue.TryDequeue(out DotNetBackgroundTaskApiWorkArgs workArgs))
            return ValueTask.CompletedTask;

        switch (workArgs.WorkKind)
        {
            case DotNetBackgroundTaskApiWorkKind.SolutionExplorer_TreeView_MultiSelect_DeleteFiles:
                return Do_SolutionExplorer_TreeView_MultiSelect_DeleteFiles(workArgs.TreeViewCommandArgs);
            case DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnInit:
                return Do_LuthetusExtensionsDotNetInitializerOnInit();
            case DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnAfterRender:
                return Do_LuthetusExtensionsDotNetInitializerOnAfterRender();
            case DotNetBackgroundTaskApiWorkKind.SubmitNuGetQuery:
                return Do_SubmitNuGetQuery(workArgs.NugetPackageManagerQuery);
            case DotNetBackgroundTaskApiWorkKind.RunTestByFullyQualifiedName:
                return Do_RunTestByFullyQualifiedName(workArgs.TreeViewStringFragment, workArgs.FullyQualifiedName, workArgs.TreeViewProjectTestModel);
            default:
                Console.WriteLine($"{nameof(DotNetBackgroundTaskApi)} {nameof(HandleEvent)} default case");
                return ValueTask.CompletedTask;
        }
    }
}
