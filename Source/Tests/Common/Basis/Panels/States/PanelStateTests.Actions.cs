using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Panels.States;

/// <summary>
/// <see cref="PanelState"/>
/// </summary>
public class PanelStateActionsTests
{
    /// <summary>
    /// <see cref="PanelState.RegisterPanelGroupAction"/>
    /// </summary>
    [Fact]
    public void RegisterPanelGroupAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);
        var registerPanelGroupAction = new PanelState.RegisterPanelGroupAction(panelGroup);
        Assert.Equal(panelGroup, registerPanelGroupAction.PanelGroup);
    }

    /// <summary>
    /// <see cref="PanelState.DisposePanelGroupAction"/>
    /// </summary>
    [Fact]
    public void DisposePanelGroupAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);
        var disposePanelGroupAction = new PanelState.DisposePanelGroupAction(panelGroup.Key);
        Assert.Equal(panelGroup.Key, disposePanelGroupAction.PanelGroupKey);
    }

    /// <summary>
    /// <see cref="PanelState.RegisterPanelTabAction"/>
    /// </summary>
    [Fact]
    public void RegisterPanelTabAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);

        // InsertAtIndexZero == false
        {
            var insertAtIndexZero = false;

            var registerPanelTabAction = new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                panelTab,
                insertAtIndexZero);

            Assert.Equal(panelGroup.Key, registerPanelTabAction.PanelGroupKey);
            Assert.Equal(panelTab, registerPanelTabAction.PanelTab);
            Assert.Equal(insertAtIndexZero, registerPanelTabAction.InsertAtIndexZero);
        }

        // InsertAtIndexZero == true
        {
            var insertAtIndexZero = true;

            var registerPanelTabAction = new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                panelTab,
                insertAtIndexZero);

            Assert.Equal(panelGroup.Key, registerPanelTabAction.PanelGroupKey);
            Assert.Equal(panelTab, registerPanelTabAction.PanelTab);
            Assert.Equal(insertAtIndexZero, registerPanelTabAction.InsertAtIndexZero);
        }
    }

    /// <summary>
    /// <see cref="PanelState.DisposePanelTabAction"/>
    /// </summary>
    [Fact]
    public void DisposePanelTabAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);

        var disposePanelTabAction = new PanelState.DisposePanelTabAction(
            panelGroup.Key,
            panelTab.Key);

        Assert.Equal(panelGroup.Key, disposePanelTabAction.PanelGroupKey);
        Assert.Equal(panelTab.Key, disposePanelTabAction.PanelTabKey);
    }

    /// <summary>
    /// <see cref="PanelState.SetActivePanelTabAction"/>
    /// </summary>
    [Fact]
    public void SetActivePanelTabAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);

        var setActivePanelTabAction = new PanelState.SetActivePanelTabAction(
            panelGroup.Key,
            panelTab.Key);

        Assert.Equal(panelGroup.Key, setActivePanelTabAction.PanelGroupKey);
        Assert.Equal(panelTab.Key, setActivePanelTabAction.PanelTabKey);
    }

    /// <summary>
    /// <see cref="PanelState.SetPanelTabAsActiveByContextRecordKeyAction"/>
    /// </summary>
    [Fact]
    public void SetPanelTabAsActiveByContextRecordKeyAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);

        var setPanelTabAsActiveByContextRecordKeyAction = new PanelState.SetPanelTabAsActiveByContextRecordKeyAction(
            ContextFacts.SolutionExplorerContext.ContextKey);

        Assert.Equal(
            ContextFacts.SolutionExplorerContext.ContextKey,
            setPanelTabAsActiveByContextRecordKeyAction.ContextRecordKey);
    }

    /// <summary>
    /// <see cref="PanelState.SetDragEventArgsAction"/>
    /// </summary>
    [Fact]
    public void SetDragEventArgsAction()
    {
        InitializePanelStateActionsTests(out var panelGroup, out var panelTab);

        var setDragEventArgsAction = new PanelState.SetDragEventArgsAction(
            (panelTab, panelGroup));

        Assert.NotNull(setDragEventArgsAction.DragEventArgs);

        Assert.Equal(panelTab, setDragEventArgsAction.DragEventArgs!.Value.PanelTab);
        Assert.Equal(panelGroup, setDragEventArgsAction.DragEventArgs.Value.PanelGroup);
    }

    private void InitializePanelStateActionsTests(
        out PanelGroup samplePanelGroup,
        out IPanelTab samplePanelTab)
    {
        samplePanelGroup = new PanelGroup(
            PanelFacts.LeftPanelGroupKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<IPanelTab>.Empty);

        var leftPanelGroupWidth = samplePanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        leftPanelGroupWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 33.3333,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var serviceCollection = new ServiceCollection()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusCommonServices(new LuthetusHostingInformation(LuthetusHostingKind.UnitTestingSynchronous, backgroundTaskService))
            .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonConfig).Assembly));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        samplePanelTab = new Panel(
            "Solution Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.SolutionExplorerContext.ContextKey,
            typeof(IconCSharpClass),
            null,
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<IDialogService>(),
            serviceProvider.GetRequiredService<IJSRuntime>());
    }
}