namespace Luthetus.Extensions.CompilerServices.Syntax;

public struct CodeBlock
{
	public CodeBlock(IReadOnlyList<ISyntax> childList)
	{
		ChildList = childList;
	}

	public IReadOnlyList<ISyntax> ChildList { get; }
	
	public bool ConstructorWasInvoked => ChildList is not null;
}