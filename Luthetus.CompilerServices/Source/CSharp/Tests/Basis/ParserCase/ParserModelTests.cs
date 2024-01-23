using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase;

/// <summary>
/// <see cref="ParserModel"/>
/// </summary>
public class ParserModelTests
{
    /// <summary>
    /// <see cref="ParserModel(CSharpBinder, TokenWalker, Stack{ISyntax}, LuthetusDiagnosticBag, CodeBlockBuilder, CodeBlockBuilder, Action{CodeBlockNode}?, Stack{Action{CodeBlockNode}})"/>
    /// <br/>----<br/>
    /// <see cref="ParserModel.Binder"/>
    /// <see cref="ParserModel.TokenWalker"/>
    /// <see cref="ParserModel.SyntaxStack"/>
    /// <see cref="ParserModel.DiagnosticBag"/>
    /// <see cref="ParserModel.GlobalCodeBlockBuilder"/>
    /// <see cref="ParserModel.CurrentCodeBlockBuilder"/>
    /// <see cref="ParserModel.FinalizeNamespaceFileScopeCodeBlockNodeAction"/>
    /// <see cref="ParserModel.FinalizeCodeBlockNodeActionStack"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "public class MyClass { }";
        var lexer = new CSharpLexer(resourceUri, sourceText);

        var binder = new CSharpBinder();
        var tokenWalker = new TokenWalker(lexer.SyntaxTokens, new LuthetusDiagnosticBag());
        var syntaxStack = new Stack<ISyntax>();
        var diagnosticBag = new LuthetusDiagnosticBag();
        var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
        var currentCodeBlockBuilder = globalCodeBlockBuilder;
        var finalizeNamespaceFileScopeCodeBlockNodeAction = new Action<CodeBlockNode>(cbn => { });
        var finalizeCodeBlockNodeActionStack = new Stack<Action<CodeBlockNode>>();

        var parserModel = new ParserModel(
            binder,
            tokenWalker,
            syntaxStack,
            diagnosticBag,
            globalCodeBlockBuilder,
            currentCodeBlockBuilder,
            finalizeNamespaceFileScopeCodeBlockNodeAction,
            finalizeCodeBlockNodeActionStack);

        Assert.Equal(binder, parserModel.Binder);
        Assert.Equal(tokenWalker, parserModel.TokenWalker);
        Assert.Equal(syntaxStack, parserModel.SyntaxStack);
        Assert.Equal(diagnosticBag, parserModel.DiagnosticBag);
        Assert.Equal(globalCodeBlockBuilder, parserModel.GlobalCodeBlockBuilder);
        Assert.Equal(currentCodeBlockBuilder, parserModel.CurrentCodeBlockBuilder);
        Assert.Equal(finalizeNamespaceFileScopeCodeBlockNodeAction, parserModel.FinalizeNamespaceFileScopeCodeBlockNodeAction);
        Assert.Equal(finalizeCodeBlockNodeActionStack, parserModel.FinalizeCodeBlockNodeActionStack);
    }
}
