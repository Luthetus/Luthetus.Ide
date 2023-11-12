using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Contexts.Models;

/// <summary>
/// <see cref="ContextHeirarchy"/>
/// </summary>
public class ContextRecordKeyHeirarchyTests
{
    /// <summary>
    /// <see cref="ContextHeirarchy.NearestAncestorKey"/>
    /// </summary>
    [Fact]
    public void TargetKey()
    {
        /*
        public Key<ContextRecord> NearestAncestorKey => KeyBag[0];
         */

        var contextRecordKeyHeirarchy = new ContextHeirarchy(
            new Key<ContextRecord>[]
            {
                ContextFacts.ActiveContextsContext.ContextKey,
                ContextFacts.GlobalContext.ContextKey,
            }.ToImmutableArray());

        Assert.Equal(
            ContextFacts.ActiveContextsContext.ContextKey,
            contextRecordKeyHeirarchy.NearestAncestorKey);
    }
}