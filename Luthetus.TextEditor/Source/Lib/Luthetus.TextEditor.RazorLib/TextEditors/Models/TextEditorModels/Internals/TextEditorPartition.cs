using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

public class TextEditorPartition : IHasTextEditorMetadata
{
    public ImmutableList<RichCharacter> RichCharacterList { get; set; }

    public int CharacterCount { get; init; }
    public RowEndingKind? OnlyRowEndingKind { get; init; }
    public IReadOnlyList<int> TabList { get; init; } = ImmutableList<int>.Empty;
    public IReadOnlyList<RowEnding> RowEndingList { get; init; } = ImmutableList<RowEnding>.Empty;
    public IReadOnlyList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountList { get; init; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty;
    public int RowCount => RowEndingList.Count;
    public int DocumentLength => CharacterCount;
}