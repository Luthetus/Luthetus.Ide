using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Types
    {
        public static readonly TypeDefinitionNode Void = new(
            new IdentifierToken(new TextEditorTextSpan(0, "void".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "void")),
            typeof(void),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Int = new(
            new IdentifierToken(new TextEditorTextSpan(0, "int".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "int")),
            typeof(int),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode String = new(
            new IdentifierToken(new TextEditorTextSpan(0, "string".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "string")),
            typeof(string),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Bool = new(
            new IdentifierToken(new TextEditorTextSpan(0, "bool".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "bool")),
            typeof(bool),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Var = new(
            new IdentifierToken(new TextEditorTextSpan(0, "var".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "var")),
            typeof(void),
            null,
            null,
            null);

        public static readonly ImmutableArray<TypeDefinitionNode> TypeDefinitionNodes = new[]
        {
            Void,
            Int,
            String,
            Bool,
            Var
        }.ToImmutableArray();
    }
}