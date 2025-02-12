using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.JsRuntimes.Models;

namespace Luthetus.Ide.RazorLib.Installations.Displays;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public partial class LuthetusIdeInitializer : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    [Inject]
    private IPanelService PanelService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommandFactory CommandFactory { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private ICodeSearchService CodeSearchService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IThemeService ThemeService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;

	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            nameof(LuthetusIdeInitializer),
            async () =>
            {
                if (TextEditorConfig.CustomThemeRecordList is not null)
                {
                    foreach (var themeRecord in TextEditorConfig.CustomThemeRecordList)
                    {
                        ThemeService.ReduceRegisterAction(themeRecord);
                    }
                }

                foreach (var terminalKey in TerminalFacts.WELL_KNOWN_KEYS)
                {
                	if (terminalKey == TerminalFacts.GENERAL_KEY)
                		AddGeneralTerminal();
                	else if (terminalKey == TerminalFacts.EXECUTION_KEY)
                		AddExecutionTerminal();
                }
                
                CodeSearchService.ReduceInitializeResizeHandleDimensionUnitAction(
					new DimensionUnit(
						() => AppOptionsService.GetAppOptionsState().Options.ResizeHandleHeightInPixels / 2,
						DimensionUnitKind.Pixels,
						DimensionOperatorKind.Subtract,
						DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW));

				InitializePanelResizeHandleDimensionUnit();
                InitializePanelTabs();
                CommandFactory.Initialize();
            });
            
        base.OnInitialized();
	}
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			if (LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Photino)
			{
				await JsRuntime.GetLuthetusIdeApi()
					.PreventDefaultBrowserKeybindings();
			}
		}
		
		await base.OnAfterRenderAsync(firstRender);
	}

	private void InitializePanelResizeHandleDimensionUnit()
	{
		// Left
		{
			var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelService.GetPanelState());
        	leftPanel.PanelService = PanelService;
		
			PanelService.ReduceInitializeResizeHandleDimensionUnitAction(
				leftPanel.Key,
				new DimensionUnit(
					() => AppOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
					DimensionUnitKind.Pixels,
					DimensionOperatorKind.Subtract,
					DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));
		}
		
		// Right
		{
			var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelService.GetPanelState());
        	rightPanel.PanelService = PanelService;
		
			PanelService.ReduceInitializeResizeHandleDimensionUnitAction(
				rightPanel.Key,
				new DimensionUnit(
					() => AppOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
					DimensionUnitKind.Pixels,
					DimensionOperatorKind.Subtract,
					DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));
		}
		
		// Bottom
		{
			var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelService.GetPanelState());
        	bottomPanel.PanelService = PanelService;
		
			PanelService.ReduceInitializeResizeHandleDimensionUnitAction(
				bottomPanel.Key,
				new DimensionUnit(
					() => AppOptionsService.GetAppOptionsState().Options.ResizeHandleHeightInPixels / 2,
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
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelService.GetPanelState());
        leftPanel.PanelService = PanelService;

        // folderExplorerPanel
        var folderExplorerPanel = new Panel(
			"Folder Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.FolderExplorerContext.ContextKey,
            typeof(FolderExplorerDisplay),
			null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(folderExplorerPanel);
        PanelService.ReduceRegisterPanelTabAction(leftPanel.Key, folderExplorerPanel, false);

        // SetActivePanelTabAction
        PanelService.ReduceSetActivePanelTabAction(leftPanel.Key, folderExplorerPanel.Key);
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelService.GetPanelState());
        rightPanel.PanelService = PanelService;
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelService.GetPanelState());
        bottomPanel.PanelService = PanelService;

        // terminalGroupPanel
        var terminalGroupPanel = new Panel(
			"Terminal",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.TerminalContext.ContextKey,
            typeof(TerminalGroupDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(terminalGroupPanel);
        PanelService.ReduceRegisterPanelTabAction(bottomPanel.Key, terminalGroupPanel, false);
		// This UI has resizable parts that need to be initialized.
        Dispatcher.Dispatch(new TerminalGroupState.InitializeResizeHandleDimensionUnitAction(
            new DimensionUnit(
            	() => AppOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract,
            	DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)));

		// activeContextsPanel
        var activeContextsPanel = new Panel(
			"Active Contexts",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.ActiveContextsContext.ContextKey,
            typeof(ContextsPanelDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(activeContextsPanel);
        PanelService.ReduceRegisterPanelTabAction(bottomPanel.Key, activeContextsPanel, false);

        // SetActivePanelTabAction
        PanelService.ReduceSetActivePanelTabAction(bottomPanel.Key, terminalGroupPanel.Key);
    }
    
    private void AddGeneralTerminal()
    {
    	if (LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Wasm ||
    		LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
    	{
    		Dispatcher.Dispatch(new TerminalState.RegisterAction(
		    	new TerminalWebsite(
					"General",
					terminal => new TerminalInteractive(terminal),
					terminal => new TerminalInputStringBuilder(terminal),
					terminal => new TerminalOutput(
						terminal,
						new TerminalOutputFormatterExpand(
							terminal,
							TextEditorService,
							CompilerServiceRegistry,
							DialogService,
						    PanelService,
							JsRuntime)),
					BackgroundTaskService,
					CommonComponentRenderers,
					NotificationService)
				{
					Key = TerminalFacts.GENERAL_KEY
				}));
    	}
    	else
    	{
    		Dispatcher.Dispatch(new TerminalState.RegisterAction(
		    	new Terminal(
					"General",
					terminal => new TerminalInteractive(terminal),
					terminal => new TerminalInputStringBuilder(terminal),
					terminal => new TerminalOutput(
						terminal,
						new TerminalOutputFormatterExpand(
							terminal,
							TextEditorService,
							CompilerServiceRegistry,
							DialogService,
						    PanelService,
							JsRuntime)),
					BackgroundTaskService,
					CommonComponentRenderers,
					Dispatcher,
					NotificationService)
				{
					Key = TerminalFacts.GENERAL_KEY
				}));
    	}
    }
    
    private void AddExecutionTerminal()
    {
    	if (LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Wasm ||
    		LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
    	{
    		Dispatcher.Dispatch(new TerminalState.RegisterAction(
		    	new TerminalWebsite(
					"Execution",
					terminal => new TerminalInteractive(terminal),
					terminal => new TerminalInputStringBuilder(terminal),
					terminal => new TerminalOutput(
						terminal,
						new TerminalOutputFormatterExpand(
							terminal,
							TextEditorService,
							CompilerServiceRegistry,
							DialogService,
	 				       PanelService,
							JsRuntime)),
					BackgroundTaskService,
					CommonComponentRenderers,
					NotificationService)
				{
					Key = TerminalFacts.EXECUTION_KEY
				}));
    	}
    	else
    	{
    		Dispatcher.Dispatch(new TerminalState.RegisterAction(
		    	new Terminal(
					"Execution",
					terminal => new TerminalInteractive(terminal),
					terminal => new TerminalInputStringBuilder(terminal),
					terminal => new TerminalOutput(
						terminal,
						new TerminalOutputFormatterExpand(
							terminal,
							TextEditorService,
							CompilerServiceRegistry,
							DialogService,
	 				       PanelService,
							JsRuntime)),
					BackgroundTaskService,
					CommonComponentRenderers,
					Dispatcher,
					NotificationService)
				{
					Key = TerminalFacts.EXECUTION_KEY
				}));
    	}
    }
}