using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// TODO: I copy and pasted <see cref="Diffs.Models.TextEditorDiffDecorationMapper"/>...
/// ...to make this class. The decorations need to be made. I only
/// implemented the highlighting itself for now, and pick the colors later.(2024-02-01)
/// </summary>
public class FindOverlayDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (FindOverlayDecorationKind)decorationByte;

        return decoration switch
        {
            FindOverlayDecorationKind.None => string.Empty,
            FindOverlayDecorationKind.LongestCommonSubsequence => "luth_te_diff-longest-common-subsequence",
            FindOverlayDecorationKind.Insertion => "luth_te_diff-insertion",
            FindOverlayDecorationKind.Deletion => "luth_te_diff-deletion",
            FindOverlayDecorationKind.Modification => "luth_te_diff-modification",
            _ => string.Empty,
        };
    }
}