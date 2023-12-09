using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Finds.Models;

namespace Luthetus.TextEditor.Tests.Basis.Finds.States;

public partial class TextEditorFindProviderStateTests
{
    public record RegisterAction(ITextEditorFindProvider FindProvider);
    public record DisposeAction(Key<ITextEditorFindProvider> FindProviderKey);
    public record SetActiveFindProviderAction(Key<ITextEditorFindProvider> FindProviderKey);
    public record SetSearchQueryAction(string SearchQuery);
}