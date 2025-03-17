using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class Types
    {
        public static readonly TypeDefinitionNode Void = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "void".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "void")),
            typeof(void),
            null,
            primaryConstructorFunctionArgumentListing: default,
            null,
            string.Empty)
            {
            	IsKeywordType = true
            };

        public static readonly TypeDefinitionNode Int = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "int".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "int")),
            typeof(int),
            null,
            primaryConstructorFunctionArgumentListing: default,
            null,
            string.Empty)
            {
            	IsKeywordType = true
            };

        public static readonly TypeDefinitionNode Char = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "char".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "char")),
            typeof(char),
            null,
            primaryConstructorFunctionArgumentListing: default,
            null,
            string.Empty)
            {
            	IsKeywordType = true
            };

        public static readonly TypeDefinitionNode String = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "string".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "string")),
            typeof(string),
            null,
            primaryConstructorFunctionArgumentListing: default,
            null,
            string.Empty)
            {
            	IsKeywordType = true
            };

        public static readonly TypeDefinitionNode Bool = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "bool".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "bool")),
            typeof(bool),
            null,
            primaryConstructorFunctionArgumentListing: default,
            null,
            string.Empty)
            {
            	IsKeywordType = true
            };

        public static readonly TypeDefinitionNode Var = new(
            AccessModifierKind.Public,
            false,
            StorageModifierKind.Class,
            new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "var".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "var")),
            typeof(void),
            null,
            primaryConstructorFunctionArgumentListing: default,
            null,
            string.Empty)
            {
            	IsKeywordType = true
            };

        public static readonly IReadOnlyList<TypeDefinitionNode> TypeDefinitionNodes = new[]
        {
            Void,
            Int,
            String,
            Bool,
            Var
        }.ToList();
    }
}