namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;

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

    // Symbols
    TypeSymbol,
    FunctionSymbol,
    VariableSymbol,
}