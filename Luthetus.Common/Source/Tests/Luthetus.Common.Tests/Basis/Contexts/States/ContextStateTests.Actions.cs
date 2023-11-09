using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Contexts.States;

/// <summary>
/// <see cref="ContextState"/>
/// </summary>
public class ContextStateActionsTests
{
    /// <summary>
    /// <see cref="ContextState.SetActiveContextRecordsAction"/>
    /// </summary>
    [Fact]
    public void SetActiveContextRecordsAction()
    {
        var contextRecordKeyHeirarchy = new ContextRecordKeyHeirarchy(
            new Key<ContextRecord>[] 
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        var setActiveContextRecordsAction = new ContextState.SetActiveContextRecordsAction(
            contextRecordKeyHeirarchy);

        Assert.Equal(contextRecordKeyHeirarchy, setActiveContextRecordsAction.ContextRecordKeyHeirarchy);
    }

    /// <summary>
    /// <see cref="ContextState.ToggleSelectInspectionTargetAction"/>
    /// </summary>
    [Fact]
    public void ToggleSelectInspectionTargetAction()
    {
        var toggleSelectInspectionTargetAction = new ContextState.ToggleSelectInspectionTargetAction();
        Assert.NotNull(toggleSelectInspectionTargetAction);
    }

    /// <summary>
    /// <see cref="ContextState.SetSelectInspectionTargetTrueAction"/>
    /// </summary>
    [Fact]
    public void SetSelectInspectionTargetTrueAction()
    {
        var setSelectInspectionTargetTrueAction = new ContextState.SetSelectInspectionTargetTrueAction();
        Assert.NotNull(setSelectInspectionTargetTrueAction);
    }

    /// <summary>
    /// <see cref="ContextState.SetSelectInspectionTargetFalseAction"/>
    /// </summary>
    [Fact]
    public void SetSelectInspectionTargetFalseAction()
    {
        var setSelectInspectionTargetFalseAction = new ContextState.SetSelectInspectionTargetFalseAction();
        Assert.NotNull(setSelectInspectionTargetFalseAction);
    }

    /// <summary>
    /// <see cref="ContextState.SetInspectionTargetAction"/>
    /// </summary>
    [Fact]
    public void SetInspectionTargetAction()
    {
        var contextRecordKeyHeirarchy = new ContextRecordKeyHeirarchy(
            new Key<ContextRecord>[]
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        var setInspectionTargetAction = new ContextState.SetInspectionTargetAction(
            contextRecordKeyHeirarchy);

        Assert.Equal(contextRecordKeyHeirarchy, setInspectionTargetAction.ContextRecordKeyHeirarchy);
    }

    /// <summary>
    /// <see cref="ContextState.AddInspectContextRecordEntryAction"/>
    /// </summary>
    [Fact]
    public void AddInspectContextRecordEntryAction()
    {
        var activeContextsContextKeyHeirarchy = new ContextRecordKeyHeirarchy(
            new Key<ContextRecord>[]
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        var activeContextsContextInspectEntry = new InspectContextRecordEntry(
            activeContextsContextKeyHeirarchy,
            new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0));

        var setInspectionTargetAction = new ContextState.AddInspectContextRecordEntryAction(
            activeContextsContextInspectEntry);

        Assert.Equal(activeContextsContextInspectEntry, setInspectionTargetAction.InspectContextRecordEntry);
    }

    /// <summary>
    /// <see cref="ContextState.SetContextKeymapAction"/>
    /// </summary>
    [Fact]
    public void SetContextKeymapAction()
    {
        throw new NotImplementedException();
    }
}
