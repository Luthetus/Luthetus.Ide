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
/// <see cref="ContextState"/>
/// </summary>
public class ContextStateReducerTests
{
    [Fact]
    public void ReduceSetFocusedContextHeirarchyAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceSetFocusedContextHeirarchyAction(
            ContextState inContextStates, SetFocusedContextHeirarchyAction setFocusedContextHeirarchyAction)
         */

        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);
        
        Assert.Equal(
            contextStateWrap.Value.FocusedContextHeirarchy.KeyBag.Single(),
            ContextFacts.GlobalContext.ContextKey);

        var setFocusedContextHeirarchyAction = new ContextState.SetFocusedContextHeirarchyAction(
            contextHeirarchy);

        dispatcher.Dispatch(setFocusedContextHeirarchyAction);

        Assert.Equal(
            contextStateWrap.Value.FocusedContextHeirarchy.KeyBag[0],
            ContextFacts.ActiveContextsContext.ContextKey);
        
        Assert.Equal(
            contextStateWrap.Value.FocusedContextHeirarchy.KeyBag[1],
            ContextFacts.GlobalContext.ContextKey);
    }

    [Fact]
    public void ReduceToggleSelectInspectedContextHeirarchyAction()
    {
        /*
        [ReducerMethod(typeof(ToggleSelectInspectedContextHeirarchyAction))]
        public static ContextState ReduceToggleSelectInspectedContextHeirarchyAction(
            ContextState inContextStates)
         */

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

    [Fact]
    public void ReduceIsSelectingInspectableContextHeirarchyAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceIsSelectingInspectableContextHeirarchyAction(
            ContextState inContextStates,
            IsSelectingInspectableContextHeirarchyAction isSelectingInspectableContextHeirarchyAction)
         */

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

    [Fact]
    public void ReduceSetInspectedContextHeirarchyAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceSetInspectedContextHeirarchyAction(
            ContextState inContextStates,
            SetInspectedContextHeirarchyAction setInspectedContextHeirarchyAction)
         */

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

    [Fact]
    public void ReduceAddInspectableContextAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceAddInspectableContextAction(
            ContextState inContextStates,
            AddInspectableContextAction addInspectableContextAction)
         */

        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        Assert.Empty(contextStateWrap.Value.InspectableContextBag);

        var inspectableContext = new InspectableContext(
            contextHeirarchy,
            new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0));

        var addInspectableContextAction = new ContextState.AddInspectableContextAction(
            inspectableContext);

        dispatcher.Dispatch(addInspectableContextAction);

        Assert.Equal(
            addInspectableContextAction.InspectableContext, 
            contextStateWrap.Value.InspectableContextBag.Single());
    }

    [Fact]
    public void ReduceSetContextKeymapAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceSetContextKeymapAction(
            ContextState inContextStates,
            SetContextKeymapAction setContextKeymapAction)
         */

        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextHeirarchy);

        var globalContext = contextStateWrap.Value.AllContextsBag.Single(
            x => x.ContextKey == ContextFacts.GlobalContext.ContextKey);

        Assert.Equal(Keymap.Empty, globalContext.Keymap);

        var command = new CommonCommand(
            "Test SetContextKeymap Action", "test-set-context-keymap-action", false,
            commandArgs => Task.CompletedTask);

        var keymap = new Keymap(Key<Keymap>.NewKey(), "Unit Test");

        _ = keymap.Map.TryAdd(
                new KeymapArgument("KeyF", false, true, true, Key<KeymapLayer>.Empty),
                command);

        var setContextKeymapAction = new ContextState.SetContextKeymapAction(
            ContextFacts.GlobalContext.ContextKey,
            keymap);

        dispatcher.Dispatch(setContextKeymapAction);

        globalContext = contextStateWrap.Value.AllContextsBag.Single(
            x => x.ContextKey == ContextFacts.GlobalContext.ContextKey);

        Assert.NotEqual(Keymap.Empty, globalContext.Keymap);
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