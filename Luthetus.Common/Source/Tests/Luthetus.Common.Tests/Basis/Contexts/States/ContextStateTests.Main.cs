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

        var allContextRecordsList = ContextFacts.AllContextsList;

        var inspectedContextRecordKeyHeirarchy = (ContextHeirarchy?)null;
        var inspectContextRecordEntryList = ImmutableArray<InspectableContext>.Empty;
        var isSelectingInspectionTarget = false;

        Assert.Equal(allContextRecordsList, contextState.AllContextsList);
        Assert.NotNull(contextState.FocusedContextHeirarchy);
        Assert.True(inspectedContextRecordKeyHeirarchy == contextState.InspectedContextHeirarchy);
        Assert.Equal(inspectContextRecordEntryList, contextState.InspectableContextList);
        Assert.Equal(isSelectingInspectionTarget, contextState.IsSelectingInspectionTarget);
    }
}