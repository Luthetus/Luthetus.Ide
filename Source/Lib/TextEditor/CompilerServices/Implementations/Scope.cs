using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public struct Scope : IScope
{
	public Scope(
		int indexKey,
	    int? parentIndexKey,
	    int startingIndexInclusive,
	    int? endingIndexExclusive)
	{
		IndexKey = indexKey;
		ParentIndexKey = parentIndexKey;
		StartingIndexInclusive = startingIndexInclusive;
		EndingIndexExclusive = endingIndexExclusive;
	}
	
	public int IndexKey { get; }
    public int? ParentIndexKey { get; }
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; set; }
}
