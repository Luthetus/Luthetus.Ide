using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public interface ITextEditorDiffApi
{
    public void Register(
        Key<TextEditorDiffModel> diffKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey);

    public void Dispose(Key<TextEditorDiffModel> diffKey);

    public TextEditorEdit CalculateFactory(Key<TextEditorDiffModel> diffKey, CancellationToken cancellationToken);

    public TextEditorDiffModel? GetOrDefault(Key<TextEditorDiffModel> diffKey);

    /// <summary>
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public ImmutableList<TextEditorDiffModel> GetDiffs();
}
