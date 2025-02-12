using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public sealed class TerminalCompilerService : CompilerService
{
    public TerminalCompilerService(
            ITextEditorService textEditorService,
            ITerminalService terminalService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new TerminalResource(resourceUri, this),
        };
    }

    public override IReadOnlyList<TextEditorTextSpan> GetTokenTextSpansFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return Array.Empty<TextEditorTextSpan>();

        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return Array.Empty<TextEditorTextSpan>();

            return _resourceMap[resourceUri].GetTokenTextSpans();
        }
    }
}
