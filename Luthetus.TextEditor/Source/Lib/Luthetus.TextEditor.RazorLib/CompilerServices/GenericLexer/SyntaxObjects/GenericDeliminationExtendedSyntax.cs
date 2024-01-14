using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericDeliminationExtendedSyntax : IGenericSyntax
{
    public GenericDeliminationExtendedSyntax(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public ImmutableArray<IGenericSyntax> ChildList => ImmutableArray<IGenericSyntax>.Empty;
    public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.DeliminationExtended;
}