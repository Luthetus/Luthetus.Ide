using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseOthers
{
	/// <summary>
	/// TODO: Delete this method, to parse a namespace identifier one should be able to just invoke 'ParseExpression(...)'
	/// </summary>
	public static ISyntax HandleNamespaceIdentifier(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        var combineNamespaceIdentifierIntoOne = new List<SyntaxToken>();

        while (!parserComputation.TokenWalker.IsEof)
        {
            if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
            {
                var matchedToken = parserComputation.TokenWalker.Match(SyntaxKind.IdentifierToken);
                combineNamespaceIdentifierIntoOne.Add(matchedToken);

                if (matchedToken.IsFabricated)
                    break;
            }
            else
            {
                if (SyntaxKind.MemberAccessToken == parserComputation.TokenWalker.Current.SyntaxKind)
                    combineNamespaceIdentifierIntoOne.Add(parserComputation.TokenWalker.Consume());
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
        
        identifierTextSpan.ClearTextCache();

        return new SyntaxToken(SyntaxKind.IdentifierToken, identifierTextSpan);
    }

    public static void StartStatement_Expression(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
    	parserComputation.CurrentCodeBlockBuilder.ChildList.Add(expressionNode);
    }
    
    /// <summary>
    /// ParseExpression while expressionPrimary.SyntaxKind == syntaxKind
    /// 
    /// if (expressionPrimary.SyntaxKind != syntaxKind)
    /// 	parserComputation.TokenWalker.Backtrack() to either the previous loops tokenIndex where
    /// 		the syntax kinds did match.
    /// 
    /// 	Or, if they never matched then parserComputation.TokenWalker.Backtrack()
    /// 		to the tokenIndex that was had when this function was invoked.
    ///
    /// Return true if a match was found, return false if NO match was found.
    ///
    /// TypeClauseNode code exists in the expression code.
	/// As a result, some statements need to read a TypeClauseNode by invoking 'ParseExpression(...)'.
	///
	/// In order to "short circut" or "force exit" from the expression code back to the statement code,
	/// if the root primary expression is not equal to the parserComputation.ForceParseExpressionSyntaxKind
	/// then stop.
	///
	/// ------------------------------
	/// Retrospective comment (2024-12-16):
	/// It appears that the 'SyntaxKind? syntaxKind'
	/// argument is nullable in order to permit
	/// usage of 'parserComputation.ForceParseExpressionInitialPrimaryExpression'
	/// without specifying a specific syntax kind?
	///
	/// The use case:
	/// FunctionInvocationNode as a statement
	/// will currently erroneously parse as a TypeClauseNode.
	///
	/// But, once the statement code receives the 'TypeClauseNode' result
	/// from 'TryParseExpression', the next SyntaxToken
	/// is OpenParenthesisToken.
	///
	/// Therefore, it is obvious at this point that we really wanted
	/// to parse a function invocation node.
	///
	/// But, if there is any code that comes after the function invocation,
	/// and prior to the statement delimiter.
	///
	/// Then a FunctionInvocationNode would not sufficiently represent the statement-expression.
	/// 
	/// i.e.: MyMethod() + 2;
	///
	/// So, I cannot 'TryParseExpression' for a SyntaxKind.FunctionInvocationNode for this reason.
	///
	/// But, I need to initialize the 'ParseExpression' method with the 'TypeClauseNode'
	/// (the 'TypeClauseNode' is in reality the function identifier / generic arguments to the function if there are any).
	///
	/// Then, the 'ParseExpression(...)' code can see that there is a 'TypeClauseNode' merging with an OpenParenthesisToken,
	/// and that the only meaning this can have is function invocation.
	///
	/// At that point, go on to move the 'TypeClauseNode' to be a function identifier, and the
	/// generic arguments for the function invocation, and go on from there.
    /// </summary>
    public static bool TryParseExpression(SyntaxKind? syntaxKind, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation, out IExpressionNode expressionNode)
    {
    	var originalTokenIndex = parserComputation.TokenWalker.Index;
    	
    	if (syntaxKind is not null)
    		parserComputation.TryParseExpressionSyntaxKindList.Add(syntaxKind.Value);
    	
    	try
    	{
    		expressionNode = ParseExpression(compilationUnit, ref parserComputation);
    		
    		/*#if DEBUG
    		Console.WriteLine($"try => {expressionNode.SyntaxKind}\n");
    		#endif*/
    		
    		if (parserComputation.TryParseExpressionSyntaxKindList.Count == 0)
    			return true;
    		else
    			return parserComputation.TryParseExpressionSyntaxKindList.Contains(expressionNode.SyntaxKind);
    	}
    	finally
    	{
    		parserComputation.TryParseExpressionSyntaxKindList.Clear();
    		parserComputation.ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
    		
    		parserComputation.ParserContextKind = CSharpParserContextKind.None;
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
			case SyntaxKind.StringInterpolatedEndToken:
			case SyntaxKind.StringInterpolatedContinueToken:
    			return true;
    		default:
    			return false;
    	}
    }
    
	/// <summary>
	/// Invoke this method when 'parserComputation.TokenWalker.Current' is the first token of the expression to be parsed.
	///
	/// In the case where the first token of the expression had already been 'Consume()'-ed then 'parserComputation.TokenWalker.Backtrack();'
	/// might be of use in order to move the parserComputation.TokenWalker backwards prior to invoking this method.
	/// </summary>
	public static IExpressionNode ParseExpression(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	/*#if DEBUG
    	Console.WriteLine("\nParseExpression(...)");
    	#endif*/
    
    	var expressionPrimary = parserComputation.ForceParseExpressionInitialPrimaryExpression;
    	var indexToken = parserComputation.TokenWalker.Index;
    	var forceExit = false;
    	
    	var indexTokenRoot = parserComputation.TokenWalker.Index;
    	var expressionPrimaryPreviousRoot = expressionPrimary;
    	
    	while (true)
        {
        	/*#if DEBUG
        	WriteExpressionList(parserComputation.ExpressionList);
        	#endif*/
        
        	var tokenCurrent = parserComputation.TokenWalker.Current;
    		
    		if (SyntaxIsEndDelimiter(tokenCurrent.SyntaxKind)) // Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		{
    			for (int i = parserComputation.ExpressionList.Count - 1; i > -1; i--)
	    		{
	    			var delimiterExpressionTuple = parserComputation.ExpressionList[i];
	    			
	    			if (delimiterExpressionTuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				if (delimiterExpressionTuple.ExpressionNode is null)
	    				{
	    					forceExit = true;
	    					break;
	    				}
	    				
	    				expressionPrimary = BubbleUpParseExpression(i, expressionPrimary, compilationUnit, ref parserComputation);
	    				break;
	    			}
	    		}
    		}
			
			// The while loop used to be 'while (!parserComputation.TokenWalker.IsEof)'
			// This caused an issue where 'BubbleUpParseExpression(...)' would not run
			// if the end of file was reached.
			//
			// Given how this parser is written, adding 'SyntaxKind.EndOfFile' to 'parserComputation.ExpressionList'
			// would follow the pattern of how 'SyntaxKind.StatementDelimiterToken' is written.
			//
			// But, the 'while (true)' loop makes me extremely uncomfortable.
			//
			// So I added '|| parserComputation.TokenWalker.IsEof' here.
			//
			// If upon further inspection on way or the other is deemed safe then this redundancy can be removed.
			if (forceExit || parserComputation.TokenWalker.IsEof) // delimiterExpressionTuple.ExpressionNode is null
			{
				expressionPrimary = BubbleUpParseExpression(0, expressionPrimary, compilationUnit, ref parserComputation);
				break;
			}
			
    		expressionPrimary = parserComputation.Binder.AnyMergeToken(expressionPrimary, ref tokenCurrent, compilationUnit, ref parserComputation);
    		
    		/*#if DEBUG
    		Console.WriteLine($"\t=> {expressionPrimary.SyntaxKind}");
    		#endif*/
    		
    		if (parserComputation.TokenWalker.Index == indexToken)
    			_ = parserComputation.TokenWalker.Consume();
    		if (parserComputation.TokenWalker.Index < indexToken)
    			throw new LuthetusTextEditorException($"Infinite loop in {nameof(ParseExpression)}");
    		
    		indexToken = parserComputation.TokenWalker.Index;
    		
    		if (parserComputation.NoLongerRelevantExpressionNode is not null) // try finally is not needed to guarantee setting 'parserComputation.NoLongerRelevantExpressionNode = null;' because this is an object reference comparison 'Object.ReferenceEquals'. Versus something more general that would break future parses if not properly cleared, like a SyntaxKind.
			{
				parserComputation.Binder.ClearFromExpressionList(parserComputation.NoLongerRelevantExpressionNode, compilationUnit, ref parserComputation);
				parserComputation.NoLongerRelevantExpressionNode = null;
			}
    		
    		if (parserComputation.TryParseExpressionSyntaxKindList.Count != 0)
    		{
    			var isExpressionRoot = true;
    			var rootSyntaxKind = SyntaxKind.EmptyExpressionNode;
    			
    			foreach (var tuple in parserComputation.ExpressionList)
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
    				success = parserComputation.TryParseExpressionSyntaxKindList.Contains(expressionPrimary.SyntaxKind);
    				
    				if (success)
    				{
    					expressionPrimaryPreviousRoot = expressionPrimary;
    					indexTokenRoot = parserComputation.TokenWalker.Index;
    				}
    			}
    			else
    			{
    				success = parserComputation.TryParseExpressionSyntaxKindList.Contains(rootSyntaxKind);
    			}
    			
    			if (!success)
    			{
    				var distance = parserComputation.TokenWalker.Index - indexTokenRoot;
		    		
	    			for (int i = 0; i < distance; i++)
	    			{
	    				_ = parserComputation.TokenWalker.Backtrack();
	    			}
	    			
	    			expressionPrimary = expressionPrimaryPreviousRoot;
	    			
		    		forceExit = true;
		    		
		    		/*#if DEBUG
		    		WriteExpressionList(parserComputation.ExpressionList);
		    		Console.WriteLine("----TryParseExpressionSyntaxKindList");
		    		#endif*/
    			}
    		}
    		
    		if (forceExit) // parserComputation.ForceParseExpressionSyntaxKind
				break;
        }
    	
    	// It is vital that this 'clear' and 'add' are done in a way that permits an invoker of the 'ParseExpression' method to 'add' a similar 'forceExit' delimiter
    	// 	Example: 'parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));'
    	parserComputation.ExpressionList.Clear();
    	parserComputation.ExpressionList.Add((SyntaxKind.EndOfFileToken, null));
    	parserComputation.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
    	
    	if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		expressionPrimary = parserComputation.Binder.ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				(AmbiguousIdentifierExpressionNode)expressionPrimary,
				compilationUnit,
				ref parserComputation);
    	}
    	
    	/*#if DEBUG
    	Console.WriteLine();
    	#endif*/
    	
    	return expressionPrimary;
    }

	/// <summary>
	/// 'BubbleUpParseExpression(i, expressionPrimary, compilationUnit);'
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
    private static IExpressionNode BubbleUpParseExpression(int indexTriggered, IExpressionNode expressionPrimary, CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var triggeredDelimiterTuple = parserComputation.ExpressionList[indexTriggered];
    	IExpressionNode? previousDelimiterExpressionNode = null;
    	
    	var initialExpressionListCount = parserComputation.ExpressionList.Count;
    	
    	/*#if DEBUG
    	var nullNodeSyntaxKindText = "null";
		Console.WriteLine($"BREAK_({triggeredDelimiterTuple.DelimiterSyntaxKind}, {triggeredDelimiterTuple.ExpressionNode?.SyntaxKind.ToString() ?? nullNodeSyntaxKindText})");
		#endif*/
		
		for (int i = initialExpressionListCount - 1; i > indexTriggered - 1; i--)
		{
			var delimiterExpressionTuple = parserComputation.ExpressionList[i];
			parserComputation.ExpressionList.RemoveAt(i);
			
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
			
			expressionPrimary = parserComputation.Binder.AnyMergeExpression(
				delimiterExpressionTuple.ExpressionNode,
				expressionPrimary, // expressionSecondary
				compilationUnit,
				ref parserComputation);
		}
		
		if (parserComputation.NoLongerRelevantExpressionNode is not null) // try finally is not needed to guarantee setting 'parserComputation.NoLongerRelevantExpressionNode = null;' because this is an object reference comparison 'Object.ReferenceEquals'. Versus something more general that would break future parses if not properly cleared, like a SyntaxKind.
		{
			parserComputation.Binder.ClearFromExpressionList(parserComputation.NoLongerRelevantExpressionNode, compilationUnit, ref parserComputation);
			parserComputation.NoLongerRelevantExpressionNode = null;
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