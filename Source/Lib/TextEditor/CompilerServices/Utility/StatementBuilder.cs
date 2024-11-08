using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class StatementBuilder
{
	public List<ISyntax> ChildList { get; } = new();
	
	public bool TryPeek(out ISyntax syntax)
	{
		if (ChildList.Count > 0)
		{
			syntax = ChildList[^1];
			return true;
		}
		
		syntax = null;
		return false;
	}
	
	public ISyntax Pop()
	{
		var syntax = ChildList[^1];
		ChildList.RemoveAt(ChildList.Count - 1);
		return syntax;
	}
}
