using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Razor.Razor.Facts;
using Luthetus.CompilerServices.Lang.Xml.Html.InjectedLanguage;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Razor;

public class RazorLexer : ILuthLexer
{
    private readonly StringWalker _stringWalker;
    private readonly List<ISyntaxToken> _syntaxTokens = new();
    private readonly LuthDiagnosticBag _diagnosticBag = new();
    private readonly RazorCompilerService _razorCompilerService;
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;

    public RazorLexer(
        ResourceUri resourceUri,
        string sourceText,
        RazorCompilerService razorCompilerService,
        CSharpCompilerService cSharpCompilerService,
        IEnvironmentProvider environmentProvider)
    {
        _stringWalker = new(resourceUri, sourceText);
        _environmentProvider = environmentProvider;

        ResourceUri = resourceUri;
        _razorCompilerService = razorCompilerService;
        _cSharpCompilerService = cSharpCompilerService;
        RazorSyntaxTree = new RazorSyntaxTree(ResourceUri, _razorCompilerService, _cSharpCompilerService, _environmentProvider);
    }

    public ResourceUri ResourceUri { get; }

    public RazorSyntaxTree RazorSyntaxTree { get; private set; }

    public ImmutableArray<ISyntaxToken> SyntaxTokens => _syntaxTokens.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList => _diagnosticBag.ToImmutableArray();

    public ImmutableArray<TextEditorTextSpan> TextEditorTextSpans { get; private set; }

    string ILuthLexer.SourceText => throw new NotImplementedException();

    public void Lex()
    {
        RazorSyntaxTree = new RazorSyntaxTree(ResourceUri, _razorCompilerService, _cSharpCompilerService, _environmentProvider);

        InjectedLanguageDefinition razorInjectedLanguageDefinition = new(
            RazorFacts.TRANSITION_SUBSTRING,
            RazorFacts.TRANSITION_SUBSTRING_ESCAPED,
            RazorSyntaxTree.ParseInjectedLanguageFragment,
            RazorSyntaxTree.ParseTagName,
            RazorSyntaxTree.ParseAttributeName,
            RazorSyntaxTree.ParseAttributeValue);

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            _stringWalker.SourceText,
            razorInjectedLanguageDefinition);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameNodes
                .Select(tns => tns.TextEditorTextSpan));
        }

        // InjectedLanguageFragmentSyntaxes
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.InjectedLanguageFragmentNodes
                .Select(ilfs => ilfs.TextEditorTextSpan));
        }

        // Attribute Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeNameNodes
                .Select(an => an.TextEditorTextSpan));
        }

        // Attribute Values
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeValueNodes
                .Select(av => av.TextEditorTextSpan));
        }

        // Comments
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.CommentNodes
                .Select(c => c.TextEditorTextSpan));
        }

        TextEditorTextSpans = textEditorTextSpans.ToImmutableArray();

        RazorSyntaxTree.ParseCodebehind();
    }
}