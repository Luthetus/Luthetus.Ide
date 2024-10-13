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
    /// <summary>
    /// Beware of this property's setter,
    /// as a hack to set the ending index later on in this
    /// type's lifecycle because it isn't immediately known.
    /// </summary>
    public int? EndingIndexExclusive { get; set; }
}
