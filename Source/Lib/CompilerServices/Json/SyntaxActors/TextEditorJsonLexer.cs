using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Json.SyntaxActors;

public class TextEditorJsonLexer : Lexer
{
    public TextEditorJsonLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            LexerKeywords.Empty)
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
            syntaxWalker.PropertyKeySyntaxes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.BooleanSyntaxes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.IntegerSyntaxes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.NullSyntaxes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.NumberSyntaxes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.StringSyntaxes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));
    }
}