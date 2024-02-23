using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public sealed class CSharpProjectCompilerService : LuthCompilerService
{
    public CSharpProjectCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            GetLexerFunc = (resource, sourceText) => new TextEditorXmlLexer(resource.ResourceUri, sourceText),
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
                new CSharpProjectResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        OnResourceRegistered();
    }
}