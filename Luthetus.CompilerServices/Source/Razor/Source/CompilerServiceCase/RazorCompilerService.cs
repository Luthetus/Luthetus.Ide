using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;

public sealed class RazorCompilerService : LuthCompilerService
{
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;

    public RazorCompilerService(
            ITextEditorService textEditorService,
            CSharpCompilerService cSharpCompilerService,
            IEnvironmentProvider environmentProvider)
        : base(textEditorService)
    {
        _cSharpCompilerService = cSharpCompilerService;
        _environmentProvider = environmentProvider;

        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new RazorResource(resourceUri, this, _textEditorService),
            GetLexerFunc = (resource, sourceText) => 
            {
                ((RazorResource)resource).HtmlSymbols.Clear();

                return new RazorLexer(
                    resource.ResourceUri,
                    sourceText,
                    this,
                    _cSharpCompilerService,
                    _environmentProvider);
            },
            GetParserFunc = null,
            GetBinderFunc = null,
            OnAfterLexAction = (resource, lexer) =>
            {
                var razorResource = (RazorResource)resource;
                var razorLexer = (RazorLexer)lexer;

                razorResource.SyntaxTokenList = razorLexer.SyntaxTokenList;
                razorResource.RazorSyntaxTree = razorLexer.RazorSyntaxTree;
            },
            OnAfterParseAction = null,
        };
    }
}