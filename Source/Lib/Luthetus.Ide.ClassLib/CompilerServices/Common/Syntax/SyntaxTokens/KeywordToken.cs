﻿using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;

public sealed record KeywordToken : ISyntaxToken
{
    public KeywordToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.KeywordToken;
    public bool IsFabricated { get; init; }
}
