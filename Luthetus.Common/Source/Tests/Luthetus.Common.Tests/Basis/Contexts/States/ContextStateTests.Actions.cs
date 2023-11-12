using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Contexts.States;

/// <summary>
/// <see cref="ContextState"/>
/// </summary>
public class ContextStateActionsTests
{
    /// <summary>
    /// <see cref="ContextState.SetFocusedContextHeirarchyAction"/>
    /// </summary>
    [Fact]
    public void SetActiveContextRecordsAction()
    {
        var contextRecordKeyHeirarchy = new ContextHeirarchy(
            new Key<ContextRecord>[] 
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        var setFocusedContextHeirarchyAction = new ContextState.SetFocusedContextHeirarchyAction(
            contextRecordKeyHeirarchy);

        Assert.Equal(contextRecordKeyHeirarchy, setFocusedContextHeirarchyAction.FocusedContextHeirarchy);
    }

    /// <summary>
    /// <see cref="ContextState.ToggleSelectInspectedContextHeirarchyAction"/>
    /// </summary>
    [Fact]
    public void ToggleSelectInspectionTargetAction()
    {
        var toggleSelectInspectionTargetAction = new ContextState.ToggleSelectInspectedContextHeirarchyAction();
        Assert.NotNull(toggleSelectInspectionTargetAction);
    }

    /// <summary>
    /// <see cref="ContextState.SetSelectInspectedContextHeirarchyAction"/>
    /// </summary>
    [Fact]
    public void SetSelectInspectedContextHeirarchyAction_True()
    {
        var setSelectInspectedContextHeirarchyAction = new ContextState.SetSelectInspectedContextHeirarchyAction(true);
        Assert.NotNull(setSelectInspectedContextHeirarchyAction);
    }

    /// <summary>
    /// <see cref="ContextState.SetSelectInspectedContextHeirarchyAction"/>
    /// </summary>
    [Fact]
    public void SetSelectInspectedContextHeirarchyAction_False()
    {
        var setSelectInspectionTargetFalseAction = new ContextState.SetSelectInspectedContextHeirarchyAction(false);
        Assert.NotNull(setSelectInspectionTargetFalseAction);
    }

    /// <summary>
    /// <see cref="ContextState.SetInspectedContextHeirarchyAction"/>
    /// </summary>
    [Fact]
    public void SetInspectedContextHeirarchyAction()
    {
        var contextRecordKeyHeirarchy = new ContextHeirarchy(
            new Key<ContextRecord>[]
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        var setInspectionTargetAction = new ContextState.SetInspectedContextHeirarchyAction(
            contextRecordKeyHeirarchy);

        Assert.Equal(contextRecordKeyHeirarchy, setInspectionTargetAction.InspectedContextHeirarchy);
    }

    /// <summary>
    /// <see cref="ContextState.AddInspectableContextAction"/>
    /// </summary>
    [Fact]
    public void AddInspectableContextAction()
    {
        var activeContextsContextKeyHeirarchy = new ContextHeirarchy(
            new Key<ContextRecord>[]
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        var activeContextsContextInspectEntry = new InspectableContext(
            activeContextsContextKeyHeirarchy,
            new MeasuredHtmlElementDimensions(0, 0, 0, 0, 0));

        var setInspectionTargetAction = new ContextState.AddInspectableContextAction(
            activeContextsContextInspectEntry);

        Assert.Equal(activeContextsContextInspectEntry, setInspectionTargetAction.InspectableContext);
    }

    /// <summary>
    /// <see cref="ContextState.SetContextKeymapAction"/>
    /// </summary>
    [Fact]
    public void SetContextKeymapAction()
    {
        var contextRecordKey = ContextFacts.GlobalContext.ContextKey;
        var keymap = Keymap.Empty;

        var setContextKeymapAction = new ContextState.SetContextKeymapAction(
            contextRecordKey,
            keymap);

        Assert.Equal(contextRecordKey, setContextKeymapAction.ContextKey);
        Assert.Equal(keymap, setContextKeymapAction.Keymap);
    }
}
