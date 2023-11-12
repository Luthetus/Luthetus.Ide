using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Contexts.States;

/// <summary>
/// <see cref="ContextState"/>
/// </summary>
public class ContextStateTests
{
    /// <summary>
    /// <see cref="ContextState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var contextState = new ContextState();

        var allContextRecordsBag = ContextFacts.AllContextsBag;

        var inspectedContextRecordKeyHeirarchy = (ContextHeirarchy?)null;
        var inspectContextRecordEntryBag = ImmutableArray<InspectableContext>.Empty;
        var isSelectingInspectionTarget = false;

        Assert.Equal(allContextRecordsBag, contextState.AllContextsBag);
        Assert.NotNull(contextState.FocusedContextHeirarchy);
        Assert.True(inspectedContextRecordKeyHeirarchy == contextState.InspectedContextHeirarchy);
        Assert.Equal(inspectContextRecordEntryBag, contextState.InspectableContextBag);
        Assert.Equal(isSelectingInspectionTarget, contextState.IsSelectingInspectionTarget);
    }
}