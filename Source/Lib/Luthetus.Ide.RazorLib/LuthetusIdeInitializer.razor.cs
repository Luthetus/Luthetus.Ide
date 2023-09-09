using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Icons.Codicon;
using Luthetus.Common.RazorLib.Panel;
using Luthetus.Common.RazorLib.Store.PanelCase;
using Luthetus.Common.RazorLib.Store.TabGroupCase;
using Luthetus.Common.RazorLib.Store.ThemeCase;
using Luthetus.Common.RazorLib.TabGroups;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.InnerDetails;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.RazorLib.BackgroundServiceCase;
using Luthetus.Ide.RazorLib.CompilerServiceExplorer;
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
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
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
                Dispatcher.Dispatch(new ThemeRecordsCollection.RegisterAction(
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
                FileSystemProvider,
                TerminalBackgroundTaskQueue,
                LuthetusCommonComponentRenderers)
            {
                TerminalSessionKey = terminalSessionKey
            };

            Dispatcher.Dispatch(new TerminalSessionsReducer.RegisterTerminalSessionAction(
                terminalSession));
        }

        InitializePanelTabs();

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
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(SolutionExplorerDisplay),
            typeof(IconFolder),
            "Solution Explorer");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            solutionExplorerPanelTab,
            false));

        var folderExplorerPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Folder Explorer");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            folderExplorerPanelTab,
            false));

        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            leftPanel.PanelRecordKey,
            solutionExplorerPanelTab.PanelTabKey));
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
            PanelTabKey.NewPanelTabKey(),
            rightPanel.ElementDimensions,
            new(),
            typeof(CompilerServiceExplorerDisplay),
            typeof(IconFolder),
            "Compiler Explorer");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            rightPanel.PanelRecordKey,
            compilerServiceExplorerPanelTab,
            false));

        var backgroundServicesPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            rightPanel.ElementDimensions,
            new(),
            typeof(BackgroundServicesDisplay),
            typeof(IconFolder),
            "Background Tasks");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            rightPanel.PanelRecordKey,
            backgroundServicesPanelTab,
            false));
    }

    private void InitializeBottomPanelTabs()
    {
        var bottomPanel = PanelFacts.GetBottomPanelRecord(PanelsCollectionWrap.Value);

        var terminalPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(TerminalDisplay),
            typeof(IconFolder),
            "Terminal");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            terminalPanelTab,
            false));

        var nuGetPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(NuGetPackageManager),
            typeof(IconFolder),
            "NuGet");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            nuGetPanelTab,
            false));

        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            bottomPanel.PanelRecordKey,
            terminalPanelTab.PanelTabKey));
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
                            tab => Dispatcher.Dispatch(new TabGroupsCollection.SetActiveTabEntryKeyAction(tab.TabEntryKey))
                        ))
                    .ToImmutableList();

                return Task.FromResult(new TabGroupLoadTabEntriesOutput(tabEntryNoTypes));
            },
            CompilerServiceExplorerState.TabGroupKey);

        Dispatcher.Dispatch(new TabGroupsCollection.RegisterAction(tabGroup));
        
        var tabGroupLoadTabEntriesOutput = await tabGroup.LoadTabEntriesAsync();

        Dispatcher.Dispatch(new TabGroupsCollection.SetTabEntriesAction(
            tabGroupLoadTabEntriesOutput.OutTabEntries));

    }
}