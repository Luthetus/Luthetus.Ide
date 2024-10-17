using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public struct Scope : IScope
{
	public Scope(
		Key<IScope> key,
	    Key<IScope>? parentKey,
	    ResourceUri resourceUri,
	    int startingIndexInclusive,
	    int? endingIndexExclusive)
	{
		Key = key;
		ParentKey = parentKey;
		ResourceUri = resourceUri;
		StartingIndexInclusive = startingIndexInclusive;
		EndingIndexExclusive = endingIndexExclusive;
	}
	
	public Key<IScope> Key { get; }
    public Key<IScope>? ParentKey { get; }
    public ResourceUri ResourceUri { get; }
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; set; }
}
