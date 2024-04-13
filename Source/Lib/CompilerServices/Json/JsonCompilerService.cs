using Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.CompilerServices.Lang.Json;

public sealed class JsonCompilerService : LuthCompilerService
{
    public JsonCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new JsonResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorJsonLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}