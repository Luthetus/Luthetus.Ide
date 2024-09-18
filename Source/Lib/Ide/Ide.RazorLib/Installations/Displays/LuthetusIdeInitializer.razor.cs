using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.Ide.RazorLib.Gits.Displays;
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
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
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
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;

	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            nameof(LuthetusIdeInitializer),
            async () =>
            {
                if (TextEditorConfig.CustomThemeRecordList is not null)
                {
                    foreach (var themeRecord in TextEditorConfig.CustomThemeRecordList)
                    {
                        Dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));
                    }
                }

                foreach (var terminalKey in TerminalFacts.WELL_KNOWN_KEYS)
                {
                	if (terminalKey == TerminalFacts.GENERAL_KEY)
                		AddGeneralTerminal();
                	else if (terminalKey == TerminalFacts.EXECUTION_KEY)
                		AddExecutionTerminal();
                }
                
                Dispatcher.Dispatch(new CodeSearchState.InitializeResizeHandleDimensionUnitAction(
					new DimensionUnit
		            {
		                ValueFunc = () => AppOptionsStateWrap.Value.Options.ResizeHandleHeightInPixels / 2,
		                DimensionUnitKind = DimensionUnitKind.Pixels,
		                DimensionOperatorKind = DimensionOperatorKind.Subtract,
		                Purpose = DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW,
		            }));

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
			var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelStateWrap.Value);
        	leftPanel.Dispatcher = Dispatcher;
		
			Dispatcher.Dispatch(new PanelState.InitializeResizeHandleDimensionUnitAction(
				leftPanel.Key,
				new DimensionUnit
	            {
	                ValueFunc = () => AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
	                DimensionUnitKind = DimensionUnitKind.Pixels,
	                DimensionOperatorKind = DimensionOperatorKind.Subtract,
	                Purpose = DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN,
	            }));
		}
		
		// Right
		{
			var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelStateWrap.Value);
        	rightPanel.Dispatcher = Dispatcher;
		
			Dispatcher.Dispatch(new PanelState.InitializeResizeHandleDimensionUnitAction(
				rightPanel.Key,
				new DimensionUnit
	            {
	                ValueFunc = () => AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
	                DimensionUnitKind = DimensionUnitKind.Pixels,
	                DimensionOperatorKind = DimensionOperatorKind.Subtract,
	                Purpose = DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN,
	            }));
		}
		
		// Bottom
		{
			var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelStateWrap.Value);
        	bottomPanel.Dispatcher = Dispatcher;
		
			Dispatcher.Dispatch(new PanelState.InitializeResizeHandleDimensionUnitAction(
				bottomPanel.Key,
				new DimensionUnit
	            {
	                ValueFunc = () => AppOptionsStateWrap.Value.Options.ResizeHandleHeightInPixels / 2,
	                DimensionUnitKind = DimensionUnitKind.Pixels,
	                DimensionOperatorKind = DimensionOperatorKind.Subtract,
	                Purpose = DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW,
	            }));
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

        // gitPanel
        var gitPanel = new Panel(
            "Git Changes",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.GitContext.ContextKey,
            typeof(GitDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(gitPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(leftPanel.Key, gitPanel, false));

        // folderExplorerPanel
        var folderExplorerPanel = new Panel(
			"Folder Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.FolderExplorerContext.ContextKey,
            typeof(FolderExplorerDisplay),
			null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(folderExplorerPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(leftPanel.Key, folderExplorerPanel, false));

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(leftPanel.Key, folderExplorerPanel.Key));
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetTopRightPanelGroup(PanelStateWrap.Value);
        rightPanel.Dispatcher = Dispatcher;
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelGroup(PanelStateWrap.Value);
        bottomPanel.Dispatcher = Dispatcher;

        // terminalGroupPanel
        var terminalGroupPanel = new Panel(
			"Terminal",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.TerminalContext.ContextKey,
            typeof(TerminalGroupDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(terminalGroupPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, terminalGroupPanel, false));
		// This UI has resizable parts that need to be initialized.
        Dispatcher.Dispatch(new TerminalGroupState.InitializeResizeHandleDimensionUnitAction(
            new DimensionUnit
            {
                ValueFunc = () => AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                Purpose = DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN,
            }));

		// activeContextsPanel
        var activeContextsPanel = new Panel(
			"Active Contexts",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.ActiveContextsContext.ContextKey,
            typeof(ContextsPanelDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(activeContextsPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(bottomPanel.Key, activeContextsPanel, false));

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(bottomPanel.Key, terminalGroupPanel.Key));
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
						       JsRuntime,
							Dispatcher)),
					BackgroundTaskService,
					CommonComponentRenderers,
					Dispatcher)
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
						       JsRuntime,
							Dispatcher)),
					BackgroundTaskService,
					CommonComponentRenderers,
					Dispatcher)
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
	 				       JsRuntime,
							Dispatcher)),
					BackgroundTaskService,
					CommonComponentRenderers,
					Dispatcher)
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
	 				       JsRuntime,
							Dispatcher)),
					BackgroundTaskService,
					CommonComponentRenderers,
					Dispatcher)
				{
					Key = TerminalFacts.EXECUTION_KEY
				}));
    	}
    }
}