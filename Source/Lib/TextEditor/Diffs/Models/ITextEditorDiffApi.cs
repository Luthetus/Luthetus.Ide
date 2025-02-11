using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public interface ITextEditorDiffApi
{
    public void Register(
        Key<TextEditorDiffModel> diffModelKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey);

    public void Dispose(Key<TextEditorDiffModel> diffModelKey);

    public Func<ITextEditorEditContext, Task> CalculateFactory(Key<TextEditorDiffModel> diffModelKey, CancellationToken cancellationToken);

    public TextEditorDiffModel? GetOrDefault(Key<TextEditorDiffModel> diffModelKey);

    /// <summary>
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public ImmutableList<TextEditorDiffModel> GetDiffModels();
    
    public event Action? TextEditorDiffStateChanged;
    
    public TextEditorDiffState GetTextEditorDiffState();
    
    public void ReduceDisposeAction(Key<TextEditorDiffModel> diffKey);

    public void ReduceRegisterAction(
        Key<TextEditorDiffModel> diffKey,
        Key<TextEditorViewModel> inViewModelKey,
        Key<TextEditorViewModel> outViewModelKey);
}
