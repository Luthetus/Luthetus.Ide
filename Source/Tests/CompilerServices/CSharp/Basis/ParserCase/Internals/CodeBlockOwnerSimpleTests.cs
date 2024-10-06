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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
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
