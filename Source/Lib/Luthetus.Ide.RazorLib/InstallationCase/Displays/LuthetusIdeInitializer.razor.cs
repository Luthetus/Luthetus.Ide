using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.Displays;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;
using Luthetus.Ide.RazorLib.FolderExplorerCase.Displays;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;
using Luthetus.Ide.RazorLib.TerminalCase.Displays;
using Luthetus.Ide.RazorLib.NugetCase.Displays;
using Luthetus.Common.RazorLib.ContextCase.Displays;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.Panel.Models;
using Luthetus.Common.RazorLib.TabCase.Models;
using Luthetus.Common.RazorLib.Panel.States;
using Luthetus.Common.RazorLib.Theme.States;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.TabCase.States;
using Luthetus.TextEditor.RazorLib.Installation.Models;
using Luthetus.TextEditor.RazorLib.Find.States;
using Luthetus.Common.RazorLib.CommandCase.Models.Ide;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.InstallationCase.Displays;

public partial class LuthetusIdeInitializer : ComponentBase
{
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorOptions LuthetusTextEditorOptions { get; set; } = null!;
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
            if (LuthetusTextEditorOptions.CustomThemeRecords is not null)
            {
                foreach (var themeRecord in LuthetusTextEditorOptions.CustomThemeRecords)
                {
                    Dispatcher.Dispatch(new ThemeState.RegisterAction(
                        themeRecord));
                }
            }

            foreach (var findProvider in LuthetusTextEditorOptions.FindProviders)
            {
                Dispatcher.Dispatch(new TextEditorFindProviderState.RegisterAction(
                    findProvider));
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

                Dispatcher.Dispatch(new TerminalSessionState.RegisterTerminalSessionAction(
                    terminalSession));
            }

            InitializePanelTabs();

            CommandFactory.Initialize();

            await InitializeCompilerServiceExplorerStateAsync();
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

        var solutionExplorerPanelTab = new PanelTab(
            Key<PanelTab>.NewKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Solution Explorer");

        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
            leftPanel.Key,
            solutionExplorerPanelTab,
            false));

        var folderExplorerPanelTab = new PanelTab(
            Key<PanelTab>.NewKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Folder Explorer");

        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
            leftPanel.Key,
            folderExplorerPanelTab,
            false));

        Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(
            leftPanel.Key,
            solutionExplorerPanelTab.Key));
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsStateWrap.Value);

        var compilerServiceExplorerPanelTab = new PanelTab(
            Key<PanelTab>.NewKey(),
            rightPanel.ElementDimensions,
            new(),
            typeof(CompilerServiceExplorerDisplay),
            typeof(IconFolder),
            "Compiler Service Explorer");

        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
            rightPanel.Key,
            compilerServiceExplorerPanelTab,
            false));
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsStateWrap.Value);

        var terminalPanelTab = new PanelTab(
            Key<PanelTab>.NewKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Terminal");

        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
            bottomPanel.Key,
            terminalPanelTab,
            false));

        var nuGetPanelTab = new PanelTab(
            Key<PanelTab>.NewKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(NuGetPackageManager),
            typeof(IconFolder),
            "NuGet");

        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
            bottomPanel.Key,
            nuGetPanelTab,
            false));

        var activeContextsPanelTab = new PanelTab(
            Key<PanelTab>.NewKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(ActiveContextsDisplay),
            typeof(IconFolder),
            "Active Contexts");

        Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
            bottomPanel.Key,
            activeContextsPanelTab,
            false));

        Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(
            bottomPanel.Key,
            terminalPanelTab.Key));
    }

    private async Task InitializeCompilerServiceExplorerStateAsync()
    {
        var tabGroup = new TabGroup(
            tabGroupLoadTabEntriesParameter =>
            {
                var viewKinds = Enum.GetValues<CompilerServiceExplorerViewKind>();

                var tabEntryNoTypes = viewKinds
                    .Select(viewKind => (TabEntryNoType)
                        new TabEntryWithType<CompilerServiceExplorerViewKind>(
                            viewKind,
                            tab => ((TabEntryWithType<CompilerServiceExplorerViewKind>)tab).Item.ToString(),
                            tab => { }
                        ))
                    .ToImmutableList();

                return Task.FromResult(new TabGroupLoadTabEntriesOutput(tabEntryNoTypes));
            },
            CompilerServiceExplorerState.TabGroupKey);

        Dispatcher.Dispatch(new TabState.RegisterTabGroupAction(tabGroup));

        var tabGroupLoadTabEntriesOutput = await tabGroup.LoadEntryBagAsync();

        Dispatcher.Dispatch(new TabState.SetTabEntryBagAction(
            tabGroup.Key,
            tabGroupLoadTabEntriesOutput.OutTabEntries));

        Dispatcher.Dispatch(new TabState.SetActiveTabEntryKeyAction(
            tabGroup.Key,
            tabGroupLoadTabEntriesOutput.OutTabEntries.Last().TabEntryKey));
    }
}