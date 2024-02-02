namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// TODO: I copy and pasted <see cref="Diffs.Models.TextEditorDiffDecorationKind"/>...
/// ...to make this class. The decorations need to be made. I only
/// implemented the highlighting itself for now, and pick the colors later.(2024-02-01)
/// </summary>
public enum TextEditorFindOverlayDecorationKind
{
    None,
    LongestCommonSubsequence,
    Insertion,
    Deletion,
    Modification
}