﻿using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelModifier"/>
/// </summary>
public partial class TextEditorModelModifierTests : TextEditorTestBase
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
        Assert.Equal(inModel.EditBlockList, outModel.EditBlockList);
        Assert.Equal(inModel.LineEndList, outModel.LineEndList);
        Assert.Equal(inModel.LineEndKindCountList, outModel.LineEndKindCountList);
        Assert.Equal(inModel.PresentationModelList, outModel.PresentationModelList);
        Assert.Equal(inModel.TabKeyPositionList, outModel.TabKeyPositionList);
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
    /// <see cref="TextEditorModelModifier.ClearContent()"/>
    /// ----------------------------------------------------
    /// This test was deemed valuable on (2024-04-13)
    /// </summary>
    [Fact]
    public void ClearContent()
    {
        // Create test data
        TextEditorModel inModel;
        TextEditorModelModifier modelModifier;
        {
            inModel = new TextEditorModel(
                new ResourceUri($"/{nameof(ClearContent)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
                (
                    "A2" + // Uppercase letter, and Digit
                    "\n" + // LineFeed
                    "\r" + // CarriageReturn
                    "z$" + // Lowercase letter, and special character
                    "\t" + // Tab
                    "\r\n" // CarriageReturnLineFeed
                ),
                null,
                null,
                // Partition size is defaulted to 4096, but explicitly passing the value ensures that
                // a change in the default value won't break this test.
                partitionSize: 4096); 

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            // Obnoxiously write the constant '9' instead of capturing the TextEditorModel
            // constructor's 'initialContent' parameter, then checking '.Length'.
            //
            // This makes it more clear if the source text changes (accidentally or intentionally).
            // If one day this assertion fails, then someone touched the source text.
            Assert.Equal(9, modelModifier.DocumentLength);

            // The file extension should NOT change as a result of clearing the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel, therefore the Count is 1.
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel, therefore the count is 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // 1 LineFeed was included in the initial content for the TextEditorModel, therefore the count is 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel, therefore the count is 1.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                // 3 line endings where included in the initial content for the TextEditorModel,
                // plus the always existing 'EndOfFile' line ending, means the Count is 4.
                Assert.Equal(4, modelModifier.LineEndList.Count);

                // When invoking the constructor for the TextEditorModel, a LineFeed was
                // the first LineEnd occurence, in regards to position from start to end.
                var lineFeed = modelModifier.LineEndList[0];
                Assert.Equal(2, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(3, lineFeed.EndPositionIndexExclusive);
                Assert.Equal(LineEndKind.LineFeed, lineFeed.LineEndKind);

                // When invoking the constructor for the TextEditorModel, a CarriageReturn was
                // the second LineEnd occurence, in regards to position from start to end.
                var carriageReturn = modelModifier.LineEndList[1];
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);
                Assert.Equal(LineEndKind.CarriageReturn, carriageReturn.LineEndKind);

                // When invoking the constructor for the TextEditorModel, a CarriageReturnLineFeed was
                // the third LineEnd occurence, in regards to position from start to end.
                var carriageReturnLineFeed = modelModifier.LineEndList[2];
                Assert.Equal(7, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(9, carriageReturnLineFeed.EndPositionIndexExclusive);
                Assert.Equal(LineEndKind.CarriageReturnLineFeed, carriageReturnLineFeed.LineEndKind);

                // A TextEditorModel always contains at least 1 LineEnd.
                // This LineEnd marks the 'EndOfFile'.
                //
                // Given that the constructor for 'TextEditorModel' takes the 'initialContent'
                // and sets the model's content as it, the 'EndOfFile' is no longer at positionIndex 0,
                // but instead shifted by the length of the 'initialContent'.
                var endOfFile = modelModifier.LineEndList[3];
                Assert.Equal(9, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(9, endOfFile.EndPositionIndexExclusive);
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
            }
        }

        // Do something
        TextEditorModel outModel;
        {
            modelModifier.ClearContent();
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // Clearing the content of a 'TextEditorModel' should result in a 'DocumentLength' of 0.
            Assert.Equal(0, modelModifier.DocumentLength);

            // The file extension should NOT change as a result of clearing the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel but,
            // now that the content is cleared, the Count is 0.
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel but,
                // after clearing the content, the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // 1 LineFeed was included in the initial content for the TextEditorModel but,
                // after clearing the content, the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel but,
                // after clearing the content, the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                // 3 line endings where included in the initial content for the TextEditorModel but,
                // after clearing the content, only the special-'EndOfFile' LineEnd should remain, so the count is 1.
                Assert.Equal(1, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd.
                // This LineEnd marks the 'EndOfFile'.
                //
                // The constructor for 'TextEditorModel' takes the 'initialContent' and sets the model's content as it,
                // this results in the 'EndOfFile' positionIndex changing.
                // But, since the content was cleared, the 'EndOfFile' positionIndex should return to 0.
                var endOfFile = modelModifier.LineEndList[0];
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
            }
        }
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
    /// <see cref="TextEditorModelModifier.SetContent(string)"/><br/>
    /// Case: Decrease counters (2024-04-14)<br/>
    /// <br/>
    /// Setting the content can result in a decrease in the amount of line endings in a text editor, as just one example.
    /// Erroneous example: one has a text editor with the text value of:<br/>
    /// ∙∙∙ (<br/>
    /// ∙∙∙∙∙∙∙ "\n" ∙ + // LineFeed<br/>
    /// ∙∙∙∙∙∙∙ "b9" ∙ + // LetterOrDigit-Lowercase<br/>
    /// ∙∙∙∙∙∙∙ "\r" ∙ + // CarriageReturn<br/>
    /// ∙∙∙∙∙∙∙ "9B" ∙ + // LetterOrDigit-Uppercase<br/>
    /// ∙∙∙∙∙∙∙ "\r\n" + // CarriageReturnLineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\t" ∙ + // Tab<br/>
    /// ∙∙∙∙∙∙∙ "$" ∙∙ + // SpecialCharacter<br/>
    /// ∙∙∙∙∙∙∙ ";" ∙∙ + // Punctuation<br/>
    /// ∙∙∙∙∙∙∙ " " ∙∙∙∙ // Space<br/>
    /// ∙∙∙ )<br/>
    /// so, a text editor with 4 line endings.<br/>
    /// <br/>
    /// Now one invokes: 'SetContent(string.Empty)'<br/>
    /// <br/>
    /// but, erroneously, the text editor still thinks there are 4 line endings in the text.
    /// Correction for the example: one has 4 line endings, then invokes 'SetContent(string.Empty)',
    /// and the text editor now thinks there is 1 line ending in the text.
    /// Note: all text editor models have at least 1 line ending, which is known as the 'EndOfFile'.
    /// </summary>
    [Fact]
    public void SetContent_DecreaseCounters()
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
            modelModifier.SetContent(
                    string.Empty
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'SetContent' parameter has a string length of '0'. Therefore, the DocumentLength becomes '0'.
            Assert.Equal(0, modelModifier.DocumentLength);

            // The file extension should NOT change as a result of setting the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel but,
            // now that the content is set to 'string.Empty', the Count is 0.
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel but,
                // after setting the content, the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // 1 LineFeed was included in the initial content for the TextEditorModel but,
                // after setting the content, the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel but,
                // after setting the content, the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                // 3 line endings where included in the initial content for the TextEditorModel but,
                // after setting the content, only the special-'EndOfFile' LineEnd should remain, so the count is 1.
                Assert.Equal(1, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd.
                // This LineEnd marks the 'EndOfFile'.
                //
                // The constructor for 'TextEditorModel' takes the 'initialContent' and sets the model's content as it,
                // this results in the 'EndOfFile' positionIndex changing.
                // But, since the content was set to 'string.Empty', the 'EndOfFile' positionIndex should return to 0.
                var endOfFile = modelModifier.LineEndList[0];
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetContent(string)"/><br/>
    /// Case: Maintain counters (2024-04-14)<br/>
    /// <br/>
    /// Setting the content can result in NO-change in the amount of line endings in a text editor.
    /// Erroneous example: one has a text editor with the text value of:<br/>
    /// ∙∙∙ (<br/>
    /// ∙∙∙∙∙∙∙ "\n" ∙ + // LineFeed<br/>
    /// ∙∙∙∙∙∙∙ "b9" ∙ + // LetterOrDigit-Lowercase<br/>
    /// ∙∙∙∙∙∙∙ "\r" ∙ + // CarriageReturn<br/>
    /// ∙∙∙∙∙∙∙ "9B" ∙ + // LetterOrDigit-Uppercase<br/>
    /// ∙∙∙∙∙∙∙ "\r\n" + // CarriageReturnLineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\t" ∙ + // Tab<br/>
    /// ∙∙∙∙∙∙∙ "$" ∙∙ + // SpecialCharacter<br/>
    /// ∙∙∙∙∙∙∙ ";" ∙∙ + // Punctuation<br/>
    /// ∙∙∙∙∙∙∙ " " ∙∙∙∙ // Space<br/>
    /// ∙∙∙ )<br/>
    /// then invokes SetContent(string) with the following string:<br/>
    /// ∙∙∙ (<br/>
    /// ∙∙∙∙∙∙∙ "\t" ∙ + // Tab<br/>
    /// ∙∙∙∙∙∙∙ ";" ∙∙ + // Punctuation<br/>
    /// ∙∙∙∙∙∙∙ "\r\n" + // CarriageReturnLineFeed<br/>
    /// ∙∙∙∙∙∙∙ " " ∙∙∙∙ // Space<br/>
    /// ∙∙∙∙∙∙∙ "\n" ∙ + // LineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\r" ∙ + // CarriageReturn<br/>
    /// ∙∙∙∙∙∙∙ "9B" ∙ + // LetterOrDigit-Uppercase<br/>
    /// ∙∙∙∙∙∙∙ "$" ∙∙ + // SpecialCharacter<br/>
    /// ∙∙∙∙∙∙∙ "b9" ∙ + // LetterOrDigit-Lowercase<br/>
    /// ∙∙∙ )<br/>
    /// Here, the text editor started with 4 line endings, and it is expected to in the end have 4 line endings.
    /// The overall count of the line endings has not changed, but the positioning of each LineEnd has.
    /// The erroneous behavior here could be, not updating the LineEndPositionList.
    /// Thereby leaving the '\n' character as having a 'StartPositionIndex' of '0',
    /// All the while, the '\n' character is in actually at a 'StartPositionIndex' of '7'.
    /// Note: all text editor models have at least 1 line ending, which is known as the 'EndOfFile'.
    /// </summary>
    [Fact]
    public void SetContent_MaintainCounters()
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
            modelModifier.SetContent(
                "\t" +   // Tab
                ";" +    // Punctuation
                "\r\n" + // CarriageReturnLineFeed
                " " +    // Space
                "\n" +   // LineFeed
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "$" +    // SpecialCharacter
                "b9"     // LetterOrDigit-Lowercase
            );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'SetContent' parameter has a string of equal length as the initialContent.
            // Therefore, the DocumentLength stays '12'.
            Assert.Equal(12, modelModifier.DocumentLength);

            // The file extension should NOT change as a result of setting the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel.
            // Now that the content is set to a "mixed up" version of the initialContent,
            // there still is 1 tab key, but it should no longer be in the same position.
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(0, modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel.
                // Now that the content is set to a "mixed up" version of the initialContent,
                // there still is 1 CarriageReturn, but it should no longer be in the same position.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                // The CarriageReturn is at positionIndex '6'. Therefore,
                // get the single entry with a value of '6' from 'LineEndPositionList',
                // and then assert that it is a CarriageReturn.
                Assert.Equal(LineEndKind.CarriageReturn, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 6).LineEndKind);

                // 1 LineFeed was included in the initial content for the TextEditorModel.
                // Now that the content is set to a "mixed up" version of the initialContent,
                // there still is 1 LineFeed, but it should no longer be in the same position.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                // The LineFeed is at positionIndex '5'. Therefore,
                // get the single entry with a value of '5' from 'LineEndPositionList',
                // and then assert that it is a LineFeed.
                Assert.Equal(LineEndKind.LineFeed, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 5).LineEndKind);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel.
                // Now that the content is set to a "mixed up" version of the initialContent,
                // there still is 1 CarriageReturnLineFeed, but it should no longer be in the same position.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                // The CarriageReturnLineFeed is at positionIndex '2'. Therefore,
                // get the single entry with a value of '2' from 'LineEndPositionList',
                // and then assert that it is a CarriageReturnLineFeed.
                Assert.Equal(LineEndKind.CarriageReturnLineFeed, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 2).LineEndKind);

                // 3 line endings where included in the initial content for the TextEditorModel.
                // The content was set to a "mixed up" version of the initialContent.
                // So, no line endings were removed, it just changed their positionIndices.
                // The LineEndPositionList.Count therefore has not changed. It remains at 4.
                Assert.Equal(4, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd.
                // This LineEnd marks the 'EndOfFile'.
                //
                // The constructor for 'TextEditorModel' takes the 'initialContent' and sets the model's content as it,
                // this results in the 'EndOfFile' positionIndex changing.
                // Since the content was set to a "mixed up" version of the initialContent,
                // the positionIndex for the 'EndOfFile' should remain unchanged.
                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.SetContent(string)"/><br/>
    /// Case: Increase counters (2024-04-14)<br/>
    /// <br/>
    /// Setting the content can result in an increase in the amount of line endings in a text editor.
    /// Erroneous example: one has a text editor with the text value of:<br/>
    /// ∙∙∙ (<br/>
    /// ∙∙∙∙∙∙∙ "\n" ∙ + // LineFeed<br/>
    /// ∙∙∙∙∙∙∙ "b9" ∙ + // LetterOrDigit-Lowercase<br/>
    /// ∙∙∙∙∙∙∙ "\r" ∙ + // CarriageReturn<br/>
    /// ∙∙∙∙∙∙∙ "9B" ∙ + // LetterOrDigit-Uppercase<br/>
    /// ∙∙∙∙∙∙∙ "\r\n" + // CarriageReturnLineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\t" ∙ + // Tab<br/>
    /// ∙∙∙∙∙∙∙ "$" ∙∙ + // SpecialCharacter<br/>
    /// ∙∙∙∙∙∙∙ ";" ∙∙ + // Punctuation<br/>
    /// ∙∙∙∙∙∙∙ " " ∙∙∙∙ // Space<br/>
    /// ∙∙∙ )<br/>
    /// then invokes SetContent(string) with the following string:<br/>
    /// ∙∙∙ (<br/>
    /// ∙∙∙∙∙∙∙ "\n" ∙ + // LineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\n" ∙ + // LineFeed<br/>
    /// ∙∙∙∙∙∙∙ "b9" ∙ + // LetterOrDigit-Lowercase<br/>
    /// ∙∙∙∙∙∙∙ "\r" ∙ + // CarriageReturn<br/>
    /// ∙∙∙∙∙∙∙ "\r" ∙ + // CarriageReturn<br/>
    /// ∙∙∙∙∙∙∙ "9B" ∙ + // LetterOrDigit-Uppercase<br/>
    /// ∙∙∙∙∙∙∙ "\r\n" + // CarriageReturnLineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\r\n" + // CarriageReturnLineFeed<br/>
    /// ∙∙∙∙∙∙∙ "\t" ∙ + // Tab<br/>
    /// ∙∙∙∙∙∙∙ "\t" ∙ + // Tab<br/>
    /// ∙∙∙∙∙∙∙ "$" ∙∙ + // SpecialCharacter<br/>
    /// ∙∙∙∙∙∙∙ ";" ∙∙ + // Punctuation<br/>
    /// ∙∙∙∙∙∙∙ ";" ∙∙ + // Punctuation<br/>
    /// ∙∙∙∙∙∙∙ " " ∙∙∙∙ // Space<br/>
    /// ∙∙∙ )<br/>
    /// Here, the text editor started with 4 line endings, and it is expected to in the end have 7 line endings.
    /// The erroneous behavior here could be, not updating the 'LineEndKindCountsList'.
    /// That is, initially the text editor has 1 'LineFeed' character.
    /// It is expected that in the end the text editor will have 2 'LineFeed' characters.
    /// </summary>
    [Fact]
    public void SetContent_IncreaseCounters()
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
            modelModifier.SetContent(
                "\n" +   // LineFeed
                "\n" +   // LineFeed
                "b9" +   // LetterOrDigit-Lowercase
                "\r" +   // CarriageReturn
                "\r" +   // CarriageReturn
                "9B" +   // LetterOrDigit-Uppercase
                "\r\n" + // CarriageReturnLineFeed
                "\r\n" + // CarriageReturnLineFeed
                "\t" +   // Tab
                "\t" +   // Tab
                "$" +    // SpecialCharacter
                ";" +    // Punctuation
                ";" +    // Punctuation
                " "      // Space
            );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            // The 'SetContent' parameter has duplicated any characters which are tracked by the text editor:
            //     '\t'   // Tabs
            //     '\r'   // CarriageReturn is tracked because LineEnd(s) are tracked
            //     '\n'   // NewLine is tracked because LineEnd(s) are tracked
            //     '\r\n' // CarriageReturnNewLine is tracked because LineEnd(s) are tracked
            // Therefore, the DocumentLength is '18'.
            Assert.Equal(18, modelModifier.DocumentLength);

            // The file extension should NOT change as a result of setting the content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the initial content for the TextEditorModel.
            //
            // For this test, any character which is "tracked" by the text editor,
            // has been duplicated.
            //
            // This will make the count for those "tracked" characters go up.
            //
            // So, since there was initially 1 tab key, then the tab keys were
            // duplicated, there are now 2 tab keys.
            Assert.Equal(2, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(12, modelModifier.TabKeyPositionList[0]);
            Assert.Equal(13, modelModifier.TabKeyPositionList[1]);

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the initial content for the TextEditorModel.
                // LineEnd(s) are tracked, therefore this CarriageReturn was duplicated.
                // Therefore, there are now 2 CarriageReturn(s).
                Assert.Equal(
                    2,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                // The first CarriageReturn is at positionIndex '4'. Therefore,
                // get the single entry with a value of '4' from 'LineEndPositionList',
                // and then assert that it is a CarriageReturn.
                Assert.Equal(LineEndKind.CarriageReturn, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 4).LineEndKind);
                // The second CarriageReturn is at positionIndex '5'. Therefore,
                // get the single entry with a value of '5' from 'LineEndPositionList',
                // and then assert that it is a CarriageReturn.
                Assert.Equal(LineEndKind.CarriageReturn, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 5).LineEndKind);

                // 1 LineFeed was included in the initial content for the TextEditorModel.
                // LineEnd(s) are tracked, therefore this LineFeed was duplicated.
                // Therefore, there are now 2 LineFeed(s).
                Assert.Equal(
                    2,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                // The first LineFeed is at positionIndex '0'. Therefore,
                // get the single entry with a value of '0' from 'LineEndPositionList',
                // and then assert that it is a LineFeed.
                Assert.Equal(LineEndKind.LineFeed, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 0).LineEndKind);
                // The second LineFeed is at positionIndex '1'. Therefore,
                // get the single entry with a value of '1' from 'LineEndPositionList',
                // and then assert that it is a LineFeed.
                Assert.Equal(LineEndKind.LineFeed, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 1).LineEndKind);

                // 1 CarriageReturnLineFeed was included in the initial content for the TextEditorModel.
                // LineEnd(s) are tracked, therefore this CarriageReturnLineFeed was duplicated.
                // Therefore, there are now 2 CarriageReturnLineFeed(s).
                Assert.Equal(
                    2,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                // The first CarriageReturnLineFeed is at positionIndex '8'. Therefore,
                // get the single entry with a value of '8' from 'LineEndPositionList',
                // and then assert that it is a CarriageReturnLineFeed.
                Assert.Equal(LineEndKind.CarriageReturnLineFeed, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 8).LineEndKind);
                // The second CarriageReturnLineFeed is at positionIndex '10'. Therefore,
                // get the single entry with a value of '10' from 'LineEndPositionList',
                // and then assert that it is a CarriageReturnLineFeed.
                Assert.Equal(LineEndKind.CarriageReturnLineFeed, modelModifier.LineEndList.Single(x => x.StartPositionIndexInclusive == 10).LineEndKind);

                // 3 line endings where included in the initial content for the TextEditorModel.
                //
                // All the LineEnd(s) were duplicated, therefore there are now 6-explicit line ends,
                // and as well, the 1-implicit EndOfFile line end.
                Assert.Equal(7, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd.
                // This LineEnd marks the 'EndOfFile'.
                //
                // The constructor for 'TextEditorModel' takes the 'initialContent' and sets the model's content as it,
                // this results in the 'EndOfFile' positionIndex changing.
                // Since any tracked characters were duplicated,
                // the EndOfFile would move by 4 characters:
                //     '\t'   // Tabs
                //     '\r'   // CarriageReturn is tracked because LineEnd(s) are tracked
                //     '\n'   // NewLine is tracked because LineEnd(s) are tracked
                //     '\r\n' // CarriageReturnNewLine is tracked because LineEnd(s) are tracked
                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(18, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(18, endOfFile.EndPositionIndexExclusive);
            }
        }
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.ClearAllStatesButKeepEditHistory()"/>
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
    /// <see cref="TextEditorModelModifier.PerformRegisterPresentationModelAction(TextEditorPresentationModel)"/>
    /// </summary>
    [Fact]
    public void PerformRegisterPresentationModelAction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.StartPendingCalculatePresentationModel(Key{TextEditorPresentationModel}, TextEditorPresentationModel)"/>
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
	/// <see cref="TextEditorModelModifier.CharList"/>
	/// </summary>
	[Fact]
    public void CharList()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
	/// <see cref="TextEditorModelModifier.DecorationByteList"/>
	/// </summary>
	[Fact]
    public void DecorationByteList()
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
    /// <see cref="TextEditorModelModifier.LineEndList"/>
    /// </summary>
    [Fact]
    public void LineEndingPositionsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.LineEndKindCountList"/>
    /// </summary>
    [Fact]
    public void LineEndingKindCountsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.PresentationModelList"/>
    /// </summary>
    [Fact]
    public void PresentationModelsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.TabKeyPositionList"/>
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
    public void OnlyLineEndKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModelModifier.UsingLineEndKind"/>
    /// </summary>
    [Fact]
    public void UsingLineEndKind()
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
    public void MostCharactersOnASingleLineTuple()
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
    /// <see cref="TextEditorModelModifier.__Add(RichCharacter)"/>
    /// </summary>
    [Fact]
    public void __Add()
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