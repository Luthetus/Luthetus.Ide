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
/// <see cref="PanelState.Reducer"/>
/// </summary>
public class PanelStateReducerTests
{
    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceRegisterPanelGroupAction(PanelState, PanelState.RegisterPanelGroupAction)"/>
    /// </summary>
    [Fact]
    public void ReduceRegisterPanelGroupAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.DoesNotContain(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        var registerPanelGroupAction = new PanelState.RegisterPanelGroupAction(panelGroup);
        dispatcher.Dispatch(registerPanelGroupAction);
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);
    }

    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceDisposePanelGroupAction(PanelState, PanelState.DisposePanelGroupAction)"/>
    /// </summary>
    [Fact]
    public void ReduceDisposePanelGroupAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.DoesNotContain(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        dispatcher.Dispatch(new PanelState.RegisterPanelGroupAction(panelGroup));
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        dispatcher.Dispatch(new PanelState.DisposePanelGroupAction(panelGroup.Key));
        Assert.DoesNotContain(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);
    }

    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceRegisterPanelTabAction(PanelState, PanelState.RegisterPanelTabAction)"/>
    /// </summary>
    [Fact]
    public void ReduceRegisterPanelTabAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        dispatcher.Dispatch(new PanelState.RegisterPanelGroupAction(panelGroup));
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        // Ensure the panelGroup is not empty.
        // This allows one to test the 'insertAtIndexZero' logic.
        // That is to say, insert at start or end logic.
        {
            dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                panelTab,
                true));

            var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
            Assert.Contains(localPanelGroup.TabList, x => x == panelTab);
            Assert.NotEmpty(localPanelGroup.TabList);
        }

        // InsertAtIndexZero == false
        {
            var insertAtIndexZero = false;

            var localPanelTab = new Panel(
                "Solution Explorer",
                Key<Panel>.NewKey(),
                Key<IDynamicViewModel>.NewKey(),
                ContextFacts.SolutionExplorerContext.ContextKey,
                typeof(IconCSharpClass),
                new(),
                dispatcher,
                serviceProvider.GetRequiredService<IDialogService>(),
                serviceProvider.GetRequiredService<IJSRuntime>());

            dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                localPanelTab,
                insertAtIndexZero));

            var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
            Assert.Contains(localPanelGroup.TabList, x => x == localPanelTab);
            Assert.Equal(localPanelGroup.TabList.Last(), localPanelTab);
        }

        // InsertAtIndexZero == true
        {
            var insertAtIndexZero = true;

            var localPanelTab = new Panel(
                "Solution Explorer",
                Key<Panel>.NewKey(),
                Key<IDynamicViewModel>.NewKey(),
                ContextFacts.SolutionExplorerContext.ContextKey,
                typeof(IconCSharpClass),
                new(),
                dispatcher,
                serviceProvider.GetRequiredService<IDialogService>(),
                serviceProvider.GetRequiredService<IJSRuntime>());

            dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                localPanelTab,
                insertAtIndexZero));

            var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
            Assert.Contains(localPanelGroup.TabList, x => x == localPanelTab);
            Assert.Equal(localPanelGroup.TabList.First(), localPanelTab);
        }
    }

    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceDisposePanelTabAction(PanelState, PanelState.DisposePanelTabAction)"/>
    /// </summary>
    [Fact]
    public void ReduceDisposePanelTabAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.Empty(panelGroup.TabList);

        dispatcher.Dispatch(new PanelState.RegisterPanelGroupAction(panelGroup));
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                panelTab,
                true));

        var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
        Assert.Contains(localPanelGroup.TabList, x => x == panelTab);
        Assert.NotEmpty(localPanelGroup.TabList);

        dispatcher.Dispatch(new PanelState.DisposePanelTabAction(
            panelGroup.Key,
            panelTab.Key));

        localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
        Assert.DoesNotContain(localPanelGroup.TabList, x => x == panelTab);
        Assert.Empty(localPanelGroup.TabList);
    }

    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceSetActivePanelTabAction(PanelState, PanelState.SetActivePanelTabAction)"/>
    /// </summary>
    [Fact]
    public void ReduceSetActivePanelTabAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.Empty(panelGroup.TabList);

        dispatcher.Dispatch(new PanelState.RegisterPanelGroupAction(panelGroup));
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                panelTab,
                true));

        var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
        Assert.Contains(localPanelGroup.TabList, x => x == panelTab);
        Assert.NotEmpty(localPanelGroup.TabList);

        Assert.NotEqual(panelTab.Key, localPanelGroup.ActiveTabKey);

        dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(
            panelGroup.Key,
            panelTab.Key));
        
        localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
        Assert.Equal(panelTab.Key, localPanelGroup.ActiveTabKey);
    }

    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceSetPanelTabAsActiveByContextRecordKeyAction(PanelState, PanelState.SetPanelTabAsActiveByContextRecordKeyAction)"/>
    /// </summary>
    [Fact]
    public void ReduceSetPanelTabAsActiveByContextRecordKeyAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.Empty(panelGroup.TabList);

        dispatcher.Dispatch(new PanelState.RegisterPanelGroupAction(panelGroup));
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        List<(ContextRecord contextRecord, IPanelTab panelTab)> panelTabTupleList = new();

        foreach (var context in ContextFacts.AllContextsList)
        {
            var localPanelTab = new Panel(
                "Solution Explorer",
                Key<Panel>.NewKey(),
                Key<IDynamicViewModel>.NewKey(),
                context.ContextKey,
                typeof(IconCSharpClass),
                new(),
                dispatcher,
                serviceProvider.GetRequiredService<IDialogService>(),
                serviceProvider.GetRequiredService<IJSRuntime>());

            panelTabTupleList.Add((context, localPanelTab));

            dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
                panelGroup.Key,
                localPanelTab,
                false));

            var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
            Assert.Contains(localPanelGroup.TabList, x => x == localPanelTab);
        }

        panelTabTupleList.Reverse();

        foreach (var panelTabTuple in panelTabTupleList)
        {
            dispatcher.Dispatch(new PanelState.SetPanelTabAsActiveByContextRecordKeyAction(
                panelTabTuple.contextRecord.ContextKey));

            var localPanelGroup = panelStateWrap.Value.PanelGroupList.Single(x => x.Key == panelGroup.Key);
            Assert.Equal(panelTabTuple.panelTab.Key, localPanelGroup.ActiveTabKey);
        }
    }

    /// <summary>
    /// <see cref="PanelState.Reducer.ReduceSetDragEventArgsAction(PanelState, PanelState.SetDragEventArgsAction)"/>
    /// </summary>
    [Fact]
    public void ReduceSetDragEventArgsAction()
    {
        InitializePanelStateReducerTests(
            out var panelGroup,
            out var panelTab,
            out var panelStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.Empty(panelGroup.TabList);

        dispatcher.Dispatch(new PanelState.RegisterPanelGroupAction(panelGroup));
        Assert.Contains(panelStateWrap.Value.PanelGroupList, x => x == panelGroup);

        dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(
            panelGroup.Key,
            panelTab,
            false));

        Assert.Null(panelStateWrap.Value.DragEventArgs);

        var dragEventArgs = (panelTab, panelGroup);
        
        dispatcher.Dispatch(new PanelState.SetDragEventArgsAction(dragEventArgs));
        Assert.Equal(panelStateWrap.Value.DragEventArgs!.Value, dragEventArgs);
    }

    private void InitializePanelStateReducerTests(
        out PanelGroup samplePanelGroup,
        out IPanelTab samplePanelTab,
        out IState<PanelState> panelStateWrap,
        out IDispatcher dispatcher,
        out ServiceProvider serviceProvider)
    {
        var backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var serviceCollection = new ServiceCollection()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusCommonServices(new LuthetusHostingInformation(LuthetusHostingKind.UnitTesting, backgroundTaskService))
            .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonConfig).Assembly));

        serviceProvider = serviceCollection.BuildServiceProvider();
        
        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        panelStateWrap = serviceProvider.GetRequiredService<IState<PanelState>>();
        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        samplePanelGroup = new PanelGroup(
            Key<PanelGroup>.NewKey(),
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<IPanelTab>.Empty);

        samplePanelGroup.Dispatcher = dispatcher;

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

        samplePanelTab = new Panel(
            "Solution Explorer",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.SolutionExplorerContext.ContextKey,
            typeof(IconCSharpClass),
            null,
            dispatcher,
            serviceProvider.GetRequiredService<IDialogService>(),
            serviceProvider.GetRequiredService<IJSRuntime>());
    }
}