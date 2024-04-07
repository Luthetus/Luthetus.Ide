using Fluxor;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public sealed class TerminalCompilerService : LuthCompilerService
{
    public TerminalCompilerService(
            ITextEditorService textEditorService,
            IState<TerminalSessionState> terminalSessionStateWrap)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new TerminalResource(resourceUri, this, terminalSessionStateWrap),
            GetLexerFunc = (resource, sourceText) => new TerminalLexer((TerminalResource)resource, sourceText),
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
