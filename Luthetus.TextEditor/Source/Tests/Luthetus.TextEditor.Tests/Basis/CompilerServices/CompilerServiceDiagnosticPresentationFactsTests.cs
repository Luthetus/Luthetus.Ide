using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public static class CompilerServiceDiagnosticPresentationFactsTests
{
    public const string CssClassString = "luth_te_compiler-service-diagnostic-presentation";

    public static readonly Key<TextEditorPresentationModel> PresentationKey = Key<TextEditorPresentationModel>.NewKey();

    public static readonly TextEditorPresentationModel EmptyPresentationModel = new(
        PresentationKey,
        0,
        CssClassString,
        new CompilerServiceDiagnosticDecorationMapper());
}
