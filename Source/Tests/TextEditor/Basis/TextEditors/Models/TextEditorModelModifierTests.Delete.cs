using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

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
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Zero_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_DocumentLength_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_NegativeOne_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Zero_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_BackspaceEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_DocumentLength_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_BackspaceEnum()
    {
        throw new NotImplementedException();
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
                    " "
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        {
            modelModifier.Delete_Unsafe(
                    rowIndex: 0,
                    columnIndex: 0,
                    count: 12,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionsList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(0, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(0, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(0, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
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
                    " "
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
                    " "
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
                    " "
                ),
             */
            modelModifier.Delete_Unsafe(
                    rowIndex: 1,
                    columnIndex: 0,
                    count: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
                    " "
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
                modelModifier.Delete_Unsafe(
                    rowIndex: 0,
                    columnIndex: -1,
                    count: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
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
                    " "
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        {
            modelModifier.Delete_Unsafe(
                    rowIndex: 0,
                    columnIndex: 0,
                    count: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(3, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
                    " "
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
                    " "
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
                    " "
                ),
             */
            modelModifier.Delete_Unsafe(
                    rowIndex: 1,
                    columnIndex: 0,
                    count: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(11, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(
                7,
                modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        2,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        3,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        5,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        7,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(11, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(11, endOfFile.EndPositionIndexExclusive);
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
                    " "
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        {
            // Deleting at positionIndex of 'DocumentLength' is awkward, because the API accepts a 'rowIndex' and 'columnIndex'
            // Here, 'rowIndex: modelModifier.LineCount - 1' gets the rowIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            modelModifier.Delete_Unsafe(
                    rowIndex: modelModifier.LineCount - 1,
                    columnIndex: 0,
                    count: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
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
                    " "
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        {
            // Deleting at positionIndex of '1 + DocumentLength' is awkward, because the API accepts a 'rowIndex' and 'columnIndex'
            // Here, 'rowIndex: modelModifier.LineCount - 1' gets the rowIndex that the 'EndOfFile' resides at.
            // The row index for 'EndOfFile', and columnIndex 0, is a valid place for insertion.
            // This equates to 'DocumentLength'.
            // Now add 1 to the columnIndex to be one position further than 'DocumentLength'.
            modelModifier.Delete_Unsafe(
                    rowIndex: modelModifier.LineCount - 1,
                    columnIndex: 1,
                    count: 1,
                    expandWord: false,
                    cancellationToken: CancellationToken.None,
                    deleteKind: TextEditorModelModifier.DeleteKind.Delete
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);

            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            Assert.Single(modelModifier.PartitionList);

            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }
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
                    " "
                ),
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);

        // Do something
        TextEditorModel outModel;
        {
            // DocumentLength is equivalent to:
            // rowIndex: modelModifier.LineCount - 1, and columnIndex: 0
            //
            // A count of '12', with 'DeleteKind.BackSpace',
            // should delete all the content in the text editor (relative to the current test data).
            modelModifier.Delete_Unsafe(
                    rowIndex: modelModifier.LineCount - 1,
                    columnIndex: 0,
                    12,
                    false,
                    CancellationToken.None,
                    TextEditorModelModifier.DeleteKind.Backspace
                );
            outModel = modelModifier.ToModel();
        }

        // Post-assertions
        {
            Assert.Equal(0, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(0, modelModifier.TabKeyPositionsList.Count);

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(0, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                Assert.Equal(0, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                Assert.Equal(0, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                Assert.Equal(1, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }
        }
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_MaintainCounters_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_NegativeOne_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Zero_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_DocumentLength_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_OnePlusDocumentLength_BackspaceEnum()
    {
        throw new NotImplementedException();
    }
    #endregion
}