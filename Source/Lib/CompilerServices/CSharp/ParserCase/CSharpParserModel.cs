using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

/// <summary>
/// The computational state for the CSharpParser is contained within this type.
/// The output of the CSharpParser is the <see cref="CSharpCompilationUnit"/>.<see cref="CSharpCompilationUnit.RootCodeBlockNode"/>
/// </summary>
public struct CSharpParserModel
{
	/// <summary>
	/// Should 0 be the global scope?
	/// </summary>
	private int _indexKey = 0;
	
	private int _symbolId = 0;

    public CSharpParserModel(
        CSharpBinder binder,
        List<SyntaxToken> tokenList,
        CSharpCodeBlockBuilder globalCodeBlockBuilder,
        CSharpCodeBlockBuilder currentCodeBlockBuilder,
        int globalScopeIndexKey,
	    NamespaceStatementNode topLevelNamespaceStatementNode)
    {
    	Binder = binder;
	    CurrentScopeIndexKey = globalScopeIndexKey;
	    CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
    
    	TokenWalker = Binder.CSharpParserModel_TokenWalker;
    	TokenWalker.Reinitialize(tokenList);
    	
        GlobalCodeBlockBuilder = globalCodeBlockBuilder;
        CurrentCodeBlockBuilder = currentCodeBlockBuilder;
        
        ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
        
        StatementBuilder = new(Binder);
        
        ParseChildScopeStack = Binder.CSharpParserModel_ParseChildScopeStack;
        ParseChildScopeStack.Clear();
        
        ExpressionList = Binder.CSharpParserModel_ExpressionList;
        ExpressionList.Clear();
        ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
        ExpressionList.Add((SyntaxKind.EndOfFileToken, null));
        
        TryParseExpressionSyntaxKindList = Binder.CSharpParserModel_TryParseExpressionSyntaxKindList;
        TryParseExpressionSyntaxKindList.Clear();
        
        AmbiguousIdentifierExpressionNode = Binder.CSharpParserModel_AmbiguousIdentifierExpressionNode;
        AmbiguousIdentifierExpressionNode.SetSharedInstance(
        	default,
	        genericParameterListing: default,
	        CSharpFacts.Types.Void.ToTypeReference(),
	        followsMemberAccessToken: false);
	        
	    TypeClauseNode = Binder.CSharpParserModel_TypeClauseNode;
	    TypeClauseNode.SetSharedInstance(
	    	typeIdentifier: default,
			valueType: null,
			genericParameterListing: default,
			isKeywordType: false);
		TypeClauseNode.IsBeingUsed = false;
			
		VariableReferenceNode = Binder.CSharpParserModel_VariableReferenceNode;
	    VariableReferenceNode.SetSharedInstance(
	    	variableIdentifierToken: default,
			variableDeclarationNode: null);
		VariableReferenceNode.IsBeingUsed = false;
    }

    public TokenWalker TokenWalker { get; }
    public CSharpStatementBuilder StatementBuilder { get; set; }
    
    /// <summary>
	/// TODO: Measure the cost of 'Peek(...)', 'TryPeek(...)' since now...
	/// ...this is a value tuple and the dequeue alone does not mean success,
	/// you have to peek first to see if the object references are equal.
	/// </summary>
    public Stack<(ICodeBlockOwner CodeBlockOwner, CSharpDeferredChildScope DeferredChildScope)> ParseChildScopeStack { get; }
    
    /// <summary>
    /// The C# IParserModel implementation will only "short circuit" if the 'SyntaxKind DelimiterSyntaxKind'
    /// is registered as a delimiter.
    ///
    /// This is done in order to speed up the while loop, as the list of short circuits doesn't have to be
    /// iterated unless the current token is a possible delimiter.
    ///
    /// Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers.SyntaxIsEndDelimiter(SyntaxKind syntaxKind) {...}
    /// </summary>
    public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ExpressionList { get; set; }
    
    public IExpressionNode? NoLongerRelevantExpressionNode { get; set; }
    public List<SyntaxKind> TryParseExpressionSyntaxKindList { get; }
    public IExpressionNode ForceParseExpressionInitialPrimaryExpression { get; set; }
    
    /// <summary>
    /// When parsing a value tuple, this needs to be remembered,
    /// then reset to the initial value foreach of the value tuple's members.
    ///
    /// 'CSharpParserContextKind.ForceStatementExpression' is related
    /// to disambiguating the less than operator '<' and
    /// generic arguments '<...>'.
    ///
    /// Any case where 'ParserContextKind' says that
    /// generic arguments '<...>' for variable declaration
    /// this needs to be available as information to each member.
    /// </summary>
    public CSharpParserContextKind ParserContextKind { get; set; }
    
    public CSharpCodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CSharpCodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
    
    public CSharpBinder Binder { get; set; }

    public int CurrentScopeIndexKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public TypeReference MostRecentLeftHandSideAssignmentExpressionTypeClauseNode { get; set; } = CSharpFacts.Types.Void.ToTypeReference();
    
    public UsingStatementListingNode? UsingStatementListingNode { get; set; }
    
    /// <summary>
    /// TODO: Consider the case where you have just an AmbiguousIdentifierExpressionNode then StatementDelimiterToken.
    /// </summary>
    public AmbiguousIdentifierExpressionNode AmbiguousIdentifierExpressionNode { get; }
    
    /// <summary>
    /// TODO: Consider the case where you have just a TypeClauseNode then StatementDelimiterToken.
    /// </summary>
    public TypeClauseNode TypeClauseNode { get; }
    
    public TypeClauseNode ConstructOrRecycleTypeClauseNode(
    	SyntaxToken typeIdentifier,
		Type? valueType,
		GenericParameterListing genericParameterListing,
		bool isKeywordType)
    {
    	if (TypeClauseNode.IsBeingUsed)
    	{
    		return new TypeClauseNode(
    			typeIdentifier,
				valueType,
				genericParameterListing,
				isKeywordType);
		}    
    	
    	TypeClauseNode.SetSharedInstance(
    		typeIdentifier,
			valueType,
			genericParameterListing,
			isKeywordType);
			
    	return TypeClauseNode;
    }
    
    /// <summary>
    /// TODO: Consider the case where you have just a VariableReferenceNode then StatementDelimiterToken.
    /// </summary>
    public VariableReferenceNode VariableReferenceNode { get; }
    
    public VariableReferenceNode ConstructOrRecycleVariableReferenceNode(
    	SyntaxToken variableIdentifierToken,
		VariableDeclarationNode variableDeclarationNode)
    {
    	if (VariableReferenceNode.IsBeingUsed)
    		return new VariableReferenceNode(variableIdentifierToken, variableDeclarationNode);
    
    	VariableReferenceNode.SetSharedInstance(variableIdentifierToken, variableDeclarationNode);
    	return VariableReferenceNode;
    }
    
    /// <summary>TODO: Delete this code it is only being used temporarily for debugging.</summary>
    // public HashSet<int> SeenTokenIndexHashSet { get; set; } = new();
    
    public int GetNextIndexKey()
    {
    	return ++_indexKey;
    }
    
    public int GetNextSymbolId()
    {
    	return ++_symbolId;
    }
}
