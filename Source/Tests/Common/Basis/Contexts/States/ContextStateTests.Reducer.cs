using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Contexts.States;

/// <summary>
/// <see cref="ContextState.Reducer"/>
/// </summary>
public class ContextStateReducerTests
{
    /// <summary>
    /// <see cref="ContextState.Reducer.ReduceSetFocusedContextHeirarchyAction(ContextState, ContextState.SetFocusedContextHeirarchyAction)"/>
    /// </summary>
    [Fact]
    public void ReduceSetFocusedContextHeirarchyAction()
    {
        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);
        
        Assert.Equal(
            contextStateWrap.Value.FocusedContextHeirarchy.KeyList.Single(),
            ContextFacts.GlobalContext.ContextKey);

        var setFocusedContextHeirarchyAction = new ContextState.SetFocusedContextHeirarchyAction(
            contextHeirarchy);

        dispatcher.Dispatch(setFocusedContextHeirarchyAction);

        Assert.Equal(
            contextStateWrap.Value.FocusedContextHeirarchy.KeyList[0],
            ContextFacts.ActiveContextsContext.ContextKey);
        
        Assert.Equal(
            contextStateWrap.Value.FocusedContextHeirarchy.KeyList[1],
            ContextFacts.GlobalContext.ContextKey);
    }

    /// <summary>
    /// <see cref="ContextState.Reducer.ReduceToggleSelectInspectedContextHeirarchyAction(ContextState)"/>
    /// </summary>
    [Fact]
    public void ReduceToggleSelectInspectedContextHeirarchyAction()
    {
        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        var toggleSelectInspectedContextHeirarchyAction = new ContextState.ToggleSelectInspectedContextHeirarchyAction();

        var inIsSelectingInspectionTarget = contextStateWrap.Value.IsSelectingInspectionTarget;

        dispatcher.Dispatch(toggleSelectInspectedContextHeirarchyAction);

        Assert.NotEqual(
            inIsSelectingInspectionTarget,
            contextStateWrap.Value.IsSelectingInspectionTarget);
    }


    /// <summary>
    /// <see cref="ContextState.Reducer.ReduceIsSelectingInspectableContextHeirarchyAction(ContextState, ContextState.IsSelectingInspectableContextHeirarchyAction)"/>
    /// </summary>
    [Fact]
    public void ReduceIsSelectingInspectableContextHeirarchyAction()
    {
        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        dispatcher.Dispatch(new ContextState.IsSelectingInspectableContextHeirarchyAction(
            true));

        Assert.True(contextStateWrap.Value.IsSelectingInspectionTarget);
        
        dispatcher.Dispatch(new ContextState.IsSelectingInspectableContextHeirarchyAction(
            false));

        Assert.False(contextStateWrap.Value.IsSelectingInspectionTarget);
    }


    /// <summary>
    /// <see cref="ContextState.Reducer.ReduceSetInspectedContextHeirarchyAction(ContextState, ContextState.SetInspectedContextHeirarchyAction)"/>
    /// </summary>
    [Fact]
    public void ReduceSetInspectedContextHeirarchyAction()
    {
        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        Assert.Null(contextStateWrap.Value.InspectedContextHeirarchy);

        var setInspectedContextHeirarchyAction = new ContextState.SetInspectedContextHeirarchyAction(
            contextHeirarchy);

        dispatcher.Dispatch(setInspectedContextHeirarchyAction);

        Assert.Equal(
            setInspectedContextHeirarchyAction.InspectedContextHeirarchy,
            contextStateWrap.Value.InspectedContextHeirarchy);
    }

    /// <summary>
    /// <see cref="ContextState.Reducer.ReduceAddInspectableContextAction(ContextState, ContextState.AddInspectableContextAction)"/>
    /// </summary>
    [Fact]
    public void ReduceAddInspectableContextAction()
    {
        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        Assert.Empty(contextStateWrap.Value.InspectableContextList);

        var inspectableContext = new InspectableContext(
            contextHeirarchy,
            new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0));

        var addInspectableContextAction = new ContextState.AddInspectableContextAction(
            inspectableContext);

        dispatcher.Dispatch(addInspectableContextAction);

        Assert.Equal(
            addInspectableContextAction.InspectableContext, 
            contextStateWrap.Value.InspectableContextList.Single());
    }


    /// <summary>
    /// <see cref="ContextState.Reducer.ReduceSetContextKeymapAction(ContextState, ContextState.SetContextKeymapAction)"/>
    /// </summary>
    [Fact]
    public void ReduceSetContextKeymapAction()
    {
        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        var globalContext = contextStateWrap.Value.AllContextsList.Single(
            x => x.ContextKey == ContextFacts.GlobalContext.ContextKey);

        Assert.Equal(IKeymap.Empty, globalContext.Keymap);

        var command = new CommonCommand(
            "Test SetContextKeymap Action", "test-set-context-keymap-action", false,
            commandArgs => Task.CompletedTask);

        var keymap = new Keymap(Key<Keymap>.NewKey(), "Unit Test");

        _ = keymap.TryRegister(
                new KeymapArgs
                {
                    Code = "KeyF",
                    ShiftKey = false,
                    CtrlKey = true,
                    AltKey = true,
                    LayerKey = Key<KeymapLayer>.Empty,
                },
                command);

        var setContextKeymapAction = new ContextState.SetContextKeymapAction(
            ContextFacts.GlobalContext.ContextKey,
            keymap);

        dispatcher.Dispatch(setContextKeymapAction);

        globalContext = contextStateWrap.Value.AllContextsList.Single(
            x => x.ContextKey == ContextFacts.GlobalContext.ContextKey);

        Assert.NotEqual(IKeymap.Empty, globalContext.Keymap);
        Assert.Equal(keymap, globalContext.Keymap);
    }

    private void InitializeContextStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<ContextState> contextStateWrap,
        out IDispatcher dispatcher,
        out ContextHeirarchy sampleContextRecordKeyHeirarchy)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(ContextState).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        contextStateWrap = serviceProvider.GetRequiredService<IState<ContextState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        sampleContextRecordKeyHeirarchy = new ContextHeirarchy(
            new Key<ContextRecord>[]
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());
    }
}