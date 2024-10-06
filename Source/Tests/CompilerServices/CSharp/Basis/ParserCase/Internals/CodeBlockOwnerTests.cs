using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

/// <summary>
/// This file is checking if a code block will push a node onto the stack that represents itself.
///
/// Because, there is currently an index out of bounds exception occurring.
///
/// It is when, within a ConstructorDefinitionNode one uses '{ }' to create
/// an "arbitrary scope".
///
/// This "arbitrary scope" does not push a node onto the stack that represents itself,
/// therefore when the close brace is encountered, and the stack is pop'd,
/// the node returned is the ConstructorDefinitionNode (which hasn't yet been "closed").
///
/// So the "arbitray scope" code block owner ends up confusing itself to be the ConstructorDefinitionNode
/// codeblock owner.
/// </summary>
public class CodeBlockOwnerTests
{
	[Fact]
	public void ArbitraryCodeBlock()
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
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		var list = new List<int>();
	
		foreach (var item in list)
		{
			FirstName = firstName;
		}
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
	public void DoWhile()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		do
		{
			FirstName = firstName;
		} while(true);
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
	public void While()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		while (true)
		{
			FirstName = firstName;
		}
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
	public void For()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		for (int i = 0; i < 10; i++)
		{
			FirstName = firstName;
		}
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
	public void If()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		if (true)
		{
			FirstName = firstName;
		}
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
	public void SwitchStatement()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		switch (true)
		{
			case true:
				FirstName = firstName;
				break;
			case false:
			{
				FirstName = firstName;
				break;
			}
			default:
				FirstName = firstName;
				break;
		}
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
	public void SwitchExpression()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		FirstName = true switch
		{
			true => firstName,
			false => firstName,
			default => firstName
		};
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
	public void Enum()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }
	
	public enum SomeKind
	{
		ThatKind,
		ThisKind,
		AnyKind,
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
	public void Try()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		try
		{
			FirstName = firstName;
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
			FirstName = firstName;
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
			FirstName = firstName;
		catch (Exception)
			throw;
		finally
			_ = firstName;
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
	public void Lock()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"public class ClassOne
{
	private readonly object _syncRoot = new();

	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		lock (_syncRoot)
		{
			FirstName = firstName;
		}
		
		lock (_syncRoot)
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
