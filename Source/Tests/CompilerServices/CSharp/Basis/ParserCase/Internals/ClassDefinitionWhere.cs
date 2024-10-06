using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ClassDefinitionWhere
{
	[Fact]
	public void Aaa1()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"public abstract class CommandWithType<T> where T : notnull
{
    protected CommandWithType(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            Func<ICommandArgs, Task> commandFunc) 
        : base(displayName, internalIdentifier, shouldBubble, commandFunc)
    {
    }
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
    }
    
    [Fact]
	public void Aaa2()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"public abstract class CommandWithType<T> : CommandNoType where T : notnull
{
    protected CommandWithType(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            Func<ICommandArgs, Task> commandFunc) 
        : base(displayName, internalIdentifier, shouldBubble, commandFunc)
    {
    }
}";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
    }
}

