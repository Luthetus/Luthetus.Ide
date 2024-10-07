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

/*

Okay, the problem seems to be that the 'deferred child parsing' state is
tied to the CSharpParserModel, which is not able to handle the recursive state required.

i.e.: The current code seems to work for simple cases, but the 'deferred child parsing' state
needs to be moved to the code block builder so that it is isolated and isn't clobbered
by other scopes (it seems).

Unless in the past I came to the conclusion that one would only ever parse them in such a
way that it doesn't clobber somehow.

I feel like 2 scopes at the global level,
	then 2 scopes in each of those,
		then 2 scopes in each of those.
		
I think this would showcase whether the state clobbers itself.

@"
public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		{
			FirstName = firstName;
		}
		
		{
			LastName = lastName;
		}
	}
	
	public string DisplayName => $"{FirstName} {LastName}";
	
	public void MyMethod()
	{
		{
			Console.WriteLine(FirstName);
		}
		
		{
			Console.WriteLine(LastName);
		}
	}
	
	public string FirstName { get; set; }
}

public class ClassTwo
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		{
			FirstName = firstName;
		}
		
		{
			LastName = lastName;
		}
	}
	
	public string DisplayName => $"{FirstName} {LastName}";
	
	public void MyMethod()
	{
		{
			Console.WriteLine(FirstName);
		}
		
		{
			Console.WriteLine(LastName);
		}
	}
	
	public string FirstName { get; set; }
}
"
*/

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Aaa3()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		{
			FirstName = firstName;
		}
	}
	
	public string FirstName { get; set; }
}";

		/*
		The only two nodes with 'ScopeDirectionKind.Both' are:
			- TypeDefinitionNode
			- NamespaceStatementNode
		
		So, the problem that I'm trying to solve here is unrelated to clobbering.
		(The exact case here only has 1 item in the namespace scope,
		 then the 1 thing is a type definition but doesn't go on to contain another type definition).
		 
		 Yet, there is this exception:
		 =============================
		 Error Message:
		   System.ArgumentOutOfRangeException : Index must be within the bounds of the List. (Parameter 'index')
		  Stack Trace:
		     at System.Collections.Generic.List`1.Insert(Int32 index, T item)
		   at Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseTokens.<>c__DisplayClass21_4.<ParseOpenBraceToken>b__4(CodeBlockNode codeBlockNode) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/CompilerServices/CSharp/ParserCase/Internals/ParseTokens.cs:line 698
		   at Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseTokens.ParseCloseBraceToken(CloseBraceToken consumedCloseBraceToken, CSharpParserModel model) in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/CompilerServices/CSharp/ParserCase/Internals/ParseTokens.cs:line 763
		   at Luthetus.CompilerServices.CSharp.ParserCase.CSharpParser.Parse() in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Lib/CompilerServices/CSharp/ParserCase/CSharpParser.cs:line 112
		   at Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.DeferParseChildScopeTests.Aaa3() in /home/hunter/Repos/Luthetus.Ide_Fork/Source/Tests/CompilerServices/CSharp/Basis/ParserCase/Internals/DeferParseChildScopeTests.cs:line 274
		   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
		   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
		   
		   In regards to clobbering though, if I put two type definitions in a namespace statement,
		   then the 2nd type definition will think it should insert itself at index 0, because
		   the first type definition is only 'queued' so checking the namespace's child count returns index 0.
		   
		   This would not so much be clobbering though, the clobbering would be
		   again 2 type definitions in a namespace statement, but then inside the type definition,
		   have a inner type definition. This inner type definition "should" look at the queue
		   and think it needs to insert itself at index 1 of the first type definition within
		   the namespace statement, but in reality the queue is ocupied by a type definition
		   from the namespace statement that hasn't been dequeued yet.
		   
		   THIS TEST CASE IN PARTICULAR IS BREAKING BECAUSE:
		   Not all code block owners are defined yet.
		   The "arbitrary code block" where one just puts '{ }' wherever they please,
		   does not put a node on the stack.
		   
		   THEREFORE:
		   The most recent code block owner other than the "undefined" '{ }' would be
		   the constructor definition that encompasses it.
		   The '{ }' then goes on to mistake itself for that constructor defintion.
		   
		   FIX:
		   Support '{ }' 'foreach' 'do-while' 'while' 'for' etc...
		*/

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Aaa4()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"
public class ClassOne { }

public class ClassTwo { }
";

		/*
		In regards to clobbering though, if I put two type definitions in a namespace statement,
		   then the 2nd type definition will think it should insert itself at index 0, because
		   the first type definition is only 'queued' so checking the namespace's child count returns index 0.
		*/

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNodeClassOne = (TypeDefinitionNode)topCodeBlock.ChildList[0];
        
        Console.WriteLine(typeDefinitionNodeClassOne.SyntaxKind);
        Console.WriteLine(typeDefinitionNodeClassOne.TypeIdentifierToken.TextSpan.GetText());
        
        foreach (var child in typeDefinitionNodeClassOne.CodeBlockNode.ChildList)
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        var typeDefinitionNodeClassTwo = (TypeDefinitionNode)topCodeBlock.ChildList[1];
        
        // The assertions will fail because ClassTwo will be inserted at index 0 of the child list
        Assert.Equal(typeDefinitionNodeClassOne.TypeIdentifierToken.TextSpan.GetText(), "ClassOne");
        Assert.Equal(typeDefinitionNodeClassTwo.TypeIdentifierToken.TextSpan.GetText(), "ClassTwo");
    }
    
    [Fact]
	public void Aaa5()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"
public class ClassOne
{
	public class ClassInner
	{
	}
}

public class ClassTwo { }
";

		/*
		   ...the clobbering would be
		   again 2 type definitions in a namespace statement, but then inside the type definition,
		   have an inner type definition. This inner type definition "should" look at the queue
		   and think it needs to insert itself at index 1 of the first type definition within
		   the namespace statement, but in reality the queue is ocupied by a type definition
		   from the namespace statement that hasn't been dequeued yet.
		   
		   The "should" in the previous paragraph refers to the fact that the code currently
		   does not even take into account the queue count when deciding the to-be index for insertion.
		*/

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        // This should throw an index out of bounds exception when the inner class
        // tries to insert itself at index 1 of the child list of the ClassOne,
        // meanwhile only index 0 is available.
    }
    
    [Fact]
	public void Aaa6()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"
namespace Luthetus.Common.RazorLib.Clipboards.Models;

public interface IClipboardService
{
    public Task<string> ReadClipboard();
    public Task SetClipboard(string value);
}

";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
    }
}
