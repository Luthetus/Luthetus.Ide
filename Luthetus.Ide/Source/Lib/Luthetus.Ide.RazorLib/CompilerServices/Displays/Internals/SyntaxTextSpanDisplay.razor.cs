using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class SyntaxTextSpanDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TextEditorTextSpan TextSpan { get; set; } = null!;

    private (TextEditorTextSpan TextEditorTextSpan, string GetTextResult) _textSpanTuple;
    private string InputValue { get; set; } = null!;

    protected override void OnParametersSet()
    {
        if (_textSpanTuple.TextEditorTextSpan != TextSpan)
        {
            _textSpanTuple = (TextSpan, TextSpan.GetText());
            InputValue = _textSpanTuple.GetTextResult;
        }

        base.OnParametersSet();
    }
}