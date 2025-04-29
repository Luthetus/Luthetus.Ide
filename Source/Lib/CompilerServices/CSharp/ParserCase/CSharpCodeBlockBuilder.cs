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
	
	public void AddChild(ISyntax syntax)
	{
		switch (syntax.SyntaxKind)
		{
			case SyntaxKind.BinaryExpressionNode:
			{
				return;
				// syntax = TryOptimizeStorageExpression((BinaryExpressionNode)syntax);
				// break;
			}
			case SyntaxKind.IfStatementNode:
			{
				return;
				// var ifStatementNode = (IfStatementNode)syntax;
				// ifStatementNode.ExpressionNode = TryOptimizeStorageExpression(ifStatementNode.ExpressionNode);
				// break;
			}
			case SyntaxKind.WhileStatementNode:
			{
				return;
				// var whileStatementNode = (WhileStatementNode)syntax;
				// whileStatementNode.ExpressionNode = TryOptimizeStorageExpression(whileStatementNode.ExpressionNode);
				// break;
			}
			case SyntaxKind.ConstructorInvocationExpressionNode:
			{
				return;
			
				// var constructorInvocationNode = (ConstructorInvocationExpressionNode)syntax;
				// 
				// if (constructorInvocationNode.FunctionParameterListing.ConstructorWasInvoked)
				// {
				// 	for (int i = 0; i < constructorInvocationNode.FunctionParameterListing.FunctionParameterEntryList.Count; i++)
				// 	{
				// 		var item = constructorInvocationNode.FunctionParameterListing.FunctionParameterEntryList[i];
				// 		item.ExpressionNode = TryOptimizeStorageExpression(item.ExpressionNode);
				// 		constructorInvocationNode.FunctionParameterListing.FunctionParameterEntryList[i] = item;
				// 	}
				// }
				// 
				// break;
			}
			case SyntaxKind.FunctionInvocationNode:
			{
				return;
				/*var functionInvocationNode = (FunctionInvocationNode)syntax;
				
				if (functionInvocationNode.FunctionParameterListing.ConstructorWasInvoked)
				{
					for (int i = 0; i < functionInvocationNode.FunctionParameterListing.FunctionParameterEntryList.Count; i++)
					{
						var item = functionInvocationNode.FunctionParameterListing.FunctionParameterEntryList[i];
						item.ExpressionNode = TryOptimizeStorageExpression(item.ExpressionNode);
						functionInvocationNode.FunctionParameterListing.FunctionParameterEntryList[i] = item;
					}
				}
				
				break;*/
			}
			case SyntaxKind.DoWhileStatementNode:
			{
				return;
				/*var doWhileStatementNode = (DoWhileStatementNode)syntax;
				
				if (doWhileStatementNode.ExpressionNode is not null)
					doWhileStatementNode.ExpressionNode = TryOptimizeStorageExpression(doWhileStatementNode.ExpressionNode);
				
				break;*/
			}
			case SyntaxKind.ReturnStatementNode:
			{
				return;
				/*var returnStatementNode = (ReturnStatementNode)syntax;
				returnStatementNode.ExpressionNode = TryOptimizeStorageExpression(returnStatementNode.ExpressionNode);
				break;*/
			}
		}
		
		ChildList.Add(syntax);
	}
	
	private IExpressionNode TryOptimizeStorageExpression(IExpressionNode syntax)
	{
		switch (syntax.SyntaxKind)
		{
			case SyntaxKind.BinaryExpressionNode:
			{
				var binaryExpressionNode = (BinaryExpressionNode)syntax;
		
				if (binaryExpressionNode.LeftExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode &&
				    binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
				{
					syntax = new BinaryExpressionLeftAndRightVariableReference(binaryExpressionNode);
				}
				else if (binaryExpressionNode.LeftExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
				{
					var binaryExpressionLeftVariableReference = new BinaryExpressionLeftVariableReference(binaryExpressionNode);
					syntax = binaryExpressionLeftVariableReference;
					
					binaryExpressionLeftVariableReference.SetRightExpressionNode(
						TryOptimizeStorageExpression(binaryExpressionLeftVariableReference.RightExpressionNode));
				}
				else if (binaryExpressionNode.RightExpressionNode.SyntaxKind == SyntaxKind.VariableReferenceNode)
				{
					var binaryExpressionRightVariableReference = new BinaryExpressionRightVariableReference(binaryExpressionNode);
					syntax = binaryExpressionRightVariableReference;
					
					binaryExpressionRightVariableReference.SetLeftExpressionNode(
						TryOptimizeStorageExpression(binaryExpressionRightVariableReference.LeftExpressionNode));
				}
				
				break;
			}
			default:
			{
				break;
			}
		}
		
		return syntax;
	}

    public CodeBlock Build()
    {
        return new CodeBlock(ChildList);
    }
}