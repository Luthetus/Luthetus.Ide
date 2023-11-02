using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basics;

public partial class BinderTests
{
    [Fact]
    public void AUTOCOMPLETE_VariableIdentifier()
    {
        var variableTypeClauseIdentifier = "int";
        var variableIdentifier = "someVariable";
        var sourceText = @$"{variableTypeClauseIdentifier} {variableIdentifier};".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var codeBlockNode = compilationUnit.TopLevelStatementsCodeBlockNode;

        // Assert variable declaration parsed
        {
            var variableDeclarationStatementNode = (VariableDeclarationNode)codeBlockNode.ChildBag.Single();

            Assert.Equal(SyntaxKind.VariableDeclarationStatementNode, variableDeclarationStatementNode.SyntaxKind);
            Assert.Equal(2, variableDeclarationStatementNode.ChildBag.Length);

            var typeClauseNode = (TypeClauseNode)variableDeclarationStatementNode.ChildBag[0];
            Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
            Assert.Equal(variableTypeClauseIdentifier, typeClauseNode.TypeIdentifier.TextSpan.GetText());

            var identifierToken = (IdentifierToken)variableDeclarationStatementNode.ChildBag[1];
            Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);
            Assert.Equal(variableIdentifier, identifierToken.TextSpan.GetText());
        }

        // Assert Autocomplete Results
        {
            var textSpan = new TextEditorTextSpan(
                0, sourceText.Length, 0, resourceUri, sourceText);

            var boundScope = compilationUnit.Binder.GetBoundScope(textSpan) as CSharpBoundScope;
            Assert.NotNull(boundScope);

            var matches = boundScope.VariableDeclarationMap.Where(x => x.Key.Contains("some"));
            Assert.Single(matches);
            Assert.Equal(matches.Single().Key, variableIdentifier);
        }
    }
}