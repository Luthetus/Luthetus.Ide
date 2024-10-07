using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParsePrototypes
{
	[Fact]
	public void TypeDefinitionNode_Test()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public string FirstName { get; set; }

	public MyClass(string firstName)
	{
		FirstName = firstName;
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void PropertyDefinitionNode_Function()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public void Aaa(string firstName)
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
		Assert.Equal(2, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var functionDefinitionNode = (FunctionDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			functionDefinitionNode.CodeBlockNode.ChildList.Single();

		Assert.Equal(
			"FirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void PropertyDefinitionNode_BEFORE_ASSIGNMENT()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public string FirstName { get; set; }

	public MyClass(string firstName)
	{
		FirstName = firstName;
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(2, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[1];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.CodeBlockNode.ChildList.Single();

		Assert.Equal(
			"FirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void PropertyDefinitionNode_AFTER_ASSIGNMENT()
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
		Assert.Equal(2, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.CodeBlockNode.ChildList.Single();

		Assert.Equal(
			"FirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void FieldDefinitionNode_BEFORE_ASSIGNMENT()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	private string _firstName;

	public MyClass(string firstName)
	{
		_firstName = firstName;
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(2, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var fieldDefinitionNode = (FieldDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];
		
		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[1];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.CodeBlockNode.ChildList.Single();

		Assert.Equal(
			"_firstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void FieldDefinitionNode_AFTER_ASSIGNMENT()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public MyClass(string firstName)
	{
		_firstName = firstName;
	}

	private string _firstName;
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(2, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.CodeBlockNode.ChildList.Single();

		Assert.Equal(
			"_firstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		var fieldDefinitionNode = (FieldDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void VariableDeclarationNode_BEFORE_ASSIGNMENT()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public MyClass(string firstName)
	{
		string localFirstName;
		localFirstName = firstName;
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(1, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var variableDeclarationNode = (VariableDeclarationNode)
			constructorDefinitionNode.CodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(
			"localFirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void VariableDeclarationNode_AFTER_ASSIGNMENT()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public MyClass(string firstName)
	{
		localFirstName = firstName;
		string localFirstName;
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(1, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.CodeBlockNode.ChildList[0];

		var variableDeclarationNode = (VariableDeclarationNode)
			constructorDefinitionNode.CodeBlockNode.ChildList[1];

		Assert.Equal(
			"localFirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		Assert.Equal(1, compilationUnit.DiagnosticsList.Length);
    }

	[Fact]
	public void Variable_Reference()
	{
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	private string _firstName;

	public MyClass(string firstName)
	{
		string localFirstName;
		
		_firstName = firstName;
		localFirstName = firstName;
		FirstName = firstName;

		Console.WriteLine(_firstName);
		Console.WriteLine(localFirstName);
		Console.WriteLine(FirstName);
	}

	public string FirstName { get; set; }
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Assert.Equal(3, typeDefinitionNode.CodeBlockNode.ChildList.Length);

		var fieldDefinitionNode = (FieldDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[0];

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[1];

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.CodeBlockNode.ChildList[2];

		// Inside constructorDefinitionNode
		{
			var localVariableDeclarationNode = (VariableDeclarationNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[0];

			var fieldVariableAssignmentExpressionNode = (VariableAssignmentExpressionNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[1];
			{
				Assert.Equal(
					"_firstName",
					fieldVariableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText());
			}

			var localVariableAssignmentExpressionNode = (VariableAssignmentExpressionNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[2];
			{
				Assert.Equal(
					"localFirstName",
					localVariableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText());
			}

			var propertyVariableAssignmentExpressionNode = (VariableAssignmentExpressionNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[3];
			{
				Assert.Equal(
					"FirstName",
					propertyVariableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText());
			}

			var fieldFunctionInvocationNode = (FunctionInvocationNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[4];
			{
				var fieldVariableReferenceNode = (VariableReferenceNode)
					fieldFunctionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList.Single().ExpressionNode;

				Assert.Equal(fieldDefinitionNode, fieldVariableReferenceNode.VariableDeclarationNode);
			}

			var localFunctionInvocationNode = (FunctionInvocationNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[5];
			{
				var localVariableReferenceNode = (VariableReferenceNode)
					localFunctionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList.Single().ExpressionNode;

				Assert.Equal(localVariableDeclarationNode, localVariableReferenceNode.VariableDeclarationNode);
			}

			var propertyFunctionInvocationNode = (FunctionInvocationNode)
				constructorDefinitionNode.CodeBlockNode.ChildList[6];
			{
				var propertyVariableReferenceNode = (VariableReferenceNode)
					propertyFunctionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList.Single().ExpressionNode;

				Assert.Equal(propertyDefinitionNode, propertyVariableReferenceNode.VariableDeclarationNode);
			}
		}

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);

		throw new NotImplementedException();
    }
}
