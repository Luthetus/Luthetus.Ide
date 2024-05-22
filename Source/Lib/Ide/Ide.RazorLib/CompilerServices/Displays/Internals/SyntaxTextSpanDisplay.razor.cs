using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

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

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
            await CompleteForm().ConfigureAwait(false);
    }

    private async Task CompleteForm()
    {
        var localTextSpanTuple = _textSpanTuple;
        var localInputValue = InputValue;
        var model = TextEditorService.ModelApi.GetOrDefault(localTextSpanTuple.TextEditorTextSpan.ResourceUri);

        if (model is null)
            return;

        var modelText = model.GetAllText();

        await TextEditorService.PostSimpleBatch(
            nameof(SyntaxTextSpanDisplay),
            string.Empty,
			null,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(
                    localTextSpanTuple.TextEditorTextSpan.ResourceUri);

                if (modelModifier is null ||
                    modelModifier.GetAllText() != modelText)
                {
                    return;
                }

                var rowInfo = modelModifier.GetLineInformationFromPositionIndex(
                    localTextSpanTuple.TextEditorTextSpan.StartingIndexInclusive);

                var columnIndex = localTextSpanTuple.TextEditorTextSpan.StartingIndexInclusive -
                    rowInfo.StartPositionIndexInclusive;

                var cursor = new TextEditorCursor(
                    rowInfo.Index,
                    columnIndex,
                    columnIndex,
                    true,
                    new TextEditorSelection(
                        localTextSpanTuple.TextEditorTextSpan.StartingIndexInclusive,
                        localTextSpanTuple.TextEditorTextSpan.EndingIndexExclusive));

                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier> { new(cursor) });

                await TextEditorService.ModelApi.InsertTextUnsafeFactory(
                        _textSpanTuple.TextEditorTextSpan.ResourceUri,
                        cursorModifierBag,
                        localInputValue,
                        CancellationToken.None)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                await modelModifier.CompilerService.ResourceWasModified(
                    _textSpanTuple.TextEditorTextSpan.ResourceUri,
                    ImmutableArray<TextEditorTextSpan>.Empty);
            }).ConfigureAwait(false);
    }
}