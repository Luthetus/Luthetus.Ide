using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Icons.Codicon;
using Luthetus.Common.RazorLib.Panel;
using Luthetus.Common.RazorLib.Store.PanelCase;
using Luthetus.Common.RazorLib.Store.TabCase;
using Luthetus.Common.RazorLib.Store.ThemeCase;
using Luthetus.Common.RazorLib.TabCase;
using Luthetus.Ide.ClassLib.CommandCase;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.InnerDetails;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.RazorLib.BackgroundServiceCase;
using Luthetus.Ide.RazorLib.CompilerServiceExplorer;
using Luthetus.Ide.RazorLib.ContextCase;
using Luthetus.Ide.RazorLib.FolderExplorer;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.SolutionExplorer;
using Luthetus.Ide.RazorLib.Terminal;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Store.Find;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib;

public partial class LuthetusIdeInitializer : ComponentBase
{
    [Inject]
    private IState<PanelsRegistry> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorOptions LuthetusTextEditorOptions { get; set; } = null!;
    [Inject]
    private ILuthetusIdeTerminalBackgroundTaskService TerminalBackgroundTaskQueue { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private LuthetusIdeFileSystemBackgroundTaskServiceWorker LuthetusIdeFileSystemBackgroundTaskServiceWorker { get; set; } = null!;
    [Inject]
    private LuthetusIdeTerminalBackgroundTaskServiceWorker LuthetusIdeTerminalBackgroundTaskServiceWorker { get; set; } = null!;
    [Inject]
    private ICommandFactory CommandFactory { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.ServerSide)
        {
            _ = Task.Run(async () => await LuthetusIdeFileSystemBackgroundTaskServiceWorker
                .StartAsync(CancellationToken.None)
                .ConfigureAwait(false));

            _ = Task.Run(async () => await LuthetusIdeTerminalBackgroundTaskServiceWorker
                .StartAsync(CancellationToken.None)
                .ConfigureAwait(false));
        }

        if (LuthetusTextEditorOptions.CustomThemeRecords is not null)
        {
            foreach (var themeRecord in LuthetusTextEditorOptions.CustomThemeRecords)
            {
                Dispatcher.Dispatch(new ThemeRecordRegistry.RegisterAction(
                    themeRecord));
            }
        }

        foreach (var findProvider in LuthetusTextEditorOptions.FindProviders)
        {
            Dispatcher.Dispatch(new TextEditorFindProviderRegistry.RegisterAction(
                findProvider));
        }

        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null,
                Dispatcher,
                FileSystemProvider,
                TerminalBackgroundTaskQueue,
                LuthetusCommonComponentRenderers)
            {
                TerminalSessionKey = terminalSessionKey
            };

            Dispatcher.Dispatch(new TerminalSessionRegistryReducer.RegisterTerminalSessionAction(
                terminalSession));
        }

        InitializePanelTabs();

        CommandFactory.Initialize();

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
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
        var leftPanel = PanelFacts.GetLeftPanelRecord(PanelsCollectionWrap.Value);

        var solutionExplorerPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Solution Explorer");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            leftPanel.Key,
            solutionExplorerPanelTab,
            false));

        var folderExplorerPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Folder Explorer");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            leftPanel.Key,
            folderExplorerPanelTab,
            false));

        Dispatcher.Dispatch(new PanelsRegistry.SetActiveEntryAction(
            leftPanel.Key,
            solutionExplorerPanelTab.Key));
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsCollectionWrap.Value);

        //var notificationsPanelTab = new PanelTab(
        //    PanelTabKey.NewPanelTabKey(),
        //    rightPanel.ElementDimensions,
        //    new(),
        //    typeof(NotificationsDisplay),
        //    typeof(IconFolder),
        //    "Notifications");

        //Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
        //    rightPanel.PanelRecordKey,
        //    notificationsPanelTab,
        //    false));

        var compilerServiceExplorerPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            rightPanel.ElementDimensions,
            new(),
            typeof(CompilerServiceExplorerDisplay),
            typeof(IconFolder),
            "Compiler Service Explorer");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            rightPanel.Key,
            compilerServiceExplorerPanelTab,
            false));

        var backgroundServicesPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            rightPanel.ElementDimensions,
            new(),
            typeof(BackgroundServicesDisplay),
            typeof(IconFolder),
            "Background Tasks");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            rightPanel.Key,
            backgroundServicesPanelTab,
            false));
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsCollectionWrap.Value);

        var terminalPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Terminal");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            bottomPanel.Key,
            terminalPanelTab,
            false));

        var nuGetPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(NuGetPackageManager),
            typeof(IconFolder),
            "NuGet");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            bottomPanel.Key,
            nuGetPanelTab,
            false));

        var activeContextsPanelTab = new PanelTab(
            PanelTabKey.NewKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(ActiveContextsDisplay),
            typeof(IconFolder),
            "Active Contexts");

        Dispatcher.Dispatch(new PanelsRegistry.RegisterEntryAction(
            bottomPanel.Key,
            activeContextsPanelTab,
            false));

        Dispatcher.Dispatch(new PanelsRegistry.SetActiveEntryAction(
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
            CompilerServiceExplorerRegistry.TabGroupKey);

        Dispatcher.Dispatch(new TabRegistry.RegisterGroupAction(tabGroup));
        
        var tabGroupLoadTabEntriesOutput = await tabGroup.LoadEntryBagAsync();

        Dispatcher.Dispatch(new TabRegistry.SetEntryBagAction(
            tabGroup.Key,
            tabGroupLoadTabEntriesOutput.OutTabEntries));

        Dispatcher.Dispatch(new TabRegistry.SetActiveEntryKeyAction(
            tabGroup.Key,
            tabGroupLoadTabEntriesOutput.OutTabEntries.Last().TabEntryKey));
    }
}