using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.BinderCase.Custom;

/// <summary>
/// <see cref="CSharpBinderTests"/>
/// for tests that are intended to be "1 to 1" foreach public API on the <see cref="CSharp.BinderCase.CSharpBinder"/>
/// </summary>
public class CustomBinderTests
{
    [Fact]
    public void NamespaceGroupNode_FileScoped()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

public class MyClass
{
    public int SomeInt { get; set; } = 7;
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();
        var namespaceGroupNodes = parser.Binder.NamespaceGroupNodes;

        var getTopLevelTypeDefinitionNodesTuples = namespaceGroupNodes
            .Select(x => (x.Key, x.Value.GetTopLevelTypeDefinitionNodes()))
            .ToArray();

        var namespaceUnderTesting = namespaceGroupNodes
            .Single(x => x.Key == "Luthetus.CompilerServices.Lang.CSharp.Tests.Basis")
            .Value;

        var topLevelTypeDefinitionNodes = namespaceUnderTesting.GetTopLevelTypeDefinitionNodes();
    }

    [Fact]
    public void NamespaceGroupNode_BlockScoped()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = @"namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis
{
    public class MyClass
    {
        public int SomeInt { get; set; } = 7;
    }
}";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();
        var namespaceGroupNodes = parser.Binder.NamespaceGroupNodes;

        var getTopLevelTypeDefinitionNodesTuples = namespaceGroupNodes
            .Select(x => (x.Key, x.Value.GetTopLevelTypeDefinitionNodes()))
            .ToArray();

        var namespaceUnderTesting = namespaceGroupNodes
            .Single(x => x.Key == "Luthetus.CompilerServices.Lang.CSharp.Tests.Basis")
            .Value;

        var topLevelTypeDefinitionNodes = namespaceUnderTesting.GetTopLevelTypeDefinitionNodes();
    }
}
