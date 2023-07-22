using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Icons.Codicon;
using Luthetus.Common.RazorLib.Store.ThemeCase;
using Luthetus.Ide.ClassLib.Panel;
using Luthetus.Ide.ClassLib.Store.PanelCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.RazorLib.FolderExplorer;
using Luthetus.Ide.RazorLib.Notification;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.SolutionExplorer;
using Luthetus.Ide.RazorLib.Terminal;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Store.Find;
using Microsoft.AspNetCore.Components;

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
    private ICommonBackgroundTaskQueue CommonBackgroundTaskQueue { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (LuthetusTextEditorOptions.CustomThemeRecords is not null)
        {
            foreach (var themeRecord in LuthetusTextEditorOptions.CustomThemeRecords)
            {
                Dispatcher.Dispatch(
                    new ThemeRecordsCollection.RegisterAction(
                        themeRecord));
            }
        }

        foreach (var findProvider in LuthetusTextEditorOptions.FindProviders)
        {
            Dispatcher.Dispatch(
                new TextEditorFindProviderState.RegisterAction(
                    findProvider));
        }

        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null,
                Dispatcher,
                FileSystemProvider,
                CommonBackgroundTaskQueue,
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
            solutionExplorerPanelTab));

        var folderExplorerPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            leftPanel.ElementDimensions,
            new(),
            typeof(FolderExplorerDisplay),
            typeof(IconFolder),
            "Folder Explorer");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            leftPanel.PanelRecordKey,
            folderExplorerPanelTab));

        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            leftPanel.PanelRecordKey,
            solutionExplorerPanelTab.PanelTabKey));
    }

    private void InitializeRightPanelTabs()
    {
        var rightPanel = PanelFacts.GetRightPanelRecord(PanelsCollectionWrap.Value);

        var notificationsPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            rightPanel.ElementDimensions,
            new(),
            typeof(NotificationHistoryDisplay),
            typeof(IconFolder),
            "Notifications");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            rightPanel.PanelRecordKey,
            notificationsPanelTab));
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
            terminalPanelTab));

        var nuGetPanelTab = new PanelTab(
            PanelTabKey.NewPanelTabKey(),
            bottomPanel.ElementDimensions,
            new(),
            typeof(NuGetPackageManager),
            typeof(IconFolder),
            "NuGet");

        Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
            bottomPanel.PanelRecordKey,
            nuGetPanelTab));

        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            bottomPanel.PanelRecordKey,
            terminalPanelTab.PanelTabKey));
    }
}