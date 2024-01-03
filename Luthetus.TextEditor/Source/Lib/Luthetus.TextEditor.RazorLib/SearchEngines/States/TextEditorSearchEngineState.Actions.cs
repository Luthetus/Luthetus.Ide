using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.States;

public partial class TextEditorSearchEngineState
{
    public record RegisterAction(ITextEditorSearchEngine SearchEngine);
    public record DisposeAction(Key<ITextEditorSearchEngine> SearchEngineKey);
    public record SetActiveSearchEngineAction(Key<ITextEditorSearchEngine> SearchEngineKey);
    public record SetSearchQueryAction(string SearchQuery);
}