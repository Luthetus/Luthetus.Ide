using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IBoundScope
{
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; }
    public ResourceUri ResourceUri { get; }
}