namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public enum GenericDecorationKind
{
	None,
	Keyword,
	KeywordControl,
	CommentSingleLine,
	CommentMultiLine,
	Error,
	EscapeCharacterPrimary, // Two consecutive escape characters can be visually distinct by alternating this and 'EscapeCharacterSecondary'
	EscapeCharacterSecondary, // Two consecutive escape characters can be visually distinct by alternating this and 'EscapeCharacterPrimary'
	StringLiteral,
	Variable,
	Function,
	PreprocessorDirective,
	DeliminationExtended,
	Type,
	Field,
	Property,
}