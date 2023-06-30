using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_VARIABLE_DECLARATION_STATEMENT()
    {
        string sourceText = @"int x;".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundVariableDefinitionStatementNode =
                (BoundVariableDeclarationStatementNode)compilationUnit.Children
                    .Single();

            Assert.Equal(
                SyntaxKind.BoundVariableDeclarationStatementNode,
                boundVariableDefinitionStatementNode.SyntaxKind);

            Assert.Equal(
                2,
                boundVariableDefinitionStatementNode.Children.Length);

            var boundClassDefinitionNode = (BoundClassDefinitionNode)boundVariableDefinitionStatementNode
                .Children[0];

            Assert.Equal(
                SyntaxKind.BoundClassDefinitionNode,
                boundClassDefinitionNode.SyntaxKind);

            Assert.Equal(
                typeof(int),
                boundClassDefinitionNode.Type);

            var identifierToken = boundVariableDefinitionStatementNode.Children[1];

            Assert.Equal(
                SyntaxKind.IdentifierToken,
                identifierToken.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_VARIABLE_DECLARATION_STATEMENT_THEN_VARIABLE_ASSIGNMENT_STATEMENT()
    {
        string sourceText = @"int x; x = 42;".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Equal(2, compilationUnit.Children.Length);

            var boundVariableDeclarationStatementNode =
                (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];

            Assert.Equal(
                SyntaxKind.BoundVariableDeclarationStatementNode,
                boundVariableDeclarationStatementNode.SyntaxKind);

            var boundVariableAssignmentStatementNode =
                (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];

            Assert.Equal(
                SyntaxKind.BoundVariableAssignmentStatementNode,
                boundVariableAssignmentStatementNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_COMPOUND_VARIABLE_DECLARATION_AND_ASSIGNMENT_STATEMENT()
    {
        string sourceText = @"int x = 42;".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Equal(2, compilationUnit.Children.Length);

            var boundVariableDeclarationStatementNode =
                (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];

            Assert.Equal(
                SyntaxKind.BoundVariableDeclarationStatementNode,
                boundVariableDeclarationStatementNode.SyntaxKind);

            var boundVariableAssignmentStatementNode =
                (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];

            Assert.Equal(
                SyntaxKind.BoundVariableAssignmentStatementNode,
                boundVariableAssignmentStatementNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_CONDITIONAL_VAR_KEYWORD()
    {
        var sourceText = @"var var = 2; var x = var * 2;".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var modelParser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = modelParser.Parse();

        // Assertions
        {
            var variableDeclarationWithIdentifierVar = (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];
            Assert.NotNull(variableDeclarationWithIdentifierVar);

            var variableAssignmentWithIdentifierVar = (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];
            Assert.NotNull(variableAssignmentWithIdentifierVar);

            var variableDeclarationWithIdentifierX = (BoundVariableDeclarationStatementNode)compilationUnit.Children[2];
            Assert.NotNull(variableDeclarationWithIdentifierX);

            var variableAssignmentWithIdentifierX = (BoundVariableAssignmentStatementNode)compilationUnit.Children[3];
            Assert.NotNull(variableAssignmentWithIdentifierX);
        }
    }

    [Fact]
    public void SHOULD_PARSE_VARIABLE_REFERENCE()
    {
        var sourceText = @"private int _count; private void IncrementCountOnClick() { _count++; }".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var modelParser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = modelParser.Parse();

        // Assertions
        {
            var boundVariableDeclarationStatementNode = (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];
            Assert.NotNull(boundVariableDeclarationStatementNode);

            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)compilationUnit.Children[1];
            Assert.NotNull(boundFunctionDefinitionNode);

            var boundIdentifierReferenceNode = (BoundIdentifierReferenceNode)compilationUnit.Children[2];
            Assert.NotNull(boundIdentifierReferenceNode);
        }
    }
}
