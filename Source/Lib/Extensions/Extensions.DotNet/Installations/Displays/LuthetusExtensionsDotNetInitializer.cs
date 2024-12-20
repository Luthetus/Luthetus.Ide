using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.Shareds.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.StartupControls.States;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.Displays;
using Luthetus.Extensions.DotNet.TestExplorers.States;
using Luthetus.Extensions.DotNet.Outputs.Displays;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.Commands;

namespace Luthetus.Extensions.DotNet.Installations.Displays;

public partial class LuthetusExtensionsDotNetInitializer : ComponentBase
{
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IDotNetCommandFactory DotNetCommandFactory { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
	private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private IState<StartupControlState> StartupControlStateWrap { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private ITextEditorHeaderRegistry TextEditorHeaderRegistry { get; set; } = null!;

	private static readonly Key<IDynamicViewModel> _newDotNetSolutionDialogKey = Key<IDynamicViewModel>.NewKey();

	private Key<PanelGroup> _leftPanelGroupKey;
	private Key<Panel> _solutionExplorerPanelKey;
	
	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
			nameof(LuthetusExtensionsDotNetInitializer),
			() =>
			{
				InitializePanelTabs();
				
				DotNetCommandFactory.Initialize();
				
                return Task.CompletedTask;
            });
			
		base.OnInitialized();
	}
	
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			BackgroundTaskService.Enqueue(
				Key<IBackgroundTask>.NewKey(),
				ContinuousBackgroundTaskWorker.GetQueueKey(),
				nameof(LuthetusExtensionsDotNetInitializer),
				() =>
				{
					var menuOptionOpenDotNetSolution = new MenuOptionRecord(
		                ".NET Solution",
		                MenuOptionKind.Other,
		                () =>
						{
							DotNetSolutionState.ShowInputFile(IdeBackgroundTaskApi, DotNetBackgroundTaskApi);
							return Task.CompletedTask;
						});
						
					Dispatcher.Dispatch(new IdeHeaderState.ModifyMenuFileAction(
						inMenu => 
						{
							var inMenuOptionOpen = inMenu.MenuOptionList.FirstOrDefault(
								x => x.DisplayName == "Open");
						
							if (inMenuOptionOpen?.SubMenu is null)
							{
								return inMenu with
								{
									MenuOptionList = inMenu.MenuOptionList.Add(menuOptionOpenDotNetSolution)
								};
							}
							
							var outMenuOptionOpen = inMenuOptionOpen with
							{
								SubMenu = inMenuOptionOpen.SubMenu with
								{
									MenuOptionList = inMenuOptionOpen.SubMenu.MenuOptionList.Add(menuOptionOpenDotNetSolution)
								}
							};
							
							var outMenu = inMenu with
							{
								MenuOptionList = inMenu.MenuOptionList.Replace(inMenuOptionOpen, outMenuOptionOpen)
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
					                SubMenu: new MenuRecord(new[] { menuOptionNewDotNetSolution }.ToImmutableArray()));
					
					            return outMenu with
								{
									MenuOptionList = outMenu.MenuOptionList.Insert(0, menuOptionNew)
								};
					        }
						}));
						
					InitializeMenuRun();
					
					Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(_leftPanelGroupKey, _solutionExplorerPanelKey));
					
					var compilerService = CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);
					
					/*if (compilerService is CSharpCompilerService cSharpCompilerService)
					{
						cSharpCompilerService.SetSymbolRendererType(typeof(Luthetus.Extensions.DotNet.TextEditors.Displays.CSharpSymbolDisplay));
					}*/
					
					TextEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.TextEditorCompilerServiceHeaderDisplay));
					
                    return Task.CompletedTask;
                });
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
		var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelStateWrap.Value);
		leftPanel.Dispatcher = Dispatcher;

		// solutionExplorerPanel
		var solutionExplorerPanel = new Panel(
			"Solution Explorer",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.SolutionExplorerContext.ContextKey,
			typeof(SolutionExplorerDisplay),
			null,
			Dispatcher,
			DialogService,
			JsRuntime);
		Dispatcher.Dispatch(new PanelState.RegisterPanelAction(solutionExplorerPanel));
		Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(leftPanel.Key, solutionExplorerPanel, false));
		
		// SetActivePanelTabAction
		//
		// HACK: capture the variables and do it in OnAfterRender so it doesn't get overwritten by the IDE
		// 	  settings the active panel tab to the folder explorer.
		_leftPanelGroupKey = leftPanel.Key;
		_solutionExplorerPanelKey = solutionExplorerPanel.Key;
	}

	private void InitializeRightPanelTabs()
	{
		var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelStateWrap.Value);
		rightPanel.Dispatcher = Dispatcher;

        // compilerServiceExplorerPanel
        var compilerServiceExplorerPanel = new Panel(
            "Compiler Service Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceExplorerContext.ContextKey,
            typeof(CompilerServiceExplorerDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(compilerServiceExplorerPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanel, false));

        // compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
            "Compiler Service Editor",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(compilerServiceEditorPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanel, false));
    }

	private void InitializeBottomPanelTabs()
	{
		var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelStateWrap.Value);
		bottomPanel.Dispatcher = Dispatcher;

        // outputPanel
        var outputPanel = new Panel(
            "Output",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.OutputContext.ContextKey,
            typeof(OutputPanelDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(outputPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, outputPanel, false));

        // testExplorerPanel
        var testExplorerPanel = new Panel(
            "Test Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.TestExplorerContext.ContextKey,
            typeof(TestExplorerDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(testExplorerPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, testExplorerPanel, false));
        // This UI has resizable parts that need to be initialized.
        Dispatcher.Dispatch(new TestExplorerState.InitializeResizeHandleDimensionUnitAction(
            new DimensionUnit
            {
                ValueFunc = () => AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                Purpose = DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN,
            }));

        // nuGetPanel
        var nuGetPanel = new Panel(
            "NuGet",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.NuGetPackageManagerContext.ContextKey,
            typeof(NuGetPackageManager),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(nuGetPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, nuGetPanel, false));
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

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));
        return Task.CompletedTask;
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
				var startupControlState = StartupControlStateWrap.Value;
				var activeStartupControl = startupControlState.ActiveStartupControl;
			
				if (activeStartupControl?.StartupProjectAbsolutePath is not null)
					BuildProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(BuildProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

		// Menu Option Clean (startup project)
        menuOptionsList.Add(new MenuOptionRecord(
			"Clean Project (startup project)",
            MenuOptionKind.Create,
            () =>
			{
				var startupControlState = StartupControlStateWrap.Value;
				var activeStartupControl = startupControlState.ActiveStartupControl;
			
				if (activeStartupControl?.StartupProjectAbsolutePath is not null)
					CleanProjectOnClick(activeStartupControl.StartupProjectAbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(CleanProjectOnClick), "activeStartupControl?.StartupProjectAbsolutePath was null", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

        // Menu Option Build Solution
        menuOptionsList.Add(new MenuOptionRecord(
			"Build Solution",
            MenuOptionKind.Delete,
            () =>
			{
				var dotNetSolutionState = DotNetSolutionStateWrap.Value;
				var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
				
				if (dotNetSolutionModel?.AbsolutePath is not null)
					BuildSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(BuildSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

		// Menu Option Clean Solution
        menuOptionsList.Add(new MenuOptionRecord(
			"Clean Solution",
            MenuOptionKind.Delete,
            () =>
			{
				var dotNetSolutionState = DotNetSolutionStateWrap.Value;
				var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
				
				if (dotNetSolutionModel?.AbsolutePath is not null)
					CleanSolutionOnClick(dotNetSolutionModel.AbsolutePath.Value);
				else
					NotificationHelper.DispatchError(nameof(CleanSolutionOnClick), "dotNetSolutionModel?.AbsolutePath was null", CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(6));
				return Task.CompletedTask;
			}));

        Dispatcher.Dispatch(new IdeHeaderState.ModifyMenuRunAction(inMenu =>
        {
        	var outMenuOptionList = inMenu.MenuOptionList.AddRange(menuOptionsList);
        	
        	return inMenu with
        	{
        		MenuOptionList = outMenuOptionList
        	};
        }));
	}

	private void BuildProjectOnClick(string projectAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildProject(projectAbsolutePathString);
        var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
        
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory.Value)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Build-Project_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Build-Project_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}

	private void CleanProjectOnClick(string projectAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanProject(projectAbsolutePathString);
		var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(projectAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
			
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory.Value)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Clean-Project_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Clean-Project_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}

	private void BuildSolutionOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetBuildSolution(solutionAbsolutePathString);
        var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
        
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory.Value)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Build-Solution_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Build-Solution_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}

	private void CleanSolutionOnClick(string solutionAbsolutePathString)
	{
		var formattedCommand = DotNetCliCommandFormatter.FormatDotnetCleanSolution(solutionAbsolutePathString);
		var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(solutionAbsolutePathString, false);
		
		var localParentDirectory = solutionAbsolutePath.ParentDirectory;
		if (localParentDirectory is null)
			return;
			
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localParentDirectory.Value)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Clean-Solution_started");
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		DotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Clean-Solution_completed");
        		return Task.CompletedTask;
        	}
        };
        	
        TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
	}
}