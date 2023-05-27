using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public class BoundClassDeclarationNode : ISyntaxNode
{
    public BoundClassDeclarationNode(
        ISyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;

        Children = new ISyntax[]
        {
            IdentifierToken
        }.ToImmutableArray();
    }

    public BoundClassDeclarationNode(
        ISyntaxToken identifierToken,
        CompilationUnit classBodyCompilationUnit)
    {
        IdentifierToken = identifierToken;
        ClassBodyCompilationUnit = classBodyCompilationUnit;

        Children = new ISyntax[]
        {
            IdentifierToken,
            ClassBodyCompilationUnit
        }.ToImmutableArray();
    }

    public ISyntaxToken IdentifierToken { get; }
    public CompilationUnit? ClassBodyCompilationUnit { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundClassDeclarationNode;

    public BoundClassDeclarationNode WithClassBody(
        CompilationUnit classBodyCompilationUnit)
    {
        return new BoundClassDeclarationNode(
            IdentifierToken,
            classBodyCompilationUnit);
    }
}
