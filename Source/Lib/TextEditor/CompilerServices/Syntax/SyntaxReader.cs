using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public static class SyntaxReader
{
    public static TextEditorTextSpan ConstructTextSpanRecursively(this ISyntaxNode node)
    {
        int? minStartingIndexInclusive = null;
        int? maxEndingIndexExclusive = null;
        ResourceUri? resourceUri = null;
        string? sourceText = null;

        foreach (var child in node.ChildList)
        {
            if (child is ISyntaxToken token)
            {
                minStartingIndexInclusive = minStartingIndexInclusive is null
                    ? token.TextSpan.StartingIndexInclusive
                    : Math.Min(token.TextSpan.StartingIndexInclusive, minStartingIndexInclusive.Value);

                maxEndingIndexExclusive = maxEndingIndexExclusive is null
                    ? token.TextSpan.EndingIndexExclusive
                    : Math.Min(token.TextSpan.EndingIndexExclusive, maxEndingIndexExclusive.Value);

                resourceUri ??= token.TextSpan.ResourceUri;
                sourceText ??= token.TextSpan.SourceText;
            }
        }

        if (minStartingIndexInclusive is null || maxEndingIndexExclusive is null)
        {
            minStartingIndexInclusive = 0;
            maxEndingIndexExclusive = 1;
        }

        return new TextEditorTextSpan(
            minStartingIndexInclusive.Value,
            maxEndingIndexExclusive.Value,
            (byte)GenericDecorationKind.None,
            resourceUri ?? new ResourceUri(string.Empty),
            sourceText ?? string.Empty);
    }
}
