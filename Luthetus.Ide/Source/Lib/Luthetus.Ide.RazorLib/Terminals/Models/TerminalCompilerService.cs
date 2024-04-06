using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public sealed class TerminalCompilerService : LuthCompilerService
{
    public TerminalCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new TerminalResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TerminalLexer(resource.ResourceUri, sourceText),
        };
    }

    public override ImmutableArray<TextEditorTextSpan> GetTokenTextSpansFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return ImmutableArray<TextEditorTextSpan>.Empty;

        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _resourceMap[resourceUri].GetTokenTextSpans();
        }
    }
}
