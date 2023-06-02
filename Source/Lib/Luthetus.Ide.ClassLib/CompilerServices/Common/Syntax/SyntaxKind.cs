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
    KeywordContextualToken,
    NumericLiteralToken,
    StringLiteralToken,
    TriviaToken,
    PreprocessorDirectiveToken,
    LibraryReferenceToken,
    PlusToken,
    PlusPlusToken,
    MinusToken,
    MinusMinusToken,
    EqualsToken,
    EqualsEqualsToken,
    QuestionMarkToken,
    QuestionMarkQuestionMarkToken,
    BangToken,
    StatementDelimiterToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    OpenBraceToken,
    CloseBraceToken,
    OpenAngleBracketToken,
    CloseAngleBracketToken,
    ColonToken,
    MemberAccessToken,
    BadToken,
    EndOfFileToken,

    // Nodes
    CompilationUnitNode,
    LiteralExpressionNode,
    BoundLiteralExpressionNode,
    BoundBinaryOperatorNode,
    BoundBinaryExpressionNode,
    PreprocessorLibraryReferenceStatementNode,
    BoundTypeNode,
    BoundFunctionDeclarationNode,
    BoundIfStatementNode,
    BoundVariableDeclarationStatementNode,
    BoundVariableAssignmentStatementNode,
    BoundFunctionInvocationNode,
    BoundReturnStatementNode,
    BoundNamespaceStatementNode,
    BoundClassDeclarationNode,
    BoundInheritanceStatementNode,
    BoundUsingDeclarationNode,
    BoundIdentifierReferenceNode,

    // Symbols
    TypeSymbol,
    FunctionSymbol,
    VariableSymbol,
}