using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpCodeBlockBuilder
{
    public CSharpCodeBlockBuilder(CSharpCodeBlockBuilder? parent, ICodeBlockOwner codeBlockOwner)
    {
        Parent = parent;
        CodeBlockOwner = codeBlockOwner;
        
        var parentScopeDirection = parent?.CodeBlockOwner.ScopeDirectionKind
        	?? ScopeDirectionKind.Both;
        
        if (parentScopeDirection == ScopeDirectionKind.Both)
        	PermitCodeBlockParsing = false;
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
    public ICodeBlockOwner CodeBlockOwner { get; }
    
	public bool PermitCodeBlockParsing { get; set; } = true;
	
	/// <summary>
	/// This belongs on the 'CSharpCodeBlockBuilder', not the 'ICodeBlockOwner'.
	///
	/// This is computational state to know whether to search
	/// for 'StatementDelimiterToken' (if this is true) as the terminator or a 'CloseBraceToken' (if this is false).
	///
	/// This is not necessary to disambiguate the SyntaxKind of the text spans that mark
	/// the start and end of the code block.
	///
	/// This is mentioned because that might be an argument for this being moved to 'ICodeBlockOwner'.
	///
	/// But, .... interrupting my thought I think I'm wrong hang on....
	///
	/// ````public void SomeFunction() => }
	/// 
	/// What should the above code snippet parse as?
	/// Should the '}' be consumed as the closing delimiter token for 'SomeFunction()'?
	///
	/// Is it the case that the closing text span of a scope is only
	/// a 'CloseBracetoken' if the start text span is from an 'OpenBraceToken'?
	///
	/// Furthermore, is it true that the start text span is only non-null
	/// if it is an 'OpenBraceToken' that started the code block?
	///
	/// An implicitly opened code block can have its start text span
	/// retrieved on a 'per ICodeBlockOwner' basis.
	///
	/// I am going to decide that:
	/// ````public void SomeFunction() => }
	///
	/// will not consume the 'CloseBraceToken' as its delimiter.
	/// This matter is open to be changed though,
	/// this decision is only being made to create consistency.
	/// </summary>
	public bool IsImplicitOpenCodeBlockTextSpan { get; set; }
	
	/*public void AddChild_Experimental()
	{
		Recur();
	}*/
	
	/*private void Recur()
	{
		if (binaryExpressionNode.LeftExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode &&
		    binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			return new BinaryExpressionLeftAndRightVariableReference(binaryExpressionNode);
		}
		else if (binaryExpressionNode.LeftExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			return new BinaryExpressionLeftVariableReference(binaryExpressionNode);
		}
		else if (binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
		{
			return new BinaryExpressionRightVariableReference(binaryExpressionNode);
		}
	}*/

    public CodeBlockNode Build()
    {
        return new CodeBlockNode(ChildList);
    }
}