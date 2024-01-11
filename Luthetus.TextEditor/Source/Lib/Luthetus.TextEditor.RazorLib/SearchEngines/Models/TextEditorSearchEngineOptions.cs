namespace Luthetus.TextEditor.RazorLib.SearchEngines.Models;

public class TextEditorSearchEngineOptions
{
    public TextEditorSearchEngineOptions()
    {
        MatchCase = new("Match case");
        MatchWholeWord = new("Match whole word");
        UseRegularExpressions = new("Use regular expressions");
        IncludeExternalItems = new("Include external items");
        IncludeMiscellaneousFiles = new("Include miscellaneous files");
        AppendResults = new("Append Results");
    }

    public TextEditorSearchEngineOptionsBoolean MatchCase { get; }
    public TextEditorSearchEngineOptionsBoolean MatchWholeWord { get; }
    public TextEditorSearchEngineOptionsBoolean UseRegularExpressions { get; }
    public TextEditorSearchEngineOptionsBoolean IncludeExternalItems { get; }
    public TextEditorSearchEngineOptionsBoolean IncludeMiscellaneousFiles { get; }
    public TextEditorSearchEngineOptionsBoolean AppendResults { get; }
}
