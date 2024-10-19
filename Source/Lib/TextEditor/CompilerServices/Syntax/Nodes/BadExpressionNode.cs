using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// If the parser cannot parse an expression, then replace
/// the expression result with an instance of this type.
///
/// The <see cref="SyntaxList"/> contains the 'primaryExpression'
/// that was attempted to have merge with the 'token' or 'secondaryExpression'.
/// As well it contains either the 'token' or the 'secondaryExpression'.
/// And then as well any other syntax that was parsed up until
/// the expression ended.
/// </summary>
public sealed class BadExpressionNode : IExpressionNode
{
    public BadExpressionNode(TypeClauseNode resultTypeClauseNode, List<ISyntax> syntaxList)
    {
    	ResultTypeClauseNode = resultTypeClauseNode;
        SyntaxList = syntaxList;

        SetChildList();
    }
    
    public BadExpressionNode(TypeClauseNode resultTypeClauseNode, ISyntax syntaxPrimary, ISyntax syntaxSecondary)
    	: this(resultTypeClauseNode, new List<ISyntax> { syntaxPrimary, syntaxSecondary })
    {
    }

    public List<ISyntax> SyntaxList { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

	/// <summary>
	/// Use <see cref="SyntaxList"/>, this type is an exception.
	///
	/// It is a bit odd to set 'ChildList' to an empty array here.
	///
	/// But, a bad expression might be rather long.
	/// Especially if the parser starts reading the rest of the file
	/// while thinking it is part of the "bad expression".
	///
	/// So, 'SyntaxList' is a 'List<T>' in order to easily
	/// add all the syntax that is part of this 'bad syntax' as it is parsed.
	///
	/// This messes with the convention of 'ChildList' containing all the nodes,
	/// or tokens that pertain to this node itself.
	///
	/// But it is believed to be worth ignoring 'ChildList' for this type.
	/// </summary>
    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BadExpressionNode;
    
    public void SetChildList()
    {
    	// It is a bit odd to set 'ChildList' to an empty array here.
    	//
    	// But, a bad expression might be rather long.
    	// Especially if the parser starts reading the rest of the file
    	// while thinking it is part of the "bad expression".
    	//
    	// So, 'SyntaxList' is a 'List<T>' in order to easily
    	// add all the syntax that is part of this 'bad syntax' as it is parsed.
    	//
    	// This messes with the convention of 'ChildList' containing all the nodes,
    	// or tokens that pertain to this node itself.
    	//
    	// But it is believed to be worth ignoring 'ChildList' for this type.
    	ChildList = Array.Empty<ISyntax>();
    }
}