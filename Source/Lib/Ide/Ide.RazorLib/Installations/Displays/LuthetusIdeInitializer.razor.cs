using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.Models;
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
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITerminalService TerminalService { get; set; } = null!;
    [Inject]
    private ITerminalGroupService TerminalGroupService { get; set; } = null!;
    [Inject]
    private ICommandFactory CommandFactory { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private ICodeSearchService CodeSearchService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

	protected override void OnInitialized()
	{
        CommonApi.BackgroundTaskApi.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            nameof(LuthetusIdeInitializer),
            async () =>
            {
                if (TextEditorConfig.CustomThemeRecordList is not null)
                {
                    foreach (var themeRecord in TextEditorConfig.CustomThemeRecordList)
                    {
                        CommonApi.ThemeApi.ReduceRegisterAction(themeRecord);
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
						() => CommonApi.AppOptionApi.GetAppOptionsState().Options.ResizeHandleHeightInPixels / 2,
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
			if (CommonApi.HostingInformationApi.LuthetusHostingKind == LuthetusHostingKind.Photino)
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
			var leftPanel = PanelFacts.GetTopLeftPanelGroup(CommonApi.PanelApi.GetPanelState());
        	leftPanel.PanelService = CommonApi.PanelApi;
		
			CommonApi.PanelApi.ReduceInitializeResizeHandleDimensionUnitAction(
				leftPanel.Key,
				new DimensionUnit(
					() => CommonApi.AppOptionApi.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
					DimensionUnitKind.Pixels,
					DimensionOperatorKind.Subtract,
					DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));
		}
		
		// Right
		{
			var rightPanel = PanelFacts.GetTopRightPanelGroup(CommonApi.PanelApi.GetPanelState());
        	rightPanel.PanelService = CommonApi.PanelApi;
		
			CommonApi.PanelApi.ReduceInitializeResizeHandleDimensionUnitAction(
				rightPanel.Key,
				new DimensionUnit(
					() => CommonApi.AppOptionApi.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
					DimensionUnitKind.Pixels,
					DimensionOperatorKind.Subtract,
					DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN));
		}
		
		// Bottom
		{
			var bottomPanel = PanelFacts.GetBottomPanelGroup(CommonApi.PanelApi.GetPanelState());
        	bottomPanel.PanelService = CommonApi.PanelApi;
		
			CommonApi.PanelApi.ReduceInitializeResizeHandleDimensionUnitAction(
				bottomPanel.Key,
				new DimensionUnit(
					() => CommonApi.AppOptionApi.GetAppOptionsState().Options.ResizeHandleHeightInPixels / 2,
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
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(CommonApi.PanelApi.GetPanelState());
        leftPanel.PanelService = CommonApi.PanelApi;

        // folderExplorerPanel
        var folderExplorerPanel = new Panel(
			"Folder Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.FolderExplorerContext.ContextKey,
            typeof(FolderExplorerDisplay),
			null,
            CommonApi,
            JsRuntime);
        CommonApi.PanelApi.ReduceRegisterPanelAction(folderExplorerPanel);
        CommonApi.PanelApi.ReduceRegisterPanelTabAction(leftPanel.Key, folderExplorerPanel, false);

        // SetActivePanelTabAction
        CommonApi.PanelApi.ReduceSetActivePanelTabAction(leftPanel.Key, folderExplorerPanel.Key);
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetTopRightPanelGroup(CommonApi.PanelApi.GetPanelState());
        rightPanel.PanelService = CommonApi.PanelApi;
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelGroup(CommonApi.PanelApi.GetPanelState());
        bottomPanel.PanelService = CommonApi.PanelApi;

        // terminalGroupPanel
        var terminalGroupPanel = new Panel(
			"Terminal",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.TerminalContext.ContextKey,
            typeof(TerminalGroupDisplay),
            null,
            CommonApi,
            JsRuntime);
        CommonApi.PanelApi.ReduceRegisterPanelAction(terminalGroupPanel);
        CommonApi.PanelApi.ReduceRegisterPanelTabAction(bottomPanel.Key, terminalGroupPanel, false);
		// This UI has resizable parts that need to be initialized.
        TerminalGroupService.ReduceInitializeResizeHandleDimensionUnitAction(
            new DimensionUnit(
            	() => CommonApi.AppOptionApi.GetAppOptionsState().Options.ResizeHandleWidthInPixels / 2,
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
            CommonApi,
            JsRuntime);
        CommonApi.PanelApi.ReduceRegisterPanelAction(activeContextsPanel);
        CommonApi.PanelApi.ReduceRegisterPanelTabAction(bottomPanel.Key, activeContextsPanel, false);

        // SetActivePanelTabAction
        CommonApi.PanelApi.ReduceSetActivePanelTabAction(bottomPanel.Key, terminalGroupPanel.Key);
    }
    
    private void AddGeneralTerminal()
    {
    	if (CommonApi.HostingInformationApi.LuthetusHostingKind == LuthetusHostingKind.Wasm ||
    		CommonApi.HostingInformationApi.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
    	{
    		TerminalService.ReduceRegisterAction(
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
                            CommonApi,
							JsRuntime)),
					CommonApi)
				{
					Key = TerminalFacts.GENERAL_KEY
				});
    	}
    	else
    	{
    		TerminalService.ReduceRegisterAction(
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
                            CommonApi,
							JsRuntime)),
					CommonApi,
					TerminalService)
				{
					Key = TerminalFacts.GENERAL_KEY
				});
    	}
    }
    
    private void AddExecutionTerminal()
    {
    	if (CommonApi.HostingInformationApi.LuthetusHostingKind == LuthetusHostingKind.Wasm ||
    		CommonApi.HostingInformationApi.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
    	{
    		TerminalService.ReduceRegisterAction(
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
							CommonApi,
							JsRuntime)),
					CommonApi)
				{
					Key = TerminalFacts.EXECUTION_KEY
				});
    	}
    	else
    	{
    		TerminalService.ReduceRegisterAction(
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
							CommonApi,
							JsRuntime)),
					CommonApi,
					TerminalService)
				{
					Key = TerminalFacts.EXECUTION_KEY
				});
    	}
    }
}