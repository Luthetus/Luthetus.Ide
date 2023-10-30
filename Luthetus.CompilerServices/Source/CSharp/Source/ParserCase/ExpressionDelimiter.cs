using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public class ExpressionDelimiter
{
    public ExpressionDelimiter(
        SyntaxKind? openSyntaxKind,
        SyntaxKind closeSyntaxKind,
        ISyntaxToken? openSyntaxToken,
        ISyntaxToken? closeSyntaxToken)
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

    public ISyntaxToken? OpenSyntaxToken { get; set; }
    public ISyntaxToken? CloseSyntaxToken { get; set; }
}
