using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;

public class TextEditorJsonLexer : LuthLexer
{
    public TextEditorJsonLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            LuthLexerKeywords.Empty)
    {
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public override void Lex()
    {
        var jsonSyntaxUnit = JsonSyntaxTree.ParseText(
            ResourceUri,
            SourceText);
        
        var syntaxNodeRoot = jsonSyntaxUnit.JsonDocumentSyntax;

        var syntaxWalker = new JsonSyntaxWalker();
        syntaxWalker.Visit(syntaxNodeRoot);

        _syntaxTokenList.AddRange(
            syntaxWalker.PropertyKeySyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.BooleanSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.IntegerSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.NullSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.NumberSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.StringSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));
    }
}