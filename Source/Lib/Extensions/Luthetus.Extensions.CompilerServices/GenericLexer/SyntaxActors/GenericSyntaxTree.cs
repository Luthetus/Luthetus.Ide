using System.Text;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.GenericLexer;
using Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxObjects;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxActors;

public class GenericSyntaxTree
{
	public GenericSyntaxTree(GenericLanguageDefinition genericLanguageDefinition)
	{
		GenericLanguageDefinition = genericLanguageDefinition;
	}

	public GenericLanguageDefinition GenericLanguageDefinition { get; }

	public virtual GenericSyntaxUnit ParseText(ResourceUri resourceUri, string content)
	{
		var documentChildList = new List<IGenericSyntax>();
		var diagnosticList = new List<TextEditorDiagnostic>();

		var stringWalker = new StringWalker(resourceUri, content);

		while (!stringWalker.IsEof)
		{
			if (stringWalker.PeekForSubstring(GenericLanguageDefinition.StringStart))
			{
				var genericStringSyntax = ParseString(stringWalker, diagnosticList);
				documentChildList.Add(genericStringSyntax);
			}
			else if (stringWalker.PeekForSubstring(GenericLanguageDefinition.CommentSingleLineStart))
			{
				var genericCommentSingleLineSyntax = ParseCommentSingleLine(stringWalker, diagnosticList);
				documentChildList.Add(genericCommentSingleLineSyntax);
			}
			else if (stringWalker.PeekForSubstring(GenericLanguageDefinition.CommentMultiLineStart))
			{
				var genericCommentMultiLineSyntax = ParseCommentMultiLine(stringWalker, diagnosticList);
				documentChildList.Add(genericCommentMultiLineSyntax);
			}
			else if (stringWalker.PeekForSubstring(GenericLanguageDefinition.FunctionInvocationStart))
			{
				if (TryParseFunctionIdentifier(stringWalker, diagnosticList, out var genericFunctionSyntax) &&
					genericFunctionSyntax is not null)
				{
					documentChildList.Add(genericFunctionSyntax);
				}
			}
			else if (!WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter) &&
					 !KeyboardKeyFacts.PunctuationCharacters.AllList.Contains(stringWalker.CurrentCharacter))
			{
				if (TryParseKeyword(stringWalker, diagnosticList, out var genericKeywordSyntax) &&
					genericKeywordSyntax is not null)
				{
					documentChildList.Add(genericKeywordSyntax);
				}
			}
			else if (stringWalker.PeekForSubstring(GenericLanguageDefinition.PreprocessorDefinition.TransitionSubstring))
			{
				var genericCommentMultiLineSyntax = ParsePreprocessorDirective(stringWalker, diagnosticList);
				documentChildList.Add(genericCommentMultiLineSyntax);
			}

			_ = stringWalker.ReadCharacter();
		}

		var genericDocumentSyntax = new GenericDocumentSyntax(new TextEditorTextSpan(
				0,
				stringWalker.PositionIndex,
				(byte)GenericDecorationKind.None,
				stringWalker.ResourceUri,
				stringWalker.SourceText),
			documentChildList);

		return new GenericSyntaxUnit(genericDocumentSyntax, diagnosticList);
	}

	public virtual GenericCommentSingleLineSyntax ParseCommentSingleLine(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList)
	{
		var startingPositionIndex = stringWalker.PositionIndex;

		while (!stringWalker.IsEof)
		{
			_ = stringWalker.ReadCharacter();

			if (stringWalker.PeekForSubstringRange(
					GenericLanguageDefinition.CommentSingleLineEndingsList,
					out _))
			{
				break;
			}
		}

		if (stringWalker.IsEof)
		{
			/*diagnosticList.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)GenericDecorationKind.Error,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));*/
		}

		var commentTextEditorTextSpan = new TextEditorTextSpan(
			startingPositionIndex,
			stringWalker.PositionIndex,
			(byte)GenericDecorationKind.CommentSingleLine,
			stringWalker.ResourceUri,
			stringWalker.SourceText);

		return new GenericCommentSingleLineSyntax(commentTextEditorTextSpan);
	}

	public virtual GenericCommentMultiLineSyntax ParseCommentMultiLine(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList)
	{
		var startingPositionIndex = stringWalker.PositionIndex;

		while (!stringWalker.IsEof)
		{
			_ = stringWalker.ReadCharacter();

			if (stringWalker.PeekForSubstring(GenericLanguageDefinition.CommentMultiLineEnd))
				break;
		}

		if (stringWalker.IsEof)
		{
			/*diagnosticList.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)GenericDecorationKind.Error,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));*/
		}

		var commentTextEditorTextSpan = new TextEditorTextSpan(
			startingPositionIndex,
			stringWalker.PositionIndex + GenericLanguageDefinition.CommentMultiLineEnd.Length,
			(byte)GenericDecorationKind.CommentMultiLine,
			stringWalker.ResourceUri,
			stringWalker.SourceText);

		return new GenericCommentMultiLineSyntax(commentTextEditorTextSpan);
	}

	public virtual GenericStringSyntax ParseString(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList)
	{
		var startingPositionIndex = stringWalker.PositionIndex;

		while (!stringWalker.IsEof)
		{
			_ = stringWalker.ReadCharacter();

			if (stringWalker.PeekForSubstring(GenericLanguageDefinition.StringEnd))
				break;
		}

		if (stringWalker.IsEof)
		{
			/*diagnosticList.ReportEndOfFileUnexpected(new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)GenericDecorationKind.Error,
                stringWalker.ResourceUri,
                stringWalker.SourceText));*/
		}

		var stringTextEditorTextSpan = new TextEditorTextSpan(
			startingPositionIndex,
			stringWalker.PositionIndex + 1,
			(byte)GenericDecorationKind.StringLiteral,
			stringWalker.ResourceUri,
			stringWalker.SourceText);

		return new GenericStringSyntax(stringTextEditorTextSpan);
	}

	/// <summary>
	/// currentCharacterIn:<br/>
	/// -Any CurrentCharacter value is valid as this method is 'try'
	/// </summary>
	private bool TryParseKeyword(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList,
		out GenericKeywordSyntax? genericKeywordSyntax)
	{
		var startingPositionIndex = stringWalker.PositionIndex;

		while (!stringWalker.IsEof)
		{
			var backtrackedToCharacter = stringWalker.BacktrackCharacter();

			if (backtrackedToCharacter == ParserFacts.END_OF_FILE)
				break;

			if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter) ||
				KeyboardKeyFacts.PunctuationCharacters.AllList.Contains(stringWalker.CurrentCharacter))
			{
				_ = stringWalker.ReadCharacter();
				break;
			}
		}

		var wordTuple = stringWalker.ReadWordTuple(KeyboardKeyFacts.PunctuationCharacters.AllList);

		var foundKeyword = GenericLanguageDefinition.KeywordsList.FirstOrDefault(
			keyword => keyword == wordTuple.value);

		if (foundKeyword is not null)
		{
			stringWalker.BacktrackCharacter();

			genericKeywordSyntax = new GenericKeywordSyntax(wordTuple.textSpan with
			{
				DecorationByte = (byte)GenericDecorationKind.Keyword
			});

			return true;
		}

		if (wordTuple.textSpan.StartInclusiveIndex != -1)
		{
			// backtrack to the original starting position
			stringWalker.BacktrackRange(stringWalker.PositionIndex - startingPositionIndex);
		}

		genericKeywordSyntax = null;
		return false;
	}

	private bool TryParseFunctionIdentifier(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList,
		out GenericFunctionSyntax? genericFunctionSyntax)
	{
		var rememberPositionIndex = stringWalker.PositionIndex;

		bool startedReadingWord = false;

		var wordBuilder = new StringBuilder();

		// Enter here at '('
		while (!stringWalker.IsEof)
		{
			var backtrackedToCharacter = stringWalker.BacktrackCharacter();

			if (backtrackedToCharacter == ParserFacts.END_OF_FILE)
				break;

			if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
			{
				if (startedReadingWord)
					break;
			}
			else if (KeyboardKeyFacts.IsPunctuationCharacter(stringWalker.CurrentCharacter) ||
					 stringWalker.PeekForSubstring(GenericLanguageDefinition.MemberAccessToken) ||
					 stringWalker.PeekForSubstring(GenericLanguageDefinition.FunctionInvocationEnd))
			{
				break;
			}
			else
			{
				startedReadingWord = true;
				wordBuilder.Insert(0, stringWalker.CurrentCharacter);
			}
		}

		var word = wordBuilder.ToString();

		if (word.Length == 0 || char.IsDigit(word[0]))
		{
			genericFunctionSyntax = null;

			_ = stringWalker.ReadRange(rememberPositionIndex - stringWalker.PositionIndex);
			return false;
		}

		genericFunctionSyntax = new GenericFunctionSyntax(
			new TextEditorTextSpan(
				stringWalker.PositionIndex + 1,
				rememberPositionIndex,
				(byte)GenericDecorationKind.Function,
				stringWalker.ResourceUri,
				stringWalker.SourceText));

		_ = stringWalker.ReadRange(rememberPositionIndex - stringWalker.PositionIndex);
		return true;
	}

	private GenericPreprocessorDirectiveSyntax ParsePreprocessorDirective(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList)
	{
		var startingPositionIndex = stringWalker.PositionIndex;

		_ = stringWalker.ReadRange(
			GenericLanguageDefinition.PreprocessorDefinition.TransitionSubstring.Length);

		while (!stringWalker.IsEof)
		{
			if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
			{
				_ = stringWalker.ReadCharacter();
				continue;
			}

			break;
		}

		var identifierBuilder = new StringBuilder();

		while (!stringWalker.IsEof)
		{
			if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
				break;

			identifierBuilder.Append(stringWalker.CurrentCharacter);

			_ = stringWalker.ReadCharacter();
		}

		var textSpan = new TextEditorTextSpan(
			startingPositionIndex,
			stringWalker.PositionIndex,
			(byte)GenericDecorationKind.PreprocessorDirective,
			stringWalker.ResourceUri,
			stringWalker.SourceText);

		var success = TryParsePreprocessorDirectiveDeliminationExtendedSyntaxes(
			stringWalker,
			diagnosticList,
			out var genericSyntax);

		var children = success && genericSyntax is not null
			? new[] { genericSyntax }
			: Array.Empty<IGenericSyntax>();

		return new GenericPreprocessorDirectiveSyntax(textSpan, children);
	}

	private bool TryParsePreprocessorDirectiveDeliminationExtendedSyntaxes(
		StringWalker stringWalker,
		List<TextEditorDiagnostic> diagnosticList,
		out IGenericSyntax? genericSyntax)
	{
		var entryPositionIndex = stringWalker.PositionIndex;

		while (!stringWalker.IsEof)
		{
			if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
			{
				_ = stringWalker.ReadCharacter();
				continue;
			}

			break;
		}

		DeliminationExtendedSyntaxDefinition? matchedDeliminationExtendedSyntax = null;

		foreach (var deliminationExtendedSyntax in GenericLanguageDefinition.PreprocessorDefinition.DeliminationExtendedSyntaxList)
		{
			if (stringWalker.PeekForSubstring(deliminationExtendedSyntax.SyntaxStart))
			{
				matchedDeliminationExtendedSyntax = deliminationExtendedSyntax;
				break;
			}
		}

		if (matchedDeliminationExtendedSyntax is not null)
		{
			var deliminationExtendedSyntaxStartingInclusiveIndex = stringWalker.PositionIndex;
			var deliminationExtendedSyntaxBuilder = new StringBuilder();

			while (!stringWalker.IsEof)
			{
				if (stringWalker.PeekForSubstring(matchedDeliminationExtendedSyntax.SyntaxEnd))
				{
					_ = stringWalker.ReadCharacter();
					break;
				}

				deliminationExtendedSyntaxBuilder.Append(stringWalker.CurrentCharacter);

				_ = stringWalker.ReadCharacter();
			}

			var textSpan = new TextEditorTextSpan(
				deliminationExtendedSyntaxStartingInclusiveIndex,
				stringWalker.PositionIndex,
				(byte)matchedDeliminationExtendedSyntax.GenericDecorationKind,
				stringWalker.ResourceUri,
				stringWalker.SourceText);

			genericSyntax = new GenericDeliminationExtendedSyntax(textSpan);

			return true;
		}

		stringWalker.BacktrackRange(stringWalker.PositionIndex - entryPositionIndex);

		genericSyntax = null;
		return false;
	}
}