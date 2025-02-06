using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
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
        var diagnosticBag = new DiagnosticBag();

        var parserModel = new CSharpParserModel(
            new TokenWalker(lexerOutput.SyntaxTokenList, diagnosticBag),
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
                    ParseTokens.ParsePreprocessorDirectiveToken(token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.IdentifierToken:
                	ParseTokens.ParseIdentifierToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenBraceToken:
                {
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
                case SyntaxKind.CloseParenthesisToken:
                    ParseTokens.ParseCloseParenthesisToken(token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                	if (parserModel.StatementBuilder.ChildList.Count == 0)
                		ParseOthers.StartStatement_Expression(compilationUnit, ref parserModel);
                	else
                    	ParseTokens.ParseOpenAngleBracketToken(token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseTokens.ParseCloseAngleBracketToken(token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseTokens.ParseCloseSquareBracketToken(token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.ColonToken:
                    ParseTokens.ParseColonToken(compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.MemberAccessToken:
                    ParseTokens.ParseMemberAccessToken(token, compilationUnit, ref parserModel);
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken(compilationUnit, ref parserModel);
                    break;
                // TODO: SyntaxKind.EqualsCloseAngleBracketToken
                case SyntaxKind.StatementDelimiterToken:
                {
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
            compilationUnit.Binder.CloseScope(parserModel.TokenWalker.Current.TextSpan, compilationUnit, ref parserModel);
        }
		
        var topLevelStatementsCodeBlock = parserModel.CurrentCodeBlockBuilder.Build(
            parserModel.DiagnosticBag.ToArray()
                .Union(compilationUnit.Binder.DiagnosticsList)
                .Union(lexerOutput.DiagnosticBag.ToList())
                .ToArray());
                
        globalCodeBlockNode.SetCodeBlockNode(
        	topLevelStatementsCodeBlock,
        	parserModel.DiagnosticBag,
        	parserModel.TokenWalker);
                
		compilationUnit.RootCodeBlockNode = globalCodeBlockNode;
		compilationUnit.Binder.FinalizeBinderSession(compilationUnit.BinderSession);
    }
}
