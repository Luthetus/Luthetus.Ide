using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseBreadthFirst
{
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Console.WriteLine(typeDefinitionNode);
	}

	/// <summary>
	/// I need to enter the class definition, then see the 'MyClass' constructor BUT, do not enter the constructor scope.
	/// Instead continue at the class definition scope Thereby finding the FirstName property,
	/// and somehow "remember" to go back and inside the constructor scope.
	/// </summary>
	[Fact]
	public void One_ClassDefinitions()
	{
		var resourceUri = new ResourceUri("UnitTests");
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
		var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.CodeBlockNode.ChildList[0];
		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
	}

	[Fact]
	public void Two_ClassDefinitions()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClassOne
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}

	public string FirstName { get; set; }
}

public class MyClassTwo
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

		var typeDefinitionNodeOne = (TypeDefinitionNode)topCodeBlock.ChildList[0];
		var typeDefinitionNodeTwo = (TypeDefinitionNode)topCodeBlock.ChildList[1];
	}

	[Fact]
	public void Namespace_Test()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace;";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();
	}

	[Fact]
	public void Namespace_FileScope_EmptyClass()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace;

public class MyClass
{
	
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();
	}

	[Fact]
	public void Namespace_BlockScope_EmptyClass()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace
{
	public class MyClass
	{
	
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();
	}

	[Fact]
	public void Namespace_FileScope_Class()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace;

public class MyClass
{
	public string FirstName { get; set; }
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();

		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.CodeBlockNode.ChildList.Single();
	}

	[Fact]
	public void Namespace_BlockScope_Class()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace
{
	public class MyClass
	{
		public MyClass()
		{
		}

		public string FirstName { get; set; }
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();

		var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.CodeBlockNode.ChildList[0];
		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
	}
}
