using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

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
    /// The source code: "<int>" is sort of nonsensical.
    ///
    /// But, nothing stops the user from typing this as a statement,
    /// 	it just is that C# won't compile it.
    ///
    /// So, if a statement starts with 'OpenAngleBracketToken',
    /// what is the best parse we can provide for that?
    ///
    /// The idea is to tell the expression parsing code to return a certain SyntaxKind.
    ///
    /// So, its another 'forceExit' but instead of matching on 'CloseAngleBracketToken'
    /// you are saying: when the top most 'SyntaxKind' is completed,
    /// return it to me (stop parsing expressions).
    ///
    /// The wording of 'top most' is referring to the recursive, "nonsensical" syntax
    /// of <<int>>.
    ///
    /// Here the outermost GenericParametersListingNode contains
    /// a GenericParametersListingNode, and neither of them have an identifier
    /// that comes before the OpenAngleBracketToken.
    /// The "more sensible" syntax would be to type 'MyClass<int>;' rather than just '<int>;'
    /// They both will not compile, but one is on an extreme end of odd syntax
    /// and needs to be specifically targeted.
    /// </summary>
    public static IExpressionNode Force_ParseExpression(SyntaxKind syntaxKind, CSharpParserModel model)
    {
    	model.ForceParseExpressionSyntaxKind = syntaxKind;
    	
    	try
    	{
    		return ParseExpression(model);
    	}
    	finally
    	{
    		model.ForceParseExpressionSyntaxKind = null;
    	}
    }

	/// <summary>
	/// Invoke this method when 'model.TokenWalker.Current' is the first token of the expression to be parsed.
	///
	/// In the case where the first token of the expression had already been 'Consume()'-ed
	/// 'model.TokenWalker.Backtrack();' might be of use in order to move the model.TokenWalker backwards
	/// prior to invoking this method.
	/// </summary>
	public static IExpressionNode ParseExpression(CSharpParserModel model)
    {
#if DEBUG
    	Console.Write("\n====START==============================================================================\n");
    	Console.Write("====START==============================================================================\n\n");

		WriteExpressionList(model.ExpressionList);
#endif

    	var expressionPrimary = (IExpressionNode)new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause());
    	var forceExit = false;
    	var hasSeenExpressionSyntaxKind = false;
    	
    	while (!model.TokenWalker.IsEof)
        {
        	var tokenCurrent = model.TokenWalker.Current;
    		
    		// The CSharpBinder.Expressions.cs code does not 'remove' from the 'model'ExpressionList'
			// But, it does at times 'add'.
			// Therefore the count needs to be stored ahead of time.
			var expressionListCount = model.ExpressionList.Count;
    		
    		// Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		if (SyntaxIsEndDelimiter(tokenCurrent.SyntaxKind))
    		{
    			for (int i = expressionListCount - 1; i > -1; i--)
	    		{
	    			var delimiterExpressionTuple = model.ExpressionList[i];
	    			
	    			if (delimiterExpressionTuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				if (delimiterExpressionTuple.ExpressionNode is null)
	    				{
	    					forceExit = true;
	    					break;
	    				}
	    				
	    				expressionPrimary = BubbleUpParseExpression(expressionListCount - 1, i - 1, expressionPrimary, model, expressionListCount: expressionListCount);
	    				model.ExpressionList.RemoveRange(i, expressionListCount - i);
	    				
	    				if (model.NoLongerRelevantExpressionNode is not null)
	    				{
	    					// try finally is not needed to guarantee setting 'model.NoLongerRelevantExpressionNode = null;'
	    					// because this is an object reference comparison  'Object.ReferenceEquals'.
	    					// 
	    					// Versus something more general that would break future parses
	    					// if not properly cleared, like a SyntaxKind.
	    					model.Binder.ClearFromExpressionList(model.NoLongerRelevantExpressionNode, model);
	    					model.NoLongerRelevantExpressionNode = null;
	    				}
	    				
	    				break;
	    			}
	    		}
    		}
			
			if (forceExit)
			{
				expressionPrimary = BubbleUpParseExpression(expressionListCount - 1, -1, expressionPrimary, model, expressionListCount: expressionListCount);
				break;
			}
			
			#if DEBUG
			Console.Write($"{expressionPrimary.SyntaxKind} + {tokenCurrent.SyntaxKind} => ");
			#endif
			
    		expressionPrimary = model.Binder.AnyMergeToken(expressionPrimary, tokenCurrent, model);
    		
    		#if DEBUG
    		Console.Write($"{expressionPrimary.SyntaxKind}\n\n");
    		
    		WriteExpressionList(model.ExpressionList);
    		#endif
    		
    		_ = model.TokenWalker.Consume();
    		
    		if (model.ForceParseExpressionSyntaxKind is not null)
    		{
    			var allNotEqual = true;
    			
    			foreach (var tuple in model.ExpressionList)
    			{
    				if (tuple.ExpressionNode is null)
    					continue;
    					
    				if (tuple.ExpressionNode.SyntaxKind == model.ForceParseExpressionSyntaxKind)
    					allNotEqual = false;
    			}
    			
    			if (allNotEqual)
    			{
    				if (hasSeenExpressionSyntaxKind)
	    				return expressionPrimary;
    			}
    			else if (!hasSeenExpressionSyntaxKind)
    			{
    				hasSeenExpressionSyntaxKind = true;
    			}
    		}
        }
    	
    	// It is vital that this 'clear' and 'add' are done in a way that:
    	// permits an invoker of the 'ParseExpression' method to 'add' a similar 'forceExit' delimiter
    	// just as 'model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));'
    	//
    	// For example, an if statement's expression is written within an OpenParenthesisToken and
    	// a CloseParenthesisToken. BUT, those parenthesis tokens are not part of the expression.
    	//
    	// They are just a 'forceExit' delimiter.
    	model.ExpressionList.Clear();
    	model.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
    	
    	#if DEBUG
    	Console.Write("====END================================================================================\n");
    	Console.Write("====END================================================================================\n\n");
    	#endif
    	
    	if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		expressionPrimary = model.Binder.ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				(AmbiguousIdentifierExpressionNode)expressionPrimary,
				model);
    	}
    	
    	return expressionPrimary;
    }

	/// <summary>
	/// 'BubbleUpParseExpression(indexStart: expressionListCount - 1, indexExclusiveEnd: -1);'
	/// 
    /// This is a hack to have SyntaxKind.StatementDelimiterToken break out of the expression.
	/// The parser is adding as the 0th item that
	/// 'SyntaxKind.StatementDelimiterToken' returns the primary expression to be 'null'.
	///
	/// One isn't supposed to deal with nulls here, instead using EmptyExpressionNode.
	/// So, if i==0 && delimiterExpressionTuple.ExpressionNode is null then
	/// this special case to break out of the expresion logic exists.
	///
	/// It needs to be part of the session.ShortCircuitList however,
	/// because if an expression uses 'SyntaxKind.StatementDelimiterToken'
	/// in their expression, they can override this 0th index entry
	/// and have primary expression "short circuit" to their choosing
	/// and the loop will continue parsing more expressions.
	///
	/// LambdaExpressionNode for example, needs to override 'SyntaxKind.StatementDelimiterToken'.
	///
	/// TODO: Better would be to permit a merge with the model.ExpressionList[1] and expressionPrimary if there were to exist a tuple at that index...
	///       ... even better still might be to "bubble" back up the recursion by joining each entry in the model.ExpressionList from last to first.
	///       and the initial merge is done between model.ExpressionList.Last and expressionPrimary.
	/// </summary>
    private static IExpressionNode BubbleUpParseExpression(int indexStart, int indexExclusiveEnd, IExpressionNode expressionPrimary, CSharpParserModel model, int expressionListCount)
    {
    	(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode) triggeredDelimiterTuple = default;
    	IExpressionNode? previousDelimiterExpressionNode = null;
    	
    	if (indexExclusiveEnd + 1 < expressionListCount)
    		triggeredDelimiterTuple = model.ExpressionList[indexExclusiveEnd + 1];
				
		for (int i = indexStart; i > indexExclusiveEnd; i--)
		{
			var delimiterExpressionTuple = model.ExpressionList[i];
			
			if (delimiterExpressionTuple.ExpressionNode is null)
				break;
			if (Object.ReferenceEquals(previousDelimiterExpressionNode, delimiterExpressionTuple.ExpressionNode))
				continue;
				
			if (delimiterExpressionTuple.ExpressionNode == triggeredDelimiterTuple.ExpressionNode)
			{
				// This line isn't ideal. But without it, one can have function invocation add
				// to the 'model.ExpressionList' (SyntaxKind.CloseParenthesisToken, functionInvocationNode)
				// and (SyntaxKind.CommaToken, functionInvocationNode).
				//
				// Yet, when hitting a 'SyntaxKind.CloseParenthesisToken' the
				// Console.Write will say that the 'SyntaxKind.CommaToken'
				// was hit.
				//
				// Probably a better way to do this, but this being fixed is low priority I'm open to hacking a fix for now.
				delimiterExpressionTuple = triggeredDelimiterTuple;
			}
			
			previousDelimiterExpressionNode = delimiterExpressionTuple.ExpressionNode;
			
			var expressionSecondary = expressionPrimary;
			
			#if DEBUG
			var delimiterExpressionNodeSyntaxKindString = delimiterExpressionTuple.ExpressionNode?.SyntaxKind.ToString() ?? "null";
	    	Console.Write($"BUBBLE_{delimiterExpressionTuple.DelimiterSyntaxKind}: {expressionPrimary.SyntaxKind} <> {delimiterExpressionNodeSyntaxKindString}\n");
			Console.Write($"{delimiterExpressionTuple.ExpressionNode.SyntaxKind} + {expressionSecondary.SyntaxKind} => ");
			#endif
			
			expressionPrimary = model.Binder.AnyMergeExpression(
				delimiterExpressionTuple.ExpressionNode,
				expressionSecondary,
				model);
			
			#if DEBUG
			Console.Write($"{expressionPrimary.SyntaxKind}\n\n");
			#endif
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
    			return true;
    		default:
    			return false;
    	}
    }
}