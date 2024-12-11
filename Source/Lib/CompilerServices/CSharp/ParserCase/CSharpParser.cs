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

public struct CSharpParser : IParser
{
    public CSharpParser(CSharpLexer lexer)
    {
    	++LuthetusDebugSomething.Parser_ConstructorInvocationCount;
    
        Lexer = lexer;
    }

    public TextEditorDiagnostic[] DiagnosticsList { get; private set; } = Array.Empty<TextEditorDiagnostic>();
    public CSharpBinder Binder { get; private set; }
    public CSharpBinderSession BinderSession { get; private set; }
    public CSharpLexer Lexer { get; }

    IBinder IParser.Binder => Binder;
    IBinderSession IParser.BinderSession => BinderSession;
    ILexer IParser.Lexer => Lexer;

    public CompilationUnit Parse(
    	IBinder binder,
        ResourceUri resourceUri)
    {
    	Binder = (CSharpBinder)binder;
        BinderSession = (CSharpBinderSession)Binder.StartBinderSession(resourceUri);
    
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
            currentCodeBlockBuilder);
            
		#if DEBUG
		model.TokenWalker.ProtectedTokenSyntaxKindList = new() { SyntaxKind.StatementDelimiterToken, SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken, };
		#endif
        
        while (true)
        {
        	// The last statement in this while loop is conditionally: '_ = model.TokenWalker.Consume();'.
        	// Knowing this to be the case is extremely important.
            var token = model.TokenWalker.Current;
            
			#if DEBUG
			Console.WriteLine(token.SyntaxKind + "___" + token.TextSpan.GetText() + "___" + model.TokenWalker.Index);
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
                	if (model.StatementBuilder.ChildList.Count == 0)
                	{
                		ParseOthers.StartStatement_Expression(model);
                	}
                	else
                	{
                		var expressionNode = ParseOthers.ParseExpression(model);
                		model.StatementBuilder.ChildList.Add(expressionNode);
                	}
                	break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParseTokens.ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)token, model);
                    break;
                case SyntaxKind.IdentifierToken:
                	ParseTokens.ParseIdentifierToken(model);
                    break;
                case SyntaxKind.OpenBraceToken:
                	model.StatementBuilder.FinishStatement(model);
					
					#if DEBUG
					model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var openBraceToken = (OpenBraceToken)model.TokenWalker.Consume();
					
					#if DEBUG
					model.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseOpenBraceToken(openBraceToken, model);
                    break;
                case SyntaxKind.CloseBraceToken:
                	model.StatementBuilder.FinishStatement(model);
					
					#if DEBUG
					model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var closeBraceToken = (CloseBraceToken)model.TokenWalker.Consume();
					
					#if DEBUG
					model.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseCloseBraceToken(closeBraceToken, model);
                    break;
                case SyntaxKind.OpenParenthesisToken:
                	if (model.StatementBuilder.ChildList.Count == 0)
                		ParseOthers.StartStatement_Expression(model);
                	else
                		ParseTokens.ParseOpenParenthesisToken(model);
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    ParseTokens.ParseCloseParenthesisToken((CloseParenthesisToken)token, model);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                	if (model.StatementBuilder.ChildList.Count == 0)
                		ParseOthers.StartStatement_Expression(model);
                	else
                    	ParseTokens.ParseOpenAngleBracketToken((OpenAngleBracketToken)token, model);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseTokens.ParseCloseAngleBracketToken((CloseAngleBracketToken)token, model);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken(model);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseTokens.ParseCloseSquareBracketToken((CloseSquareBracketToken)token, model);
                    break;
                case SyntaxKind.ColonToken:
                    ParseTokens.ParseColonToken(model);
                    break;
                case SyntaxKind.MemberAccessToken:
                    ParseTokens.ParseMemberAccessToken((MemberAccessToken)token, model);
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken(model);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                	model.StatementBuilder.FinishStatement(model);
					
					#if DEBUG
					model.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
					#endif
					
					var statementDelimiterToken = (StatementDelimiterToken)model.TokenWalker.Consume();
					
					#if DEBUG
					model.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
					#endif
					
                    ParseTokens.ParseStatementDelimiterToken(statementDelimiterToken, model);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (model.SyntaxStack.TryPeek(out var syntax) &&
                        syntax is EndOfFileToken)
                    {
                        _ = model.SyntaxStack.Pop();
                    }

                    if (model.SyntaxStack.TryPop(out var notUsedSyntax))
                    {
                    	if (notUsedSyntax is null)
                    	{
                    	}
                        else if (notUsedSyntax is IExpressionNode)
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
                    else if (model.StatementBuilder.ChildList.Any())
                    {
                    	foreach (var item in model.StatementBuilder.ChildList)
                    	{
                    		model.CurrentCodeBlockBuilder.ChildList.Add(item);
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
				if (model.CurrentCodeBlockBuilder.ParseChildScopeQueue.TryDequeue(out var deferredChildScope))
					deferredChildScope.PrepareMainParserLoop(model.TokenWalker.Index, model);
				else
					break;
			}
			
			if (model.TokenWalker.ConsumeCounter == 0)
			{
				// This means either:
				// 	- None of the methods for syntax could make sense of the token, so they didn't consume it.
				// 	- For whatever reason the method that handled the syntax made sense of the token, but never consumed it.
				// 	- The token was consumed, then for some reason a backtrack occurred.
				//
				// To avoid an infinite loop, this will ensure at least 1 token is consumed each iteration of the while loop.
				// 
				// (and that the token index increased by at least 1 from the previous loop; this is implicitly what is implied).
				_ = model.TokenWalker.Consume();
			}
			else if (model.TokenWalker.ConsumeCounter < 0)
			{
				// This means that a syntax invoked 'model.TokenWalker.Backtrack()'.
				// Without invoking an equal amount of 'model.TokenWalker.Consume()' to avoid an infinite loop.
				throw new LuthetusTextEditorException($"model.TokenWalker.ConsumeCounter:{model.TokenWalker.ConsumeCounter} < 0");
			}
			
			model.TokenWalker.ConsumeCounterReset();
        }

        if (model.CurrentCodeBlockBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            Binder.CloseScope(model.TokenWalker.Current.TextSpan, model);
        }

        DiagnosticsList = model.DiagnosticBag.ToArray();

        var topLevelStatementsCodeBlock = model.CurrentCodeBlockBuilder.Build(
            DiagnosticsList
                .Union(Binder.DiagnosticsList)
                .Union(Lexer.DiagnosticList)
                .ToArray());

		Binder.FinalizeBinderSession(BinderSession);
        return new CompilationUnit(
            topLevelStatementsCodeBlock,
            Lexer,
            this,
            Binder);
    }
}

// TODO: Are the comments below still of value? If they are not of value anymore then delete them.

//{
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
//}

//{
	// Goal: re-write the parser a bit (2024-11-07)
	// ============================================
	// It took 8 seconds for the Luthetus.Ide.sln to parse after turning off much of the syntax.
	// 	(i.e.: the lexer ran entirely, but the parser was just iterating the while loop without doing much of anything).
	//     (upon further inspection the parser's while loop still is doing a bit more than I thought,
	// 		but still the baseline time of 8 seconds can be compared with the time after I make a change.)
	//
	// With the syntax on it was taking 1 minute 13 seconds.
	//
	// Task Manager after the solution was parsed on Windows using a published version of the code.
	// --------------------------------------------------------------------------------------------
	// Apps:
	// 	'.NET Host' 278 MB
	// Background processes
	// 	'.NET Host' 492 MB
	// 	'.NET Host' 265 MB
	//
	// After about 5 minutes the Task Manager looks like this (5 minutes is not at all an accurate timespan, but it "felt" like 5 minutes had passed)
	// -----------------------------------------------------------------------------------------------------------------------------------------------
	// Apps:
	// 	'.NET Host' 370 MB
	// Background processes
	// 	(these are gone now)
	//
	// So, now I can move back from git history each syntax one by one and see the impact of adding that specific syntax back.
	//
	// Most pressing is:
	// - Determining whether syntax is an 'expression' or 'statement'.
	// - As well the handling of 'ambiguous syntax'.
	//
	// Suspicions:
	// - Anytime a 'statement' and a 'expression' are ambiguous,
	// 	then there will be a keyword to disambiguate.
	// - If I add back the 'expression' code first,
	// 	then the statements will be far clearer
	// 	because I hadn't implemented the 'expression' code
	// 	with any "complexity" of whether it actually is a statement.
	// 	I'll have a very "simple" version of the parser to look at,
	// 	and it is thought that the statement disambiguation will be obvious.
	// - I don't think I can rely on the surrounding code block's "kind"
	// 	and whether that code block allows a certain syntax.
	//   	- The programmer can type any syntax they want, and do so anywhere,
	//       	it just might not compile.
	//       	But, the parser still has to "survive" the ordeal.
	//
	// I added that if an IdentifierToken is found by the main loop,
	// and the 'StatementBuilderStack.Count == 0',
	// that ParseExpression(...) would be invoked.
	//
	// It went from 8 seconds for entire .sln to 26 seconds.
	// As well a lot of syntax highlighting began to appear,
	// albeit seemingly incorrect because definitions aren't being parsed
	// so it cannot tell whether an identifier is a reference to a type or a local, or etc...
	//
	// A confusion I remember having was with (I think it was) function definitions.
	// And their return type. Something to do with it being an ambiguous identifier
	// while you are stepping through the nodes, and you have to wait to decide what it was.
	//
	// I sort of rambled when I said that but I am fairly certain that two ideas can be relied on:
	// - If you are in an expression, you can look at syntax through the lens of an expression
	// 	until you are given syntax that brings you back to the possibiity of statements.
	// 	A lambda statement is confusing. Because it is an expression that wants to
	//     return to the statement loop, yet its part of a larger expression
	// 	and being handled by the expression loop.
	// - There have got to be some keywords that when you come across them,
	// 	it is immediately apparent that you aren't looking at an expression.
	// 	Or at the least it could be a path to go down that leads to disambiguation.
	//
	// Function definition
	// ===================
	// ````int Aaa() => 3;
	// ````
	// ````int Bbb()
	// ````{
	// ````	return 3;
	// ````}
	// 
	// Variable declaration
	// ====================
	// ````var x = 2;
	// ````int y = 3;
	// 
	// Lambda expression
	// =================
	// ````// Is this syntax even legal?
	// ````int x => 3;
	// ````
	// ````(int x) => 3;
	//
	// Comma Separated Expression
	// ==========================
	// ````// lambda expression
	// ````(x, y) => 2;
	// ````(int x, Person y) => 2;
	// ````// tuple expression
	// ````(int X, Person Y) tuple = (3, new Person());
	//
	// Explicit Cast Expression
	// ==========================
	// ````(Person)someVariable;
	// 
	// Parenthesized Expression
	// ==========================
	// ````(Person)someVariable;
	//
	// Constructor Definition
	// ======================
	// ````public class Person
	// ````{
	// ````	public Person()
	// ````	{
	// ````	}
	// ````}
	// 
	// Constructor Invocation
	// ======================
	// ````new Person();
	// ````// object initialization
	// ````new Person();
	// ````new Person { };
	// ````new Person { FirstName = "John", ... };
	// ````new Person(id: Guid.NewGuid) { FirstName = "John", ... };
	// ````// collection initialization
	// ````new List<int> { 1, 2, 3, };
	// ````new List<int>(capacity: 5) { 1, 2, 3, };
	//
	// Class definition
	// ================
	// ````class Aaa { }
	// ````class Bbb(/*primary constructor*/) { }
	//
	// I want to write out in order the definitions/expressions, ordered by how similar the syntax is to one another.
	// 
	// Types are a "simple" case due to the storage modifier keyword 'class, interface, ...'.
	//
	// It went from 26 seconds to 28 seconds
	// after adding TypeDefinitionNode parsing.
	//
	// 
//}

//{
	// I think the key to parsing this properly is having the input "Person" parse properly.
	//
	// An expression can contain a 'TypeClauseNode', so it ought to be an IExpressionNode.
	// An expression can contain a 'TypeClauseNode IdentifierOrContextualToken', so it ought to be an IExpressionNode.
	//
	// If I open Visual Studio and I type "Person" into a C# file that is completely empty other than that text.
	// will it be treated as a type or a variable or something else?
	//
	// Because it seems that there is a pattern of either 'TypeClause' or 'TypeClauseNode IdentifierOrContextualToken'.
	//
	// And, the 'TypeClauseNode' could be made up of one or more ISyntaxToken.
	// 
	// It almost feels like you'd want to parse for 'TypeClause'
	// and 'TypeClauseNode IdentifierOrContextualToken'.
	// But in the sense of they both get the same starting point in the TokenWalker.
	// All the state is separate.
	//
	// Probably a 'bool TryParse_TypeClause_IdentifierOrContextualToken(out var aaa)'
	// and 'bool TryParse_TypeClause(out var aaa)'.
	//
	// You'd maybe invoke 'TryParse_TypeClause_IdentifierOrContextualToken' first,
	// then reset the TokenWalker state back to what it was and invoke
	// TryParse_TypeClause' to see that returns true/successful.
	//
	// Visual Studio treats the text "Person" when it is the only text in a C# file to be:
	// "The type or namespace name 'Person' could not be found (are you missing a using directive or an assembly reference?)
	//
	// If I open the demo website and put the same text into an empty C# file what is it treated as?
	// With the 'v 0.9.7.1 :: Release :: Luthetus.Ide.RazorLib' demo it is a Type for the syntax highlighting
	// and has a similar error diagnostic with "type or namespace could not be found" wording.
	//
	// Visual Studio was different in that it for syntax highlighting "did not give the text a color - it was just colored as default text".
	// Whereas the demo website colored the text as a Type.
	//
	// I put the Visual Studio part about default text color in quotes because I believe that is also the color they use for namespaces (default text and namespaces are the same color).
	//
	// I changed the color in Visual Studio that is assigned to namespaces to be different from the default text.
	// The color of the text did not change, therefore it was being colored as default text it seems.
	
	// I am writing the C# Parser statement first but it might be better to do things expression first.
	
	// Idea: "Everything is an expression unless it is a statement"?
	//
	// This might just be equivalent to saying "do things expression first".
	// But for some reason this wording is sticking out in my mind.
	
	// But, some expressions can be a statement, so being a statement does not stop something from being an expression.
//}
