using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public static class CSharpParser
{
	public static int ErrorCount { get; set; }
	public static int TotalAmbiguousIdentifierExpressionNodeFailCount { get; set; }

    public static void Parse(CSharpCompilationUnit compilationUnit, CSharpBinder binder, ref CSharpLexerOutput lexerOutput)
    {
    	var globalCodeBlockNode = new GlobalCodeBlockNode();
    	
    	var globalOpenCodeBlockTextSpan = new TextEditorTextSpan(
		    0,
		    1,
		    decorationByte: default,
		    compilationUnit.ResourceUri,
		    string.Empty,
		    string.Empty);
    	
		var globalCodeBlockBuilder = binder.NewScopeAndBuilderFromOwner_GlobalScope_Hack(
	    	globalCodeBlockNode,
	        globalOpenCodeBlockTextSpan,
	        compilationUnit);
        
        var currentCodeBlockBuilder = globalCodeBlockBuilder;

        var parserModel = new CSharpParserModel(
            binder,
	        lexerOutput.SyntaxTokenList,
	        globalCodeBlockBuilder,
	        currentCodeBlockBuilder,
	        0,
            binder.TopLevelNamespaceStatementNode);
            
		/*#if DEBUG
		parserModel.TokenWalker.ProtectedTokenSyntaxKindList = new() { SyntaxKind.StatementDelimiterToken, SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken, };
		#endif*/
		
		var loopCount = 0;
		
		// + 10 because a valid case where 'parserModel.TokenWalker.TokenList.Count + 1' was found
		// and adding an extra 9 of padding shouldn't matter to the CPU.
		// (I think the case referred to was 'public class Abc { }' but this is from memory alone).
		// 
        var loopLimit = parserModel.TokenWalker.TokenList.Count + 10;
        
        while (true)
        {
        	if (loopCount++ > loopLimit)
        	{
        		++ErrorCount;
        		
        		Console.WriteLine(
        			$"ErrorCount:{ErrorCount}; ResourceUri:{compilationUnit.ResourceUri.Value}; loopLimit:{loopLimit}; tokenCount:{lexerOutput.SyntaxTokenList.Count};");
        		break;
        	}

        	// The last statement in this while loop is conditionally: '_ = parserModel.TokenWalker.Consume();'.
        	// Knowing this to be the case is extremely important.
            var token = parserModel.TokenWalker.Current;
           
			/*#if DEBUG
			Console.WriteLine(token.SyntaxKind + "___" + token.TextSpan.GetText() + "___" + parserModel.TokenWalker.Index);
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
                	if (parserModel.StatementBuilder.ChildList.Count == 0)
                	{
                		ParseOthers.StartStatement_Expression(compilationUnit, ref parserModel);
                	}
                	else
                	{
                		var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
                		parserModel.StatementBuilder.ChildList.Add(expressionNode);
                	}
                	break;
                case SyntaxKind.IdentifierToken:
                	ParseTokens.ParseIdentifierToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenBraceToken:
                {
                	// TODO: This is being inlined within ParseTokens.ParseGetterOrSetter(...)...
                	// just to check whether this code running is a valid solution.
                	// If this is found to work, the inlined code should not stay there long term.
                
                	var deferredParsingOccurred = parserModel.StatementBuilder.FinishStatement(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
					if (deferredParsingOccurred)
						break;
						
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var openBraceToken = parserModel.TokenWalker.Consume();
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseOpenBraceToken(openBraceToken, compilationUnit, ref parserModel);
                    break;
                }
                case SyntaxKind.CloseBraceToken:
                {
                	var deferredParsingOccurred = parserModel.StatementBuilder.FinishStatement(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
					if (deferredParsingOccurred)
						break;
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					// When consuming a 'CloseBraceToken' it is possible for the
					// TokenWalker to change the 'Index' to a value that is
					// more than 1 larger than the current index.
					//
					// This is an issue because some code presumes that
					// 'parserModel.TokenWalker.Index - 1' will always give them
					// the index of the previous token.
					//
					// So, the ParseCloseBraceToken(...) method needs
					// to be passed the index that was consumed in order to
					// get the CloseBraceToken.
					var closeBraceTokenIndex = parserModel.TokenWalker.Index;
					
					if (parserModel.ParseChildScopeStack.Count > 0 &&
					    parserModel.ParseChildScopeStack.Peek().CodeBlockOwner == parserModel.CurrentCodeBlockBuilder.CodeBlockOwner)
					{
						parserModel.TokenWalker.SetNullDeferredParsingTuple();
					}
					
					var closeBraceToken = parserModel.TokenWalker.Consume();
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseCloseBraceToken(closeBraceToken, closeBraceTokenIndex, compilationUnit, ref parserModel);
                    break;
                }
                case SyntaxKind.OpenParenthesisToken:
                	ParseTokens.ParseOpenParenthesisToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                	if (parserModel.StatementBuilder.ChildList.Count == 0)
                		ParseOthers.StartStatement_Expression(compilationUnit, ref parserModel);
                	else
                    	_ = parserModel.TokenWalker.Consume();
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                case SyntaxKind.CloseParenthesisToken:
                case SyntaxKind.CloseAngleBracketToken:
                case SyntaxKind.CloseSquareBracketToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.MemberAccessToken:
                    _ = parserModel.TokenWalker.Consume();
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.EqualsCloseAngleBracketToken:
                {
                	_ = parserModel.TokenWalker.Consume(); // Consume 'EqualsCloseAngleBracketToken'
                	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
	        		parserModel.CurrentCodeBlockBuilder.AddChild(expressionNode);
                	break;
            	}
                case SyntaxKind.StatementDelimiterToken:
                {
                	// TODO: This is being inlined within ParseTokens.ParseGetterOrSetter(...)...
                	// just to check whether this code running is a valid solution.
                	// If this is found to work, the inlined code should not stay there long term.

                	var deferredParsingOccurred = parserModel.StatementBuilder.FinishStatement(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
					if (deferredParsingOccurred)
						break;
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var statementDelimiterToken = parserModel.TokenWalker.Consume();
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseStatementDelimiterToken(statementDelimiterToken, compilationUnit, ref parserModel);
                    break;
                }
                case SyntaxKind.EndOfFileToken:
                    break;
                default:
                    if (UtilityApi.IsContextualKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordContextualToken(compilationUnit, ref parserModel);
                    else if (UtilityApi.IsKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordToken(compilationUnit, ref parserModel);
                    break;
            }

            if (token.SyntaxKind == SyntaxKind.EndOfFileToken)
			{
				bool deferredParsingOccurred = false;
				
				if (parserModel.ParseChildScopeStack.Count > 0)
				{
					var tuple = parserModel.ParseChildScopeStack.Peek();
					
					if (Object.ReferenceEquals(tuple.CodeBlockOwner, parserModel.CurrentCodeBlockBuilder.CodeBlockOwner))
					{
						tuple = parserModel.ParseChildScopeStack.Pop();
						tuple.DeferredChildScope.PrepareMainParserLoop(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
						deferredParsingOccurred = true;
					}
				}
				
				if (!deferredParsingOccurred)
				{
					// This second 'deferredParsingOccurred' is for any lambda expressions with one or many statements in its body.
					deferredParsingOccurred = parserModel.StatementBuilder.FinishStatement(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
					if (!deferredParsingOccurred)
						break;
				}
			}
			
			if (parserModel.TokenWalker.ConsumeCounter == 0)
			{
				// This means either:
				// 	- None of the methods for syntax could make sense of the token, so they didn't consume it.
				// 	- For whatever reason the method that handled the syntax made sense of the token, but never consumed it.
				// 	- The token was consumed, then for some reason a backtrack occurred.
				//
				// To avoid an infinite loop, this will ensure at least 1 token is consumed each iteration of the while loop.
				// 
				// (and that the token index increased by at least 1 from the previous loop; this is implicitly what is implied).
				_ = parserModel.TokenWalker.Consume();
			}
			else if (parserModel.TokenWalker.ConsumeCounter < 0)
			{
				// This means that a syntax invoked 'parserModel.TokenWalker.Backtrack()'.
				// Without invoking an equal amount of 'parserModel.TokenWalker.Consume()' to avoid an infinite loop.
				throw new LuthetusTextEditorException($"parserModel.TokenWalker.ConsumeCounter:{parserModel.TokenWalker.ConsumeCounter} < 0");
			}
			
			parserModel.TokenWalker.ConsumeCounterReset();
        }

        if (parserModel.CurrentCodeBlockBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            parserModel.Binder.CloseScope(parserModel.TokenWalker.Current.TextSpan, compilationUnit, ref parserModel);
        }
		
        var topLevelStatementsCodeBlock = parserModel.CurrentCodeBlockBuilder.Build();
                
        globalCodeBlockNode.SetCodeBlockNode(
        	topLevelStatementsCodeBlock,
        	compilationUnit.__DiagnosticList,
        	parserModel.TokenWalker);
                
		compilationUnit.RootCodeBlockNode = globalCodeBlockNode;
		
		/*if (parserModel.AmbiguousIdentifierExpressionNode.FailCount > 0)
		{
			++TotalAmbiguousIdentifierExpressionNodeFailCount;
			Console.WriteLine($"AmbiguousIdentifierExpressionNode !_wasDecided FailCount:{parserModel.AmbiguousIdentifierExpressionNode.FailCount} SuccessCount:{parserModel.AmbiguousIdentifierExpressionNode.SuccessCount} ResourceUri:{compilationUnit.ResourceUri.Value}; TotalAmbiguousIdentifierExpressionNodeFailCount:{TotalAmbiguousIdentifierExpressionNodeFailCount}");
		}*/
		
		parserModel.Binder.FinalizeCompilationUnit(compilationUnit);
	}
}
