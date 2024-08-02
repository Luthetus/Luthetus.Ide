using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.Gits.Displays;

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

                foreach (var terminalKey in TerminalFacts.WELL_KNOWN_TERMINAL_KEYS)
                {
                	if (terminalKey == TerminalFacts.GENERAL_TERMINAL_KEY)
                	{
                		NEW_TERMINAL_CODE();
                	}
                	else if (terminalKey == TerminalFacts.EXECUTION_TERMINAL_KEY)
                	{
                		EXECUTION_TERMINAL_CODE();
                	}
                }

                InitializePanelTabs();
                CommandFactory.Initialize();
            });
            
        base.OnInitialized();
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
    
    private void NEW_TERMINAL_CODE()
    {
    	Dispatcher.Dispatch(new TerminalState.NEW_TERMINAL_CODE_RegisterAction(
	    	new NEW_Terminal(
				"General",
				terminal => new TerminalInteractive(terminal),
				terminal => new TerminalInputStringBuilder(terminal),
				terminal => new TerminalOutput(
					terminal,
					new TerminalOutputFormatterExpand(
						terminal,
						TextEditorService,
						CompilerServiceRegistry,
						Dispatcher)),
				BackgroundTaskService,
				CommonComponentRenderers,
				Dispatcher)));
    }
    
    private void EXECUTION_TERMINAL_CODE()
    {
    	Dispatcher.Dispatch(new TerminalState.EXECUTION_TERMINAL_CODE_RegisterAction(
	    	new NEW_Terminal(
				"Execution",
				terminal => new TerminalInteractive(terminal),
				terminal => new TerminalInputStringBuilder(terminal),
				terminal => new TerminalOutput(
					terminal,
					new TerminalOutputFormatterExpand(
						terminal,
						TextEditorService,
						CompilerServiceRegistry,
						Dispatcher)),
				BackgroundTaskService,
				CommonComponentRenderers,
				Dispatcher)));
    }
}