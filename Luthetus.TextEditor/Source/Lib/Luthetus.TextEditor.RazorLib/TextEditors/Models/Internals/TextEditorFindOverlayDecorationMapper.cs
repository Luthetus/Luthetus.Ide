using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// TODO: I copy and pasted <see cref="Diffs.Models.TextEditorDiffDecorationMapper"/>...
/// ...to make this class. The decorations need to be made. I only
/// implemented the highlighting itself for now, and pick the colors later.(2024-02-01)
/// </summary>
public class TextEditorFindOverlayDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (TextEditorFindOverlayDecorationKind)decorationByte;

        return decoration switch
        {
            TextEditorFindOverlayDecorationKind.None => string.Empty,
            TextEditorFindOverlayDecorationKind.LongestCommonSubsequence => "luth_te_diff-longest-common-subsequence",
            TextEditorFindOverlayDecorationKind.Insertion => "luth_te_diff-insertion",
            TextEditorFindOverlayDecorationKind.Deletion => "luth_te_diff-deletion",
            TextEditorFindOverlayDecorationKind.Modification => "luth_te_diff-modification",
            _ => string.Empty,
        };
    }
}