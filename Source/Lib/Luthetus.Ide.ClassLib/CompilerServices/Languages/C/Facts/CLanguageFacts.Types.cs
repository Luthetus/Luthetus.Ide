using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.Facts;

public partial class CLanguageFacts
{
    public class Types
    {
        public static readonly BoundClassDeclarationNode Int = new(
            new IdentifierToken(new TextEditorTextSpan(0, 0, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "int")),
            typeof(int),
            null,
            null,
            null);

        public static readonly BoundClassDeclarationNode String = new(
            new IdentifierToken(new TextEditorTextSpan(0, 0, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "string")),
            typeof(string),
            null,
            null,
            null);
        
        public static readonly BoundClassDeclarationNode Void = new(
            new IdentifierToken(new TextEditorTextSpan(0, 0, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "void")),
            typeof(void),
            null,
            null,
            null);
    }
}