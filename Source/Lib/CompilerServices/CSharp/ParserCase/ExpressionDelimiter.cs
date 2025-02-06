using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class ExpressionDelimiter
{
    public ExpressionDelimiter(
        SyntaxKind? openSyntaxKind,
        SyntaxKind closeSyntaxKind,
        SyntaxToken? openSyntaxToken,
        SyntaxToken? closeSyntaxToken)
    {
        OpenSyntaxKind = openSyntaxKind;
        CloseSyntaxKind = closeSyntaxKind;
        OpenSyntaxToken = openSyntaxToken;
        CloseSyntaxToken = closeSyntaxToken;
    }

    /// <summary>
    /// <see cref="OpenSyntaxKind"/> is nullable because an expression can be deliminated
    /// by a single token. For example, the statement delimiter token: (semicolon, ';')
    /// In this case of single token delimination, one only provides the <see cref="CloseSyntaxToken"/>.
    /// </summary>
    public SyntaxKind? OpenSyntaxKind { get; set; }
    public SyntaxKind CloseSyntaxKind { get; set; }

    public SyntaxToken? OpenSyntaxToken { get; set; }
    public SyntaxToken? CloseSyntaxToken { get; set; }
}
