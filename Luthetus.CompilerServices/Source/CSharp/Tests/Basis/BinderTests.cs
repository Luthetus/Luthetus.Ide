using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
