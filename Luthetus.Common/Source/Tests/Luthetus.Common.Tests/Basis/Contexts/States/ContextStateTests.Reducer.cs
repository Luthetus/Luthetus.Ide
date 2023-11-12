using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
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
            out var contextRecordKeyHeirarchy);

        var setFocusedContextHeirarchyAction = new ContextState.SetFocusedContextHeirarchyAction(
            contextRecordKeyHeirarchy);

        dispatcher.Dispatch(setFocusedContextHeirarchyAction);

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceToggleSelectInspectedContextHeirarchyAction()
    {
        /*
        [ReducerMethod(typeof(ToggleSelectInspectedContextHeirarchyAction))]
        public static ContextState ReduceToggleSelectInspectedContextHeirarchyAction(
            ContextState inContextStates)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceIsSelectingInspectableContextHeirarchyAction_True()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceIsSelectingInspectableContextHeirarchyAction(
            ContextState inContextStates,
            IsSelectingInspectableContextHeirarchyAction isSelectingInspectableContextHeirarchyAction)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceIsSelectingInspectableContextHeirarchyAction_False()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceIsSelectingInspectableContextHeirarchyAction(
            ContextState inContextStates,
            IsSelectingInspectableContextHeirarchyAction isSelectingInspectableContextHeirarchyAction)
         */

        throw new NotImplementedException();
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

        throw new NotImplementedException();
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

        throw new NotImplementedException();
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

        throw new NotImplementedException();
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