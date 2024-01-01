using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

/// <summary>
/// <see cref="TextEditorModelModifier"/>
/// </summary>
public partial class TextEditorModelModifierTests
{
    /// <summary>
    /// <see cref="TextEditorModelModifier(TextEditorModel)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ToModel()"/>
    /// </summary>
    [Fact]
    public void ToTextEditorModel()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearContentBag()"/>
    /// </summary>
    [Fact]
    public void ClearContentBag()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearRowEndingPositionsBag()"/>
    /// </summary>
    [Fact]
    public void ClearRowEndingPositionsBag()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearRowEndingKindCountsBag()"/>
    /// </summary>
    [Fact]
    public void ClearRowEndingKindCountsBag()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearTabKeyPositionsBag()"/>
    /// </summary>
    [Fact]
    public void ClearTabKeyPositionsBag()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyOnlyRowEndingKind()"/>
    /// </summary>
    [Fact]
    public void ModifyOnlyRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyUsingRowEndingKind(RowEndingKind)"/>
    /// </summary>
    [Fact]
    public void ModifyUsingRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyResourceData(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void ModifyResourceData()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyDecorationMapper(IDecorationMapper)"/>
    /// </summary>
    [Fact]
    public void ModifyDecorationMapper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyCompilerService(ICompilerService)"/>
    /// </summary>
    [Fact]
    public void ModifyCompilerService()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyTextEditorSaveFileHelper(TextEditorSaveFileHelper)"/>
    /// </summary>
    [Fact]
    public void ModifyTextEditorSaveFileHelper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyContent(string)"/>
    /// </summary>
    [Fact]
    public void ModifyContent()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyResetStateButNotEditHistory()"/>
    /// </summary>
    [Fact]
    public void ModifyResetStateButNotEditHistory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.HandleKeyboardEvent(KeyboardEventArgs, TextEditorCursorModifierBag, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void HandleKeyboardEvent()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(
            out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new TextEditorCursorModifierBag(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.RowIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.HandleKeyboardEvent(
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE.ToString(),
                Code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE
            },
            cursorModifierBag,
            CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(1, outCursor.RowIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal('\n' + inText, outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.EditByInsertion(string, TextEditorCursorModifierBag, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void EditByInsertion()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(
            out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new TextEditorCursorModifierBag(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.RowIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.EditByInsertion("\n", cursorModifierBag, CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(1, outCursor.RowIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal('\n' + inText, outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.DeleteTextByMotion(MotionKind, TextEditorCursorModifierBag, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotion()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(
            out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new TextEditorCursorModifierBag(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.RowIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.DeleteTextByMotion(MotionKind.Delete, cursorModifierBag, CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(0, outCursor.RowIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal(inText[1..], outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.DeleteByRange(int, TextEditorCursorModifierBag, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void DeleteByRange()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(
            out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new TextEditorCursorModifierBag(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.RowIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.DeleteByRange(3, cursorModifierBag, CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(0, outCursor.RowIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal(inText[3..], outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.PerformRegisterPresentationModelAction(TextEditorPresentationModel)"/>
    /// </summary>
    [Fact]
    public void PerformRegisterPresentationModelAction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.PerformCalculatePresentationModelAction(Key{TextEditorPresentationModel})"/>
    /// </summary>
    [Fact]
    public void PerformCalculatePresentationModelAction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearEditBlocks()"/>
    /// </summary>
    [Fact]
    public void ClearEditBlocks()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.UndoEdit()"/>
    /// </summary>
    [Fact]
    public void UndoEdit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.RedoEdit()"/>
    /// </summary>
    [Fact]
    public void RedoEdit()
    {
        throw new NotImplementedException();
    }
}