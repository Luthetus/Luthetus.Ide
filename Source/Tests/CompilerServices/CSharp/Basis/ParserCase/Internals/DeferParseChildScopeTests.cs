using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;


/// <summary>
/// Goal Fix:
///
/// System.ArgumentOutOfRangeException : Index must be within the bounds of the List. (Parameter 'index')
///	Stack Trace:
///  	at System.Collections.Generic.List`1.Insert(Int32 index, T item)
///	at Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseTokens.<>c__DisplayClass21_4.<ParseOpenBraceToken>b__4(CodeBlockNode codeBlockNode) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/CompilerServices/CSharp/ParserCase/Internals/ParseTokens.cs:line 662
///	at Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseTokens.ParseCloseBraceToken(CloseBraceToken consumedCloseBraceToken, CSharpParserModel model) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/CompilerServices/CSharp/ParserCase/Internals/ParseTokens.cs:line 727
///	at Luthetus.CompilerServices.CSharp.ParserCase.CSharpParser.Parse() in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/CompilerServices/CSharp/ParserCase/CSharpParser.cs:line 102
///	at Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.OptionalArgumentsTests.Aaa_Smaller1() in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Tests/CompilerServices/CSharp/Basis/ParserCase/Internals/OptionalArgumentsTests.cs:line 142
///	at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
///	at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
///
/// The code already exists but it has an index out of bounds exception.
/// </summary>
public class DeferParseChildScopeTests
{
	/// <summary>
	/// It has been awhile since I wrote the code for this, so I need to first confirm that I
	/// understand what its doing, and that for the working cases everything goes as expected.
	/// </summary>
	[Fact]
	public void Aaa1()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
		// The idea of the defered parsing of child scopes, is to perform breadth first parsing.
		// In the following sourceText, 'FirstName' is declared after the constructor is defined.
		// But, the breadth first parsing allowed for 'FirstName' to be in scope for the constructor's body.
        var sourceText =
@"public class MyClass
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}
	
	public string FirstName { get; set; }
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Aaa2()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
		// The idea of the defered parsing of child scopes, is to perform breadth first parsing.
		// In the following sourceText, 'FirstName' is declared after the constructor is defined.
		// But, the breadth first parsing allowed for 'FirstName' to be in scope for the constructor's body.
        var sourceText =
@"public class MyClass
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}
	
	public MyClass(string firstName, bool trueOrFalse)
	{
		FirstName = firstName;
	}
	
	public string FirstName { get; set; }
}";

/*

 0: PublicTokenKeyword
 1: ClassTokenKeyword
 2: IdentifierToken           MyClass
 3: OpenBraceToken
 4: PublicTokenKeyword
 5: IdentifierToken           MyClass
 6: OpenParenthesisToken
 7: StringTokenKeyword
 8: IdentifierToken           firstName
 9: CloseParenthesisToken
10: OpenBraceToken
11: IdentifierToken           FirstName
12: EqualsToken
13: IdentifierToken           firstName
14: StatementDelimiterToken
15: CloseBraceToken
16: PublicTokenKeyword
17: IdentifierToken           MyClass
18: OpenParenthesisToken
19: StringTokenKeyword
20: IdentifierToken           firstName
21: CommaToken
22: BoolTokenKeyword
23: IdentifierToken           trueOrFalse
24: CloseParenthesisToken
25: OpenBraceToken
26: IdentifierToken           FirstName
27: EqualsToken
28: IdentifierToken           firstName
29: StatementDelimiterToken
30: CloseBraceToken
31: PublicTokenKeyword
32: StringTokenKeyword
33: IdentifierToken           FirstName
34: OpenBraceToken
35: GetTokenContextualKeyword 
36: StatementDelimiterToken
37: SetTokenContextualKeyword
38: StatementDelimiterToken
39: CloseBraceToken
40: CloseBraceToken
41: EndOfFileToken

*/

/*

// 0     1     2
public class MyClass
{//3
	// 4      5   6  7        8    9
	public MyClass(string firstName)
	{//10
		// 11    12     13  14
		FirstName = firstName;
	}//15
	
	// 16    17  18  19      20   21  22      23    24
	public MyClass(string firstName, bool trueOrFalse)
	{//25
		// 26    27    28   29
		FirstName = firstName;
	}//30
	
	// 31    32     33     34 35  36 37  38 39
	public string FirstName { get  ; set  ; }
}//40 EOF41

*/

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
    }
}
