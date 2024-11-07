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

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpParser : IParser
{
    public CSharpParser(CSharpLexer lexer)
    {
        Lexer = lexer;
        Binder = new CSharpBinder();
        BinderSession = (CSharpBinderSession)Binder.StartBinderSession(lexer.ResourceUri);
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; private set; } = ImmutableArray<TextEditorDiagnostic>.Empty;
    public CSharpBinder Binder { get; private set; }
    public CSharpBinderSession BinderSession { get; private set; }
    public CSharpLexer Lexer { get; }

    IBinder IParser.Binder => Binder;
    IBinderSession IParser.BinderSession => BinderSession;
    ILexer IParser.Lexer => Lexer;

    /// <summary>This method is used when parsing many files as a single compilation. The first binder instance would be passed to the following parsers. The resourceUri is passed in so if a file is parsed for a second time, the previous symbols can be deleted so they do not duplicate.</summary>
    public CompilationUnit Parse(
        IBinder previousBinder,
        ResourceUri resourceUri)
    {
        Binder = (CSharpBinder)previousBinder;
        BinderSession = (CSharpBinderSession)Binder.StartBinderSession(resourceUri);
        return Parse();
    }

    public CompilationUnit Parse()
    {
        var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
        var currentCodeBlockBuilder = globalCodeBlockBuilder;
        var diagnosticBag = new DiagnosticBag();

        var model = new CSharpParserModel(
            Binder,
            BinderSession,
            new TokenWalker(Lexer.SyntaxTokenList, diagnosticBag),
            new Stack<ISyntax>(),
            diagnosticBag,
            globalCodeBlockBuilder,
            currentCodeBlockBuilder,
            null,
            new Stack<Action<CodeBlockNode>>());
            
        #if DEBUG
        model.TokenWalker.ProtectedTokenSyntaxKindList = new List<SyntaxKind>
        {
        	SyntaxKind.StatementDelimiterToken,
        	SyntaxKind.OpenBraceToken,
        	SyntaxKind.CloseBraceToken,
        };
        #endif
            
        while (true)
        {
        	// The last statement in this while loop is conditionally: '_ = model.TokenWalker.Consume();'.
        	// Knowing this to be the case is extremely important.
            var token = model.TokenWalker.Current;
            
            #if DEBUG
            Console.WriteLine("___" + token.SyntaxKind + "___" + token.TextSpan.GetText());
            #endif

            switch (token.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.CharLiteralToken:
				case SyntaxKind.StringLiteralToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.StarToken:
                case SyntaxKind.DollarSignToken:
                case SyntaxKind.AtToken:
                	if (model.StatementBuilderStack.Count == 0)
                	{
                		ParseOthers.StartStatement_Expression(model);
                	}
                	else
                	{
                		var expressionNode = ParseOthers.ParseExpression(model);
                		model.StatementBuilderStack.Push(expressionNode);
                	}
                	break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParseTokens.ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)token, model);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseTokens.ParseIdentifierToken(model);
                    break;
                case SyntaxKind.OpenBraceToken:
                	model.StatementBuilderStack.Clear();
                    ParseTokens.ParseOpenBraceToken((OpenBraceToken)token, model);
                    break;
                case SyntaxKind.CloseBraceToken:
                	model.StatementBuilderStack.Clear();
                    ParseTokens.ParseCloseBraceToken((CloseBraceToken)token, model);
                    break;
                case SyntaxKind.OpenParenthesisToken:
                    ParseTokens.ParseOpenParenthesisToken((OpenParenthesisToken)token, model);
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    ParseTokens.ParseCloseParenthesisToken((CloseParenthesisToken)token, model);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                    ParseTokens.ParseOpenAngleBracketToken((OpenAngleBracketToken)token, model);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseTokens.ParseCloseAngleBracketToken((CloseAngleBracketToken)token, model);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken((OpenSquareBracketToken)token, model);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseTokens.ParseCloseSquareBracketToken((CloseSquareBracketToken)token, model);
                    break;
                case SyntaxKind.ColonToken:
                    ParseTokens.ParseColonToken((ColonToken)token, model);
                    break;
                case SyntaxKind.MemberAccessToken:
                    ParseTokens.ParseMemberAccessToken((MemberAccessToken)token, model);
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken((EqualsToken)token, model);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                	model.StatementBuilderStack.Clear();
                    ParseTokens.ParseStatementDelimiterToken((StatementDelimiterToken)token, model);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (model.SyntaxStack.TryPeek(out var syntax) &&
                        syntax is EndOfFileToken)
                    {
                        _ = model.SyntaxStack.Pop();
                    }

                    if (model.SyntaxStack.TryPop(out var notUsedSyntax))
                    {
                        if (notUsedSyntax is IExpressionNode)
                        {
                            model.CurrentCodeBlockBuilder.ChildList.Add(notUsedSyntax);
                        }
                        else if (notUsedSyntax.SyntaxKind == SyntaxKind.AmbiguousIdentifierNode)
                        {
                            var ambiguousIdentifierNode = (AmbiguousIdentifierNode)notUsedSyntax;
                            model.CurrentCodeBlockBuilder.ChildList.Add(notUsedSyntax);
                            model.DiagnosticBag.ReportUndefinedTypeOrNamespace(
                                ambiguousIdentifierNode.IdentifierToken.TextSpan,
                                ambiguousIdentifierNode.IdentifierToken.TextSpan.GetText());
                        }
                    }
                    break;
                default:
                    if (UtilityApi.IsContextualKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordContextualToken(model);
                    else if (UtilityApi.IsKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordToken(model);
                    break;
            }

            if (token.SyntaxKind == SyntaxKind.EndOfFileToken)
			{
				if (model.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var action))
				{
					// TODO: After the child scope is parsed the current code block builder
					//       needs to be restored.
					
					// TODO: Why would 'DequeueChildScopeCounter' be incremented after the fact (instead of before)?
					// Response: maybe it returns to the main while loop in CSharpParser.cs so it doesn't matter the order.
				
					action.Invoke(model.TokenWalker.Index - 1);
					model.CurrentCodeBlockBuilder.DequeueChildScopeCounter++;
				}
				else
				{
					break;
				}
			}
			
			// In regards to whether to '_ = model.TokenWalker.Consume();'
			// at the start of the while loop, or at the end:
			//
			// The end is better because:
			// 
			// - Anytime one wants to check the previous token,
			// with the 'Consume()' at the end of the loop then one can use 'model.TokenWalker.Previous'.
			// But, if 'Consume()' is at the start of the loop then one has to use 'model.TokenWalker.Peek(-2)' and that feels very janky.
			// 
			// - As well, helps ensure a "handle-syntax" does not 'Consume()' someone elses syntax.
			// If an individual "syntax" cannot make sense of the current token,
			// then it can just return to the main loop and the main loop will deal with it.
			//
			// On a different note:
			//
			// 'should return to the main loop'
			// - The end of every statement
			// - The start of a code block
			// - The end of a code block
			//
			// The goal with every iteration of the main loop,
			// is to narrow down the possible syntax that is occuring.
			// Then to invoke some other method that will handle that syntax
			// entirely, and then return to the main loop for the next syntax to start being parsed.
			//
			// For example, multiple consecutive keywords might be an ambiguous syntax.
			// Therefore, the main loop should push these keywords onto the 'StatementStack'.
			// Then, eventually a disambiguating token will be encountered.
			// At this point, the corresponding method to handle the syntax can be invoked,
			// and the method can pop off the 'StatementStack' to see the "related" tokens.
			//
			// There are two loops:
			// - Statement Loop
			// - Expression Loop
			//
			// Statement loop exists in CSharpParser.cs (this file).
			// Expression loop exists in ParseOthers.cs (a relative path from this file: './Internals/ParseOthers.cs').
			//
			// The parser should gracefully handle syntax that does not compile.
			// i.e.: if a syntactical error can be recovered from, then
			// the parser is expected to recover.
			//
			// For example, "(;"
			// For the sake of the example, this is presumed to start a ParenthesizedExpressionNode.
			// It goes on to see a StatementDelimiterToken before it sees a CloseParenthesisToken.
			// Instead of cascading syntactical errors to the text that comes after this (imagine this is part of a source code file)
			// the ParenthesizedExpressionNode should report a diagnostic error,
			// but then return back to the main loop.
			// The main loop will see a StatementDelimiterToken, and then a new statement will begin
			// as if nothing went wrong. This allows the rest of the text file to parse with the "same result".
			// 
			// The parsing is at times done 'breadth first'.
			// Because, a class definition might be:
			//
			// ````public class MyClass
			// ````{
			// ````	public MyClass(string firstName)
			// ````    {
			// ````		FirstName = firstName;
			// ````	}
			// ````
			// ````	public string FirstName { get; set; }
			// ````}
			//
			// In this scenario the 'FirstName' property is being referenced from within the class's constructor.
			// Yet, the constructor is defined above the property it references.
			//
			// Instead of parsing 'depth first', in this scenario the index of the token that
			// starts the code block body of the constructor definition is remembered,
			// then the parser skips until it finds the matching close brace
			// (by counting the open brace and close braces until it evens out).
			//
			// Once the class definition is finished being parsed, then the deferred parsing will start.
			// This will modify the 'TokenWalker' such that it now is pointing at the previously "remembered"
			// token that starts the code block of the constructor definition.
			//
			// As well, the 'TokenWalker' is told what index to return to once finished parsing the child scope.
			// In this case the 'TokenWalker' would return to the index that represents the 'CloseBraceToken'
			// of the class definition.
			// 
			// If there were many child scopes that needed to be 'defer parsed' then the same
			// code will be hit as was initially, and each time a child scope is 'defer parsed'
			// it is removed from the list of child scopes that need to be 'defer parsed'.
			//
			// Regarding StatementDelimiterToken, OpenBraceToken, and CloseBraceToken:
			// - These syntax hold a greater meaning than the "individual syntax".
			// For example: "1;".
			// In the example '1' is a NumericLiteralToken.
			// Furthermore, a NumericLiteralToken can be parsed as a 'LiteralExpressionNode'.
			// But, the ending StatementDelimiterToken does not belong to the 'LiteralExpressionNode'.
			// It is to be "provided" to the main statement loop, to be processed there.
			// 
			// In short, syntax shouldn't consume tokens that don't belong to them.
			// And it is being stated that 'StatementDelimiterToken, OpenBraceToken, and CloseBraceToken'
			// in general do not belong to a syntax.
			//
			// Some exceptions to this would be a property definition "public string FirstName { get; set; }".
			// Here, the OpenBraceToken and CloseBraceToken are not being used in the same sense
			// as they are when defining scope.
			// 
			// So, the property definition should consume those OpenBraceToken and CloseBraceToken.
			//
			// However, consider this example:
			//
			// ````public string FirstName
			// ````{
			// ````	get
			// ````	{
			// ````		return _firstName;
			// ````	}
			// ````	set => _firstName = value;
			// ````}
			//
			// Here, the property's "getter" is defining a code block.
			// So, the brace tokens that deliminate the "getter", ought to
			// "visit" the main loop for a moment. Then it can continue on with the property syntax.
			
			if (model.TokenWalker.ConsumeCounter == 0)
			{
				// This means none of the methods for syntax could make sense of the token.
				//
				// Note: StatementDelimiterToken, OpenBraceToken, and CloseBraceToken
				// 	  if used for scope delimination can sometimes end up here
				//       (most often due to these tokens being contiguous one after another).
				
				#if DEBUG
				model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
				#endif
				
				_ = model.TokenWalker.Consume();
				
				#if DEBUG
				model.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
				#endif
			}
			else if (model.TokenWalker.ConsumeCounter < 0)
			{
				// This means that a syntax invoked 'model.TokenWalker.Backtrack()'.
				// Without invoking an equal amount of 'model.TokenWalker.Consume()' to avoid an infinite loop.
				throw new LuthetusTextEditorException($"model.TokenWalker.ConsumeCounter:{model.TokenWalker.ConsumeCounter} < 0");
			}
			
			model.TokenWalker.ConsumeCounterReset();
        }

        if (model.FinalizeNamespaceFileScopeCodeBlockNodeAction is not null &&
            model.CurrentCodeBlockBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            Binder.DisposeScope(model.TokenWalker.Current.TextSpan, model);

            model.FinalizeNamespaceFileScopeCodeBlockNodeAction.Invoke(
                model.CurrentCodeBlockBuilder.Build());

            model.CurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder.Parent;
        }

        DiagnosticsList = DiagnosticsList.AddRange(model.DiagnosticBag.ToImmutableArray());

        var topLevelStatementsCodeBlock = model.CurrentCodeBlockBuilder.Build(
            DiagnosticsList
                .Union(Binder.DiagnosticsList)
                .Union(Lexer.DiagnosticList)
                .ToImmutableArray());

		Binder.FinalizeBinderSession(BinderSession);
        return new CompilationUnit(
            topLevelStatementsCodeBlock,
            Lexer,
            this,
            Binder);
    }
}