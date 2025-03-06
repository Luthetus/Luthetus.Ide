using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.CompilerServices.Css.Decoration;
using Luthetus.CompilerServices.Css.SyntaxEnums;
using Luthetus.CompilerServices.Css.Facts;
using Luthetus.CompilerServices.Css.SyntaxObjects;

namespace Luthetus.CompilerServices.Css.SyntaxActors;

public class CssSyntaxTree
{
    public static CssSyntaxUnit ParseText(
        ResourceUri resourceUri,
        string sourceText)
    {
        // Items to return wrapped in a CssSyntaxUnit
        var cssDocumentChildren = new List<ICssSyntax>();
        var textEditorCssDiagnosticBag = new List<TextEditorDiagnostic>();

        // Step through the string 'character by character'
        var stringWalker = new StringWalker(resourceUri, sourceText);

        // Order matters with the methods of pattern, 'Consume{Something}'
        // Example: 'ConsumeComment'
        while (!stringWalker.IsEof)
        {
            if (char.IsLetterOrDigit(stringWalker.CurrentCharacter))
                ConsumeIdentifier(stringWalker, cssDocumentChildren, textEditorCssDiagnosticBag);

            if (stringWalker.PeekForSubstring(CssFacts.COMMENT_START))
                ConsumeComment(stringWalker, cssDocumentChildren, textEditorCssDiagnosticBag);

            if (stringWalker.CurrentCharacter == CssFacts.STYLE_BLOCK_START)
                ConsumeStyleBlock(stringWalker, cssDocumentChildren, textEditorCssDiagnosticBag);

            _ = stringWalker.ReadCharacter();
        }

        var cssDocumentSyntax = new CssDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)CssDecorationKind.None,
                stringWalker.ResourceUri,
                stringWalker.SourceText),
            cssDocumentChildren);

        var cssSyntaxUnit = new CssSyntaxUnit(
            cssDocumentSyntax,
            textEditorCssDiagnosticBag);

        return cssSyntaxUnit;
    }

    /// <summary>
    /// <see cref="ConsumeComment"/> will immediately invoke
    /// <see cref="StringWalker.ReadCharacter"/> once
    ///  invoked.
    /// </summary>
    private static void ConsumeComment(
        StringWalker stringWalker,
        List<ICssSyntax> cssDocumentChildren,
        List<TextEditorDiagnostic> diagnosticList)
    {
        var commentStartingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            var closingOfCommentTextFound = stringWalker
                .PeekForSubstring(CssFacts.COMMENT_END);

            if (closingOfCommentTextFound)
            {
                // Skip the rest of the comment closing text
                _ = stringWalker.ReadRange(CssFacts.COMMENT_END.Length - 1);

                var commentTextSpan = new TextEditorTextSpan(
                    commentStartingPositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)CssDecorationKind.Comment,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText);

                var commentToken = new CssCommentSyntax(
                    commentTextSpan,
					Array.Empty<ICssSyntax>());

                cssDocumentChildren.Add(commentToken);

                return;
            }
        }
    }

    /// <summary>
    /// <see cref="ConsumeStyleBlock"/> will immediately invoke
    /// <see cref="StringWalker.ReadCharacter"/> once
    ///  invoked.
    /// </summary>
    private static void ConsumeStyleBlock(
        StringWalker stringWalker,
        List<ICssSyntax> cssDocumentChildren,
        List<TextEditorDiagnostic> diagnosticList)
    {
        var expectedStyleBlockChild = CssSyntaxKind.PropertyName;

        // when pendingChildStartingPositionIndex == -1 it is to
        // mean that there is NOT a pending child
        var pendingChildStartingPositionIndex = -1;

        var textOfChildAlreadyFound = false;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (pendingChildStartingPositionIndex == -1)
            {
                if (stringWalker.CurrentCharacter == CssFacts.STYLE_BLOCK_END)
                    break;

                if (stringWalker.PeekForSubstring(CssFacts.COMMENT_START))
                {
                    ConsumeComment(stringWalker, cssDocumentChildren, diagnosticList);
                    continue;
                }
            }

            char childEndingCharacter;
            CssDecorationKind childDecorationKind;

            switch (expectedStyleBlockChild)
            {
                case CssSyntaxKind.PropertyName:
                    childEndingCharacter = CssFacts.PROPERTY_NAME_END;
                    childDecorationKind = CssDecorationKind.PropertyName;
                    break;
                case CssSyntaxKind.PropertyValue:
                    childEndingCharacter = CssFacts.PROPERTY_VALUE_END;
                    childDecorationKind = CssDecorationKind.PropertyValue;
                    break;
                default:
                    throw new LuthetusTextEditorException($"The {nameof(CssSyntaxKind)} of" +
                                                   $" {expectedStyleBlockChild} was unexpected.");
            }

            // Skip preceding and trailing whitespace
            // relative to the child's text
            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter) &&
                pendingChildStartingPositionIndex == -1)
            {
                continue;
            }

            // Start of a child's text
            if (pendingChildStartingPositionIndex == -1)
            {
                pendingChildStartingPositionIndex = stringWalker.PositionIndex;
                continue;
            }

            // End of a child's text
            if (!textOfChildAlreadyFound &&
                stringWalker.CurrentCharacter == childEndingCharacter)
            {
                var childTextSpan = new TextEditorTextSpan(
                    pendingChildStartingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)childDecorationKind,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText);

                ICssSyntax childSyntax;

                switch (expectedStyleBlockChild)
                {
                    case CssSyntaxKind.PropertyName:
                        childSyntax = new CssPropertyNameSyntax(
                            childTextSpan,
                            Array.Empty<ICssSyntax>());
                        break;
                    case CssSyntaxKind.PropertyValue:
                        childSyntax = new CssPropertyValueSyntax(
                            childTextSpan,
                            Array.Empty<ICssSyntax>());
                        break;
                    default:
                        throw new LuthetusTextEditorException($"The {nameof(CssSyntaxKind)} of" +
                                                       $" {expectedStyleBlockChild} was unexpected.");
                }

                cssDocumentChildren.Add(childSyntax);

                textOfChildAlreadyFound = true;
            }

            // Clear and ready state for finding the next expected child
            if (stringWalker.CurrentCharacter == childEndingCharacter)
            {
                pendingChildStartingPositionIndex = -1;
                textOfChildAlreadyFound = false;

                switch (expectedStyleBlockChild)
                {
                    case CssSyntaxKind.PropertyName:
                        expectedStyleBlockChild = CssSyntaxKind.PropertyValue;
                        break;
                    case CssSyntaxKind.PropertyValue:
                        expectedStyleBlockChild = CssSyntaxKind.PropertyName;
                        break;
                    default:
                        throw new LuthetusTextEditorException($"The {nameof(CssSyntaxKind)} of" +
                                                       $" {expectedStyleBlockChild} was unexpected.");
                }
            }

            // Relies on the if statement before this that ensures
            // the current character is not whitespace
            if (stringWalker.CurrentCharacter != childEndingCharacter)
            {
                var unexpectedTokenTextSpan = new TextEditorTextSpan(
                    pendingChildStartingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)CssDecorationKind.UnexpectedToken,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText);

                /*diagnosticBag.ReportUnexpectedToken(
                    unexpectedTokenTextSpan,
                    stringWalker.CurrentCharacter.ToString());*/

                continue;
            }
        }
    }

    /// <summary>
    /// <see cref="ConsumeIdentifier"/> firstly grabs the
    /// starting position index for the identifier.
    /// Afterwards it invokes <see cref="StringWalker.ReadCharacter"/>.
    /// </summary>
    private static void ConsumeIdentifier(
        StringWalker stringWalker,
        List<ICssSyntax> cssDocumentChildren,
        List<TextEditorDiagnostic> diagnosticList)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter) ||
                CssFacts.STYLE_BLOCK_START == stringWalker.CurrentCharacter)
            {
                break;
            }
        }

        var identifierTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            (byte)CssDecorationKind.Identifier,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        var identifierSyntax = new CssIdentifierSyntax(
            identifierTextSpan,
            Array.Empty<ICssSyntax>());

        cssDocumentChildren.Add(identifierSyntax);
    }
}