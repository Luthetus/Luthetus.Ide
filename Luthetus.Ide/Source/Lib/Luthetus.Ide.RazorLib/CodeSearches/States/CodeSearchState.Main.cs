using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

[FeatureState]
public partial record CodeSearchState(
    string Query,
    string? StartingAbsolutePathForSearch,
    CodeSearchFilterKind CodeSearchFilterKind,
    ImmutableList<string> ResultList,
    string PreviewFilePath,
    Key<TextEditorViewModel> PreviewViewModelKey)
{
    public CodeSearchState() : this(
        string.Empty,
        null,
        CodeSearchFilterKind.None,
        ImmutableList<string>.Empty,
        string.Empty,
        Key<TextEditorViewModel>.Empty)
    {
    }
}
