using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public sealed class CSharpProjectCompilerService : LuthCompilerService
{
    public CSharpProjectCompilerService(
            ITextEditorService textEditorService)
        : base(
            textEditorService,
            (resourceUri, sourceText) => new TextEditorXmlLexer(resourceUri, sourceText),
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
                new CSharpProjectResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        OnResourceRegistered();
    }
}