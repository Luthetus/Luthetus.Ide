using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

/// <summary>
/// Given:
///     public void Aaa(int number) { }
///     public void Bbb(int number) { }
/// Then:
///     "Already defined variable: 'number'"
///     will appear as an error diagnostic on the second
///     function definition.
///     
///     Reason: function arguments are being
///     declared at the same scope as the function definition itself.
///     
///     Fix: function arguments need to be defined within the scope of the function body instead.
/// </summary>
public class ParseFunctionVariablesOneScopeTooHigh
{
	[Fact]
	public void Aaa1()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
public void Aaa(int number) { }
public void Bbb(int number) { }
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var aaaFunctionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList[0];
        var bbbFunctionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.ChildList[1];
        
        var aaaFunctionBoundScope = compilationUnit.Binder.GetBoundScope(aaaFunctionDefinitionNode.OpenBraceToken.TextSpan);
        Assert.NotNull(aaaFunctionBoundScope);
        Console.WriteLine($"aaaFunctionBoundScope.VariableDeclarationMap.Count: {aaaFunctionBoundScope.VariableDeclarationMap.Count}");
        
        var bbbFunctionBoundScope = compilationUnit.Binder.GetBoundScope(bbbFunctionDefinitionNode.OpenBraceToken.TextSpan);
        Assert.NotNull(bbbFunctionBoundScope);
        Console.WriteLine($"bbbFunctionBoundScope.VariableDeclarationMap.Count: {bbbFunctionBoundScope.VariableDeclarationMap.Count}");
        
        var globalScope = compilationUnit.Binder.GetBoundScope(aaaFunctionDefinitionNode.FunctionIdentifierToken.TextSpan);
        Assert.NotNull(globalScope);
        Console.WriteLine($"globalScope.VariableDeclarationMap.Count: {globalScope.VariableDeclarationMap.Count}");
        
        foreach (var variable in globalScope.VariableDeclarationMap.Values)
        {
        	Console.WriteLine(variable.IdentifierToken.TextSpan.GetText());
        }
        
        Assert.Equal(0, globalScope.VariableDeclarationMap.Count);
        Assert.Equal(1, aaaFunctionBoundScope.VariableDeclarationMap.Count);
        Assert.Equal(1, bbbFunctionBoundScope.VariableDeclarationMap.Count);
        
        /*
        The fix is: when the function body is being parsed,
        to check the syntax stack for a function definition node,
        then to add its function arguments to the scope?
        
        A better solution might involve having the child scope be created at the '(' instead of the '{'
        for function definitions.
        
        But, even further, there is the generic argument, which might be the return type,
        yet those come before the '('.
        
        Will probably for now just check the syntax stack when parsing the function body.
        */
    }

	// TODO: Undo CarriageReturnLineFeed seems buggy
	
	private void Aaa(int number) { }
	private void Bbb(int number) { }
}
