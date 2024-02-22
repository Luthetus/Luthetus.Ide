using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

public partial class TextEditorFindAllState
{
    public record RegisterAction(ITextEditorSearchEngine SearchEngine);
    public record DisposeAction(Key<ITextEditorSearchEngine> SearchEngineKey);
    public record SetSearchQueryAction(string SearchQuery);
    public record SetStartingDirectoryPathAction(string StartingDirectoryPath);
}