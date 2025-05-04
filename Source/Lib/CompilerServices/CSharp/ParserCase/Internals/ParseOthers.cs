using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseOthers
{
	/// <summary>
	/// TODO: Delete this method, to parse a namespace identifier one should be able to just invoke 'ParseExpression(...)'
	///
	/// 'isNamespaceStatement' refers to 'namespace Luthetus.CompilerServices;'
	/// </summary>
	public static SyntaxToken HandleNamespaceIdentifier(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel, bool isNamespaceStatement)
    {
        TextEditorTextSpan textSpan = default;
        int count = 0;

        while (!parserModel.TokenWalker.IsEof)
        {
            if (count % 2 == 0)
            {
                var matchedToken = parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
                count++;
                
                if (textSpan == default)
                {
                	textSpan = matchedToken.TextSpan;
                	textSpan.ClearTextCache();
                	
                	// !StatementDelimiterToken because presumably the final namespace is already being handled.
                	if (isNamespaceStatement && parserModel.TokenWalker.Next.SyntaxKind != SyntaxKind.StatementDelimiterToken)
                		parserModel.Binder.AddNamespaceToCurrentScope(textSpan.GetText(), compilationUnit, ref parserModel);
                }
                else
                {
                	textSpan = textSpan with
			        {
			            EndExclusiveIndex = matchedToken.TextSpan.EndExclusiveIndex
			        };
			        textSpan.ClearTextCache();
			        
			        // !StatementDelimiterToken because presumably the final namespace is already being handled.
			        if (isNamespaceStatement && parserModel.TokenWalker.Next.SyntaxKind != SyntaxKind.StatementDelimiterToken)
			        	parserModel.Binder.AddNamespaceToCurrentScope(textSpan.GetText(), compilationUnit, ref parserModel);
                }

                if (matchedToken.IsFabricated)
                    break;
            }
            else
            {
                if (SyntaxKind.MemberAccessToken == parserModel.TokenWalker.Current.SyntaxKind)
                {
                	_ = parserModel.TokenWalker.Consume();
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        if (count == 0)
            return default;

        return new SyntaxToken(SyntaxKind.IdentifierToken, textSpan);
    }

    public static void StartStatement_Expression(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
    	parserModel.CurrentCodeBlockBuilder.AddChild(expressionNode);
    }
    
    /// <summary>
    /// ParseExpression while expressionPrimary.SyntaxKind == syntaxKind
    /// 
    /// if (expressionPrimary.SyntaxKind != syntaxKind)
    /// 	parserModel.TokenWalker.Backtrack() to either the previous loops tokenIndex where
    /// 		the syntax kinds did match.
    /// 
    /// 	Or, if they never matched then parserModel.TokenWalker.Backtrack()
    /// 		to the tokenIndex that was had when this function was invoked.
    ///
    /// Return true if a match was found, return false if NO match was found.
    ///
    /// TypeClauseNode code exists in the expression code.
	/// As a result, some statements need to read a TypeClauseNode by invoking 'ParseExpression(...)'.
	///
	/// In order to "short circut" or "force exit" from the expression code back to the statement code,
	/// if the root primary expression is not equal to the parserModel.ForceParseExpressionSyntaxKind
	/// then stop.
	///
	/// ------------------------------
	/// Retrospective comment (2024-12-16):
	/// It appears that the 'SyntaxKind? syntaxKind'
	/// argument is nullable in order to permit
	/// usage of 'parserModel.ForceParseExpressionInitialPrimaryExpression'
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
    public static bool TryParseExpression(SyntaxKind? syntaxKind, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel, out IExpressionNode expressionNode)
    {
    	var originalTokenIndex = parserModel.TokenWalker.Index;
    	
    	if (syntaxKind is not null)
    		parserModel.TryParseExpressionSyntaxKindList.Add(syntaxKind.Value);
    	
    	try
    	{
    		expressionNode = ParseExpression(compilationUnit, ref parserModel);
    		
    		/*#if DEBUG
    		Console.WriteLine($"try => {expressionNode.SyntaxKind}\n");
    		#else
			Console.WriteLine($"{nameof(TryParseExpression)} has debug 'Console.Write...' that needs commented out.");
    		#endif*/
    		
    		if (parserModel.TryParseExpressionSyntaxKindList.Count == 0)
    			return true;
    		else
    			return parserModel.TryParseExpressionSyntaxKindList.Contains(expressionNode.SyntaxKind);
    	}
    	finally
    	{
    		parserModel.TryParseExpressionSyntaxKindList.Clear();
    		parserModel.ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
    		
    		parserModel.ParserContextKind = CSharpParserContextKind.None;
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
	/// Invoke this method when 'parserModel.TokenWalker.Current' is the first token of the expression to be parsed.
	///
	/// In the case where the first token of the expression had already been 'Consume()'-ed then 'parserModel.TokenWalker.Backtrack();'
	/// might be of use in order to move the parserModel.TokenWalker backwards prior to invoking this method.
	/// </summary>
	public static IExpressionNode ParseExpression(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	/*#if DEBUG
    	Console.WriteLine("\nParseExpression(...)");
		#else
		Console.WriteLine($"{nameof(ParseExpression)} has debug 'Console.Write...' that needs commented out.");
    	#endif*/
    
    	var expressionPrimary = parserModel.ForceParseExpressionInitialPrimaryExpression;
    	var indexToken = parserModel.TokenWalker.Index;
    	var forceExit = false;
    	
    	var indexTokenRoot = parserModel.TokenWalker.Index;
    	var expressionPrimaryPreviousRoot = expressionPrimary;
    	
    	while (true)
        {
        	/*#if DEBUG
        	WriteExpressionList(parserModel.ExpressionList);
        	#else
			Console.WriteLine($"{nameof(ParseExpression)} has debug 'Console.Write...' that needs commented out.");
        	#endif*/
        
        	var tokenCurrent = parserModel.TokenWalker.Current;
    		
    		if (SyntaxIsEndDelimiter(tokenCurrent.SyntaxKind)) // Check if the tokenCurrent is a token that is used as a end-delimiter before iterating the list?
    		{
    			for (int i = parserModel.ExpressionList.Count - 1; i > -1; i--)
	    		{
	    			var delimiterExpressionTuple = parserModel.ExpressionList[i];
	    			
	    			if (delimiterExpressionTuple.DelimiterSyntaxKind == tokenCurrent.SyntaxKind)
	    			{
	    				if (delimiterExpressionTuple.ExpressionNode is null)
	    				{
	    					forceExit = true;
	    					break;
	    				}
	    				
	    				expressionPrimary = BubbleUpParseExpression(i, expressionPrimary, compilationUnit, ref parserModel);
	    				break;
	    			}
	    		}
    		}
			
			// The while loop used to be 'while (!parserModel.TokenWalker.IsEof)'
			// This caused an issue where 'BubbleUpParseExpression(...)' would not run
			// if the end of file was reached.
			//
			// Given how this parser is written, adding 'SyntaxKind.EndOfFile' to 'parserModel.ExpressionList'
			// would follow the pattern of how 'SyntaxKind.StatementDelimiterToken' is written.
			//
			// But, the 'while (true)' loop makes me extremely uncomfortable.
			//
			// So I added '|| parserModel.TokenWalker.IsEof' here.
			//
			// If upon further inspection on way or the other is deemed safe then this redundancy can be removed.
			if (forceExit || parserModel.TokenWalker.IsEof) // delimiterExpressionTuple.ExpressionNode is null
			{
				expressionPrimary = BubbleUpParseExpression(0, expressionPrimary, compilationUnit, ref parserModel);
				break;
			}
			
    		expressionPrimary = parserModel.Binder.AnyMergeToken(expressionPrimary, ref tokenCurrent, compilationUnit, ref parserModel);
    		
    		/*#if DEBUG
    		Console.WriteLine($"\t=> {expressionPrimary.SyntaxKind}");
			#else
			Console.WriteLine($"{nameof(ParseExpression)} has debug 'Console.Write...' that needs commented out.");
    		#endif*/
    		
    		if (parserModel.TokenWalker.Index == indexToken)
    			_ = parserModel.TokenWalker.Consume();
    		if (parserModel.TokenWalker.Index < indexToken)
    			throw new LuthetusTextEditorException($"Infinite loop in {nameof(ParseExpression)}");
    		
    		indexToken = parserModel.TokenWalker.Index;
    		
    		if (parserModel.NoLongerRelevantExpressionNode is not null) // try finally is not needed to guarantee setting 'parserModel.NoLongerRelevantExpressionNode = null;' because this is an object reference comparison 'Object.ReferenceEquals'. Versus something more general that would break future parses if not properly cleared, like a SyntaxKind.
			{
				parserModel.Binder.ClearFromExpressionList(parserModel.NoLongerRelevantExpressionNode, compilationUnit, ref parserModel);
				parserModel.NoLongerRelevantExpressionNode = null;
			}
    		
    		if (parserModel.TryParseExpressionSyntaxKindList.Count != 0)
    		{
    			var isExpressionRoot = true;
    			var rootSyntaxKind = SyntaxKind.EmptyExpressionNode;
    			
    			foreach (var tuple in parserModel.ExpressionList)
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
    				success = parserModel.TryParseExpressionSyntaxKindList.Contains(expressionPrimary.SyntaxKind);
    				
    				if (success)
    				{
    					expressionPrimaryPreviousRoot = expressionPrimary;
    					indexTokenRoot = parserModel.TokenWalker.Index;
    				}
    			}
    			else
    			{
    				success = parserModel.TryParseExpressionSyntaxKindList.Contains(rootSyntaxKind);
    			}
    			
    			if (!success)
    			{
    				var distance = parserModel.TokenWalker.Index - indexTokenRoot;
		    		
	    			for (int i = 0; i < distance; i++)
	    			{
	    				_ = parserModel.TokenWalker.Backtrack();
	    			}
	    			
	    			expressionPrimary = expressionPrimaryPreviousRoot;
	    			
		    		forceExit = true;
		    		
		    		/*#if DEBUG
		    		WriteExpressionList(parserModel.ExpressionList);
		    		Console.WriteLine("----TryParseExpressionSyntaxKindList");
					#else
					Console.WriteLine($"{nameof(ParseExpression)} has debug 'Console.Write...' that needs commented out.");
		    		#endif*/
    			}
    		}
    		
    		if (forceExit) // parserModel.ForceParseExpressionSyntaxKind
				break;
        }
    	
    	// It is vital that this 'clear' and 'add' are done in a way that permits an invoker of the 'ParseExpression' method to 'add' a similar 'forceExit' delimiter
    	// 	Example: 'parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));'
    	parserModel.ExpressionList.Clear();
    	parserModel.ExpressionList.Add((SyntaxKind.EndOfFileToken, null));
    	parserModel.ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
    	
    	if (expressionPrimary.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		expressionPrimary = parserModel.Binder.ForceDecisionAmbiguousIdentifier(
				EmptyExpressionNode.Empty,
				(AmbiguousIdentifierExpressionNode)expressionPrimary,
				compilationUnit,
				ref parserModel);
    	}
    	
    	/*#if DEBUG
    	Console.WriteLine();
		#else
		Console.WriteLine($"{nameof(ParseExpression)} has debug 'Console.Write...' that needs commented out.");
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
    private static IExpressionNode BubbleUpParseExpression(int indexTriggered, IExpressionNode expressionPrimary, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var triggeredDelimiterTuple = parserModel.ExpressionList[indexTriggered];
    	IExpressionNode? previousDelimiterExpressionNode = null;
    	
    	var initialExpressionListCount = parserModel.ExpressionList.Count;
    	
    	/*#if DEBUG
    	var nullNodeSyntaxKindText = "null";
		Console.WriteLine($"BREAK_({triggeredDelimiterTuple.DelimiterSyntaxKind}, {triggeredDelimiterTuple.ExpressionNode?.SyntaxKind.ToString() ?? nullNodeSyntaxKindText})");
		#endif*/
		
		for (int i = initialExpressionListCount - 1; i > indexTriggered - 1; i--)
		{
			var delimiterExpressionTuple = parserModel.ExpressionList[i];
			parserModel.ExpressionList.RemoveAt(i);
			
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
			
			expressionPrimary = parserModel.Binder.AnyMergeExpression(
				delimiterExpressionTuple.ExpressionNode,
				expressionPrimary, // expressionSecondary
				compilationUnit,
				ref parserModel);
		}
		
		if (parserModel.NoLongerRelevantExpressionNode is not null) // try finally is not needed to guarantee setting 'parserModel.NoLongerRelevantExpressionNode = null;' because this is an object reference comparison 'Object.ReferenceEquals'. Versus something more general that would break future parses if not properly cleared, like a SyntaxKind.
		{
			parserModel.Binder.ClearFromExpressionList(parserModel.NoLongerRelevantExpressionNode, compilationUnit, ref parserModel);
			parserModel.NoLongerRelevantExpressionNode = null;
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