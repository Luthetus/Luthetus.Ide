using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

/// <summary>
/// This file is just checking if various code block owners parse properly.
///
/// As opposed to <see cref="CodeBlockOwnerTests"/> which is testing whether they nest properly.
/// </summary>
public class CodeBlockOwnerSimpleTests
{
	[Fact]
	public void ArbitraryCodeBlock()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"{
	var a = 2;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (ArbitraryCodeBlockNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Foreach_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"foreach (var item in list)
{
	var a = 2;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var foreachNodeOne = (ForeachStatementNode)topCodeBlock.ChildList[0];
        
        Assert.Equal(1, topCodeBlock.ChildList.Length);
        
        var foreachBoundScope = compilationUnit.Binder.GetBoundScope(foreachNodeOne.OpenBraceToken.TextSpan);
        Assert.NotNull(foreachBoundScope);
        Console.WriteLine($"foreachBoundScope.VariableDeclarationMap.Count: {foreachBoundScope.VariableDeclarationMap.Count}");
        
        var globalScope = compilationUnit.Binder.GetBoundScope(foreachNodeOne.ForeachKeywordToken.TextSpan);
        Assert.NotNull(globalScope);
        Console.WriteLine($"globalScope.VariableDeclarationMap.Count: {globalScope.VariableDeclarationMap.Count}");
        
        foreach (var variable in globalScope.VariableDeclarationMap.Values)
        {
        	Console.WriteLine(variable.IdentifierToken.TextSpan.GetText());
        }
        
        Assert.Equal(0, globalScope.VariableDeclarationMap.Count);
        Assert.Equal(1, foreachBoundScope.VariableDeclarationMap.Count);
    }
    
    [Fact]
	public void Foreach()
	{ 
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"var list = new List<int>();

foreach (var item in list)
{
	var a = 2;
}

foreach (var item in list)
	var a = 2;

foreach (var item in list)
	;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[i++];
        var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[i++];
        
        var foreachNodeOne = (ForeachStatementNode)topCodeBlock.ChildList[i++];
        var foreachNodeTwo = (ForeachStatementNode)topCodeBlock.ChildList[i++];
        var foreachNodeThree = (ForeachStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(5, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void DoWhile_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"do
{
	var a = 2;
} while(true);";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        foreach (var child in topCodeBlock.ChildList)
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        var doWhileNodeOne = (DoWhileStatementNode)topCodeBlock.ChildList.Single();
        Assert.Equal(1, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void DoWhile()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"do
{
	var a = 2;
} while(true);

do
	var a = 2;
while(true);

do
	;
while(true);";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var doWhileNodeOne = (DoWhileStatementNode)topCodeBlock.ChildList[i++];
        var doWhileNodeTwo = (DoWhileStatementNode)topCodeBlock.ChildList[i++];
        var doWhileNodeThree = (DoWhileStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(3, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void While_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"while (true)
{
	var a = 2;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var whileNodeOne = (WhileStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(1, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void While()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"while (true)
{
	var a = 2;
}

while (true)
	var a = 2;

while (true)
	;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var whileNodeOne = (WhileStatementNode)topCodeBlock.ChildList[i++];
        var whileNodeTwo = (WhileStatementNode)topCodeBlock.ChildList[i++];
        var whileNodeThree = (WhileStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(3, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void For_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"for (int i = 0; i < 10; i++)
{
	var a = 2;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        foreach (var child in topCodeBlock.ChildList)
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        var forNodeOne = (ForStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(1, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void For()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"for (int i = 0; i < 10; i++)
{
	var a = 2;
}

for (int j = 0; j < 10; j++)
	var a = 2;
	
for (int j = 0; j < 10; j++)
	;

for (;;)
	;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var forNodeOne = (ForStatementNode)topCodeBlock.ChildList[i++];
        var forNodeTwo = (ForStatementNode)topCodeBlock.ChildList[i++];
        var forNodeThree = (ForStatementNode)topCodeBlock.ChildList[i++];
        var forNodeFour = (ForStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(4, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void If_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"if (true)
{
	var a = 2;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var ifNodeOne = (IfStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(1, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void If()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"if (true)
{
	var a = 2;
}

if (true)
	var a = 2;
	
if (true)
	;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var ifNodeOne = (IfStatementNode)topCodeBlock.ChildList[i++];
        var ifNodeTwo = (IfStatementNode)topCodeBlock.ChildList[i++];
        var ifNodeThree = (IfStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(3, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void SwitchStatement_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"switch (true)
{
	case true:
		break;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var switchNode = (SwitchStatementNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void SwitchStatement()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"switch (true)
{
	case true:
		var a = 2;
		break;
	case false:
	{
		var b = 2;
		break;
	}
	default:
		var c = 2;
		break;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var switchNode = (SwitchStatementNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void SwitchExpression()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"FirstName = true switch
{
	true => firstName,
	false => firstName,
	default => firstName
};";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var switchExpressionNode = (SwitchExpressionNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Ternary()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"var a = true ? ""true"" : ""false"";";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[i++];
        var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[i++];
        
        throw new NotImplementedException("var ternaryNode = (TernaryNode)variableAssignmentNode.ExpressionNode;");
    }
    
    [Fact]
	public void Enum()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public enum SomeKind
{
	ThatKind,
	ThisKind,
	AnyKind,
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var enumDefinitionNode = (EnumDefinitionNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Try_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"try
{
	var a = 2;
}
catch (Exception e)
{
	throw;
}
finally
{
	_ = firstName;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var tryNodeOne = (TryStatementNode)topCodeBlock.ChildList[i++];
        
        foreach (var child in topCodeBlock.ChildList)
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        Assert.Equal(1, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void Try()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"try
{
	var a = 2;
}
catch (Exception e)
{
	throw;
}
finally
{
	_ = firstName;
}

// Again but without the catch capturing the variable.
try
{
	var a = 2;
}
catch (Exception)
{
	throw;
}
finally
{
	_ = firstName;
}

// Again but with single statements.
try
	var a = 2;
catch (Exception)
	throw;
finally
	_ = firstName;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var tryNodeOne = (TryStatementNode)topCodeBlock.ChildList[i++];
        var tryNodeTwo = (TryStatementNode)topCodeBlock.ChildList[i++];
        var tryNodeThree = (TryStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(3, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void Lock_Simple()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"lock (_syncRoot)
{
	var a = 2;
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var lockNodeOne = (LockStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(1, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void Lock()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"lock (_syncRoot)
{
	var a = 2;
}

lock (_syncRoot)
	var a = 2;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var i = 0;
        
        var lockNodeOne = (LockStatementNode)topCodeBlock.ChildList[i++];
        var lockNodeTwo = (LockStatementNode)topCodeBlock.ChildList[i++];
        
        Assert.Equal(2, topCodeBlock.ChildList.Length);
    }
    
    [Fact]
	public void NamespaceFilescope()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void NamespaceBlockscope()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void ObjectInitialization()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void LambdaExpression()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void LambdaBlock()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void RecordWith()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void Yield()
	{
		throw new NotImplementedException();
    }
    
    [Fact]
	public void Unsafe()
	{
		throw new NotImplementedException();
    }
}
