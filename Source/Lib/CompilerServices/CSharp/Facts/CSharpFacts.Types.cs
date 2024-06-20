using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

public partial class CSharpFacts
{
    public class Types
    {
        public static readonly TypeDefinitionNode Void = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "void".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "void")),
            typeof(void),
            null,
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Int = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "int".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "int")),
            typeof(int),
            null,
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Char = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "char".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "char")),
            typeof(char),
            null,
            null,
            null,
            null);

        public static readonly TypeDefinitionNode String = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "string".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "string")),
            typeof(string),
            null,
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Bool = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "bool".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "bool")),
            typeof(bool),
            null,
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Var = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "var".Length, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "var")),
            typeof(void),
            null,
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