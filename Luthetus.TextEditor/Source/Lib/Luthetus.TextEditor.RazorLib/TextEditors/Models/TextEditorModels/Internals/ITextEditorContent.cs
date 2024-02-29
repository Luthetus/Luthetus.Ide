using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

public interface ITextEditorContent : IHasTextEditorMetadata
{
    public ImmutableList<TextEditorPartition> PartitionList { get; }
    public IReadOnlyList<RichCharacter> AllRichCharacters { get; }
}
