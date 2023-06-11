using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Types
    {
        public static readonly BoundClassDefinitionNode Void = new(
            new IdentifierToken(new TextEditorTextSpan(0, "void".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "void")),
            typeof(void),
            null,
            null,
            null);

        public static readonly BoundClassDefinitionNode Int = new(
            new IdentifierToken(new TextEditorTextSpan(0, "int".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "int")),
            typeof(int),
            null,
            null,
            null);

        public static readonly BoundClassDefinitionNode String = new(
            new IdentifierToken(new TextEditorTextSpan(0, "string".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "string")),
            typeof(string),
            null,
            null,
            null);

        public static readonly BoundClassDefinitionNode Bool = new(
            new IdentifierToken(new TextEditorTextSpan(0, "bool".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "bool")),
            typeof(bool),
            null,
            null,
            null);

        public static readonly BoundClassDefinitionNode Var = new(
            new IdentifierToken(new TextEditorTextSpan(0, "var".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "var")),
            typeof(void),
            null,
            null,
            null);
    }
}