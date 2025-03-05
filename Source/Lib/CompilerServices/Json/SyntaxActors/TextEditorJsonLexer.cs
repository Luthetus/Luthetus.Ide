using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Json.SyntaxActors;

public class TextEditorJsonLexer
{
	public static LexerKeywords LexerKeyWords = LexerKeywords.Empty;

    public TextEditorJsonLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

	public ResourceUri ResourceUri { get; }
	public string SourceText { get; }
	public List<SyntaxToken> SyntaxTokenList { get; } = new();

    public void Lex()
    {
        var jsonSyntaxUnit = JsonSyntaxTree.ParseText(
            ResourceUri,
            SourceText);
        
        var syntaxNodeRoot = jsonSyntaxUnit.JsonDocumentSyntax;

        var syntaxWalker = new JsonSyntaxWalker();
        syntaxWalker.Visit(syntaxNodeRoot);

        SyntaxTokenList.AddRange(
            syntaxWalker.PropertyKeySyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.BooleanSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.IntegerSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.NullSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.NumberSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.StringSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));
    }
}