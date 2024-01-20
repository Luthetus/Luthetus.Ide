using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis;

public class BinderTests
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

        var aaa = 2;
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

        var aaa = 2;
    }
}
