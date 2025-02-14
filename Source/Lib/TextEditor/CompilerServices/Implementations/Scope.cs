using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public struct Scope : IScope
{
	public Scope(
		ICodeBlockOwner codeBlockOwner,
		int indexKey,
	    int? parentIndexKey,
	    int startingIndexInclusive,
	    int? endingIndexExclusive)
	{
		CodeBlockOwner = codeBlockOwner;
		IndexKey = indexKey;
		ParentIndexKey = parentIndexKey;
		StartingIndexInclusive = startingIndexInclusive;
		EndingIndexExclusive = endingIndexExclusive;
	}
	
	public ICodeBlockOwner CodeBlockOwner { get; }
	public int IndexKey { get; }
    public int? ParentIndexKey { get; }
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; set; }
}
