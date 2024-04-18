using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
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
        Assert.Throws<ApplicationException>(() =>
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
                    deleteKind: DeleteKind.Delete
                );
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);

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
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
            // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            var cursor = new TextEditorCursor(
                lineIndex: modelModifier.LineCount - 1,
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);

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
        Assert.Throws<ApplicationException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
                // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
                // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
                // This equates to 'DocumentLength'.
                // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
                var cursor = new TextEditorCursor(
                    lineIndex: modelModifier.LineCount - 1,
                    columnIndex: 1,
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
                        deleteKind: DeleteKind.Delete
                    );
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
        Assert.Throws<ApplicationException>(() =>
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
                    deleteKind: DeleteKind.Backspace
                );
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);

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
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
            // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            var cursor = new TextEditorCursor(
                lineIndex: modelModifier.LineCount - 1,
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);

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
        Assert.Throws<ApplicationException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
                // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
                // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
                // This equates to 'DocumentLength'.
                // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
                var cursor = new TextEditorCursor(
                    lineIndex: modelModifier.LineCount - 1,
                    columnIndex: 1,
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
                        deleteKind: DeleteKind.Backspace
                    );
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

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
             Given content:
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
             Then delete:
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

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
        Assert.Throws<ApplicationException>(() =>
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
                    deleteKind: DeleteKind.Delete
                );
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

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
             Given content:
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
             Then delete:
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

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
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
            // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            var cursor = new TextEditorCursor(
                lineIndex: modelModifier.LineCount - 1,
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(modelModifier.LineCount - 1, cursorModifier.LineIndex);
                Assert.Equal(0, cursorModifier.ColumnIndex);
                Assert.Equal(0, cursorModifier.PreferredColumnIndex);
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
        Assert.Throws<ApplicationException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
                // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
                // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
                // This equates to 'DocumentLength'.
                // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
                var cursor = new TextEditorCursor(
                    lineIndex: modelModifier.LineCount - 1,
                    columnIndex: 1,
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
                        deleteKind: DeleteKind.Delete
                    );
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
            // DocumentLength is equivalent to:
            // lineIndex: modelModifier.LineCount - 1, and columnIndex: 0
            //
            // A count of '12', with 'DeleteKind.BackSpace',
            // should delete all the content in the text editor (relative to the current test data).
            var cursor = new TextEditorCursor(
                lineIndex: modelModifier.LineCount - 1,
                columnIndex: 0,
                isPrimaryCursor: true);

            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    12,
                    false,
                    CancellationToken.None,
                    DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

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
             Given content:
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
             Then delete:
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

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
        Assert.Throws<ApplicationException>(() =>
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
                    deleteKind: DeleteKind.Backspace
                );
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

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
             Given content:
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
             Then delete:
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

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
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
            // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            var cursor = new TextEditorCursor(
                lineIndex: modelModifier.LineCount - 1,
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                var aaa = modelModifier.GetLineInformation(modelModifier.LineEndList.Count - 1);
                var bbb = modelModifier.GetLineInformation(modelModifier.LineEndList.Count);

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
        Assert.Throws<ApplicationException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
                // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
                // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
                // This equates to 'DocumentLength'.
                // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
                var cursor = new TextEditorCursor(
                    lineIndex: modelModifier.LineCount - 1,
                    columnIndex: 1,
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
                        deleteKind: DeleteKind.Backspace
                    );
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
        {
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
                    EndingPositionIndex: modelModifier.LineEndList[^1].EndPositionIndexExclusive));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });
            
            modelModifier.Delete(
                    cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
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
        {
            /*
             Given content:
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
             Then delete:
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
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        Assert.Throws<ApplicationException>(() =>
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
                    deleteKind: DeleteKind.Delete
                );
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
        {
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
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
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(3, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        {
            /*
             Given content:
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
             Then delete:
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
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        {
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
            // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: (-1) + modelModifier.LineEndList[^1].EndPositionIndexExclusive,
                    EndingPositionIndex: modelModifier.LineEndList[^1].EndPositionIndexExclusive));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        Assert.Throws<ApplicationException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
                // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
                // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
                // This equates to 'DocumentLength'.
                // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
                var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1 + modelModifier.LineEndList[^1].EndPositionIndexExclusive,
                    EndingPositionIndex: 2 + modelModifier.LineEndList[^1].EndPositionIndexExclusive));

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                        cursorModifierBag: cursorModifierBag,
                        columnCount: 1,
                        expandWord: false,
                        cancellationToken: CancellationToken.None,
                        deleteKind: DeleteKind.Delete
                    );
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
        {
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
                    EndingPositionIndex: modelModifier.LineEndList[^1].EndPositionIndexExclusive));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(0, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
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
        {
            /*
             Given content:
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
             Then delete:
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
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        Assert.Throws<ApplicationException>(() =>
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
                    deleteKind: DeleteKind.Backspace
                );
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
        {
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 0,
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
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(3, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        {
            /*
             Given content:
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
             Then delete:
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
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1,
                    EndingPositionIndex: 2));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        {
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
            // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: (-1) + modelModifier.LineEndList[^1].EndPositionIndexExclusive,
                    EndingPositionIndex: modelModifier.LineEndList[^1].EndPositionIndexExclusive));

            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Delete(
                    cursorModifierBag: cursorModifierBag,
                    columnCount: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
        Assert.Throws<ApplicationException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'lineIndex' and 'columnIndex'
                // Here, 'lineIndex: modelModifier.LineCount - 1' gets the lineIndex that the 'EndOfFile' resides at.
                // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
                // This equates to 'DocumentLength'.
                // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
                var cursor = new TextEditorCursor(
                LineIndex: 0,
                ColumnIndex: 0,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                Selection: new TextEditorSelection(
                    AnchorPositionIndex: 1 + modelModifier.LineEndList[^1].EndPositionIndexExclusive,
                    EndingPositionIndex: 2 + modelModifier.LineEndList[^1].EndPositionIndexExclusive));

                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Delete(
                        cursorModifierBag: cursorModifierBag,
                        columnCount: 1,
                        expandWord: false,
                        cancellationToken: CancellationToken.None,
                        deleteKind: DeleteKind.Backspace
                    );
                outModel = modelModifier.ToModel();
            }
        });
    }
    #endregion
}