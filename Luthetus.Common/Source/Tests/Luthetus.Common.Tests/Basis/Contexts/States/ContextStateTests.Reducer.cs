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
    public void ReduceSetActiveContextRecordsAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceSetActiveContextRecordsAction(
            ContextState inContextStates, SetActiveContextRecordsAction setActiveContextRecordsAction)
         */

        InitializeContextStateReducerTests(
            out var serviceProvider,
            out var contextStateWrap,
            out var dispatcher,
            out var contextRecordKeyHeirarchy);

        var setActiveContextRecordsAction = new ContextState.SetFocusedContextHeirarchyAction(
            contextRecordKeyHeirarchy);

        dispatcher.Dispatch(setActiveContextRecordsAction);

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceToggleInspectAction()
    {
        /*
        [ReducerMethod(typeof(ToggleSelectInspectionTargetAction))]
        public static ContextState ReduceToggleInspectAction(
            ContextState inContextStates)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceSetSelectInspectionTargetTrueAction()
    {
        /*
        [ReducerMethod(typeof(SetSelectInspectionTargetTrueAction))]
        public static ContextState ReduceSetSelectInspectionTargetTrueAction(
            ContextState inContextStates)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceSetSelectInspectionTargetFalseAction()
    {
        /*
        [ReducerMethod(typeof(SetSelectInspectionTargetFalseAction))]
        public static ContextState ReduceSetSelectInspectionTargetFalseAction(
            ContextState inContextStates)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceSetInspectionTargetAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceSetInspectionTargetAction(
            ContextState inContextStates, SetInspectionTargetAction setInspectionTargetAction)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceAddInspectContextRecordEntryAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceAddInspectContextRecordEntryAction(
            ContextState inContextStates, AddInspectContextRecordEntryAction addInspectContextRecordEntryAction)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReduceSetContextKeymapAction()
    {
        /*
        [ReducerMethod]
        public static ContextState ReduceSetContextKeymapAction(
            ContextState inContextStates, SetContextKeymapAction setContextKeymapAction)
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