using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public static class DiffPresentationFacts
{
    public const string CssClassString = "luth_te_diff-presentation";

    public static readonly Key<TextEditorPresentationModel> InPresentationKey = Key<TextEditorPresentationModel>.NewKey();
    public static readonly Key<TextEditorPresentationModel> OutPresentationKey = Key<TextEditorPresentationModel>.NewKey();

    /// <summary>
    /// TODO: Change the name of this from 'EmptyInPresentationModel' because its confusingly named.
    /// </summary>
    public static readonly TextEditorPresentationModel EmptyInPresentationModel = new(
        InPresentationKey,
        0,
        CssClassString,
        new TextEditorDiffDecorationMapper());

    /// <summary>
    /// TODO: Change the name of this from 'EmptyOutPresentationModel' because its confusingly named.
    /// </summary>
    public static readonly TextEditorPresentationModel EmptyOutPresentationModel = new(
        OutPresentationKey,
        0,
        CssClassString,
        new TextEditorDiffDecorationMapper());
}