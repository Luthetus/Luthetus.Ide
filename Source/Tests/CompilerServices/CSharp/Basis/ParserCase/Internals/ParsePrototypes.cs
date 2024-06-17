using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

/*
The following tests in 'ParseVariablesTests' are failing:
	- VariableDeclaration_WITH_ImplicitType
	- VariableDeclaration_WITH_ImplicitType_WITH_VarIdentifier
	- VariableAssignment_WITH_StringLiteral_WITH_VarIdentifier
The other 13 of 16 are passing.
*/

public class ParsePrototypes
{
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
		Assert.Equal(2, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[1];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();

		Assert.Equal(
			"FirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);

		throw new NotImplementedException();
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
		Assert.Equal(2, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();

		Assert.Equal(
			"FirstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);

		throw new NotImplementedException();
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
		Assert.Equal(2, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var fieldDefinitionNode = (FieldDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];
		
		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[1];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();

		Assert.Equal(
			"_firstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);

		throw new NotImplementedException();
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
		Assert.Equal(2, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList.Single();

		Assert.Equal(
			"_firstName",
			variableAssignmentExpression.VariableIdentifierToken.TextSpan.GetText());

		var fieldDefinitionNode = (FieldDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);

		throw new NotImplementedException();
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
		Assert.Equal(1, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		var variableDeclarationNode = (VariableDeclarationNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[1];

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
		Assert.Equal(1, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		var variableAssignmentExpression = (VariableAssignmentExpressionNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[0];

		var variableDeclarationNode = (VariableDeclarationNode)
			constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[1];

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
		Assert.Equal(3, typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Length);

		var fieldDefinitionNode = (FieldDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];

		var constructorDefinitionNode = (ConstructorDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[1];

		var propertyDefinitionNode = (PropertyDefinitionNode)
			typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[2];

		// Inside constructorDefinitionNode
		{
			var localVariableDeclarationNode = (VariableDeclarationNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[0];

			var fieldVariableAssignmentExpressionNode = (VariableAssignmentExpressionNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[1];
			{
				Assert.Equal(
					"_firstName",
					fieldVariableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText());
			}

			var localVariableAssignmentExpressionNode = (VariableAssignmentExpressionNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[2];
			{
				Assert.Equal(
					"localFirstName",
					localVariableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText());
			}

			var propertyVariableAssignmentExpressionNode = (VariableAssignmentExpressionNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[3];
			{
				Assert.Equal(
					"FirstName",
					propertyVariableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText());
			}

			var fieldFunctionInvocationNode = (FunctionInvocationNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[4];
			{
				var fieldVariableReferenceNode = (VariableReferenceNode)
					fieldFunctionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodes.Single();

				Assert.Equal(fieldDefinitionNode, fieldVariableReferenceNode.VariableDeclarationNode);
			}

			var localFunctionInvocationNode = (FunctionInvocationNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[5];
			{
				var localVariableReferenceNode = (VariableReferenceNode)
					localFunctionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodes.Single();

				Assert.Equal(localVariableDeclarationNode, localVariableReferenceNode.VariableDeclarationNode);
			}

			var propertyFunctionInvocationNode = (FunctionInvocationNode)
				constructorDefinitionNode.FunctionBodyCodeBlockNode.ChildList[6];
			{
				var propertyVariableReferenceNode = (VariableReferenceNode)
					propertyFunctionInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodes.Single();

				Assert.Equal(propertyDefinitionNode, propertyVariableReferenceNode.VariableDeclarationNode);
			}
		}

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);

		throw new NotImplementedException();
    }
/*
Perhaps one can solve this problem by doing a breadth first search instead of depth first?

But, I think its a combination of breadth, and depth in some sense.

I need to first do breadth first searching on any particular node and collect any
'definition' nodes.

Afterwards I can go back to the original node and perform a depth first search?

Is the reason some nodes have the word "Definition" in them, whereas others have the
word "Declaration" due to their scope covering the entire code block (whether before or after
the text that defines it). And then "Declaration" has a scope covering any text
after its declaration?

And yet, if a class declares a property (by my current implementation) this is a
"VariableDeclarationNode". But I can access this "VariableDeclarationNode" anywhere
in the scope of the class (provided that contextually there is an implication
that the type had to have been instantiated) (but then again there are static properties too).

I have an enum named "VariableKind".
What would I do in the case that a language decided to implement a "variable" like
type other than { "Local", "Field", "Property", "Closure" }?

I remember at the time that I created the enum "VariableKind",
that it was to solve the problem of searching a collection of variables.

And that, if I had separate lists for "Local", "Field", "Property",
that I could check all 3 lists for an identifier that matched my text.
But I was worried about maintaining 3 lists like this, that one day
I'll forget to check one of the 3 lists.

The better solution is likely to have an 'IVariable' interface.
Then, the 'PropertyDefinitionNode' type could be added.
This would then solve the inconsistency in my "Definition" vs "Declaration".


*/
}
