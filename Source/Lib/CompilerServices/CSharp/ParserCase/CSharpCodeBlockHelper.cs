namespace Luthetus.CompilerServices.CSharp.ParserCase;

public static class CSharpCodeBlockHelper
{
	public List<ISyntax> ChildList { get; } = new();
    public CSharpCodeBlockBuilder? Parent { get; }
    /// <summary>
    /// Given: "public class MyClass { ... }"<br/><br/>Then: The 'MyClass' body-code-block would
    /// have an owner of 'TypeDefinitionNode'.<br/><br/>
    /// Purpose: When parsing a class definition's constructor. I need to know if the code block I
    /// exist in is one which a class owns. Furthermore, I need to verify that the code-block-owner's
    /// Identifier is equal to the constructor's identifier.
    /// </summary>
    public ICodeBlockOwner? CodeBlockOwner { get; }
    
    public bool StatementDelimiterCanCloseScope { get; set; }
    
    public Queue<CSharpDeferredChildScope> ParseChildScopeQueue { get; set; } = new();
	public bool PermitInnerPendingCodeBlockOwnerToBeParsed { get; set; }
	public int? DequeuedIndexForChildList { get; set; }

	// CARE THE NULLS
	public void SetNextCodeBlockOwner(
		ICodeBlockOwner? nextCodeBlockOwner,
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel)
	{
		if (nextCodeBlockOwner.OpenBraceToken.ConstructorWasInvoked)
			throw new LuthetusTextEditorException($"{nameof(SetNextCodeBlockOwner)}(...) -> if (nextCodeBlockOwner.OpenBraceToken.ConstructorWasInvoked)");
			
    	compilationUnit.Binder.OpenScope(nextCodeBlockOwner, CSharpFacts.Types.Void.ToTypeClause(), parserModel.TokenWalker.Current.TextSpan, compilationUnit);
		
		parserModel.CurrentCodeBlockBuilder = new(parent: parserModel.CurrentCodeBlockBuilder, codeBlockOwner: nextCodeBlockOwner)
		{
			StatementDelimiterCanCloseScope = true
		};
		
		compilationUnit.Binder.OnBoundScopeCreatedAndSetAsCurrent(nextCodeBlockOwner, compilationUnit, ref parserModel);
	}
}
