using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;

public class TextEditorXmlLexer : LuthLexer
{
    public TextEditorXmlLexer(
            ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LuthLexerKeywords(ImmutableArray<string>.Empty, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty))
    {
    }

    public override void Lex()
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();
        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        // Tag Names
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.TagNameNodes.Select(x => new BadToken(x.TextEditorTextSpan)));

        // InjectedLanguageFragmentSyntaxes
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.InjectedLanguageFragmentNodes.Select(x => new BadToken(x.TextEditorTextSpan)));

        // Attribute Names
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeNameNodes.Select(x => new BadToken(x.TextEditorTextSpan)));

        // Attribute Values
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeValueNodes.Select(x => new BadToken(x.TextEditorTextSpan)));

        // Comments
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.CommentNodes.Select(x => new BadToken(x.TextEditorTextSpan)));
    }
}