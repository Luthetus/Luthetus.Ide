using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.Models;

public static class DiffPresentationFactsTests
{
    public const string CssClassString = "luth_te_diff-presentation";

    public static readonly Key<TextEditorPresentationModel> InPresentationKey = Key<TextEditorPresentationModel>.NewKey();
    public static readonly Key<TextEditorPresentationModel> OutPresentationKey = Key<TextEditorPresentationModel>.NewKey();

    public static readonly TextEditorPresentationModel EmptyInPresentationModel = new(
        InPresentationKey,
        0,
        CssClassString,
        new TextEditorDiffDecorationMapper());
    
    public static readonly TextEditorPresentationModel EmptyOutPresentationModel = new(
        OutPresentationKey,
        0,
        CssClassString,
        new TextEditorDiffDecorationMapper());
}