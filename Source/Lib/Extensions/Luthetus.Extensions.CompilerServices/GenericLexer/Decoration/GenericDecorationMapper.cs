using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Extensions.CompilerServices.GenericLexer.Decoration;

public class GenericDecorationMapper : IDecorationMapper
{
	public string Map(byte decorationByte)
	{
		var decoration = (GenericDecorationKind)decorationByte;

		return decoration switch
		{
			GenericDecorationKind.None => string.Empty,
			GenericDecorationKind.Keyword => "luth_te_keyword",
			GenericDecorationKind.KeywordControl => "luth_te_keyword-control",
			GenericDecorationKind.EscapeCharacterPrimary => "luth_te_string-literal-escape-character",
			GenericDecorationKind.EscapeCharacterSecondary => "luth_te_string-literal-escape-character luth_te_string-literal-escape-character-secondary",
			GenericDecorationKind.StringLiteral => "luth_te_string-literal",
			GenericDecorationKind.Variable => "luth_te_variable",
			GenericDecorationKind.CommentSingleLine => "luth_te_comment",
			GenericDecorationKind.CommentMultiLine => "luth_te_comment",
			GenericDecorationKind.Function => "luth_te_method",
			GenericDecorationKind.PreprocessorDirective => "luth_te_preprocessor-directive",
			GenericDecorationKind.DeliminationExtended => "luth_te_string-literal",
			GenericDecorationKind.Type => "luth_te_type",
			_ => string.Empty,
		};
	}
}