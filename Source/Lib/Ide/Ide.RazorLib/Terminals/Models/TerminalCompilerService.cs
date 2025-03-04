using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

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

    public override IReadOnlyList<SyntaxToken> GetTokensFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return Array.Empty<SyntaxToken>();

        lock (_resourceMapLock)
        {
            if (!_resourceMap.ContainsKey(resourceUri))
                return Array.Empty<SyntaxToken>();

            return _resourceMap[resourceUri].GetTokens();
        }
    }
}
