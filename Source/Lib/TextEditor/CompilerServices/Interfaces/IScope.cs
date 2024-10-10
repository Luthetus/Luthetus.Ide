using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IScope
{
	public Key<IScope> Key { get; }
    public Key<IScope>? ParentKey { get; }
    public ResourceUri ResourceUri { get; }
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; }
}
