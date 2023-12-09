using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericKeywordSyntaxTests
{
    public GenericKeywordSyntax(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public ImmutableArray<IGenericSyntax> ChildBag => ImmutableArray<IGenericSyntax>.Empty;
    public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.Keyword;
}