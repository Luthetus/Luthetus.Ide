using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Keys.Models;
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

        var allContextRecordsBag = ContextFacts.AllContextRecordsBag;

        var inspectedContextRecordKeyHeirarchy = (ContextRecordKeyHeirarchy?)null;
        var inspectContextRecordEntryBag = ImmutableArray<InspectContextRecordEntry>.Empty;
        var isSelectingInspectionTarget = false;

        Assert.Equal(allContextRecordsBag, contextState.AllContextRecordsBag);
        Assert.NotNull(contextState.FocusedContextRecordKeyHeirarchy);
        Assert.True(inspectedContextRecordKeyHeirarchy == contextState.InspectedContextRecordKeyHeirarchy);
        Assert.Equal(inspectContextRecordEntryBag, contextState.InspectContextRecordEntryBag);
        Assert.Equal(isSelectingInspectionTarget, contextState.IsSelectingInspectionTarget);
    }
}