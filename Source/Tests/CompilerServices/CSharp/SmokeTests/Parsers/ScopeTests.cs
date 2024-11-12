using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;

public class ScopeTests
{
	public class Test
	{
		public Test(string sourceText)
		{
			SourceText = sourceText;
			ResourceUri = new ResourceUri("./unitTesting.txt");
			Lexer = new CSharpLexer(ResourceUri, SourceText);
	        Lexer.Lex();
	        Parser = new CSharpParser(Lexer);
	        CompilationUnit = Parser.Parse();
		}
		
		public string SourceText { get; set; }
		public ResourceUri ResourceUri { get; set; }
		public CSharpLexer Lexer { get; set; }
		public CSharpParser Parser { get; set; }
		public CompilationUnit CompilationUnit { get; set; }
	}
	
	[Fact]
    public void GlobalScope()
    {
    	var test = new Test(@"");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_ArbitraryScope()
    {
    	var test = new Test(@"{}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_ArbitraryScope_ArbitraryScope()
    {
    	var test = new Test(@"{} {}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_ArbitraryScope_Depth_ArbitraryScope_ArbitraryScope()
    {
    	var test = new Test(@"{ {} {} }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode()
    {
    	var test = new Test(@"public class Person { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_ConstructorDefinitionNode()
    {
    	var test = new Test(@"public class Person { public Person() { } }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNode()
    {
    	var test = new Test(@"public class Person { public string FirstName { get; set; } }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNodeWithAttribute()
    {
    	var test = new Test(
@"public class Person
{
	[Parameter, EditorRequired]
	public string FirstName { get; set; }
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNodeWithAttributeThatInvokesConstructor()
    {
    	var test = new Test(
@"public class Person
{
	[Parameter(Name=""Aaa""), EditorRequired(3)]
	public string FirstName { get; set; }
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNodeGetterAndSetterCodeBlock()
    {
    	var test = new Test(
@"public class Person
{
	public string FirstName
	{
		get
		{
			return _firstName;
		}
		set
		{
			_firstName = value;
		}
	}
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_FunctionDefinitionNode()
    {
    	var test = new Test(
@"public class Person
{
	public void MyMethod()
	{
	}
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_ArbitraryCodeBlock()
    {
    	var test = new Test(
@"public class Person
{
	{
	}
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_FunctionDefinitionNode_Depth_ArbitraryCodeBlock()
    {
    	var test = new Test(
@"public void MyMethod()
{
	{
	}
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_PropertyDefinitionNode()
    {
    	var test = new Test(@"public string FirstName { get; set; }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_PropertyDefinitionNodeGetterAndSetterCodeBlock()
    {
    	var test = new Test(
@"public class Person
{
	public string FirstName
	{
		get
		{
			return _firstName;
		}
		set
		{
			_firstName = value;
		}
	}
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_RecordWith_ArbitraryScope()
    {
    	// The record 'with' keyword isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the record 'with' keyword syntax.
    
    	var test = new Test(
@"var x = new PersonRecord();

x = x with
{
	FirstName = ""John"",
	LastName = ""Doe"",
}

{}");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_ObjectInitialization_ArbitraryScope()
    {
    	// The object initialization syntax isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the object initialization syntax.
    
    	var test = new Test(
@"var x = new PersonRecord
{
	FirstName = ""John"",
	LastName = ""Doe"",
};

{}");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_CollectionInitialization_ArbitraryScope()
    {
    	// The collection initialization syntax isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the collection initialization syntax.
    
    	var test = new Test(
@"var x = new List<int>
{
	1,
	2,
};

{}");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
    
    [Fact]
    public void GlobalScope_CollectionInitializationFromArray_ArbitraryScope()
    {
    	// The collection initialization syntax isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the collection initialization syntax.
    
    	var test = new Test(
@"var x = new int[]
{
	1,
	2,
};

{}");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		throw new NotImplementedException();
    }
	
	private void WriteChildrenIndented(ISyntaxNode node, string name = "node")
    {
    	Console.WriteLine($"foreach (var child in {name}.GetChildList())");
		foreach (var child in node.GetChildList())
		{
			Console.WriteLine("\t" + child.SyntaxKind);
		}
		Console.WriteLine();
    }
    
    private void WriteChildrenIndentedRecursive(ISyntaxNode node, string name = "node", int indentation = 0)
    {
    	var indentationStringBuilder = new StringBuilder();
    	for (int i = 0; i < indentation; i++)
    		indentationStringBuilder.Append('\t');
    	
    	Console.WriteLine($"{indentationStringBuilder.ToString()}{node.SyntaxKind}");
    	
    	// For the child tokens
    	indentationStringBuilder.Append('\t');
    	var childIndentation = indentationStringBuilder.ToString();
    	
		foreach (var child in node.GetChildList())
		{
			if (child is ISyntaxNode syntaxNode)
			{
				WriteChildrenIndentedRecursive(syntaxNode, "node", indentation + 1);
			}
			else if (child is ISyntaxToken syntaxToken)
			{
				Console.WriteLine($"{childIndentation}{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
			}
		}
		
		if (indentation == 0)
			Console.WriteLine();
    }
}
