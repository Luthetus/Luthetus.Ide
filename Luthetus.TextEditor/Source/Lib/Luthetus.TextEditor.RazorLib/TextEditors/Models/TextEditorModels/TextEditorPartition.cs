using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <param name="CharList">
/// Idea("Use an Array"): Frequently, <see cref="CharList"/> needs to be converted to a <see cref="string"/>.
///       The code looks like the following example:
///           'new string(modifier.PartitionList.First().CharList.ToArray())'
///       |
///       If the <see cref="CharList"/> were an array of characters, would this be
///       a meaningful optimization (if even an optimization at all).
/// <br/><br/>
/// Idea("Use a string"): Frequently, <see cref="CharList"/> needs to be converted to a <see cref="string"/>.
///       The code looks like the following example:
///           'new string(modifier.PartitionList.First().CharList.ToArray())'
///       |
///       If the <see cref="CharList"/> were a string, would this be
///       a meaningful optimization (if even an optimization at all).
///       |
///       This idea comes from the fact that, the collection type for the <see cref="CharList"/>
///       is already immutable.
///       |
///       So a readonly string perhaps is equivalent?
///       |
///       What would insertion operations and etc... look like, when performed directly
///       on a string?
///       |
///       Perhaps, better than making <see cref="CharList"/> a string, is to
///       cache the 'CharList' as a string, via a method that one can invoke
///       to return a string from a <see cref="TextEditorModel"/>.
/// </param>
public record TextEditorPartition(
    ImmutableList<char> CharList,
    List<byte> DecorationByteList)
{
    public static readonly TextEditorPartition Empty = new(ImmutableList<char>.Empty, new());
}
