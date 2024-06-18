using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

public interface ICodeBlockOwner
{
	public ScopeDirectionKind ScopeDirectionKind { get; }
}
