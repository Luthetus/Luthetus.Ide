using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelModifier"/>
/// </summary>
public partial class ModelModifierTests
{
    /// <summary>
    /// <see cref="TextEditorModelModifier(TextEditorModel)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorModelModifier.ToModel()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        var outModel = modelModifier.ToModel();
        Assert.Equal(inModel.CharList, outModel.CharList);
        Assert.Equal(inModel.DecorationByteList, outModel.DecorationByteList);
        Assert.Equal(inModel.EditBlocksList, outModel.EditBlocksList);
        Assert.Equal(inModel.LineEndPositionList, outModel.LineEndPositionList);
        Assert.Equal(inModel.LineEndKindCountList, outModel.LineEndKindCountList);
        Assert.Equal(inModel.PresentationModelList, outModel.PresentationModelList);
        Assert.Equal(inModel.TabKeyPositionsList, outModel.TabKeyPositionsList);
        Assert.Equal(inModel.OnlyLineEndKind, outModel.OnlyLineEndKind);
        Assert.Equal(inModel.UsingLineEndKind, outModel.UsingLineEndKind);
        Assert.Equal(inModel.ResourceUri, outModel.ResourceUri);
        Assert.Equal(inModel.ResourceLastWriteTime, outModel.ResourceLastWriteTime);
        Assert.Equal(inModel.FileExtension, outModel.FileExtension);
        Assert.Equal(inModel.DecorationMapper, outModel.DecorationMapper);
        Assert.Equal(inModel.CompilerService, outModel.CompilerService);
        Assert.Equal(inModel.TextEditorSaveFileHelper, outModel.TextEditorSaveFileHelper);
        Assert.Equal(inModel.EditBlockIndex, outModel.EditBlockIndex);
        Assert.Equal(inModel.MostCharactersOnASingleLineTuple, outModel.MostCharactersOnASingleLineTuple);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearContentList()"/>
    /// </summary>
    [Fact]
    public void ClearContentList()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearContentList();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.CharList, outModel.CharList);
        Assert.NotEqual(inModel.DecorationByteList, outModel.DecorationByteList);
        Assert.Equal(ImmutableList<char>.Empty, outModel.CharList);
        Assert.Equal(ImmutableList<byte>.Empty, outModel.DecorationByteList);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearRowEndingPositionsList()"/>
    /// </summary>
    [Fact]
    public void ClearRowEndingPositionsList()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearRowEndingPositionsList();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.LineEndPositionList, outModel.LineEndPositionList);
        Assert.Equal(ImmutableList<LineEnd>.Empty, outModel.LineEndPositionList);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearRowEndingKindCountsList()"/>
    /// </summary>
    [Fact]
    public void ClearRowEndingKindCountsList()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearRowEndingKindCountsList();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.LineEndKindCountList, outModel.LineEndKindCountList);
        Assert.Equal(ImmutableList<(LineEndKind rowEndingKind, int count)>.Empty, outModel.LineEndKindCountList);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearTabKeyPositionsList()"/>
    /// </summary>
    [Fact]
    public void ClearTabKeyPositionsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearOnlyRowEndingKind()"/>
    /// </summary>
    [Fact]
    public void ClearOnlyRowEndingKind()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.ClearOnlyRowEndingKind();

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.OnlyLineEndKind, outModel.OnlyLineEndKind);
        Assert.Null(outModel.OnlyLineEndKind);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetUsingLineEndKind(LineEndKind)"/>
    /// </summary>
    [Fact]
    public void ModifyUsingRowEndingKind()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        modelModifier.SetUsingLineEndKind(LineEndKind.CarriageReturn);

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.UsingLineEndKind, outModel.UsingLineEndKind);
        Assert.Equal(LineEndKind.CarriageReturn, outModel.UsingLineEndKind);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetResourceData(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void ModifyResourceData()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);
        var modelModifier = new TextEditorModelModifier(inModel);

        var resourceUri = new ResourceUri("/abc123.txt");

        // Add one second to guarantee the date times differ.
        var dateTime = DateTime.UtcNow.AddSeconds(1);

        modelModifier.SetResourceData(resourceUri, dateTime);

        var outModel = modelModifier.ToModel();
        Assert.NotEqual(inModel.ResourceUri, outModel.ResourceUri);
        Assert.NotEqual(inModel.ResourceLastWriteTime, outModel.ResourceLastWriteTime);
        Assert.Equal(resourceUri, outModel.ResourceUri);
        Assert.Equal(dateTime, outModel.ResourceLastWriteTime);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetDecorationMapper(IDecorationMapper)"/>
    /// </summary>
    [Fact]
    public void ModifyDecorationMapper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetCompilerService(ILuthCompilerService)"/>
    /// </summary>
    [Fact]
    public void ModifyCompilerService()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetTextEditorSaveFileHelper(SaveFileHelper)"/>
    /// </summary>
    [Fact]
    public void ModifyTextEditorSaveFileHelper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetContent(string)"/>
    /// </summary>
    [Fact]
    public void ModifyContent()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearAllStatesButEditHistory()"/>
    /// </summary>
    [Fact]
    public void ModifyResetStateButNotEditHistory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.HandleKeyboardEvent(KeyboardEventArgs, CursorModifierBagTextEditor, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void HandleKeyboardEvent()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.LineIndex);
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

        Assert.Equal(1, outCursor.LineIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal('\n' + inText, outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void Insert()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.LineIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.Insert("\n", cursorModifierBag, CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(1, outCursor.LineIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal('\n' + inText, outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.DeleteTextByMotion(MotionKind, CursorModifierBagTextEditor, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotion()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.LineIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.DeleteTextByMotion(MotionKind.Delete, cursorModifierBag, CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(0, outCursor.LineIndex);
        Assert.Equal(0, outCursor.ColumnIndex);

        var outModel = modelModifier.ToModel();
        var outText = outModel.GetAllText();

        Assert.Equal(inText[1..], outText);
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.DeleteByRange(int, CursorModifierBagTextEditor, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void DeleteByRange()
    {
        TestsHelper.ConstructTestTextEditorModel(out var inModel);

        var inText = inModel.GetAllText();

        var modelModifier = new TextEditorModelModifier(inModel);

        var inCursor = new TextEditorCursor(0, 0, true);

        var cursorModifier = new TextEditorCursorModifier(inCursor);

        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>
            {
                cursorModifier
            });

        Assert.Equal(0, cursorModifier.LineIndex);
        Assert.Equal(0, cursorModifier.ColumnIndex);

        modelModifier.DeleteByRange(3, cursorModifierBag, CancellationToken.None);

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(0, outCursor.LineIndex);
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
    /// <see cref="TextEditorModelModifier.StartPendingCalculatePresentationModel(Key{TextEditorPresentationModel})"/>
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

    /// <summary>
	/// <see cref="TextEditorModelModifier.ContentList"/>
	/// </summary>
	[Fact]
    public void ContentList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.EditBlockList"/>
    /// </summary>
    [Fact]
    public void EditBlocksList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.LineEndPositionList"/>
    /// </summary>
    [Fact]
    public void RowEndingPositionsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.LineEndingKindCountsList"/>
    /// </summary>
    [Fact]
    public void RowEndingKindCountsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.PresentationModelsList"/>
    /// </summary>
    [Fact]
    public void PresentationModelsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.TabKeyPositionsList"/>
    /// </summary>
    [Fact]
    public void TabKeyPositionsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.OnlyLineEndKind"/>
    /// </summary>
    [Fact]
    public void OnlyRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.UsingLineEndKind"/>
    /// </summary>
    [Fact]
    public void UsingRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ResourceUri"/>
    /// </summary>
    [Fact]
    public void ResourceUri()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ResourceLastWriteTime"/>
    /// </summary>
    [Fact]
    public void ResourceLastWriteTime()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.FileExtension"/>
    /// </summary>
    [Fact]
    public void FileExtension()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.DecorationMapper"/>
    /// </summary>
    [Fact]
    public void DecorationMapper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.CompilerService"/>
    /// </summary>
    [Fact]
    public void CompilerService()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.TextEditorSaveFileHelper"/>
    /// </summary>
    [Fact]
    public void TextEditorSaveFileHelper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.EditBlockIndex"/>
    /// </summary>
    [Fact]
    public void EditBlockIndex()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.MostCharactersOnASingleLineTuple"/>
    /// </summary>
    [Fact]
    public void MostCharactersOnASingleRowTuple()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.RenderStateKey"/>
    /// </summary>
    [Fact]
    public void RenderStateKey()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.TextEditorKeymap"/>
    /// </summary>
    [Fact]
    public void TextEditorKeymap()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.TextEditorOptions"/>
    /// </summary>
    [Fact]
    public void TextEditorOptions()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.__Add(RichCharacter)"/>
    /// </summary>
    [Fact]
    public void PartitionList_Add()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void PartitionList_Add_SHOULD_INSERT_INTO_PARTITION_WITH_AVAILABLE_SPACE()
    {
        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             string.Empty,
             null,
             null,
             partitionSize: 5);

        var modifier = new TextEditorModelModifier(model);

        // Assert that the first partition is empty at the start.
        Assert.Empty(modifier.PartitionList.First().CharList);

        // Assert that more space than just one partition will be needed.
        var sourceText = "Hello World!";
        Assert.True(sourceText.Length > model.PartitionSize);

        var firstPartitionStringValue = new string(modifier.PartitionList.First().CharList.ToArray());

        for (int i = 0; i < sourceText.Length; i++)
        {
            var firstPartition = modifier.PartitionList.First();

            var richCharacter = new RichCharacter { Value = sourceText[i] };
            modifier.__Add(richCharacter);

            if (i < model.PartitionSize)
            {
                // Assert that the first n loops write to the first partition, because it has available space
                // This is asserted by checking that the string value of the first partition has changed.
                var newStringValue = new string(modifier.PartitionList.First().CharList.ToArray());
                Assert.NotEqual(firstPartitionStringValue, newStringValue);
                firstPartitionStringValue = newStringValue;
            }
            else
            {
                // Assert that the last (n + 1) loops do NOT write to the first partition, because it no longer has available space
                // This is asserted by checking that the string value of the first partition has NOT changed.
                var newStringValue = new string(modifier.PartitionList.First().CharList.ToArray());
                Assert.Equal(firstPartitionStringValue, newStringValue);
            }
        }

        // Assert that the output is correct.
        Assert.Equal(
            new string(modifier.CharList.ToArray()),
            sourceText);
    }

    [Fact]
    public void PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED_V2()
    {
        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             string.Empty,
             null,
             null,
             partitionSize: 5);

        var modifier = new TextEditorModelModifier(model);

        // Assert that only one partition exists at the start.
        Assert.Single(modifier.PartitionList);

        // Assert that more space will be needed.
        var sourceText = "Hello World!";
        Assert.True(sourceText.Length > model.PartitionSize);

        for (int i = 0; i < sourceText.Length; i++)
        {
            if (i == model.PartitionSize)
            {
                // Assert that up until this loop iteration only 1 partition has existed.
                Assert.Single(modifier.PartitionList);
            }

            var richCharacter = new RichCharacter { Value = sourceText[i] };
            modifier.__Add(richCharacter);

            if (i == model.PartitionSize)
            {
                // Assert that this loop iteration caused another partition to be made
                Assert.Equal(2, modifier.PartitionList.Count);

                // Furthermore, assert that the first partition contains
                // (PARTITION_SIZE / 2 + (PARTITION_SIZE % 2)) entries, and the second partition contains
                // (PARTITION_SIZE / 2)
                Assert.Equal(
                    model.PartitionSize / 2 + model.PartitionSize % 2,
                    modifier.PartitionList.First().CharList.Count);

                Assert.Equal(
                    // This had to be changed to include a '+1' because the insertion already occurred.
                    model.PartitionSize / 2 + 1,
                    modifier.PartitionList.Last().CharList.Count);
            }
        }

        Assert.Equal(
            "Hel",
            new string(modifier.PartitionList[0].CharList.ToArray()));

        Assert.Equal(
            "lo ",
            new string(modifier.PartitionList[1].CharList.ToArray()));

        Assert.Equal(
            "Wor",
            new string(modifier.PartitionList[2].CharList.ToArray()));

        Assert.Equal(
            "ld!",
            new string(modifier.PartitionList[3].CharList.ToArray()));

        // Assert that the output is correct.
        Assert.Equal(
            new string(modifier.CharList.ToArray()),
            sourceText);
    }

    [Fact]
    public void BACKSPACE_REMOVES_CHARACTER_FROM_PREVIOUS_PARTITION()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //var fileExtension = ExtensionNoPeriodFacts.TXT;
        //var resourceUri = new ResourceUri("/unitTesting.txt");
        //var resourceLastWriteTime = DateTime.UtcNow;
        //var sourceText = "Hello World!";

        //var model = new TextEditorModel(
        //     resourceUri,
        //     resourceLastWriteTime,
        //     fileExtension,
        //     sourceText,
        //     null,
        //     null,
        //     partitionSize: 5);

        //var modifier = new TextEditorModelModifier(model);

        //// Assert that there is more than one partition.
        //Assert.True(modifier.PartitionList.Count > 1);

        //// Get the count for first partition, so one can put a cursor, at this value.
        //// This is equivalent to the second partition at a relative position index of 0.
        //// That is to say, we want a cursor between the first and second partitions.
        //var countFirstPartition = modifier.PartitionList[0].Count;

        //var rowAndColumnIndicesTuple = model.GetRowAndColumnIndicesFromPositionIndex(countFirstPartition);

        //var cursor = new TextEditorCursor(
        //    rowAndColumnIndicesTuple.rowIndex,
        //    rowAndColumnIndicesTuple.columnIndex,
        //    true);

        //var cursorModifierBag = new TextEditorCursorModifierBag(
        //    Key<TextEditorViewModel>.Empty,
        //    new List<TextEditorCursorModifier> { new(cursor) });

        //// Prior to the backspace, PartitionList is as follows:
        ////
        //// PartitionList.SetPartitionSize(5);
        //// # {
        //// #     [ 'H', 'e', 'l' ]
        //// #     [ 'l', 'o', ' ' ]
        //// #     [ 'W', 'o', 'r' ]
        //// #     [ 'l', 'd', '!' ]
        //// # }

        //modifier.DeleteTextByMotion(MotionKind.Backspace, cursorModifierBag, CancellationToken.None);

        //// After the backspace, PartitionList is as follows:
        ////
        //// PartitionList.SetPartitionSize(5);
        //// # {
        //// #     [ 'H', 'e', ]
        //// #     [ 'l', 'o', ' ' ]
        //// #     [ 'W', 'o', 'r' ]
        //// #     [ 'l', 'd', '!' ]
        //// # }
        //Assert.Equal(
        //    "He",
        //    new string(modifier.PartitionList[0].Select(x => x.Value).ToArray()));

        //Assert.Equal(
        //    "lo ",
        //    new string(modifier.PartitionList[1].Select(x => x.Value).ToArray()));

        //Assert.Equal(
        //    "Wor",
        //    new string(modifier.PartitionList[2].Select(x => x.Value).ToArray()));

        //Assert.Equal(
        //    "ld!",
        //    new string(modifier.PartitionList[3].Select(x => x.Value).ToArray()));

        //// Assert that the output is correct.
        //Assert.Equal(
        //    "Helo World!",
        //    new string(modifier.ContentList.Select(x => x.Value).ToArray()));
    }

    /// <summary>
    /// TODO: This test method name 'DELETE_REMOVES_CHARACTER_FROM_NEXT_PARTITION()' does not make sense...
    /// If one has their cursor such that 'delete' will remove the first character of the next partition.
    /// Then, they have their cursor between two partitions, (or at position index 0).
    /// Therefore, the cursor is within the partition that they are removing from.
    /// </summary>
    [Fact]
    public void DELETE_REMOVES_CHARACTER_FROM_NEXT_PARTITION()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //var fileExtension = ExtensionNoPeriodFacts.TXT;
        //var resourceUri = new ResourceUri("/unitTesting.txt");
        //var resourceLastWriteTime = DateTime.UtcNow;
        //var sourceText = "Hello World!";

        //var model = new TextEditorModel(
        //     resourceUri,
        //     resourceLastWriteTime,
        //     fileExtension,
        //     sourceText,
        //     null,
        //     null,
        //     partitionSize: 5);

        //var modifier = new TextEditorModelModifier(model);

        //// Assert that there is more than one partition.
        //Assert.True(modifier.PartitionList.Count > 1);

        //// Get the count for first partition, so one can put a cursor, at this value.
        //// This is equivalent to the second partition at a relative position index of 0.
        //// That is to say, we want a cursor between the first and second partitions.
        //var countFirstPartition = modifier.PartitionList[0].Count;

        //var rowAndColumnIndicesTuple = model.GetRowAndColumnIndicesFromPositionIndex(countFirstPartition);

        //var cursor = new TextEditorCursor(
        //    rowAndColumnIndicesTuple.rowIndex,
        //    rowAndColumnIndicesTuple.columnIndex,
        //    true);

        //var cursorModifierBag = new TextEditorCursorModifierBag(
        //    Key<TextEditorViewModel>.Empty,
        //    new List<TextEditorCursorModifier> { new(cursor) });

        //// Prior to the delete, PartitionList is as follows:
        ////
        //// PartitionList.SetPartitionSize(5);
        //// # {
        //// #     [ 'H', 'e', 'l' ]
        //// #     [ 'l', 'o', ' ' ]
        //// #     [ 'W', 'o', 'r' ]
        //// #     [ 'l', 'd', '!' ]
        //// # }

        //modifier.DeleteTextByMotion(MotionKind.Delete, cursorModifierBag, CancellationToken.None);

        //// After the delete, PartitionList is as follows:
        ////
        //// PartitionList.SetPartitionSize(5);
        //// # {
        //// #     [ 'H', 'e', 'l' ]
        //// #     [ 'o', ' ', ]
        //// #     [ 'W', 'o', 'r' ]
        //// #     [ 'l', 'd', '!' ]
        //// # }
        //Assert.Equal(
        //    "Hel",
        //    new string(modifier.PartitionList[0].Select(x => x.Value).ToArray()));

        //Assert.Equal(
        //    "o ",
        //    new string(modifier.PartitionList[1].Select(x => x.Value).ToArray()));

        //Assert.Equal(
        //    "Wor",
        //    new string(modifier.PartitionList[2].Select(x => x.Value).ToArray()));

        //Assert.Equal(
        //    "ld!",
        //    new string(modifier.PartitionList[3].Select(x => x.Value).ToArray()));

        //// Assert that the output is correct.
        //Assert.Equal(
        //    "Helo World!",
        //    new string(modifier.ContentList.Select(x => x.Value).ToArray()));
    }

    [Fact]
    public void SELECTING_FIRST_CHARACTER_OF_PARTITION_THEN_BACKSPACE_REMOVES_FIRST_CHARACTER()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void SELECTING_LAST_CHARACTER_OF_PARTITION_THEN_DELETE_REMOVES_LAST_CHARACTER()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void SELECTION_THAT_SPANS_MORE_THAN_ONE_PARTITION_IS_REMOVED_PROPERLY_WITH_BACKSPACE()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void SELECTION_THAT_SPANS_MORE_THAN_ONE_PARTITION_IS_REMOVED_PROPERLY_WITH_DELETE()
    {
        throw new NotImplementedException();
    }
}