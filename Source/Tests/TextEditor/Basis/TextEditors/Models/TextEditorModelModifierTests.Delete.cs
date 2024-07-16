using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModelModifier;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelModifier"/>
/// </summary>
public partial class TextEditorModelModifierTests
{
    #region Delete_From_EmptyEditor
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_NegativeOne_DeleteEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: -1,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete);
                outModel = modelModifier.ToModel();
            }
        });
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Zero_DeleteEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_DocumentLength_DeleteEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);
            
            var cursor = new TextEditorCursor(
                lineIndex: lastLine.Index,
                columnIndex: lastLine.LastValidColumnIndex,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });
            
            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_DeleteEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                var cursor = new TextEditorCursor(
                    lineIndex: lastLine.Index,
                    columnIndex: 1 + lastLine.LastValidColumnIndex,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_NegativeOne_BackspaceEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: -1,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Zero_BackspaceEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            // Deleting at positionIndex '0' with 'DeleteKind.Backspace' should have no effect.
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_DocumentLength_BackspaceEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

            var cursor = new TextEditorCursor(
                lineIndex: lastLine.Index,
                columnIndex: lastLine.LastValidColumnIndex,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_BackspaceEnum()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    string.Empty
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                var cursor = new TextEditorCursor(
                    lineIndex: lastLine.Index,
                    columnIndex: 1 + lastLine.LastValidColumnIndex,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace);
                outModel = modelModifier.ToModel();
            }
        });
    }
    #endregion

    #region Delete_From_NotEmptyEditor
    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_DecreaseCounters_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 12,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_MaintainCounters_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                lineIndex: 1,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_NegativeOne_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: -1,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Zero_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(3, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                lineIndex: 1,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_ExpandWord_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                lineIndex: 1,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: true,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(10, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                6,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(1, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(2, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(4, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(6, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(10, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(10, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_DocumentLength_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

            var cursor = new TextEditorCursor(
                lineIndex: lastLine.Index,
                columnIndex: lastLine.LastValidColumnIndex,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(12, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                Assert.Equal(lastLine.Index, cursorModifier.LineIndex);
                Assert.Equal(lastLine.LastValidColumnIndex, cursorModifier.ColumnIndex);
                Assert.Equal(lastLine.LastValidColumnIndex, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                var cursor = new TextEditorCursor(
                    lineIndex: lastLine.Index,
                    columnIndex: 1 + lastLine.LastValidColumnIndex,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_DecreaseCounters_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

            var cursor = new TextEditorCursor(
                lineIndex: lastLine.Index,
                columnIndex: lastLine.LastValidColumnIndex,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                12,
                false,
                DeleteKind.Backspace,
                CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_MaintainCounters_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                lineIndex: 1,
                columnIndex: 1,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_NegativeOne_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: -1,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Zero_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            // Deleting at positionIndex '0' with 'DeleteKind.Backspace' should have no effect.
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(12, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                lineIndex: 1,
                columnIndex: 1,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }
    
    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_ExpandWord_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                       ^Delete this 'b9' with { Ctrl + Backspace }

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                lineIndex: 1,
                columnIndex: 2,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: true,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(10, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(6, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(1, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(2, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(4, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(6, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(10, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(10, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_DocumentLength_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

            var cursor = new TextEditorCursor(
                lineIndex: lastLine.Index,
                columnIndex: lastLine.LastValidColumnIndex,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                Assert.Equal(lastLine.Index, cursorModifier.LineIndex);
                Assert.Equal(lastLine.LastValidColumnIndex, cursorModifier.ColumnIndex);
                Assert.Equal(lastLine.LastValidColumnIndex, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                var cursor = new TextEditorCursor(
                    lineIndex: lastLine.Index,
                    columnIndex: 1 + lastLine.LastValidColumnIndex,
                    isPrimaryCursor: true);

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace);
                outModel = modelModifier.ToModel();
            }
        });
    }
    #endregion

    #region Delete_From_CursorSelection
    [Fact]
    public void Delete_From_CursorSelection_Causes_DecreaseCounters_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 0,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
                    EndingPositionIndex: modelModifier.LineEndList[^1].EndPositionIndexExclusive));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });
            
            modelModifier.Delete(
                cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_Causes_MaintainCounters_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_NegativeOne_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    LineIndex: 0,
                    ColumnIndex: 0,
                    PreferredColumnIndex: 1,
                    IsPrimaryCursor: true,
                    Selection: new TextEditorSelection(
                        AnchorPositionIndex: -1,
                        EndingPositionIndex: 1));

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_Zero_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 0,
                PreferredColumnIndex: 0,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
                    EndingPositionIndex: 1));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(3, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 1,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_ExpandWord_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 1,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: true,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_DocumentLength_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

            var cursor = new TextEditorCursor(
                LineIndex: lastLine.Index,
                ColumnIndex: lastLine.LastValidColumnIndex,
                PreferredColumnIndex: lastLine.LastValidColumnIndex,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: (-1) + modelModifier.CharCount,
                    EndingPositionIndex: modelModifier.CharCount));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Delete);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(3, cursorModifier.LineIndex);
                Assert.Equal(3, cursorModifier.ColumnIndex);
                Assert.Equal(3, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_OnePlusDocumentLength_DeleteEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                var cursor = new TextEditorCursor(
                    LineIndex: lastLine.Index,
                    ColumnIndex: 2 + lastLine.LastValidColumnIndex,
                    PreferredColumnIndex: 2 + lastLine.LastValidColumnIndex,
                    IsPrimaryCursor: true,
                    Selection: new TextEditorSelection(
                        AnchorPositionIndex: 1 + lastLine.LastValidColumnIndex,
                        EndingPositionIndex: 2  + lastLine.LastValidColumnIndex));

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_CursorSelection_Causes_DecreaseCounters_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 0,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
                    EndingPositionIndex: modelModifier.LineEndList[^1].EndPositionIndexExclusive));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_Causes_MaintainCounters_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 1,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_NegativeOne_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    LineIndex: 0,
                    ColumnIndex: 0,
                    PreferredColumnIndex: 1,
                    IsPrimaryCursor: true,
                    Selection: new TextEditorSelection(
                        AnchorPositionIndex: -1,
                        EndingPositionIndex: 1));

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace);
                outModel = modelModifier.ToModel();
            }
        });
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_Zero_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 0,
                PreferredColumnIndex: 0,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
                    EndingPositionIndex: 1));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(3, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(0, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 1,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }
    
    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_ExpandWord_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            /*
             delete:
                (
                    "\n" +   // LineFeed

                    "b9" +   // LetterOrDigit-Lowercase
                     ^Delete this 'b'

                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
             */
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 1,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: true,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(2, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(3, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(5, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_DocumentLength_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

            var cursor = new TextEditorCursor(
                LineIndex: lastLine.Index,
                ColumnIndex: lastLine.LastValidColumnIndex,
                PreferredColumnIndex: lastLine.LastValidColumnIndex,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: (-1) + modelModifier.CharCount,
                    EndingPositionIndex: modelModifier.CharCount));

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                cursorModifierBag: cursorModifierBag,
                columnCount: 1,
                expandWord: false,
                cancellationToken: CancellationToken.None,
                deleteKind: DeleteKind.Backspace);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(3, cursorModifier.LineIndex);
                Assert.Equal(3, cursorModifier.ColumnIndex);
                Assert.Equal(3, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    [Fact]
    public void Delete_From_CursorSelection_At_PositionIndex_EqualTo_OnePlusDocumentLength_BackspaceEnum()
    {
        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content:
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var lastLine = modelModifier.GetLineInformation(modelModifier.LineCount - 1);

                var cursor = new TextEditorCursor(
                    LineIndex: lastLine.Index,
                    ColumnIndex: 2 + lastLine.LastValidColumnIndex,
                    PreferredColumnIndex: 2 + lastLine.LastValidColumnIndex,
                    IsPrimaryCursor: true,
                    Selection: new TextEditorSelection(
                        AnchorPositionIndex: 1 + lastLine.LastValidColumnIndex,
                        EndingPositionIndex: 2 + lastLine.LastValidColumnIndex));

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace);
                outModel = modelModifier.ToModel();
            }
        });
    }
    #endregion
}