using Luthetus.CompilerServices.Lang.JavaScript.JavaScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.JavaScript;

public sealed class JavaScriptCompilerService : LuthCompilerService
{
    public JavaScriptCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            GetLexerFunc = (resource, sourceText) => new TextEditorJavaScriptLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }

    public override void RegisterResource(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            _resourceMap.Add(
                resourceUri,
                new JavaScriptResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        OnResourceRegistered();
    }
}