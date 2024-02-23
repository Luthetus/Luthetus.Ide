using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.C;

public sealed class CCompilerService : LuthCompilerService
{
    public CCompilerService(ITextEditorService textEditorService)
        : base(
            textEditorService,
            (resourceUri, sourceText) => new CLexer(resourceUri, sourceText),
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
                new CResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        OnResourceRegistered();
    }
}