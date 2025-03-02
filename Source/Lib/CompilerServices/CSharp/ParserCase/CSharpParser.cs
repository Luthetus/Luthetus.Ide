using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public static class CSharpParser
{
	public static int ErrorCount { get; set; }

    public static void Parse(CSharpCompilationUnit compilationUnit, ref CSharpLexerOutput lexerOutput)
    {
    	var globalCodeBlockNode = new GlobalCodeBlockNode();
    	
    	var globalOpenCodeBlockTextSpan = new TextEditorTextSpan(
		    0,
		    1,
		    decorationByte: default,
		    compilationUnit.ResourceUri,
		    string.Empty,
		    string.Empty);
    	
		var globalCodeBlockBuilder = compilationUnit.Binder.NewScopeAndBuilderFromOwner_GlobalScope_Hack(
	    	globalCodeBlockNode,
	        globalCodeBlockNode.GetReturnTypeClauseNode(),
	        globalOpenCodeBlockTextSpan,
	        compilationUnit);
        
        var currentCodeBlockBuilder = globalCodeBlockBuilder;

        var parserComputation = new CSharpParserComputation(
            new TokenWalker(lexerOutput.SyntaxTokenList),
            globalCodeBlockBuilder,
            currentCodeBlockBuilder);
            
		#if DEBUG
		parserComputation.TokenWalker.ProtectedTokenSyntaxKindList = new() { SyntaxKind.StatementDelimiterToken, SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken, };
		#endif
		
		var loopCount = 0;
		
		// + 10 because a valid case where 'parserComputation.TokenWalker.TokenList.Count + 1' was found
		// and adding an extra 9 of padding shouldn't matter to the CPU.
		// (I think the case referred to was 'public class Abc { }' but this is from memory alone).
		// 
        var loopLimit = parserComputation.TokenWalker.TokenList.Count + 10;
        
        while (true)
        {
        	if (loopCount++ > loopLimit)
        	{
        		++ErrorCount;
        		
        		Console.WriteLine(
        			$"ErrorCount:{ErrorCount}; ResourceUri:{compilationUnit.ResourceUri.Value}; loopLimit:{loopLimit}; tokenCount:{lexerOutput.SyntaxTokenList.Count};");
        		break;
        	}

        	// The last statement in this while loop is conditionally: '_ = parserComputation.TokenWalker.Consume();'.
        	// Knowing this to be the case is extremely important.
            var token = parserComputation.TokenWalker.Current;
           
			/*#if DEBUG
			Console.WriteLine(token.SyntaxKind + "___" + token.TextSpan.GetText() + "___" + parserComputation.TokenWalker.Index);
			#else
			Console.WriteLine($"{nameof(CSharpParser)}.{nameof(Parse)} has debug 'Console.Write...' that needs commented out.");
			#endif*/

            switch (token.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.CharLiteralToken:
				case SyntaxKind.StringLiteralToken:
				case SyntaxKind.StringInterpolatedStartToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.StarToken:
                case SyntaxKind.DollarSignToken:
                case SyntaxKind.AtToken:
                	if (parserComputation.StatementBuilder.ChildList.Count == 0)
                	{
                		ParseOthers.StartStatement_Expression(compilationUnit, ref parserComputation);
                	}
                	else
                	{
                		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
                		parserComputation.StatementBuilder.ChildList.Add(expressionNode);
                	}
                	break;
                case SyntaxKind.IdentifierToken:
                	ParseTokens.ParseIdentifierToken(compilationUnit, ref parserComputation);
                    break;
                case SyntaxKind.OpenBraceToken:
                {
                	var deferredParsingOccurred = parserComputation.StatementBuilder.FinishStatement(parserComputation.TokenWalker.Index, compilationUnit, ref parserComputation);
					if (deferredParsingOccurred)
						break;
						
					#if DEBUG
					parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var openBraceToken = parserComputation.TokenWalker.Consume();
					
					#if DEBUG
					parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseOpenBraceToken(openBraceToken, compilationUnit, ref parserComputation);
                    break;
                }
                case SyntaxKind.CloseBraceToken:
                {
                	var deferredParsingOccurred = parserComputation.StatementBuilder.FinishStatement(parserComputation.TokenWalker.Index, compilationUnit, ref parserComputation);
					if (deferredParsingOccurred)
						break;
					
					#if DEBUG
					parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					// When consuming a 'CloseBraceToken' it is possible for the
					// TokenWalker to change the 'Index' to a value that is
					// more than 1 larger than the current index.
					//
					// This is an issue because some code presumes that
					// 'parserComputation.TokenWalker.Index - 1' will always give them
					// the index of the previous token.
					//
					// So, the ParseCloseBraceToken(...) method needs
					// to be passed the index that was consumed in order to
					// get the CloseBraceToken.
					var closeBraceTokenIndex = parserComputation.TokenWalker.Index;
					
					if (parserComputation.ParseChildScopeStack.Count > 0 &&
					    parserComputation.ParseChildScopeStack.Peek().CodeBlockOwner == parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner)
					{
						parserComputation.TokenWalker.SetNullDeferredParsingTuple();
					}
					
					var closeBraceToken = parserComputation.TokenWalker.Consume();
					
					#if DEBUG
					parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseCloseBraceToken(closeBraceToken, closeBraceTokenIndex, compilationUnit, ref parserComputation);
                    break;
                }
                case SyntaxKind.OpenParenthesisToken:
                	ParseTokens.ParseOpenParenthesisToken(compilationUnit, ref parserComputation);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken(compilationUnit, ref parserComputation);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                	if (parserComputation.StatementBuilder.ChildList.Count == 0)
                		ParseOthers.StartStatement_Expression(compilationUnit, ref parserComputation);
                	else
                    	_ = parserComputation.TokenWalker.Consume();
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                case SyntaxKind.CloseParenthesisToken:
                case SyntaxKind.CloseAngleBracketToken:
                case SyntaxKind.CloseSquareBracketToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.MemberAccessToken:
                    _ = parserComputation.TokenWalker.Consume();
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken(compilationUnit, ref parserComputation);
                    break;
                // TODO: SyntaxKind.EqualsCloseAngleBracketToken
                case SyntaxKind.StatementDelimiterToken:
                {
                	var deferredParsingOccurred = parserComputation.StatementBuilder.FinishStatement(parserComputation.TokenWalker.Index, compilationUnit, ref parserComputation);
					if (deferredParsingOccurred)
						break;
					
					#if DEBUG
					parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var statementDelimiterToken = parserComputation.TokenWalker.Consume();
					
					#if DEBUG
					parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseStatementDelimiterToken(statementDelimiterToken, compilationUnit, ref parserComputation);
                    break;
                }
                case SyntaxKind.EndOfFileToken:
                    break;
                default:
                    if (UtilityApi.IsContextualKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordContextualToken(compilationUnit, ref parserComputation);
                    else if (UtilityApi.IsKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordToken(compilationUnit, ref parserComputation);
                    break;
            }

            if (token.SyntaxKind == SyntaxKind.EndOfFileToken)
			{
				bool deferredParsingOccurred = false;
				
				if (parserComputation.ParseChildScopeStack.Count > 0)
				{
					var tuple = parserComputation.ParseChildScopeStack.Peek();
					
					if (Object.ReferenceEquals(tuple.CodeBlockOwner, parserComputation.CurrentCodeBlockBuilder.CodeBlockOwner))
					{
						tuple = parserComputation.ParseChildScopeStack.Pop();
						tuple.DeferredChildScope.PrepareMainParserLoop(parserComputation.TokenWalker.Index, compilationUnit, ref parserComputation);
						deferredParsingOccurred = true;
					}
				}
				
				if (!deferredParsingOccurred)
				{
					// This second 'deferredParsingOccurred' is for any lambda expressions with one or many statements in its body.
					deferredParsingOccurred = parserComputation.StatementBuilder.FinishStatement(parserComputation.TokenWalker.Index, compilationUnit, ref parserComputation);
					if (!deferredParsingOccurred)
						break;
				}
			}
			
			if (parserComputation.TokenWalker.ConsumeCounter == 0)
			{
				// This means either:
				// 	- None of the methods for syntax could make sense of the token, so they didn't consume it.
				// 	- For whatever reason the method that handled the syntax made sense of the token, but never consumed it.
				// 	- The token was consumed, then for some reason a backtrack occurred.
				//
				// To avoid an infinite loop, this will ensure at least 1 token is consumed each iteration of the while loop.
				// 
				// (and that the token index increased by at least 1 from the previous loop; this is implicitly what is implied).
				_ = parserComputation.TokenWalker.Consume();
			}
			else if (parserComputation.TokenWalker.ConsumeCounter < 0)
			{
				// This means that a syntax invoked 'parserComputation.TokenWalker.Backtrack()'.
				// Without invoking an equal amount of 'parserComputation.TokenWalker.Consume()' to avoid an infinite loop.
				throw new LuthetusTextEditorException($"parserComputation.TokenWalker.ConsumeCounter:{parserComputation.TokenWalker.ConsumeCounter} < 0");
			}
			
			parserComputation.TokenWalker.ConsumeCounterReset();
        }

        if (parserComputation.CurrentCodeBlockBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            compilationUnit.Binder.CloseScope(parserComputation.TokenWalker.Current.TextSpan, compilationUnit, ref parserComputation);
        }
		
        var topLevelStatementsCodeBlock = parserComputation.CurrentCodeBlockBuilder.Build();
                
        globalCodeBlockNode.SetCodeBlockNode(
        	topLevelStatementsCodeBlock,
        	compilationUnit.__DiagnosticList,
        	parserComputation.TokenWalker);
                
		compilationUnit.RootCodeBlockNode = globalCodeBlockNode;
		compilationUnit.Binder.FinalizeBinderSession(compilationUnit);
	}
}
