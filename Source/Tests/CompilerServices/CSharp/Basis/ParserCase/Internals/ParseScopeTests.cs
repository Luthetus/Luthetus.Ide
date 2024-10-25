using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

/// <summary>
/// Syntax that breaks scope:
///     - [] Object Initialization
///     - [] Collection Initialization
///     - [] getter code blocks
///     - [] setter code blocks
///     - [] lambda code blocks
///     - [] record with keyword?
///
/// Also, why is 'var' as the first token in a scope no longer parsing correctly?
///
/// Again also, I moved the diagnostic bag to the session how are the diagnostics
/// rendering when I didn't do anything beyond move them (I didn't tell the code to look at the new location)?
///
/// Even still also, 'record' and 'record struct' primary constructor syntax isn't working?
/// </summary>
public class ParseScopeTests
{
	[Fact]
	public void ObjectInitialization()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void CollectionInitialization_NoParenthesis()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"var numbers = new List<int>
{
    0,
    1,
    2,
};";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
        Assert.Equal(2, topCodeBlock.GetChildList().Length);
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void CollectionInitialization_WithParenthesis()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"var numbers = new List<int>()
{
    0,
    1,
    2,
};";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
        Assert.Equal(2, topCodeBlock.GetChildList().Length);
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void GetterCodeBlocks()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void SetterCodeBlocks()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void LambdaCodeBlocks()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void RecordWithKeyword()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void RecordPrimaryConstructor_A()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText =
@"
public record MyRecord(int Aaa)
{
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var recordDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
        
        foreach (var child in recordDefinitionNode.GetChildList())
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        Console.WriteLine(((IdentifierToken)recordDefinitionNode.GetChildList()[0]).TextSpan.GetText());
        
        var constructorDefinitionNode = recordDefinitionNode.GetChildList()[0];
        var propertyDefinitionNode = recordDefinitionNode.GetChildList()[1];
        var functionDefinitionNode = recordDefinitionNode.GetChildList()[2];
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void RecordPrimaryConstructor_B()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText =
@"
public record MyRecord(int Aaa)
{
	public MyRecordStruct()
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{
	}
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var recordDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
        
        foreach (var child in recordDefinitionNode.GetChildList())
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        var constructorDefinitionNode = recordDefinitionNode.GetChildList()[0];
        var propertyDefinitionNode = recordDefinitionNode.GetChildList()[1];
        var functionDefinitionNode = recordDefinitionNode.GetChildList()[2];
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void RecordStructPrimaryConstructor()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText =
@"
public record struct MyRecordStruct(int Aaa)
{
	public MyRecordStruct()
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{
	}
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var recordDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[0];
        var recordStructDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[1];
        var classDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[2];
        Assert.Equal(3, topCodeBlock.GetChildList().Length);
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void ClassPrimaryConstructor()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText =
@"
public class MyClass(int Aaa)
{
	public MyClass()
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{
	}
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var recordDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[0];
        var recordStructDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[1];
        var classDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[2];
        Assert.Equal(3, topCodeBlock.GetChildList().Length);
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void RecordAndRecordStructAndClassPrimaryConstructor()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText =
@"
public record MyRecord(int Aaa)
{
}

public record struct MyRecordStruct(int Aaa)
{
}

public class MyClass(int Aaa)
{
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var recordDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[0];
        var recordStructDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[1];
        var classDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList()[2];
        Assert.Equal(3, topCodeBlock.GetChildList().Length);
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void VarContextualKeyword()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText =
@"
public void MyMethod()
{
	var aaa = 1;
	var bbb = 2;
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList().Single();
        
        foreach (var child in functionDefinitionNode.CodeBlockNode.GetChildList())
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        Assert.Equal(4, functionDefinitionNode.CodeBlockNode.GetChildList().Length);
        
		throw new NotImplementedException();
	}
	
	[Fact]
	public void EnsureDiagnosticBagWorksProperly()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
        var sourceText = @"";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
		throw new NotImplementedException();
	}

	/// <summary>
	/// Goal: correctly parse all the scopes (2024-10-13)
	/// =================================================
	///
	/// Example:
	/// --------------------------------------------------------------------------------
	/// 	namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;
	/// 	
	/// 	public class ParseScopeTests
	/// 	{
	/// 		
	/// 	}
	///
	/// --------------------------------------------------------------------------------
	///
	/// When the cursor enters a child scope, it seemingly cannot return
	/// to the parent scope (this is erroneous).
	///
	/// The source text defines a file scoped namespace,
	/// then inside of that a type definition.
	///
	/// If one starts with the cursor at positionIndex 0,
	/// then presses 'ArrowDown' until they reach the end of the file,
	/// they'll see that the scopes appear to be fine.
	///
	/// But, once one reaches the end of the file it is shown
	/// that your current scope is within the type definition.
	///
	/// However, that type definition ended a few characters prior
	/// to the end of the file.
	///
	/// So, why is the type definition erroneously the active
	/// scope in this situation?
	///
	/// Generally, it appears that the wording of the problem is
	/// that a child scope cannot return to its parent scope.
	///
	/// The reason for this statement is that sibling nodes
	/// don't seem to be an issue.
	///
	/// Example:
	/// --------------------------------------------------------------------------------
	/// 	namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;
	/// 	
	/// 	public class OneParseScopeTests
	/// 	{
	/// 		
	/// 	}
	///	 
	/// 	public class TwoParseScopeTests
	/// 	{
	/// 		
	/// 	}
	/// 
	/// --------------------------------------------------------------------------------
	/// 
	/// In this example, both OneParseScopeTests and TwoParseScopeTests appear
	/// to have their scope correctly parsed.
	///
	/// But again, upon reaching the end of file,
	/// the most recently parsed scope seems to still be active.
	///
	/// In this case 'TwoParseScopeTests' has its scope active,
	/// but again it had closed its scope a few characters
	/// prior to the end of file.
	///
	/// It is of note though, after parsing OneParseScopeTests
	/// that TwoParseScopeTests was still correctly parsed,
	/// even though upon leaving OneParseScopeTests scope
	/// the active scope remained as 'OneParseScopeTests'
	/// rather than be returned to the parent scope.
	///
	/// i.e.: if the user's cursor is between the two
	/// type definitions, they'll see their active scope
	/// as the first type definition (erroneously).
	///
	/// -----------------------------------------------------
	///
	/// The issue is fixed.
	///
	/// It's funny because I was so full of anxiety and dread
	/// that this wasn't working.
	///
	/// I was struggling to bring myself to look at the code.
	///
	/// But, I took one look at the
	/// 'public IScope? GetScope(ResourceUri resourceUri, int positionIndex)'
	/// method.
	///
	/// And instantly I saw it and was like, "yeah that'll do it"
	/// I never compared the ending of the scope, only the start of the scope.
	///
	/// Oddly enough this was only for this positionIndex overload.
	/// The other overloads were correct so I just copy and pasted it.
	/// 
	/// Even better the TextSpan overload now just invokes
	/// the positionIndex overload instead of copy/paste duplicating code.
	///
	/// In other words: the issue was getting the scope,
	/// not that the scopes were parsed incorrectly.
	///
	/// That being said, there are some cases of scopes being parsed
	/// incorrectly. These being object/collection initialization,
	/// so I can fix that next.
	/// </summary>
	[Fact]
	public void Aaa1()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
 	   // There is an extra space on the final line of this text
		// for the sake of my sanity, so I can go 1 positionIndex further
		// in addition to the other spot. My head is just yelling at
		// me with anxiety to do this.
        var sourceText = @"namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseScopeTests
{
	
}
 ";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.GetChildList().Single();
        
        throw new NotImplementedException();
    }
}
