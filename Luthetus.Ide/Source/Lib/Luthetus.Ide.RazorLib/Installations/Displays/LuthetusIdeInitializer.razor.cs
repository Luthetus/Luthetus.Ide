using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.Nugets.Displays;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.CompilerServices.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.TestExplorers.Displays;

namespace Luthetus.Ide.RazorLib.Installations.Displays;

public partial class LuthetusIdeInitializer : ComponentBase
{
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommandFactory CommandFactory { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            BackgroundTaskService.Enqueue(
                Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(LuthetusIdeInitializer),
                () =>
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

                    foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
                    {
                        var displayName = $"BAD_WellKnownTerminalSessionKey:{terminalSessionKey.Guid}";

                        if (terminalSessionKey == TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY)
                            displayName = "Execution";
                        else if (terminalSessionKey == TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY)
                            displayName = "General";

                        var terminalSession = new TerminalSession(
                            displayName,
                            null,
                            Dispatcher,
                            BackgroundTaskService,
                            LuthetusCommonComponentRenderers)
                        {
                            TerminalSessionKey = terminalSessionKey
                        };

                        Dispatcher.Dispatch(new TerminalSessionState.RegisterTerminalSessionAction(terminalSession));
                    }

                    InitializePanelTabs();

                    CommandFactory.Initialize();

                    return Task.CompletedTask;
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
        var leftPanel = PanelFacts.GetLeftPanelRecord(PanelsStateWrap.Value);

        // solutionExplorerPanel
        var solutionExplorerPanel = new Panel(
			"Solution Explorer",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.SolutionExplorerContext.ContextKey,
			typeof(SolutionExplorerDisplay),
			null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(solutionExplorerPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, solutionExplorerPanel, false));

        // folderExplorerPanel
        var folderExplorerPanel = new Panel(
			"Folder Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.FolderExplorerContext.ContextKey,
            typeof(FolderExplorerDisplay),
			null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(folderExplorerPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, folderExplorerPanel, false));

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(leftPanel.Key, solutionExplorerPanel.Key));
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsStateWrap.Value);

        // compilerServiceExplorerPanel
        var compilerServiceExplorerPanel = new Panel(
			"Compiler Service Explorer",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.CompilerServiceExplorerContext.ContextKey,
            typeof(CompilerServiceExplorerDisplay),
			null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(compilerServiceExplorerPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanel, false));

        // compilerServiceEditorPanel
        var compilerServiceEditorPanel = new Panel(
			"Compiler Service Editor",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.CompilerServiceEditorContext.ContextKey,
            typeof(CompilerServiceEditorDisplay),
            null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(compilerServiceEditorPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanel, false));

        // TODO: The ITextEditorDiffApi.Calculate method is being commented out as of (2024-02-23). It needs to be re-written...
        // ...so that it uses the text editor's edit context by using ITextEditorService.Post()
        //
        // // gitChangesPanel
        // var gitChangesPanel = new Panel(
        //     Key<Panel>.NewKey(),
        //     rightPanel.ElementDimensions,
        //     typeof(GitChangesDisplay),
        //     "Git")
        // {
        //     ContextRecordKey = ContextFacts.GitContext.ContextKey
        // };
        // Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(gitChangesPanel));
        // Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, gitChangesPanel, false));
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsStateWrap.Value);

        // terminalGroupPanel
        var terminalGroupPanel = new Panel(
			"Terminal",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.TerminalContext.ContextKey,
            typeof(TerminalGroupDisplay),
            null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(terminalGroupPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, terminalGroupPanel, false));

        // nuGetPanel
        var nuGetPanel = new Panel(
			"NuGet",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.NuGetPackageManagerContext.ContextKey,
            typeof(NuGetPackageManager),
            null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(nuGetPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, nuGetPanel, false));

        // activeContextsPanel
        var activeContextsPanel = new Panel(
			"Active Contexts",
			Key<Panel>.NewKey(),
			Key<IDynamicViewModel>.NewKey(),
			ContextFacts.ActiveContextsContext.ContextKey,
            typeof(ContextsPanelDisplay),
            null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(activeContextsPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, activeContextsPanel, false));

        // testExplorerPanel
        var testExplorerPanel = new Panel(
			"Test Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
			ContextFacts.TestExplorerContext.ContextKey,
			typeof(TestExplorerDisplay),
            null);
        Dispatcher.Dispatch(new PanelsState.RegisterPanelAction(testExplorerPanel));
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, testExplorerPanel, false));

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(bottomPanel.Key, terminalGroupPanel.Key));
    }
}