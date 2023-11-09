using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.Models;

/// <param name="KeyHeirarchyBag">
/// Contains the <see cref="TargetKey"/> at index 0, then the immediate parent of target
/// at index 1. This pattern follows for 'parent of parent' until there is no parent.
/// Each parent getting the next index in list to store their key.
/// </param>
public record ContextRecordKeyHeirarchy(ImmutableArray<Key<ContextRecord>> KeyHeirarchyBag)
{
    /// <summary>
    /// The key of the ContextRecord in question
    /// </summary>
    public Key<ContextRecord> TargetKey => KeyHeirarchyBag[0];
}