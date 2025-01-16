using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class Types
    {
        public static readonly TypeDefinitionNode Void = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "void".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "void")),
            typeof(void),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Int = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "int".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "int")),
            typeof(int),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Char = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "char".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "char")),
            typeof(char),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode String = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "string".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "string")),
            typeof(string),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Bool = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "bool".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "bool")),
            typeof(bool),
            null,
            null,
            null);

        public static readonly TypeDefinitionNode Var = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new IdentifierToken(new TextEditorTextSpan(0, "var".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "var")),
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