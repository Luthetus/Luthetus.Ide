using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxActors;

public class TextEditorXmlLexer : Lexer
{
    public TextEditorXmlLexer(
            ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            LexerKeywords.Empty)
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