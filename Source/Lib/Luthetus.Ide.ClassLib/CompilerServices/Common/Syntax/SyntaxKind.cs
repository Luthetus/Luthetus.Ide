namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

/// <summary>
/// In order to share identical logic with C and CSharp code analysis I need to have them share the SyntaxKind enum. I don't like this because some enum members are used in one language but not the other.
/// </summary>
public enum SyntaxKind
{
    // Tokens
    CommentMultiLineToken,
    CommentSingleLineToken,
    IdentifierToken,
    KeywordToken,
    NumericLiteralToken,
    StringLiteralToken,
    TriviaToken,
    PreprocessorDirectiveToken,
    LibraryReferenceToken,
    PlusToken,
    EqualsToken,
    StatementDelimiterToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    OpenBraceToken,
    CloseBraceToken,
    EndOfFileToken,

    // Nodes
    CompilationUnit,
    LiteralExpressionNode,
    BoundLiteralExpressionNode,
    BoundBinaryOperatorNode,
    BoundBinaryExpressionNode,
    PreprocessorLibraryReferenceStatement,
    BoundTypeNode,
    BoundFunctionDeclarationNode,
    BoundVariableDeclarationStatementNode,
    BoundVariableAssignmentStatementNode,
    BoundFunctionInvocationNode,
    BoundReturnStatementNode,
    BoundNamespaceStatementNode,
    BoundClassDeclarationNode,

    // Symbols
    TypeSymbol,
    FunctionSymbol,
    VariableSymbol,
}