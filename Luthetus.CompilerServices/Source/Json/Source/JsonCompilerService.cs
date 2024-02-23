using Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.Json;

public sealed class JsonCompilerService : LuthCompilerService
{
    public JsonCompilerService(ITextEditorService textEditorService)
        : base(
            textEditorService,
            (resourceUri, sourceText) => new TextEditorJsonLexer(resourceUri, sourceText),
            lexer => new LuthParser(lexer))
    {
    }

    public override void RegisterResource(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            _resourceMap.Add(
                resourceUri,
                new JsonResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        OnResourceRegistered();
    }
}