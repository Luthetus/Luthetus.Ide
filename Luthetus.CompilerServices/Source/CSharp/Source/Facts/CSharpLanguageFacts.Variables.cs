using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Variables
    {
        public static readonly VariableDeclarationStatementNode Undefined = new(
            Types.Undefined.ToTypeClause(),
            new IdentifierToken(new TextEditorTextSpan(0, "undefined".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "undefined")),
            false);
    }
}