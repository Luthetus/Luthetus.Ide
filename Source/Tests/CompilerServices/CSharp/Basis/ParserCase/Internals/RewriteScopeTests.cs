using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class RewriteScopeTests
{
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
		
        var sourceText =
@"
public class MyClass
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}

	public string FirstName { get; set; }
}
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
        
        foreach (var child in typeDefinitionNode.CodeBlockNode.ChildList)
        {
        	Console.WriteLine($"child.SyntaxKind: {child.SyntaxKind}");
        }
        
        /*foreach (var kvp in compilationUnit.Binder.ScopeList)
        {
        	Console.WriteLine($"Key: \"{kvp.Key.Value}\"");
        	
        	foreach (var scope in kvp.Value)
        	{
        		Console.WriteLine($"\t{scope}");
        	}
        }*/
    }
}
