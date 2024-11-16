using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

// 64 77

public static class ParseOthers
{
	/// <summary>
	/// TODO: Delete this method, to parse a namespace identifier one should be able to just invoke 'ParseExpression(...)'
	/// </summary>
	public static ISyntax HandleNamespaceIdentifier(CSharpParserModel model)
    {
        var combineNamespaceIdentifierIntoOne = new List<ISyntaxToken>();

        while (!model.TokenWalker.IsEof)
        {
            if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
            {
                var matchedToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
                combineNamespaceIdentifierIntoOne.Add(matchedToken);

                if (matchedToken.IsFabricated)
                    break;
            }
            else
            {
                if (SyntaxKind.MemberAccessToken == model.TokenWalker.Current.SyntaxKind)
                    combineNamespaceIdentifierIntoOne.Add(model.TokenWalker.Consume());
                else
                    break;
            }
        }

        if (combineNamespaceIdentifierIntoOne.Count == 0)
        {
            return new EmptyNode();
        }

        var identifierTextSpan = combineNamespaceIdentifierIntoOne.First().TextSpan with
        {
            EndingIndexExclusive = combineNamespaceIdentifierIntoOne.Last().TextSpan.EndingIndexExclusive
        };

        return new IdentifierToken(identifierTextSpan);
    }

    public static void StartStatement_Expression(CSharpParserModel model)
    {
    	var expressionNode = ParseOthers.ParseExpression(model);
    	model.CurrentCodeBlockBuilder.ChildList.Add(expressionNode);
    }
    
    /// <summary>
    /// ParseExpression while expressionPrimary.SyntaxKind == syntaxKind
    /// 
    /// if (expressionPrimary.SyntaxKind != syntaxKind)
    /// 	model.TokenWalker.Backtrack() to either the previous loops tokenIndex where
    /// 		the syntax kinds did match.
    /// 
    /// 	Or, if they never matched then model.TokenWalker.Backtrack()
    /// 		to the tokenIndex that was had when this function was invoked.
    ///
    /// Return true if a match was found, return false if NO match was found.
    ///
    /// TypeClauseNode code exists in the expression code.
	/// As a result, some statements need to read a TypeClauseNode by invoking 'ParseExpression(...)'.
	///
	/// In order to "short circut" or "force exit" from the expression code back to the statement code,
	/// if the root primary expression is not equal to the model.ForceParseExpressionSyntaxKind
	/// then stop.
    /// </summary>
    public static bool TryParseExpression(SyntaxKind syntaxKind, CSharpParserModel model, out IExpressionNode expressionNode)
    {
    	var originalTokenIndex = model.TokenWalker.Index;
    	model.ForceParseExpressionSyntaxKind = syntaxKind;
    	
    	try
    	{
    		expressionNode = ParseExpression(model);
    		
    		#if DEBUG
    		Console.WriteLine($"try => {expressionNode.SyntaxKind}\n");
    		#endif
    		
    		return expressionNode.SyntaxKind == syntaxKind;
    	}
    	finally
    	{
    		model.ForceParseExpressionSyntaxKind = null;
    		model.ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
    	}
    }
    
    public static bool SyntaxIsEndDelimiter(SyntaxKind syntaxKind)
    {
    	switch (syntaxKind)
    	{
    		case SyntaxKind.CloseParenthesisToken:
			case SyntaxKind.CommaToken:
			case SyntaxKind.CloseAngleBracketToken:
			case SyntaxKind.OpenBraceToken:
			case SyntaxKind.CloseBraceToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.StatementDelimiterToken:
			case SyntaxKind.ColonToken:
			case SyntaxKind.CloseSquareBracketToken:
    			return true;
    		default:
    			return false;
    	}
    }
    
	/// <summary>
	/// Invoke this method when 'model.TokenWalker.Current' is the first token of the expression to be parsed.
	///
	/// In the case where the first token of the expression had already been 'Consume()'-ed then 'model.TokenWalker.Backtrack();'
	/// might be of use in order to move the model.TokenWalker backwards prior to invoking this method.
	/// </summary>
	public static IExpressionNode ParseExpression(CSharpParserModel model)
    {
    	#if DEBUG
    	Console.WriteLine("\nParseExpression(...)");
    	#endif
    
    	var expressionPrimary = model.ForceParseExpressionInitialPrimaryExpression;
    	var indexToken = model.TokenWalker.Index;
    	var forceExit = false;
    	
    	var indexTokenRoot = model.TokenWalker.Index;
    	var expressionPrimaryPreviousRoot = expressionPrimary;
    	
    	while (!model.TokenWalker.IsEof)
        {
        	#if DEBUG
        	WriteExpressionList(model.ExpressionList);
        	#endif
        
        	var tokenCurrent = model.TokenWalker.Current;
    		
    		if (SyntaxIsEndDelimiter(tokenCurrent.SyntaxKind)) // Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		{
    			for (int i = model.ExpressionList.Count - 1; i > -1; i--)
	    		{
	    			var delimiterExpressionTuple = model.ExpressionList[i];
	    			
	    			if (delimiterExpressionTuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				if (delimiterExpressionTuple.ExpressionNode is null)
	    				{
	    					forceExit = true;
	    					break;
	    				}
	    				
	    				expressionPrimary = BubbleUpParseExpression(i, expressionPrimary, model);
	    				break;
	    			}
	    		}
    		}
			
			if (forceExit) // delimiterExpressionTuple.ExpressionNode is null
			{
				expressionPrimary = BubbleUpParseExpression(0, expressionPrimary, model);
				break;
			}
			
    		expressionPrimary = model.Binder.AnyMergeToken(expressionPrimary, tokenCurrent, model);
    		
    		#if DEBUG
    		Console.WriteLine($"\t=> {expressionPrimary.SyntaxKind}");
    		#endif
    		
    		if (model.TokenWalker.Index == indexToken)
    			_ = model.TokenWalker.Consume();
    		if (model.TokenWalker.Index < indexToken)
    			throw new LuthetusTextEditorException($"Infinite loop in {nameof(ParseExpression)}");
    		
    		indexToken = model.TokenWalker.Index;
    		
    		if (model.NoLongerRelevantExpressionNode is not null) // try finally is not needed to guarantee setting 'model.NoLongerRelevantExpressionNode = null;' because this is an object reference comparison 'Object.ReferenceEquals'. Versus something more general that would break future parses if not properly cleared, like a SyntaxKind.
			{
				model.Binder.ClearFromExpressionList(model.NoLongerRelevantExpressionNode, model);
				model.NoLongerRelevantExpressionNode = null;
			}
    		
    		if (model.ForceParseExpressionSyntaxKind is not null)
    		{
    			var isExpressionRoot = true;
    			var rootSyntaxKind = SyntaxKind.EmptyExpressionNode;
    			
    			foreach (var tuple in model.ExpressionList)
    			{
    				if (tuple.ExpressionNode is null)
    					continue;
    					
    				isExpressionRoot = false;
    				rootSyntaxKind = tuple.ExpressionNode.SyntaxKind;
    				break;
    			}
    			
    			var success = true;
    			
    			if (isExpressionRoot)
    			{
    				success = expressionPrimary.SyntaxKind == model.ForceParseExpressionSyntaxKind;
    				
    				if (success)
    				{
    					expressionPrimaryPreviousRoot = expressionPrimary;
    					indexTokenRoot = model.TokenWalker.Index;
    				}
    			}
    			else
    			{
    				success = rootSyntaxKind == model.ForceParseExpressionSyntaxKind;
    			}
    			
    			if (!success)
    			{
    				var distance = model.TokenWalker.Index - indexTokenRoot;
		    		
	    			for (int i = 0; i < distance; i++)
	    			{
	    				_ = model.TokenWalker.Backtrack();
	    			}
	    			
	    			expressionPrimary = expressionPrimaryPreviousRoot;
	    			
		    		forceExit = true;
		    		
		    		#if DEBUG
		    		Console.WriteLine("----ForceParseExpressionSyntaxKind");
		    		#endif
    			}
    		}
    		
    		if (forceExit) // model.ForceParseExpressionSyntaxKind
				break;
        }
    	
    	// It is vital that this 'clear' and 'add' are done in a way that permits an invoker of the 'ParseExpression' method to 'add' a similar 'forceExit' delimiter
    	// 	Example: 'model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));'
    	model.ExpressionList.Clear();
    	model.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
    	
    	if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		expressionPrimary = model.Binder.ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				(AmbiguousIdentifierExpressionNode)expressionPrimary,
				model);
    	}
    	
    	#if DEBUG
    	Console.WriteLine();
    	#endif
    	
    	return expressionPrimary;
    }

	/// <summary>
	/// 'BubbleUpParseExpression(i, expressionPrimary, model);'
	/// 
    /// This is to have SyntaxKind.StatementDelimiterToken break out of the expression.
	/// The parser is adding as the 0th item that
	/// 'SyntaxKind.StatementDelimiterToken' returns the primary expression to be 'null'.
	///
	/// One isn't supposed to deal with nulls here, instead using EmptyExpressionNode.
	/// So, if delimiterExpressionTuple.ExpressionNode is null then
	/// this special case to break out of the expresion logic exists.
	///
	/// It needs to be part of the session.ShortCircuitList however,
	/// because if an expression uses 'SyntaxKind.StatementDelimiterToken'
	/// in their expression, they can override this 0th index entry
	/// and have primary expression "short circuit" to their choosing
	/// and the loop will continue parsing more expressions.
	///
	/// LambdaExpressionNode for example, needs to override 'SyntaxKind.StatementDelimiterToken'.
	/// </summary>
    private static IExpressionNode BubbleUpParseExpression(int indexTriggered, IExpressionNode expressionPrimary, CSharpParserModel model)
    {
    	var triggeredDelimiterTuple = model.ExpressionList[indexTriggered];
    	IExpressionNode? previousDelimiterExpressionNode = null;
    	
    	var initialExpressionListCount = model.ExpressionList.Count;
    	
    	#if DEBUG
		Console.WriteLine($"BREAK_({triggeredDelimiterTuple.DelimiterSyntaxKind}, {triggeredDelimiterTuple.ExpressionNode.SyntaxKind})");
		#endif
		
		for (int i = initialExpressionListCount - 1; i > indexTriggered - 1; i--)
		{
			var delimiterExpressionTuple = model.ExpressionList[i];
			model.ExpressionList.RemoveAt(i);
			
			if (delimiterExpressionTuple.ExpressionNode is null)
				break; // This implies to forcibly return back to the statement while loop.
			if (Object.ReferenceEquals(previousDelimiterExpressionNode, delimiterExpressionTuple.ExpressionNode))
				continue; // This implies that an individual IExpressionNode existed in the list for more than one SyntaxKind. All entries for a node are continguous, so if the previous node were the same object, then it was already handled.
			if (Object.ReferenceEquals(triggeredDelimiterTuple.ExpressionNode, delimiterExpressionTuple.ExpressionNode) &&
				triggeredDelimiterTuple.DelimiterSyntaxKind != delimiterExpressionTuple.DelimiterSyntaxKind)
			{
				continue; // This implies that the triggered syntax kind was not the first syntax kind found for the given 'triggeredDelimiterTuple.ExpressionNode'. (example: a FunctionParametersListingNode might make two entries in the list. 1 for SyntaxKind.CloseParenthesisToken, another for SyntaxKind.CommaToken. If 'SyntaxKind.CloseParenthesisToken' is triggered the 'SyntaxKind.CommaToken' will be hit by this loop first. So it would need to be skipped.
			}
			
			previousDelimiterExpressionNode = delimiterExpressionTuple.ExpressionNode;
			
			expressionPrimary = model.Binder.AnyMergeExpression(
				delimiterExpressionTuple.ExpressionNode,
				expressionPrimary, // expressionSecondary
				model);
		}
		
		if (model.NoLongerRelevantExpressionNode is not null) // try finally is not needed to guarantee setting 'model.NoLongerRelevantExpressionNode = null;' because this is an object reference comparison 'Object.ReferenceEquals'. Versus something more general that would break future parses if not properly cleared, like a SyntaxKind.
		{
			model.Binder.ClearFromExpressionList(model.NoLongerRelevantExpressionNode, model);
			model.NoLongerRelevantExpressionNode = null;
		}
		
		return expressionPrimary;
    }
    
    private static void WriteExpressionList(List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> expressionList)
    {
    	foreach (var tuple in expressionList)
    	{
    		Console.Write('{');
    		Console.Write(tuple.DelimiterSyntaxKind);
    		Console.Write(',');
    		Console.Write(tuple.ExpressionNode?.SyntaxKind.ToString() ?? "null");
    		Console.Write('}');
    		Console.Write(", ");
    	}
    	
    	Console.WriteLine();
    }
}