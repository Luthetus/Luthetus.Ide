using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public interface IGenericSyntax
{
    public TextEditorTextSpan TextSpan { get; }
    public IReadOnlyList<IGenericSyntax> ChildList { get; }
    public GenericSyntaxKind GenericSyntaxKind { get; }
}