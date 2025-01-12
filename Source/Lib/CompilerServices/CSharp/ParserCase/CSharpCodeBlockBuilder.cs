using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpCodeBlockBuilder
{
    public CSharpCodeBlockBuilder(CSharpCodeBlockBuilder? parent, ICodeBlockOwner? codeBlockOwner)
    {
        Parent = parent;
        CodeBlockOwner = codeBlockOwner;
    }

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
    
    /// <summary>
    /// Method with generic type constraint:
    /// ````public void M<T>(T? item) where T : struct { } // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/where-generic-type-constraint
	///
	/// Any syntax that goes from parentheses to OpenBraceToken / CloseBraceToken to define a scope:
	/// ````foreach (var item in list)
    /// ````{
	/// ````    Console.WriteLine(item);
	/// ````}
	///
	/// Any syntax that goes from parenthesis to a "single statement body" deliminated by StatementDelimiterToken:
	/// ````foreach (var item in list)
	/// ````    Console.WriteLine(item);
	///
	/// The idea is that syntax which defines a scope does not necessarily flow
	/// in a simple way.
	///
	/// "Any syntax that goes from parentheses to OpenBraceToken / CloseBraceToken to define a scope"
	/// is a fairly simple case.
	/// One could go immediately from the CloseParenthesisToken to the OpenBraceToken.
	///
	/// But, if there is any syntax between the syntax that identifies
	/// a code block owner, and the actual code block itself, things get more complicated.
    /// </summary>
    public ICodeBlockOwner? InnerPendingCodeBlockOwner { get; private set; }
    
    public Queue<CSharpDeferredChildScope> ParseChildScopeQueue { get; set; } = new();
	public bool PermitInnerPendingCodeBlockOwnerToBeParsed { get; set; }
	public int? DequeuedIndexForChildList { get; set; }

	public void SetInnerPendingCodeBlockOwner(
		ICodeBlockOwner? innerPendingCodeBlockOwner,
		CSharpCompilationUnit compilationUnit,
		ref CSharpParserModel parserModel)
	{
		InnerPendingCodeBlockOwner = innerPendingCodeBlockOwner;
		
		// ParseOpenBraceToken(OpenBraceToken openBraceToken, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
		if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenBraceToken)
	    {
	    	if (parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner is not null &&
	    		parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner.SyntaxKind != SyntaxKind.NamespaceStatementNode &&
	        	!parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner.OpenBraceToken.ConstructorWasInvoked)
	        {
	        	var pendingChild = parserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner;
	        
	        	compilationUnit.Binder.OpenScope(pendingChild, CSharpFacts.Types.Void.ToTypeClause(), parserModel.TokenWalker.Current.TextSpan, compilationUnit);
	        	
				parserModel.CurrentCodeBlockBuilder = new(parent: parserModel.CurrentCodeBlockBuilder, codeBlockOwner: pendingChild);
				
				//pendingChild.SetStatementDelimiterToken(statementDelimiterToken, parserModel.DiagnosticBag, parserModel.TokenWalker);
				
				compilationUnit.Binder.OnBoundScopeCreatedAndSetAsCurrent(pendingChild, compilationUnit, ref parserModel);
				
		        //compilationUnit.Binder.CloseScope(statementDelimiterToken.TextSpan, compilationUnit, ref parserModel);
	        }
	    }
	}

    public CodeBlockNode Build()
    {
        return new CodeBlockNode(ChildList.ToArray());
    }

    public CodeBlockNode Build(TextEditorDiagnostic[] diagnostics)
    {
        return new CodeBlockNode(ChildList.ToArray(), diagnostics);
    }
}