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
    public void Delete_From_EmptyEditor_Causes_DecreaseCounters_DeleteEnum()
    {
        throw new NotImplementedException();

        // Create test data
        TextEditorModel inModel;
        TextEditorModelModifier modelModifier;
        {
            inModel = new TextEditorModel(
                new ResourceUri($"/{nameof(Delete_From_EmptyEditor_Causes_DecreaseCounters_DeleteEnum)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
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
                null,
                null,
                // Provide a value for the optional parameter, so a change in the default value won't break this test.
                partitionSize: 4096);

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }

        // Do something
        TextEditorModel outModel;
        {
            modelModifier.Delete_Unsafe(
                    rowIndex: 0,
                    columnIndex: 0,
                    12,
                    false,
                    CancellationToken.None,
                    TextEditorModelModifier.DeleteKind.Delete
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
    public void Delete_From_EmptyEditor_Causes_MaintainCounters_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_Causes_IncreaseCounters_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Exclusive_LowerBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Inclusive_LowerBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Inclusive_UpperBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Exclusive_UpperBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_Causes_DecreaseCounters_BackspaceEnum()
    {
        throw new NotImplementedException();

        // Create test data
        TextEditorModel inModel;
        TextEditorModelModifier modelModifier;
        {
            inModel = new TextEditorModel(
                new ResourceUri($"/{nameof(Delete_From_EmptyEditor_Causes_DecreaseCounters_BackspaceEnum)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
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
                null,
                null,
                // Provide a value for the optional parameter, so a change in the default value won't break this test.
                partitionSize: 4096);

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }

        // Do something
        TextEditorModel outModel;
        {
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
    public void Delete_From_EmptyEditor_Causes_MaintainCounters_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_Causes_IncreaseCounters_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Exclusive_LowerBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Inclusive_LowerBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_BackspaceEnum()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Inclusive_UpperBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_EmptyEditor_At_PositionIndex_EqualTo_Exclusive_UpperBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Delete_From_NotEmptyEditor
    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_DecreaseCounters_DeleteEnum()
    {
        throw new NotImplementedException();

        // Create test data
        TextEditorModel inModel;
        TextEditorModelModifier modelModifier;
        {
            inModel = new TextEditorModel(
                new ResourceUri($"/{nameof(Delete_From_NotEmptyEditor_Causes_DecreaseCounters_DeleteEnum)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
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
                null,
                null,
                // Provide a value for the optional parameter, so a change in the default value won't break this test.
                partitionSize: 4096);

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }

        // Do something
        TextEditorModel outModel;
        {
            modelModifier.Delete_Unsafe(
                    rowIndex: 0,
                    columnIndex: 0,
                    12,
                    false,
                    CancellationToken.None,
                    TextEditorModelModifier.DeleteKind.Delete
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
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_IncreaseCounters_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Exclusive_LowerBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Inclusive_LowerBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Inclusive_UpperBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Exclusive_UpperBound_DeleteEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_Causes_DecreaseCounters_BackspaceEnum()
    {
        throw new NotImplementedException();

        // Create test data
        TextEditorModel inModel;
        TextEditorModelModifier modelModifier;
        {
            inModel = new TextEditorModel(
                new ResourceUri($"/{nameof(Delete_From_NotEmptyEditor_Causes_DecreaseCounters_BackspaceEnum)}.txt"),
                DateTime.UtcNow,
                ExtensionNoPeriodFacts.TXT,
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
                null,
                null,
                // Provide a value for the optional parameter, so a change in the default value won't break this test.
                partitionSize: 4096);

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            Assert.Equal(12, modelModifier.DocumentLength);
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);
            Assert.Single(modelModifier.PartitionList);
            Assert.Equal(1, modelModifier.TabKeyPositionsList.Count);
            Assert.Equal(8, modelModifier.TabKeyPositionsList.Single());

            // LineEnd related code-block-grouping:
            {
                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                var carriageReturn = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                Assert.Equal(3, carriageReturn.StartPositionIndexInclusive);
                Assert.Equal(4, carriageReturn.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                var lineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                Assert.Equal(0, lineFeed.StartPositionIndexInclusive);
                Assert.Equal(1, lineFeed.EndPositionIndexExclusive);

                Assert.Equal(1, modelModifier.LineEndKindCountsList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                var carriageReturnLineFeed = modelModifier.LineEndPositionList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                Assert.Equal(6, carriageReturnLineFeed.StartPositionIndexInclusive);
                Assert.Equal(8, carriageReturnLineFeed.EndPositionIndexExclusive);

                Assert.Equal(4, modelModifier.LineEndPositionList.Count);

                var endOfFile = modelModifier.LineEndPositionList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }

        // Do something
        TextEditorModel outModel;
        {
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
    public void Delete_From_NotEmptyEditor_Causes_IncreaseCounters_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Exclusive_LowerBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Inclusive_LowerBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_Between_0_And_DocumentLength_Exclusive_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Inclusive_UpperBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Delete_From_NotEmptyEditor_At_PositionIndex_EqualTo_Exclusive_UpperBound_BackspaceEnum()
    {
        throw new NotImplementedException();
    }
    #endregion
}