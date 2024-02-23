using Luthetus.CompilerServices.Lang.JavaScript.JavaScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.JavaScript;

public sealed class JavaScriptCompilerService : LuthCompilerService
{
    public JavaScriptCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new JavaScriptResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorJavaScriptLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}