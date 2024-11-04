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
    public static void HandleNamespaceReference(
        IdentifierToken consumedIdentifierToken,
        NamespaceGroupNode resolvedNamespaceGroupNode,
        CSharpParserModel model)
    {
        model.Binder.BindNamespaceReference(consumedIdentifierToken, model);

        if (SyntaxKind.MemberAccessToken == model.TokenWalker.Current.SyntaxKind)
        {
            var memberAccessToken = model.TokenWalker.Consume();
            var memberIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (memberIdentifierToken.IsFabricated)
            {
                model.DiagnosticBag.ReportUnexpectedToken(
                    model.TokenWalker.Current.TextSpan,
                    model.TokenWalker.Current.SyntaxKind.ToString(),
                    SyntaxKind.IdentifierToken.ToString());
            }

            // Check all the TypeDefinitionNodes that are in the namespace
            var typeDefinitionNodes = resolvedNamespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            var typeDefinitionNode = typeDefinitionNodes.SingleOrDefault(td =>
                td.TypeIdentifierToken.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText());

            if (typeDefinitionNode is null)
            {
                model.DiagnosticBag.ReportNotDefinedInContext(
                    model.TokenWalker.Current.TextSpan,
                    consumedIdentifierToken.TextSpan.GetText());
            }
            else
            {
                ParseTypes.HandleTypeReference(
                    memberIdentifierToken,
                    typeDefinitionNode,
                    model);
            }
        }
        else
        {
            // TODO: (2023-05-28) Report an error diagnostic for 'namespaces are not statements'. Something like this I'm not sure.
            model.TokenWalker.Consume();
        }
    }

    public static void HandleNamespaceIdentifier(CSharpParserModel model)
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
            model.SyntaxStack.Push(new EmptyNode());
            return;
        }

        var identifierTextSpan = combineNamespaceIdentifierIntoOne.First().TextSpan with
        {
            EndingIndexExclusive = combineNamespaceIdentifierIntoOne.Last().TextSpan.EndingIndexExclusive
        };

        model.SyntaxStack.Push(new IdentifierToken(identifierTextSpan));
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
    	
    	while (!model.TokenWalker.IsEof)
        {
        	var tokenCurrent = model.TokenWalker.Current;
    		
    		// Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		if (SyntaxIsEndDelimiter(tokenCurrent.SyntaxKind))
    		{
    			for (int i =  model.ExpressionList.Count - 1; i > -1; i--)
	    		{
	    			var delimiterExpressionTuple = model.ExpressionList[i];
	    			
	    			if (delimiterExpressionTuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				if (delimiterExpressionTuple.ExpressionNode is null)
	    				{
	    					//for (int z = 0; z < 10; z++)
	    					//	Console.WriteLine("forceExit = true;");
	    					
	    					forceExit = true;
	    					break;
	    				}
	    				
	    				expressionPrimary = BubbleUpParseExpression(model.ExpressionList.Count - 1, i - 1, expressionPrimary, model);
	    				model.ExpressionList.RemoveRange(i, model.ExpressionList.Count - i);
	    				break;
	    				
	    				#if DEBUG
	    				WriteExpressionList(model.ExpressionList);
	    				#endif
	    			}
	    		}
    		}
			
			if (forceExit)
			{
				expressionPrimary = BubbleUpParseExpression(model.ExpressionList.Count - 1, -1, expressionPrimary, model);
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
	/// 'BubbleUpParseExpression(indexStart: model.ExpressionList.Count - 1, indexExclusiveEnd: -1);'
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
    private static IExpressionNode BubbleUpParseExpression(int indexStart, int indexExclusiveEnd, IExpressionNode expressionPrimary, CSharpParserModel model)
    {
    	(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode) triggeredDelimiterTuple = default;
    	IExpressionNode? previousDelimiterExpressionNode = null;
    	
    	if (indexExclusiveEnd + 1 < model.ExpressionList.Count)
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