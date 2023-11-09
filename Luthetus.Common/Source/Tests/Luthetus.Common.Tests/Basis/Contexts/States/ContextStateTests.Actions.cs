using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ContextState.SetSelectInspectionTargetTrueAction"/>
    /// </summary>
    [Fact]
    public void SetSelectInspectionTargetTrueAction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ContextState.SetSelectInspectionTargetFalseAction"/>
    /// </summary>
    [Fact]
    public void SetSelectInspectionTargetFalseAction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ContextState.SetInspectionTargetAction"/>
    /// </summary>
    [Fact]
    public void SetInspectionTargetAction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ContextState.AddInspectContextRecordEntryAction"/>
    /// </summary>
    [Fact]
    public void AddInspectContextRecordEntryAction()
    {
        throw new NotImplementedException();
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
