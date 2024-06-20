namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

public enum GenericDecorationKind
{
    None,
    Keyword,
    KeywordControl,
    CommentSingleLine,
    CommentMultiLine,
    Error,
	EscapeCharacter,
    StringLiteral,
    Variable,
    Function,
    PreprocessorDirective,
    DeliminationExtended,
    Type,
    Field,
    Property,
}