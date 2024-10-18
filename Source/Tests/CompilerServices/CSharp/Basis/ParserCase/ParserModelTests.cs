using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.LexerCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase;

/// <summary>
/// <see cref="ParserModel"/>
/// </summary>
public class ParserModelTests
{
    /// <summary>
    /// <see cref="ParserModel(CSharpBinder, TokenWalker, Stack{ISyntax}, LuthDiagnosticBag, CodeBlockBuilder, CodeBlockBuilder, Action{CodeBlockNode}?, Stack{Action{CodeBlockNode}})"/>
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
        var binderSession = binder.StartBinderSession(resourceUri);
        var tokenWalker = new TokenWalker(lexer.SyntaxTokenList, new DiagnosticBag());
        var syntaxStack = new Stack<ISyntax>();
        var expressionStack = new Stack<ISyntax>();
        var diagnosticBag = new DiagnosticBag();
        var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
        var currentCodeBlockBuilder = globalCodeBlockBuilder;
        var finalizeNamespaceFileScopeCodeBlockNodeAction = new Action<CodeBlockNode>(cbn => { });
        var finalizeCodeBlockNodeActionStack = new Stack<Action<CodeBlockNode>>();

        var parserModel = new ParserModel(
            binder,
            (CSharpBinderSession)binderSession,
            tokenWalker,
            syntaxStack,
            expressionStack,
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
