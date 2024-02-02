using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

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