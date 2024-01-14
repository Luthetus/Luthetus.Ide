using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public interface IGenericSyntax
{
    public TextEditorTextSpan TextSpan { get; }
    public ImmutableArray<IGenericSyntax> ChildList { get; }
    public GenericSyntaxKind GenericSyntaxKind { get; }
}