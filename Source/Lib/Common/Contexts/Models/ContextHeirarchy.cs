using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.Models;

/// <param name="KeyList">
/// Contains the <see cref="NearestAncestorKey"/> at index 0, then the immediate parent of target
/// at index 1. This pattern follows for 'parent of parent' until there is no parent.
/// Each parent getting the next index in list to store their key.
/// </param>
public record ContextHeirarchy(ImmutableArray<Key<ContextRecord>> KeyList)
{
    /// <summary>
    /// The key of the nearest-ancestor <see cref="Displays.ContextBoundary"/> ContextRecord in question
    /// </summary>
    public Key<ContextRecord> NearestAncestorKey => KeyList[0];
}