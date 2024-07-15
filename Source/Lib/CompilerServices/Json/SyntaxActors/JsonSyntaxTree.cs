using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.CompilerServices.Json.SyntaxObjects;
using Luthetus.CompilerServices.Json.Facts;
using Luthetus.CompilerServices.Json.Decoration;

namespace Luthetus.CompilerServices.Json.SyntaxActors;

public class JsonSyntaxTree
{
    public static JsonSyntaxUnit ParseText(
        ResourceUri resourceUri,
        string content)
    {
        // Items to return wrapped in a JsonSyntaxUnit
        var jsonDocumentChildren = new List<IJsonSyntax>();
        var textEditorJsonDiagnosticBag = new DiagnosticBag();

        // Step through the string 'character by character'
        var stringWalker = new StringWalker(resourceUri, content);

        // Order matters with the methods of pattern, 'Consume{Something}'
        // Example: 'ConsumeComment'
        while (!stringWalker.IsEof)
        {
            if (stringWalker.CurrentCharacter == JsonFacts.OBJECT_START)
            {
                var jsonObjectSyntax = ConsumeObject(
                    stringWalker,
                    textEditorJsonDiagnosticBag);

                jsonDocumentChildren.Add(jsonObjectSyntax);
            }
            else if (stringWalker.CurrentCharacter == JsonFacts.ARRAY_START)
            {
                var jsonObjectSyntax = ConsumeArray(
                    stringWalker,
                    textEditorJsonDiagnosticBag);

                jsonDocumentChildren.Add(jsonObjectSyntax);
            }

            _ = stringWalker.ReadCharacter();
        }

        var jsonDocumentSyntax = new JsonDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.None,
                stringWalker.ResourceUri,
                stringWalker.SourceText),
            jsonDocumentChildren.ToImmutableArray());

        var jsonSyntaxUnit = new JsonSyntaxUnit(
            jsonDocumentSyntax,
            textEditorJsonDiagnosticBag);

        return jsonSyntaxUnit;
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.OBJECT_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.OBJECT_END"/><br/>
    /// </summary>
    private static JsonObjectSyntax ConsumeObject(
        StringWalker stringWalker,
        DiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        var jsonPropertySyntaxes = new List<JsonPropertySyntax>();

        // While loop state
        JsonPropertyKeySyntax? pendingJsonPropertyKeySyntax = null;
        var foundPropertyDelimiterBetweenKeyAndValue = false;
        JsonPropertyValueSyntax? pendingJsonPropertyValueSyntax = null;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            // Skip whitespace
            while (!stringWalker.IsEof)
            {
                if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
                    _ = stringWalker.ReadCharacter();
                else
                    break;
            }

            if (JsonFacts.OBJECT_END == stringWalker.CurrentCharacter)
                break;

            if (JsonFacts.PROPERTY_ENTRY_DELIMITER == stringWalker.CurrentCharacter)
                continue;

            if (pendingJsonPropertyKeySyntax is null)
            {
                pendingJsonPropertyKeySyntax = ConsumePropertyKey(
                    stringWalker,
                    diagnosticBag);
            }
            else if (!foundPropertyDelimiterBetweenKeyAndValue)
            {
                while (!stringWalker.IsEof)
                {
                    if (JsonFacts.PROPERTY_DELIMITER_BETWEEN_KEY_AND_VALUE != stringWalker.CurrentCharacter)
                        _ = stringWalker.ReadCharacter();
                    else
                        break;
                }

                // If Eof ended the loop to find the delimiter
                // the outer while loop will finish as well so
                // no EOF if is needed just set found to true
                foundPropertyDelimiterBetweenKeyAndValue = true;
            }
            else
            {
                pendingJsonPropertyValueSyntax = ConsumePropertyValue(
                    stringWalker,
                    diagnosticBag);

                var jsonPropertySyntax = new JsonPropertySyntax(
                    new TextEditorTextSpan(
                        startingPositionIndex,
                        stringWalker.PositionIndex,
                        (byte)JsonDecorationKind.PropertyKey,
                        stringWalker.ResourceUri,
                        stringWalker.SourceText),
                    pendingJsonPropertyKeySyntax,
                    pendingJsonPropertyValueSyntax);

                // Reset while loop state
                pendingJsonPropertyKeySyntax = null;
                foundPropertyDelimiterBetweenKeyAndValue = false;
                pendingJsonPropertyValueSyntax = null;

                jsonPropertySyntaxes.Add(jsonPropertySyntax);
            }
        }

        if (pendingJsonPropertyKeySyntax is not null)
        {
            // This is to mean the property value is
            // invalid in some regard
            //
            // Still however, render the syntax highlighting
            // for the valid property key.

            var jsonPropertySyntax = new JsonPropertySyntax(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.PropertyKey,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText),
                pendingJsonPropertyKeySyntax,
                JsonPropertyValueSyntax.GetInvalidJsonPropertyValueSyntax());

            jsonPropertySyntaxes.Add(jsonPropertySyntax);
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));
        }

        var jsonObjectSyntax = new JsonObjectSyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.None,
                stringWalker.ResourceUri,
                stringWalker.SourceText),
            jsonPropertySyntaxes.ToImmutableArray());

        return jsonObjectSyntax;
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.PROPERTY_KEY_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.PROPERTY_KEY_END"/><br/>
    /// </summary>
    private static JsonPropertyKeySyntax ConsumePropertyKey(
        StringWalker stringWalker,
        DiagnosticBag diagnosticBag)
    {
        // +1 to not include the quote that begins the key's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (JsonFacts.PROPERTY_KEY_END == stringWalker.CurrentCharacter)
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));
        }

        var jsonPropertyKey = new JsonPropertyKeySyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.PropertyKey,
                stringWalker.ResourceUri,
                stringWalker.SourceText),
            ImmutableArray<IJsonSyntax>.Empty);

        return jsonPropertyKey;
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// - Any character that is not <see cref="WhitespaceFacts.ALL_LIST"/> (whitespace)<br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.PROPERTY_ENTRY_DELIMITER"/><br/>
    /// - <see cref="WhitespaceFacts.ALL_LIST"/> (whitespace)<br/>
    /// - The <see cref="JsonFacts.OBJECT_END"/> of the object which contains the property value<br/>
    /// </summary>
    private static JsonPropertyValueSyntax ConsumePropertyValue(
        StringWalker stringWalker,
        DiagnosticBag diagnosticBag)
    {
        int startingPositionIndex = stringWalker.PositionIndex;

        IJsonSyntax underlyingJsonSyntax;

        if (stringWalker.CurrentCharacter == JsonFacts.ARRAY_START)
        {
            underlyingJsonSyntax = ConsumeArray(
                stringWalker,
                diagnosticBag);
        }
        else if (stringWalker.CurrentCharacter == JsonFacts.OBJECT_START)
        {
            underlyingJsonSyntax = ConsumeObject(
                stringWalker,
                diagnosticBag);
        }
        else
        {
            if (stringWalker.CurrentCharacter == JsonFacts.STRING_START)
            {
                underlyingJsonSyntax = ConsumeString(
                    stringWalker,
                    diagnosticBag);
            }
            else
            {
                underlyingJsonSyntax = ConsumeAmbiguousValue(
                    stringWalker,
                    diagnosticBag);
            }
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));
        }

        var jsonPropertyValue = new JsonPropertyValueSyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.None,
                stringWalker.ResourceUri,
                stringWalker.SourceText),
            underlyingJsonSyntax);

        return jsonPropertyValue;
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.ARRAY_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.ARRAY_END"/><br/>
    /// </summary>
    private static JsonArraySyntax ConsumeArray(
        StringWalker stringWalker,
        DiagnosticBag diagnosticBag)
    {
        // +1 to not include the bracket that begins this values's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;

        var jsonObjectSyntaxes = new List<JsonObjectSyntax>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            // Skip whitespace
            while (!stringWalker.IsEof)
            {
                if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
                    _ = stringWalker.ReadCharacter();
                else
                    break;
            }

            if (JsonFacts.ARRAY_END == stringWalker.CurrentCharacter)
                break;

            if (JsonFacts.ARRAY_ENTRY_DELIMITER == stringWalker.CurrentCharacter)
                continue;

            if (stringWalker.CurrentCharacter == JsonFacts.OBJECT_START)
            {
                var jsonObjectSyntax = ConsumeObject(
                    stringWalker,
                    diagnosticBag);

                jsonObjectSyntaxes.Add(jsonObjectSyntax);
            }
        }

        return new JsonArraySyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.None,
                stringWalker.ResourceUri,
                stringWalker.SourceText),
            jsonObjectSyntaxes.ToImmutableArray());
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.STRING_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.STRING_END"/><br/>
    /// </summary>
    private static JsonStringSyntax ConsumeString(
        StringWalker stringWalker,
        DiagnosticBag diagnosticBag)
    {
        // +1 to not include the quote that begins this values's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (JsonFacts.STRING_END == stringWalker.CurrentCharacter)
                break;
        }

        return new JsonStringSyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.String,
                stringWalker.ResourceUri,
                stringWalker.SourceText));
    }

    /// <summary>
    /// The JSON DataTypes which qualify as ambiguous are:<br/>
    /// -number<br/>
    /// -integer<br/>
    /// -boolean<br/>
    /// -null<br/>
    /// <br/>
    /// One must ensure the value cannot be of the following
    /// DataTypes prior to invoking this method:<br/>
    /// -array<br/>
    /// -object<br/>
    /// -string<br/>
    /// </summary>
    private static IJsonSyntax ConsumeAmbiguousValue(
        StringWalker stringWalker,
        DiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        var firstWordTuple = stringWalker.ReadWordTuple(new[]
        {
            ','
        }.ToImmutableArray());

        if (JsonFacts.NULL_STRING_VALUE == firstWordTuple.value)
        {
            return new JsonNullSyntax(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.Keyword,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));
        }
        else if (JsonFacts.BOOLEAN_ALL_STRING_VALUES.Contains(firstWordTuple.value))
        {
            return new JsonBooleanSyntax(new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.Keyword,
                stringWalker.ResourceUri,
                stringWalker.SourceText));
        }
        else
        {
            if (firstWordTuple.value.Contains(JsonFacts.NUMBER_DECIMAL_PLACE_SEPARATOR))
            {
                return new JsonNumberSyntax(new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.Number,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));
            }

            return new JsonIntegerSyntax(new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.Integer,
                stringWalker.ResourceUri,
                stringWalker.SourceText));
        }
    }
}