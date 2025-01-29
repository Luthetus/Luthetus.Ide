using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public static class CSharpParser
{
    public static void Parse(CSharpCompilationUnit compilationUnit)
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
        var diagnosticBag = new DiagnosticBag();

        var parserModel = new CSharpParserModel(
            new TokenWalker(compilationUnit.LexerOutput.SyntaxTokenList, compilationUnit.LexerOutput.TriviaTextSpanList, diagnosticBag),
            new Stack<ISyntax>(),
            diagnosticBag,
            globalCodeBlockBuilder,
            currentCodeBlockBuilder);
            
		#if DEBUG
		parserModel.TokenWalker.ProtectedTokenSyntaxKindList = new() { SyntaxKind.StatementDelimiterToken, SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken, };
		#endif
        
        while (true)
        {
        	// The last statement in this while loop is conditionally: '_ = parserModel.TokenWalker.Consume();'.
        	// Knowing this to be the case is extremely important.
            var token = parserModel.TokenWalker.Current;
            
			/*#if DEBUG
			Console.WriteLine(token.SyntaxKind + "___" + token.TextSpan.GetText() + "___" + parserModel.TokenWalker.Index);
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
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParseTokens.ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.IdentifierToken:
                	ParseTokens.ParseIdentifierToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenBraceToken:
                	parserModel.StatementBuilder.FinishStatement(compilationUnit, ref parserModel);
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var openBraceToken = (OpenBraceToken)parserModel.TokenWalker.Consume();
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseOpenBraceToken(openBraceToken, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.CloseBraceToken:
                	parserModel.StatementBuilder.FinishStatement(compilationUnit, ref parserModel);
					
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
					var closeBraceToken = (CloseBraceToken)parserModel.TokenWalker.Consume();
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseCloseBraceToken(closeBraceToken, closeBraceTokenIndex, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenParenthesisToken:
                	ParseTokens.ParseOpenParenthesisToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    ParseTokens.ParseCloseParenthesisToken((CloseParenthesisToken)token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                	if (parserModel.StatementBuilder.ChildList.Count == 0)
                		ParseOthers.StartStatement_Expression(compilationUnit, ref parserModel);
                	else
                    	ParseTokens.ParseOpenAngleBracketToken((OpenAngleBracketToken)token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseTokens.ParseCloseAngleBracketToken((CloseAngleBracketToken)token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseTokens.ParseCloseSquareBracketToken((CloseSquareBracketToken)token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.ColonToken:
                    ParseTokens.ParseColonToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.MemberAccessToken:
                    ParseTokens.ParseMemberAccessToken((MemberAccessToken)token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken(compilationUnit, ref parserModel);
                    break;
                // TODO: SyntaxKind.EqualsCloseAngleBracketToken
                case SyntaxKind.StatementDelimiterToken:
                	parserModel.StatementBuilder.FinishStatement(compilationUnit, ref parserModel);
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var statementDelimiterToken = (StatementDelimiterToken)parserModel.TokenWalker.Consume();
					
					#if DEBUG
					parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseStatementDelimiterToken(statementDelimiterToken, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (parserModel.SyntaxStack.TryPeek(out var syntax) &&
                        syntax is EndOfFileToken)
                    {
                        _ = parserModel.SyntaxStack.Pop();
                    }

                    if (parserModel.SyntaxStack.TryPop(out var notUsedSyntax))
                    {
                    	if (notUsedSyntax is null)
                    	{
                    	}
                        else if (notUsedSyntax is IExpressionNode)
                        {
                            parserModel.CurrentCodeBlockBuilder.ChildList.Add(notUsedSyntax);
                        }
                        else if (notUsedSyntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierNode)
                        {
                            var ambiguousIdentifierNode = (AmbiguousIdentifierNode)notUsedSyntax;
                            parserModel.CurrentCodeBlockBuilder.ChildList.Add(notUsedSyntax);
                            parserModel.DiagnosticBag.ReportUndefinedTypeOrNamespace(
                                ambiguousIdentifierNode.IdentifierToken.TextSpan,
                                ambiguousIdentifierNode.IdentifierToken.TextSpan.GetText());
                        }
                    }
                    else if (parserModel.StatementBuilder.ChildList.Any())
                    {
                    	foreach (var item in parserModel.StatementBuilder.ChildList)
                    	{
                    		parserModel.CurrentCodeBlockBuilder.ChildList.Add(item);
                    	}
                    }
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
				if (parserModel.CurrentCodeBlockBuilder.ParseChildScopeQueue is not null &&
					parserModel.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var deferredChildScope))
				{
					deferredChildScope.PrepareMainParserLoop(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
				}
				else
				{
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
            compilationUnit.Binder.CloseScope(parserModel.TokenWalker.Current.TextSpan, compilationUnit, ref parserModel);
        }
		
        var topLevelStatementsCodeBlock = parserModel.CurrentCodeBlockBuilder.Build(
            parserModel.DiagnosticBag.ToArray()
                .Union(compilationUnit.Binder.DiagnosticsList)
                .Union(compilationUnit.LexerOutput.DiagnosticBag.ToList())
                .ToArray());
                
        globalCodeBlockNode.SetCodeBlockNode(
        	topLevelStatementsCodeBlock,
        	parserModel.DiagnosticBag,
        	parserModel.TokenWalker);
                
		compilationUnit.RootCodeBlockNode = globalCodeBlockNode;
		compilationUnit.Binder.FinalizeBinderSession(compilationUnit.BinderSession);
    }
}
