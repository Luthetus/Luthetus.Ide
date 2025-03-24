using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModel"/>
/// </summary>
public partial class TextEditorModelModifierTests
{
    #region Insert_Into_EmptyEditor
    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into an empty editor, at positionIndex equal to 0 (2024-04-14)<br/>
    /// Purpose: Index 0 is the first valid positionIndex, thus it bears significance as a boundary.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_EmptyEditor_At_PositionIndex_Zero()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: string.Empty,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            var cursor = new TextEditorCursor(lineIndex: 0, columnIndex: 0, isPrimaryCursor: true);
            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Insert(
                "\n" +   // LineFeed
                "b9" +   // LetterOrDigit-Lowercase
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "\r\n" + // CarriageReturnLineFeed
                "\t" +   // Tab
                "$" +    // SpecialCharacter
                ";" +    // Punctuation
                " ",     // Space
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'SetContent' parameter has a string length of '12',
            // But, as of this comment, the insertion of line ending characters
            // other than line feed is not supported.
            // Therefore, the "\r" and the "\r\n" are replaced with "\n".
            //
            // As a result of the "\r\n" replacement with "\n",
            // one character was lost, therefore the length ends up being 11.
            Assert.Equal(11, modelModifier.CharCount);

            // The file extension should NOT change as a result of setting the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the string that was inserted.
            // Therefore, the Count is 1.
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was inserted.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // 1 LineFeed was inserted.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear.
                // Because 3 total LineEnds were inserted, the count is 3.
                Assert.Equal(
                    3,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                
                // 1 CarriageReturnLineFeed was inserted.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                var lineFeedPositionList = modelModifier.LineEndList
                    .Where(x => x.LineEndKind == LineEndKind.LineFeed)
                    .ToArray();

                // First LineEnd
                var lineFeedPosition = lineFeedPositionList[0];
                Assert.Equal(0, lineFeedPosition.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeedPosition.EndPositionIndexExclusive);
                // Second LineEnd
                lineFeedPosition = lineFeedPositionList[1];
                Assert.Equal(3, lineFeedPosition.StartPositionIndexInclusive);
                Assert.Equal(4, lineFeedPosition.EndPositionIndexExclusive);
                // Third LineEnd
                lineFeedPosition = lineFeedPositionList[2];
                Assert.Equal(6, lineFeedPosition.StartPositionIndexInclusive);
                Assert.Equal(7, lineFeedPosition.EndPositionIndexExclusive);

                // 3 line endings where inserted,
                // There are 4 in total if one then includes the special-'EndOfFile' LineEnd.
                Assert.Equal(4, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The insert will have moved the 'EndOfFile' LineEnd from positionIndex 0 to a larger value.
                // Specifically, the 'EndOfFile' should move by the length of the text inserted.
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

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into an empty editor, at positionIndex equal to DocumentLength (2024-04-14)<br/>
    /// Purpose: Index of 'DocumentLength' is the last valid positionIndex, thus it bears significance as a boundary.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_EmptyEditor_At_PositionIndex_DocumentLength()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: string.Empty,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        TextEditorCursorModifier cursorModifier;
        {
            // DocumentLength is 0 for an empty text editor model
            var cursor = new TextEditorCursor(lineIndex: 0, columnIndex: 0, isPrimaryCursor: true);
            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Insert(
                "\n" +   // LineFeed
                "b9" +   // LetterOrDigit-Lowercase
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "\r\n" + // CarriageReturnLineFeed
                "\t" +   // Tab
                "$" +    // SpecialCharacter
                ";" +    // Punctuation
                " ",     // Space
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'SetContent' parameter has a string length of '12',
            // But, as of this comment, the insertion of line ending characters
            // other than line feed is not supported.
            // Therefore, the "\r" and the "\r\n" are replaced with "\n".
            //
            // As a result of the "\r\n" replacement with "\n",
            // one character was lost, therefore the length ends up being 11.
            Assert.Equal(11, modelModifier.CharCount);

            // The file extension should NOT change as a result of setting the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel but,
            // now that the content is set to 'string.Empty', the Count is 0.
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(7, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was inserted.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // 1 LineFeed was inserted.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear.
                // Because 3 total LineEnds were inserted, the count is 3.
                Assert.Equal(
                    3,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                // 1 CarriageReturnLineFeed was inserted.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                var lineFeedPositionList = modelModifier.LineEndList
                    .Where(x => x.LineEndKind == LineEndKind.LineFeed)
                    .ToArray();

                // First LineEnd
                var lineFeedPosition = lineFeedPositionList[0];
                Assert.Equal(0, lineFeedPosition.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeedPosition.EndPositionIndexExclusive);
                // Second LineEnd
                lineFeedPosition = lineFeedPositionList[1];
                Assert.Equal(3, lineFeedPosition.StartPositionIndexInclusive);
                Assert.Equal(4, lineFeedPosition.EndPositionIndexExclusive);
                // Third LineEnd
                lineFeedPosition = lineFeedPositionList[2];
                Assert.Equal(6, lineFeedPosition.StartPositionIndexInclusive);
                Assert.Equal(7, lineFeedPosition.EndPositionIndexExclusive);

                // 3 line endings where inserted,
                // There are 4 in total if one then includes the special-'EndOfFile' LineEnd.
                Assert.Equal(4, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The insert will have moved the 'EndOfFile' LineEnd from positionIndex 0 to a larger value.
                // Specifically, the 'EndOfFile' should move by the length of the text inserted.
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

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into an empty editor, at positionIndex equal to -1 (2024-04-14)<br/>
    /// Purpose: Index -1 is a value which is less than than the lower-bound for valid positionIndices,
    ///          thus it bears significance as a unique case.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_EmptyEditor_At_PositionIndex_Negative_One()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: string.Empty,
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
                var cursor = new TextEditorCursor(lineIndex: 0, columnIndex: -1, isPrimaryCursor: true);
                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Insert(
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " ",     // Space
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
                outModel = modelModifier.ToModel();
            }
        });
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into an empty editor, at a positionIndex equal to '1 + DocumentLength' (2024-04-14)<br/>
    /// Purpose: Index '1 + DocumentLength' is a value which is greater than than the upper-bound for valid positionIndices,
    ///          thus it bears significance as a unique case.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_EmptyEditor_At_Position_One_Plus_DocumentLength()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: string.Empty,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<LuthetusTextEditorException>((Action)(() =>
        {
            // Do something
            TextEditorModel outModel;
            TextEditorCursorModifier cursorModifier;
            {
                var lastLine = modelModifier.GetLineInformation((int)(modelModifier.LineCount - 1));

                var cursor = new TextEditorCursor(
                    lineIndex: lastLine.Index,
                    columnIndex: 1 + lastLine.LastValidColumnIndex,
                    isPrimaryCursor: true);
                cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Insert(
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " ",     // Space
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
                outModel = modelModifier.ToModel();
            }
        }));
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into an empty editor, a null string (2024-04-14)<br/>
    /// Purpose: The string argument of Insert(...) has 3 states as far as the text editor is concerned.<br/>
    ///              -It is null<br/>
    ///              -It is empty<br/>
    ///              -It not null, and it is not empty<br/>
    ///              -NOTE: the string being whitespace is a valid possibility, it falls under "not null, and not empty"
    ///                     just as if it were a LetterOrDigit<br/>
    ///           This bears significance as the 'null' case.
    /// </summary>
    [Fact]
    public void Insert_Into_EmptyEditor_Null_String()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: string.Empty,
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Post-assertions
        //
        // This test expects the 'Do Something' step to throw an exception.
        // That is all that needs to be tested here.
        Assert.Throws<NullReferenceException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                    lineIndex: 0,
                    columnIndex: 0,
                    isPrimaryCursor: true);
                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Insert(
                    // The null-forgiving operator was used here
                    // because the test purposefully wants to pass in null,
                    // to see what happens.
                    value: null!,
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
                outModel = modelModifier.ToModel();
            }
        });
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into an empty editor, a value of 'string.Empty' (2024-04-14)<br/>
    /// Purpose: The string argument of Insert(...) has 3 states as far as the text editor is concerned.<br/>
    ///              -It is null<br/>
    ///              -It is empty<br/>
    ///              -It not null, and it is not empty<br/>
    ///              -NOTE: the string being whitespace is a valid possibility, it falls under "not null, and not empty"
    ///                     just as if it were a LetterOrDigit<br/>
    ///           This bears significance as the 'empty' case.
    /// </summary>
    [Fact]
    public void Insert_Into_EmptyEditor_Empty_String()
    {
        var (inModel, modelModifier) = EmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: string.Empty,
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

            modelModifier.Insert(
                value: string.Empty,
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        //
        // Here, the post assertions are equivalent to the Pre-Assertions because
        // inserting 'string.Empty' will run all of the insertion code, but have no effect.
        {
            // Obnoxiously write the constant value for the initialContent's length instead of capturing the TextEditorModel
            // constructor's 'initialContent' parameter, then checking '.Length'.
            //
            // This makes it more clear if the source text changes (accidentally or intentionally).
            // If one day this assertion fails, then someone touched the source text.
            Assert.Equal(0, modelModifier.CharCount);

            // The file extension should NOT change as a result of inserting content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
            // Therefore, the count remains 0.
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, the count remains 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, the count remains 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, the count remains 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, the count remains 1, due to the existance of the special-'EndOfFile' line ending.
                Assert.Equal(1, modelModifier.LineEndList.Count);

                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, LineEndPositionList is expected to not contain any CarriageReturn(s)
                // Assert that the only line ending in the text is the 'EndOfFile'.
                Assert.Equal(LineEndKind.EndOfFile, modelModifier.LineEndList.Single().LineEndKind);

                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, LineEndPositionList is expected to not contain any LineFeed(s)
                // Assert that the only line ending in the text is the 'EndOfFile'.
                Assert.Equal(LineEndKind.EndOfFile, modelModifier.LineEndList.Single().LineEndKind);

                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, LineEndPositionList is expected to not contain any CarriageReturnLineFeed(s)
                // Assert that the only line ending in the text is the 'EndOfFile'.
                Assert.Equal(LineEndKind.EndOfFile, modelModifier.LineEndList.Single().LineEndKind);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // Inserting 'string.Empty' will run all of the insertion code, but have no effect.
                // Therefore, the 'EndOfFile' is unchanged.
                var endOfFile = modelModifier.LineEndList.Single();
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
    #endregion

    #region Insert_Into_NotEmptyEditor
    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, at positionIndex equal to 0 (2024-04-14)<br/>
    /// Purpose: Index 0 is the first valid positionIndex, thus it bears significance as a boundary.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_At_PositionIndex_Zero()
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

            modelModifier.Insert(
                "\n" +   // LineFeed
                "b9" +   // LetterOrDigit-Lowercase
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "\r\n" + // CarriageReturnLineFeed
                "\t" +   // Tab
                "$" +    // SpecialCharacter
                ";" +    // Punctuation
                " ",     // Space
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'Insert(...)' parameter has a string length of '12',
            // But, as of this comment, the insertion of line ending characters
            // other than line feed is not supported.
            // Therefore, the "\r" and the "\r\n" are replaced with "\n".
            //
            // As a result of the "\r\n" replacement with "\n",
            // one character was lost, therefore the length ends up being 11.
            //
            // The inital content's length was 12,
            // so the length after insertion is 23
            Assert.Equal(23, modelModifier.CharCount);

            // The file extension should NOT change as a result of inserting content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel.
            // Furthermore, 1 tab key was inserted.
            // Therefore the count is 2.
            Assert.Equal(2, modelModifier.TabKeyPositionList.Count);
            // First tab key
            var tabKeyPosition = modelModifier.TabKeyPositionList[0];
            Assert.Equal(7, tabKeyPosition);
            // Second tab key
            tabKeyPosition = modelModifier.TabKeyPositionList[1];
            Assert.Equal(19, tabKeyPosition);

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count remains 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(14, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(15, carriageReturn.EndPositionIndexExclusive);

                // 1 LineFeed was included in the initial content for the TextEditorModel.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear.
                // Because 3 total LineEnds were inserted, the count of LineFeed(s) is 4.
                Assert.Equal(
                    4,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeedMatches = modelModifier.LineEndList.Where(x => x.LineEndKind == LineEndKind.LineFeed).ToArray();
                // First LineFeed
                var lineFeed = lineFeedMatches[0];
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);
                // Second LineFeed
                lineFeed = lineFeedMatches[1];
                Assert.Equal(3, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(4, lineFeed.EndPositionIndexExclusive);
                // Third LineFeed
                lineFeed = lineFeedMatches[2];
                Assert.Equal(6, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(7, lineFeed.EndPositionIndexExclusive);
                // Fourth LineFeed
                lineFeed = lineFeedMatches[3];
                Assert.Equal(11, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(12, lineFeed.EndPositionIndexExclusive);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count remains 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(17, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(19, carriageReturnLineFeed.EndPositionIndexExclusive);

                // 3 line endings were part of the constructor's initialContent,
                // 3 line endings where inserted,
                // Therefore, there are 7 in total if one then includes the special-'EndOfFile' LineEnd.
                Assert.Equal(7, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The insert will have moved the 'EndOfFile' LineEnd from positionIndex 12 to a larger value.
                // Specifically, the 'EndOfFile' should move by the length of the text inserted.
                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(23, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(23, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                // Insertion was done at (rowIndex: 0, columnIndex: 0).
                //
                // The text inserted has its endpoint at (rowIndex: 3, columnIndex: 4)
                //
                // Therefore, end result is cursor_rowIndex + insertionContentFinalRowIndex,
                // and the columnIndex ends up being the insertionContentFinalColumnIndex.
                //
                // Which is (rowIndex: 3, columnIndex: 4)
                Assert.Equal(3, cursorModifier.LineIndex);
                Assert.Equal(4, cursorModifier.ColumnIndex);
                Assert.Equal(4, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, at positionIndex equal to DocumentLength (2024-04-14)<br/>
    /// Purpose: Index of 'DocumentLength' is the last valid positionIndex, thus it bears significance as a boundary.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_At_PositionIndex_DocumentLength()
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

            modelModifier.Insert(
                "\n" +   // LineFeed
                "b9" +   // LetterOrDigit-Lowercase
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "\r\n" + // CarriageReturnLineFeed
                "\t" +   // Tab
                "$" +    // SpecialCharacter
                ";" +    // Punctuation
                " ",     // Space
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'Insert(...)' parameter has a string length of '12',
            // But, as of this comment, the insertion of line ending characters
            // other than line feed is not supported.
            // Therefore, the "\r" and the "\r\n" are replaced with "\n".
            //
            // As a result of the "\r\n" replacement with "\n",
            // one character was lost, therefore the length ends up being 11.
            //
            // The inital content's length was 12,
            // so the length after insertion is 23
            Assert.Equal(23, modelModifier.CharCount);

            // The file extension should NOT change as a result of inserting content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel.
            // Furthermore, 1 tab key was inserted.
            // Therefore the count is 2.
            Assert.Equal(2, modelModifier.TabKeyPositionList.Count);
            // First tab key
            var tabKeyPosition = modelModifier.TabKeyPositionList[0];
            Assert.Equal(8, tabKeyPosition);
            // Second tab key
            tabKeyPosition = modelModifier.TabKeyPositionList[1];
            Assert.Equal(19, tabKeyPosition);

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count remains 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                // 1 LineFeed was included in the initial content for the TextEditorModel.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear.
                // Because 3 total LineEnds were inserted, the count of LineFeed(s) is 4.
                Assert.Equal(
                    4,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeedMatches = modelModifier.LineEndList.Where(x => x.LineEndKind == LineEndKind.LineFeed).ToArray();
                // First LineFeed
                var lineFeed = lineFeedMatches[0];
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);
                // Second LineFeed
                lineFeed = lineFeedMatches[1];
                Assert.Equal(12, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(13, lineFeed.EndPositionIndexExclusive);
                // Third LineFeed
                lineFeed = lineFeedMatches[2];
                Assert.Equal(15, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(16, lineFeed.EndPositionIndexExclusive);
                // Fourth LineFeed
                lineFeed = lineFeedMatches[3];
                Assert.Equal(18, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(19, lineFeed.EndPositionIndexExclusive);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel.
                // But, for the time being, only LineFeed insertions are permitted to avoid insertion of '\r'
                // into a '\n' that follows causing a "\r\n" to appear. Therefore the count remains 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                // 3 line endings were part of the constructor's initialContent,
                // 3 line endings where inserted,
                // Therefore, there are 7 in total if one then includes the special-'EndOfFile' LineEnd.
                Assert.Equal(7, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The insert will have moved the 'EndOfFile' LineEnd from positionIndex 12 to a larger value.
                // Specifically, the 'EndOfFile' should move by the length of the text inserted.
                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(23, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(23, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                // Insertion was done at (rowIndex: 3, columnIndex: 4).
                //
                // The text inserted has its endpoint at (rowIndex: 3, columnIndex: 4)
                //
                // Therefore, end result is cursor_rowIndex + insertionContentFinalRowIndex,
                // and the columnIndex ends up being the insertionContentFinalColumnIndex.
                //
                // Which is (rowIndex: 4, columnIndex: 4)
                Assert.Equal(6, cursorModifier.LineIndex);
                Assert.Equal(4, cursorModifier.ColumnIndex);
                Assert.Equal(4, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, bool, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, at positionIndex between 0 and DocumentLength; exclusive bounds (2024-04-14)<br/>
    /// Purpose: With 0 being the first valid position, and DocumentLength being the last valid position;
    ///          testing a value between the valid positionIndex boundaries bears significance as a unique case.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive()
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
                lineIndex: 1,
                columnIndex: 1,
                isPrimaryCursor: true);
            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.SetLineEndKindPreference(LineEndKind.LineFeed);

            modelModifier.Insert(
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " ",     // Space
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(23, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(2, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                // The constructor invocation inserted 'as is', prior to the insertion step.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(14, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(15, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(
                    4,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeedMatches = modelModifier.LineEndList.Where(x => x.LineEndKind == LineEndKind.LineFeed).ToArray();
                // First LineFeed
                var lineFeed = lineFeedMatches[0];
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);
                // Second LineFeed
                lineFeed = lineFeedMatches[1];
                Assert.Equal(2, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(3, lineFeed.EndPositionIndexExclusive);
                // Third LineFeed
                lineFeed = lineFeedMatches[2];
                Assert.Equal(5, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(6, lineFeed.EndPositionIndexExclusive);
                // Fourth LineFeed
                lineFeed = lineFeedMatches[3];
                Assert.Equal(8, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(9, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                // StartPositionIndexInclusive
                Assert.Equal(17, carriageReturnLineFeed.StartPositionIndexInclusive);
                // EndPositionIndexExclusive
                Assert.Equal(19, carriageReturnLineFeed.EndPositionIndexExclusive);
                
                Assert.Equal(7, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(23, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(23, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(4, cursorModifier.LineIndex);
                Assert.Equal(4, cursorModifier.ColumnIndex);
                Assert.Equal(4, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, at positionIndex equal to -1 (2024-04-14)<br/>
    /// Purpose: Index -1 is a value which is less than than the lower-bound for valid positionIndices,
    ///          thus it bears significance as a unique case.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_At_PositionIndex_Negative_One()
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

                modelModifier.Insert(
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " ",     // Space
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
                outModel = modelModifier.ToModel();
            }
        });
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, at a positionIndex equal to '1 + DocumentLength' (2024-04-14)<br/>
    /// Purpose: Index '1 + DocumentLength' is a value which is greater than than the upper-bound for valid positionIndices,
    ///          thus it bears significance as a unique case.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_At_Position_One_Plus_DocumentLength()
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
        Assert.Throws<LuthetusTextEditorException>((Action)(() =>
        {
            // Do something
            TextEditorModel outModel;
            TextEditorCursorModifier cursorModifier;
            {
                var lastLine = modelModifier.GetLineInformation((int)(modelModifier.LineCount - 1));

                var cursor = new TextEditorCursor(
                    lineIndex: lastLine.Index,
                    columnIndex: 1 + lastLine.LastValidColumnIndex,
                    isPrimaryCursor: true);
                cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Insert(
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " ",     // Space
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
                outModel = modelModifier.ToModel();
            }
        }));
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, a null string (2024-04-14)<br/>
    /// Purpose: The string argument of Insert(...) has 3 states as far as the text editor is concerned.<br/>
    ///              -It is null<br/>
    ///              -It is empty<br/>
    ///              -It not null, and it is not empty<br/>
    ///              -NOTE: the string being whitespace is a valid possibility, it falls under "not null, and not empty"
    ///                     just as if it were a LetterOrDigit<br/>
    ///           This bears significance as the 'null' case.
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_Null_String()
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
        Assert.Throws<NullReferenceException>(() =>
        {
            // Do something
            TextEditorModel outModel;
            {
                var cursor = new TextEditorCursor(
                        lineIndex: 0,
                        columnIndex: 0,
                        isPrimaryCursor: true);
                var cursorModifier = new TextEditorCursorModifier(cursor);
                var cursorModifierBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier>() { cursorModifier });

                modelModifier.Insert(
                    // The null-forgiving operator was used here
                    // because the test purposefully wants to pass in null,
                    // to see what happens.
                    value: null!,
                    cursorModifierBag,
                    cancellationToken: CancellationToken.None);
                outModel = modelModifier.ToModel();
            }
        });
    }

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, a value of 'string.Empty' (2024-04-14)<br/>
    /// Purpose: The string argument of Insert(...) has 3 states as far as the text editor is concerned.<br/>
    ///              -It is null<br/>
    ///              -It is empty<br/>
    ///              -It not null, and it is not empty<br/>
    ///              -NOTE: the string being whitespace is a valid possibility, it falls under "not null, and not empty"
    ///                     just as if it were a LetterOrDigit<br/>
    ///           This bears significance as the 'empty' case.
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_Empty_String()
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

            modelModifier.Insert(
                value: string.Empty,
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        //
        // Here, the post assertions are equivalent to the Pre-Assertions because
        // inserting 'string.Empty' will run all of the insertion code, but have no effect.
        {
            // Obnoxiously write the constant value for the initialContent's length instead of capturing the TextEditorModel
            // constructor's 'initialContent' parameter, then checking '.Length'.
            //
            // This makes it more clear if the source text changes (accidentally or intentionally).
            // If one day this assertion fails, then someone touched the source text.
            Assert.Equal(12, modelModifier.CharCount);

            // The file extension should NOT change as a result of inserting content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the string that was passed to the constructor.
            // And the insertion is expected to do nothing.
            // Therefore, the Count remains 1.
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the string that was passed to the constructor.
                // And the insertion is expected to do nothing.
                // Therefore, the Count remains 1.
                // NOTE: While the Insert(...) method does not allow '\r' or '\r\n', the constructor does.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                // 1 LineFeed was included in the string that was passed to the constructor.
                // And the insertion is expected to do nothing.
                // Therefore, the Count remains 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                // 1 CarriageReturnLineFeed was included in the string that was passed to the constructor.
                // And the insertion is expected to do nothing.
                // Therefore, the Count remains 1.
                // NOTE: While the Insert(...) method does not allow '\r' or '\r\n', the constructor does.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                // 3 line endings where included in the string that was passed to the constructor,
                // And the insertion is expected to do nothing.
                // Therefore, there are 4 in total if one then includes the special-'EndOfFile' LineEnd.
                Assert.Equal(4, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The insertion is expected to do nothing,
                // therefore the positionIndex remains 12.
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

    /// <summary>
    /// <see cref="TextEditorModel.Insert(string, CursorModifierBagTextEditor, bool, CancellationToken)"/><br/>
    /// Case: Insert into a not empty editor, at positionIndex between 0 and DocumentLength; exclusive bounds
    ///       while the cursor has an active selection(2024-04-14)<br/>
    /// Purpose: insertion while the cursor has an active selection means that the selected text
    ///          must be deleted prior to insertion, thus making this a unique case.<br/>
    /// </summary>
    [Fact]
    public void Insert_Into_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_With_Selection()
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
            // Select "B\r\n"
            var cursor = new TextEditorCursor(
                LineIndex: 1,
                ColumnIndex: 1,
                PreferredColumnIndex: 1,
                IsPrimaryCursor: true,
                new TextEditorSelection(
                    5,
                    8));
            cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.SetLineEndKindPreference(LineEndKind.LineFeed);
            modelModifier.Insert(
                "\n" +   // LineFeed
                "b9" +   // LetterOrDigit-Lowercase
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "\r\n" + // CarriageReturnLineFeed
                "\t" +   // Tab
                "$" +    // SpecialCharacter
                ";" +    // Punctuation
                " ",     // Space
                cursorModifierBag,
                cancellationToken: CancellationToken.None);
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(20, modelModifier.CharCount);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(2, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeedMatches = modelModifier.LineEndList.Where(x => x.LineEndKind == LineEndKind.LineFeed).ToArray();
                // First LineFeed
                var lineFeed = lineFeedMatches[0];
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);
                // Second LineFeed
                lineFeed = lineFeedMatches[1];
                Assert.Equal(5, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(6, lineFeed.EndPositionIndexExclusive);
                // Third LineFeed
                lineFeed = lineFeedMatches[2];
                Assert.Equal(8, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(9, lineFeed.EndPositionIndexExclusive);
                // Fourth LineFeed
                lineFeed = lineFeedMatches[3];
                Assert.Equal(11, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(12, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(6, modelModifier.LineEndList.Count);

                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(20, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(20, endOfFile.EndPositionIndexExclusive);
            }

            // Cursor related code-block-grouping:
            {
                Assert.Equal(5, cursorModifier.LineIndex);
                Assert.Equal(4, cursorModifier.ColumnIndex);
                Assert.Equal(4, cursorModifier.PreferredColumnIndex);
                Assert.True(cursorModifier.IsPrimaryCursor);
                Assert.Equal(0, cursorModifier.SelectionEndingPositionIndex);
                Assert.Null(cursorModifier.SelectionAnchorPositionIndex);
            }
        }
    }
    #endregion
}