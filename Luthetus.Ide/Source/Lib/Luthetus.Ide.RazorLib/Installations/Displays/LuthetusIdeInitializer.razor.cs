using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.Nugets.Displays;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.CompilerServices.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.Outputs.Displays;
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
                        var terminalSession = new TerminalSession(
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

        // solutionExplorerPanelTab
        var solutionExplorerPanelTab = new Panel(
            Key<Panel>.NewKey(),
            leftPanel.ElementDimensions,
			typeof(SolutionExplorerDisplay),
            "Solution Explorer")
        {
            ContextRecordKey = ContextFacts.SolutionExplorerContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, solutionExplorerPanelTab, false));

        // folderExplorerPanelTab
        var folderExplorerPanelTab = new Panel(
            Key<Panel>.NewKey(),
            leftPanel.ElementDimensions,
            typeof(FolderExplorerDisplay),
            "Folder Explorer")
        {
            ContextRecordKey = ContextFacts.FolderExplorerContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, folderExplorerPanelTab, false));

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(leftPanel.Key, solutionExplorerPanelTab.Key));
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsStateWrap.Value);

        // compilerServiceExplorerPanelTab
        var compilerServiceExplorerPanelTab = new Panel(
            Key<Panel>.NewKey(),
            rightPanel.ElementDimensions,
            typeof(CompilerServiceExplorerDisplay),
            "Compiler Service Explorer")
        {
            ContextRecordKey = ContextFacts.CompilerServiceExplorerContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanelTab, false));

        // compilerServiceEditorPanelTab
        var compilerServiceEditorPanelTab = new Panel(
            Key<Panel>.NewKey(),
            rightPanel.ElementDimensions,
            typeof(CompilerServiceEditorDisplay),
            "Compiler Service Editor")
        {
            ContextRecordKey = ContextFacts.CompilerServiceEditorContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanelTab, false));

        // TODO: The ITextEditorDiffApi.Calculate method is being commented out as of (2024-02-23). It needs to be re-written...
        // ...so that it uses the text editor's edit context by using ITextEditorService.Post()
        //
        // // gitChangesPanelTab
        // var gitChangesPanelTab = new Panel(
        //     Key<Panel>.NewKey(),
        //     rightPanel.ElementDimensions,
        //     typeof(GitChangesDisplay),
        //     "Git")
        // {
        //     ContextRecordKey = ContextFacts.GitContext.ContextKey
        // };
        // Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, gitChangesPanelTab, false));
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsStateWrap.Value);

        // terminalPanelTab
        var terminalPanelTab = new Panel(
            Key<Panel>.NewKey(),
            bottomPanel.ElementDimensions,
            typeof(IntegratedTerminalDisplay),
            "Terminal")
        {
            ContextRecordKey = ContextFacts.TerminalContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, terminalPanelTab, false));

        // outputPanelTab
        var outputPanelTab = new Panel(
            Key<Panel>.NewKey(),
            bottomPanel.ElementDimensions,
            typeof(OutputPanelDisplay),
            "Output")
        {
            ContextRecordKey = ContextFacts.OutputContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, outputPanelTab, false));

        // nuGetPanelTab
        var nuGetPanelTab = new Panel(
            Key<Panel>.NewKey(),
            bottomPanel.ElementDimensions,
            typeof(NuGetPackageManager),
            "NuGet")
        {
            ContextRecordKey = ContextFacts.NuGetPackageManagerContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, nuGetPanelTab, false));

        // activeContextsPanelTab
        var activeContextsPanelTab = new Panel(
            Key<Panel>.NewKey(),
            bottomPanel.ElementDimensions,
            typeof(ContextsPanelDisplay),
            "Active Contexts")
        {
            ContextRecordKey = ContextFacts.ActiveContextsContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, activeContextsPanelTab, false));

        // testExplorerPanelTab
        var testExplorerPanelTab = new Panel(
            Key<Panel>.NewKey(),
            bottomPanel.ElementDimensions,
            typeof(TestExplorerDisplay),
            "Test Explorer")
        {
            ContextRecordKey = ContextFacts.TestExplorerContext.ContextKey
        };
        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, testExplorerPanelTab, false));

        // SetActivePanelTabAction
        Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(bottomPanel.Key, terminalPanelTab.Key));
    }
}