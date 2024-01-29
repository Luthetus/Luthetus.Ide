using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class SyntaxTextSpanDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorTextSpan TextSpan { get; set; } = null!;

    private (TextEditorTextSpan TextEditorTextSpan, string GetTextResult) _textSpanTuple;

    private string InputValue { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        if (_textSpanTuple.TextEditorTextSpan != TextSpan)
        {
            _textSpanTuple = (TextSpan, TextSpan.GetText());
            InputValue = _textSpanTuple.GetTextResult;
        }

        base.OnParametersSet();
    }

    private void CompleteForm()
    {
        var localTextSpanTuple = _textSpanTuple;
        var localInputValue = InputValue;

        TextEditorService.Post(nameof(SyntaxTextSpanDisplay), editContext =>
        {
            var modelModifier = editContext.GetModelModifier(
                localTextSpanTuple.TextEditorTextSpan.ResourceUri);

            if (modelModifier is null)
                return Task.CompletedTask;

            var rowInfo = modelModifier.GetRowInformationFromPositionIndex(
                localTextSpanTuple.TextEditorTextSpan.StartingIndexInclusive);

            var columnIndex = localTextSpanTuple.TextEditorTextSpan.StartingIndexInclusive -
                rowInfo.RowStartPositionIndexInclusive;

            var cursor = new TextEditorCursor(
                rowInfo.RowIndex,
                columnIndex,
                columnIndex,
                true,
                new TextEditorSelection(
                    localTextSpanTuple.TextEditorTextSpan.StartingIndexInclusive,
                    localTextSpanTuple.TextEditorTextSpan.EndingIndexExclusive));

            var cursorModifierBag = new TextEditorCursorModifierBag(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier> { new(cursor) });

            return TextEditorService.ModelApi.InsertTextUnsafeFactory(
                    _textSpanTuple.TextEditorTextSpan.ResourceUri,
                    cursorModifierBag,
                    localInputValue,
                    CancellationToken.None)
                .Invoke(editContext);
        });
    }
}