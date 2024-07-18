using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.JavaScript.SyntaxActors;

namespace Luthetus.CompilerServices.JavaScript;

public sealed class JavaScriptCompilerService : CompilerService
{
    public JavaScriptCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new JavaScriptResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorJavaScriptLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new Parser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}