using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericDocumentSyntaxTests
{
    public GenericDocumentSyntax(
        TextEditorTextSpan textSpan,
        ImmutableArray<IGenericSyntax> childBag)
    {
        TextSpan = textSpan;
        ChildBag = childBag;
    }

    public TextEditorTextSpan TextSpan { get; }
    public ImmutableArray<IGenericSyntax> ChildBag { get; }
    public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.Document;
}