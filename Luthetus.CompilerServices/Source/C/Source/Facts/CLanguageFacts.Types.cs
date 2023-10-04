// TODO: All C logic is commented out due to breaking changes in the TextEditor API. The...
// ...C# compiler service is the focus while API decisions are being made, lest twice the code...
// ...need be modified for every API change (2023-10-04)
//
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.GenericLexer.Decoration;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.SyntaxNodes;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.SyntaxTokens;
//using Luthetus.TextEditor.RazorLib.Lexing;

//namespace Luthetus.CompilerServices.Lang.C.Facts;

//public partial class CLanguageFacts
//{
//    public class Types
//    {
//        public static readonly TypeDefinitionNode Int = new(
//            new IdentifierToken(new TextEditorTextSpan(0, 0, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "int")),
//            null);

//        public static readonly TypeDefinitionNode String = new(
//            new IdentifierToken(new TextEditorTextSpan(0, 0, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "string")),
//            null);

//        public static readonly TypeDefinitionNode Void = new(
//            new IdentifierToken(new TextEditorTextSpan(0, 0, (byte)GenericDecorationKind.None, new ResourceUri(string.Empty), "void")),
//            null);
//    }
//}