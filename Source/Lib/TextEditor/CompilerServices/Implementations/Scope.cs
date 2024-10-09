using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public record struct Scope(
		Key<IScope> Key,
	    Key<IScope>? ParentKey,
	    ResourceUri ResourceUri,
	    int StartingIndexInclusive,
	    int? EndingIndexExclusive)
	: IScope;
