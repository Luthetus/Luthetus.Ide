using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public record TextEditorPartition(
    ImmutableList<char> CharList,
    List<byte> DecorationByteList)
{
    public static readonly TextEditorPartition Empty = new(ImmutableList<char>.Empty, new());
}
