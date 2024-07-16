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
using Luthetus.Ide.RazorLib.Outputs.Displays;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.CompilerServices.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.TestExplorers.Displays;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
    	// TODO: This needs to be moved to 'OnInitializedAsync' otherwise...
    	//       ...if one refreshes the application then this code, as it is now,
    	//       will not run a second time.
        if (firstRender)
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

                    foreach (var searchEngine in TextEditorConfig.SearchEngineList)
                    {
                        Dispatcher.Dispatch(new TextEditorFindAllState.RegisterAction(searchEngine));
                    }

                    foreach (var terminalKey in TerminalFacts.WELL_KNOWN_TERMINAL_KEYS)
                    {
                        var displayName = $"BAD_WellKnownTerminalKey:{terminalKey.Guid}";

                        if (terminalKey == TerminalFacts.EXECUTION_TERMINAL_KEY)
                            displayName = "Execution";
                        else if (terminalKey == TerminalFacts.GENERAL_TERMINAL_KEY)
                            displayName = "General";

                        var terminal = await Terminal.Factory(
                            displayName,
                            null,
                            Dispatcher,
                            BackgroundTaskService,
                            TextEditorService,
                            CommonComponentRenderers,
                            CompilerServiceRegistry,
							terminalKey);

                        Dispatcher.Dispatch(new TerminalState.RegisterAction(terminal));
                    }

                    InitializePanelTabs();
                    CommandFactory.Initialize();
                });
        }

        await base.OnAfterRenderAsync(firstRender);
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

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(bottomPanel.Key, terminalGroupPanel.Key));
    }
}