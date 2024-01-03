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
using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

/// <summary>
/// <see cref="TextEditorModelModifier"/>
/// </summary>
public partial class TextEditorModelModifierTests
{
    /// <summary>
    /// <see cref="TextEditorModelModifier(TextEditorModel)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorModelModifier.ToModel()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        var outModel = modelModifier.ToModel();
        Assert.Equal(inModel.ContentBag, outModel.ContentBag);
        Assert.Equal(inModel.EditBlocksBag, outModel.EditBlocksBag);
        Assert.Equal(inModel.RowEndingPositionsBag, outModel.RowEndingPositionsBag);
        Assert.Equal(inModel.RowEndingKindCountsBag, outModel.RowEndingKindCountsBag);
        Assert.Equal(inModel.PresentationModelsBag, outModel.PresentationModelsBag);
        Assert.Equal(inModel.TabKeyPositionsBag, outModel.TabKeyPositionsBag);
        Assert.Equal(inModel.OnlyRowEndingKind, outModel.OnlyRowEndingKind);
        Assert.Equal(inModel.UsingRowEndingKind, outModel.UsingRowEndingKind);
        Assert.Equal(inModel.ResourceUri, outModel.ResourceUri);
        Assert.Equal(inModel.ResourceLastWriteTime, outModel.ResourceLastWriteTime);
        Assert.Equal(inModel.FileExtension, outModel.FileExtension);
        Assert.Equal(inModel.DecorationMapper, outModel.DecorationMapper);
        Assert.Equal(inModel.CompilerService, outModel.CompilerService);
        Assert.Equal(inModel.TextEditorSaveFileHelper, outModel.TextEditorSaveFileHelper);
        Assert.Equal(inModel.EditBlockIndex, outModel.EditBlockIndex);
        Assert.Equal(inModel.MostCharactersOnASingleRowTuple, outModel.MostCharactersOnASingleRowTuple);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearContentBag()"/>
    /// </summary>
    [Fact]
    public void ClearContentBag()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearContentBag();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.ContentBag, outModel.ContentBag);
        Assert.Equal(ImmutableList<RichCharacter>.Empty, outModel.ContentBag);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearRowEndingPositionsBag()"/>
    /// </summary>
    [Fact]
    public void ClearRowEndingPositionsBag()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearRowEndingPositionsBag();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.RowEndingPositionsBag, outModel.RowEndingPositionsBag);
        Assert.Equal(ImmutableList<RowEnding>.Empty, outModel.RowEndingPositionsBag);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearRowEndingKindCountsBag()"/>
    /// </summary>
    [Fact]
    public void ClearRowEndingKindCountsBag()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearRowEndingKindCountsBag();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.RowEndingKindCountsBag, outModel.RowEndingKindCountsBag);
        Assert.Equal(ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty, outModel.RowEndingKindCountsBag);
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
    /// <see cref="TextEditorModelModifier.ClearOnlyRowEndingKind()"/>
    /// </summary>
    [Fact]
    public void ClearOnlyRowEndingKind()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearOnlyRowEndingKind();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.OnlyRowEndingKind, outModel.OnlyRowEndingKind);
        Assert.Null(outModel.OnlyRowEndingKind);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyUsingRowEndingKind(RowEndingKind)"/>
    /// </summary>
    [Fact]
    public void ModifyUsingRowEndingKind()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ModifyUsingRowEndingKind(RowEndingKind.CarriageReturn);

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.UsingRowEndingKind, outModel.UsingRowEndingKind);
        Assert.Equal(RowEndingKind.CarriageReturn, outModel.UsingRowEndingKind);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ModifyResourceData(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void ModifyResourceData()
    {
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        var resourceUri = new ResourceUri("/abc123.txt");
        
        // Add one second to guarantee the date times differ.
        var dateTime = DateTime.UtcNow.AddSeconds(1);

        modelModifier.ModifyResourceData(resourceUri, dateTime);

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.ResourceUri, outModel.ResourceUri);
        Assert.NotEqual(inModel.ResourceLastWriteTime, outModel.ResourceLastWriteTime);
        Assert.Equal(resourceUri, outModel.ResourceUri);
        Assert.Equal(dateTime, outModel.ResourceLastWriteTime);
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
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);

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
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);

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
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);

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
        TextEditorServicesTestsHelper.ConstructTestTextEditorModel(out var inModel);

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