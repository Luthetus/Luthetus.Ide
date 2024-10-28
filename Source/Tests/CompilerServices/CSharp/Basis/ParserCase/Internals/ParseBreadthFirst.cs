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

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
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

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
		var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.CodeBlockNode.GetChildList()[0];
		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.CodeBlockNode.GetChildList()[1];

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

		var typeDefinitionNodeOne = (TypeDefinitionNode)topCodeBlock.GetChildList()[0];
		var typeDefinitionNodeTwo = (TypeDefinitionNode)topCodeBlock.GetChildList()[1];
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

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
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

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
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

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
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

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();

		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.CodeBlockNode.GetChildList().Single();
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

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();

		var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.CodeBlockNode.GetChildList()[0];
		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.CodeBlockNode.GetChildList()[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
	}
}
