using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Nugets.Displays;
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.CompilerServices.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.Terminals.Displays;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.TestExplorers.Displays;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Ide.RazorLib.Gits.Displays;
using Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

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
            if (TextEditorConfig.CustomThemeRecordList is not null)
            {
                foreach (var themeRecord in TextEditorConfig.CustomThemeRecordList)
                {
                    Dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));
                }
            }

            foreach (var searchEngine in TextEditorConfig.SearchEngineList)
            {
                Dispatcher.Dispatch(new TextEditorSearchEngineState.RegisterAction(searchEngine));
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
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
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
        {
            var solutionExplorerPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                leftPanel.ElementDimensions,
                new(),
                typeof(SolutionExplorerDisplay),
                typeof(IconFolder),
                "Solution Explorer")
            {
                ContextRecordKey = ContextFacts.SolutionExplorerContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, solutionExplorerPanelTab, false));
        }

        // folderExplorerPanelTab
        {
            var folderExplorerPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                leftPanel.ElementDimensions,
                new(),
                typeof(FolderExplorerDisplay),
                typeof(IconFolder),
                "Folder Explorer")
            {
                ContextRecordKey = ContextFacts.FolderExplorerContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, folderExplorerPanelTab, false));
        }

        // solutionExplorerPanelTab
        {
            var testIntegratedTerminalPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                leftPanel.ElementDimensions,
                new(),
                typeof(TestIntegratedTerminal),
                typeof(IconFolder),
                "TestIntegratedTerminal")
            {
                ContextRecordKey = ContextFacts.TestIntegratedTerminalContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(leftPanel.Key, testIntegratedTerminalPanelTab, false));
        }

        // SetActivePanelTabAction
        {
            var targetActivePanelTab = PanelsStateWrap.Value.PanelGroupList
                .First(x => x.Key == leftPanel.Key)
                .TabList
                .First(x => x.ContextRecordKey == ContextFacts.SolutionExplorerContext.ContextKey);

            Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(leftPanel.Key, targetActivePanelTab.Key));
        }
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsStateWrap.Value);

        // compilerServiceExplorerPanelTab
        {
            var compilerServiceExplorerPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                rightPanel.ElementDimensions,
                new(),
                typeof(CompilerServiceExplorerDisplay),
                typeof(IconFolder),
                "Compiler Service Explorer")
            {
                ContextRecordKey = ContextFacts.CompilerServiceExplorerContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, compilerServiceExplorerPanelTab, false));
        }

        // compilerServiceEditorPanelTab
        {
            var compilerServiceEditorPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                rightPanel.ElementDimensions,
                new(),
                typeof(CompilerServiceEditorDisplay),
                typeof(IconFolder),
                "Compiler Service Editor")
            {
                ContextRecordKey = ContextFacts.CompilerServiceEditorContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, compilerServiceEditorPanelTab, false));
        }

        // gitChangesPanelTab
        {
            var gitChangesPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                rightPanel.ElementDimensions,
                new(),
                typeof(GitChangesDisplay),
                typeof(IconFolder),
                "Git")
            {
                ContextRecordKey = ContextFacts.GitContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(rightPanel.Key, gitChangesPanelTab, false));
        }
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsStateWrap.Value);

        // terminalPanelTab
        {
            var terminalPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                bottomPanel.ElementDimensions,
                new(),
                typeof(TerminalDisplay),
                typeof(IconFolder),
                "Terminal")
            {
                ContextRecordKey = ContextFacts.TerminalContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, terminalPanelTab, false));
        }

        // nuGetPanelTab
        {
            var nuGetPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                bottomPanel.ElementDimensions,
                new(),
                typeof(NuGetPackageManager),
                typeof(IconFolder),
                "NuGet")
            {
                ContextRecordKey = ContextFacts.NuGetPackageManagerContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, nuGetPanelTab, false));
        }

        // activeContextsPanelTab
        {
            var activeContextsPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                bottomPanel.ElementDimensions,
                new(),
                typeof(ContextsPanelDisplay),
                typeof(IconFolder),
                "Active Contexts")
            {
                ContextRecordKey = ContextFacts.ActiveContextsContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, activeContextsPanelTab, false));
        }

        // testExplorerPanelTab
        {
            var testExplorerPanelTab = new PanelTab(
                Key<PanelTab>.NewKey(),
                bottomPanel.ElementDimensions,
                new(),
                typeof(TestExplorerDisplay),
                typeof(IconFolder),
                "Test Explorer")
            {
                ContextRecordKey = ContextFacts.TestExplorerContext.ContextKey
            };

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(bottomPanel.Key, testExplorerPanelTab, false));
        }

        // SetActivePanelTabAction
        {
            var targetActivePanelTab = PanelsStateWrap.Value.PanelGroupList
                .First(x => x.Key == bottomPanel.Key)
                .TabList
                .First(x => x.ContextRecordKey == ContextFacts.TestExplorerContext.ContextKey);

            Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(bottomPanel.Key, targetActivePanelTab.Key));
        }
    }
}