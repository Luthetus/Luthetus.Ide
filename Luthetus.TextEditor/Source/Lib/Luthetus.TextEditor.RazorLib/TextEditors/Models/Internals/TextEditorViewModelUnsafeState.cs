namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// This class is a hack and was made on (2024-03-09).<br/><br/>
/// 
/// The reason for this class: This same hack existed prior, but as value type properties that
/// were on the <see cref="TextEditorViewModel"/>.<br/><br/>
/// 
/// The <see cref="TextEditorViewModel"/> is an immutable type.<br/><br/>
/// 
/// Originally the value type properties were added to <see cref="TextEditorViewModel"/> as a hack to
/// permit the UI to mark the cursor as being 'within view', but not trigger a re-render.<br/><br/>
/// 
/// But, since <see cref="TextEditorViewModel"/> is immutable, this hack was unreliable.
/// At times, the value type that was changed, was from an out of date view model. And as result had no effect.<br/><br/>
/// 
/// By object-boxing the value types, the hack is more reliable.<br/><br/>
/// </summary>
public class TextEditorViewModelUnsafeState
{
    /// <summary>
    /// If one opens a file with the 'Enter' key, they might want focus to then be set on that
    /// newly opened file. However, perhaps one wants the 'Space' key to also open the file,
    /// but not set focus to it.
    /// </summary>
    public bool ShouldSetFocusAfterNextRender { get; set; }
    public bool ShouldRevealCursor { get; set; }
    /// <summary>
    /// Relates to whether the cursor is within the viewable area of the Text Editor on the UI
    /// </summary>
    public bool CursorIsIntersecting { get; set; }
}
