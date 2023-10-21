using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface IBoundScope
{
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; }
    public ResourceUri ResourceUri { get; }
}