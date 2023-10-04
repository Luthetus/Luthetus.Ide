using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public class TextEditorDiffResult
{
    private TextEditorDiffResult(
        string beforeText,
        string afterText,
        TextEditorDiffCell[,] diffMatrix,
        (int sourceWeight, int beforeIndex, int afterIndex) highestSourceWeightTuple,
        string longestCommonSubsequence,
        ImmutableList<TextEditorTextSpan> beforeLongestCommonSubsequenceTextSpanBag,
        ImmutableList<TextEditorTextSpan> afterLongestCommonSubsequenceTextSpanBag)
    {
        BeforeText = beforeText;
        AfterText = afterText;
        DiffMatrix = diffMatrix;
        HighestSourceWeightTuple = highestSourceWeightTuple;
        LongestCommonSubsequence = longestCommonSubsequence;
        BeforeLongestCommonSubsequenceTextSpanBag = beforeLongestCommonSubsequenceTextSpanBag;
        AfterLongestCommonSubsequenceTextSpanBag = afterLongestCommonSubsequenceTextSpanBag;
    }

    public string BeforeText { get; }
    public string AfterText { get; }
    public TextEditorDiffCell[,] DiffMatrix { get; }
    public (int sourceWeight, int beforeIndex, int afterIndex) HighestSourceWeightTuple { get; }
    public string LongestCommonSubsequence { get; }
    public ImmutableList<TextEditorTextSpan> BeforeLongestCommonSubsequenceTextSpanBag { get; }
    public ImmutableList<TextEditorTextSpan> AfterLongestCommonSubsequenceTextSpanBag { get; }

    /// <summary>
    /// This method aims to implement the "An O(ND) Difference Algorithm"
    /// <br/><br/>
    /// Watching https://www.youtube.com/watch?v=9n8jI2267MM
    /// </summary>
    public static TextEditorDiffResult Calculate(
        ResourceUri beforeResourceUri,
        string beforeSourceText,
        ResourceUri afterResourceUri,
        string afterSourceText)
    {
        // Need to build a square two dimensional array.
        //
        // Envisioning that 'beforeText' represents the rows
        // and 'afterText' represents the columns.
        var beforeTextLength = beforeSourceText.Length;
        var afterTextLength = afterSourceText.Length;

        var squaredSize = Math.Max(beforeTextLength, afterTextLength);

        var diffMatrix = new TextEditorDiffCell[squaredSize, squaredSize];

        (int sourceWeight, int beforeIndex, int afterIndex) highestSourceWeightTuple = (-1, -1, -1);

        for (int beforeIndex = 0; beforeIndex < beforeTextLength; beforeIndex++)
        {
            char beforeCharValue = beforeSourceText[beforeIndex];

            for (int afterIndex = 0; afterIndex < afterTextLength; afterIndex++)
            {
                char afterCharValue = afterSourceText[afterIndex];

                var cellSourceWeight = GetCellSourceWeight(
                    diffMatrix,
                    beforeIndex,
                    afterIndex);

                var rowLargestWeightPriorToCurrentColumn = GetRowLargestWeightPriorToCurrentColumn(
                    diffMatrix,
                    beforeIndex,
                    afterIndex);

                if (beforeCharValue == afterCharValue &&
                    cellSourceWeight > rowLargestWeightPriorToCurrentColumn)
                {
                    diffMatrix[beforeIndex, afterIndex] = new TextEditorDiffCell(
                        beforeCharValue,
                        afterCharValue,
                        cellSourceWeight,
                        true);

                    if (cellSourceWeight > highestSourceWeightTuple.sourceWeight)
                        highestSourceWeightTuple = (cellSourceWeight, beforeIndex, afterIndex);
                }
                else
                {
                    diffMatrix[beforeIndex, afterIndex] = new TextEditorDiffCell(
                        beforeCharValue,
                        afterCharValue,
                        rowLargestWeightPriorToCurrentColumn,
                        false);
                }
            }

            for (
                int fabricatedAfterIndex = afterTextLength;
                fabricatedAfterIndex < squaredSize;
                fabricatedAfterIndex++)
            {
                // This for loop sets the cells in the fabricated column
                // in order to create a square matrix
                // where afterTextLength < beforeTextLength

                var rowLargestWeightPriorToCurrentColumn = GetRowLargestWeightPriorToCurrentColumn(
                    diffMatrix,
                    beforeIndex,
                    fabricatedAfterIndex);

                diffMatrix[beforeIndex, fabricatedAfterIndex] = new TextEditorDiffCell(
                    beforeCharValue,
                    null,
                    rowLargestWeightPriorToCurrentColumn,
                    false);
            }
        }

        for (
            int fabricatedBeforeIndex = beforeTextLength;
            fabricatedBeforeIndex < squaredSize;
            fabricatedBeforeIndex++)
        {
            // This for loop sets the cells in the fabricated row
            // in order to create a square matrix
            // where beforeTextLength < afterTextLength
            //
            // TODO: This logic should to be removed. Instead of looking at this algorithm from the perspective of 'before' and 'after' text. It might be best to have the perspective be 'vertical' and 'horizontal' text. Then the 'vertical' text is equal to the longer of the two string inputs. By having the row count defined by the longer of the two strings you only would ever fabricate columns. This would allow for a lot of code to be removed.

            for (
                int fabricatedAfterIndex = 0;
                fabricatedAfterIndex < squaredSize;
                fabricatedAfterIndex++)
            {
                var rowLargestWeightPriorToCurrentColumn = GetRowLargestWeightPriorToCurrentColumn(
                    diffMatrix,
                    fabricatedBeforeIndex,
                    fabricatedAfterIndex);

                diffMatrix[fabricatedBeforeIndex, fabricatedAfterIndex] = new TextEditorDiffCell(
                    null,
                    null,
                    rowLargestWeightPriorToCurrentColumn,
                    false);
            }
        }

        var longestCommonSubsequenceBuilder = new StringBuilder();

        var beforePositionIndicesOfLongestCommonSubsequenceHashSet = new HashSet<int>();
        var afterPositionIndicesOfLongestCommonSubsequenceHashSet = new HashSet<int>();

        var afterPositionIndicesOfInsertionHashSet = new HashSet<int>();

        var beforePositionIndicesOfDeletionHashSet = new HashSet<int>();

        var beforePositionIndicesOfModificationHashSet = new HashSet<int>();
        var afterPositionIndicesOfModificationHashSet = new HashSet<int>();

        // Read the LongestCommonSubsequence by backtracking to the highest weights
        {
            var runningRowIndex = highestSourceWeightTuple.beforeIndex;
            var runningColumnIndex = highestSourceWeightTuple.afterIndex;

            var targetSourceWeight = highestSourceWeightTuple.sourceWeight;

            var foundLargestWeight = false;

            var restoreColumnIndex = runningColumnIndex;

            while (runningRowIndex != -1 && runningColumnIndex != -1)
            {
                if (!foundLargestWeight)
                    restoreColumnIndex = runningColumnIndex;

                var cell = diffMatrix[runningRowIndex, runningColumnIndex];

                if (cell.IsSourceOfRowWeight &&
                    cell.Weight == targetSourceWeight)
                {
                    if (cell.BeforeCharValue != cell.AfterCharValue)
                    {
                        throw new ApplicationException(
                            $"The {nameof(cell.BeforeCharValue)}:'{cell.BeforeCharValue}' was not equal to the {nameof(cell.AfterCharValue)}:'{cell.AfterCharValue}'");
                    }

                    longestCommonSubsequenceBuilder.Append(cell.BeforeCharValue);

                    // Decoration logic for longest common subsequence
                    {
                        beforePositionIndicesOfLongestCommonSubsequenceHashSet.Add(runningRowIndex);
                        afterPositionIndicesOfLongestCommonSubsequenceHashSet.Add(runningColumnIndex);
                    }

                    restoreColumnIndex = runningColumnIndex - 1;

                    // Forces the row index decrementation then a column index restoration.
                    runningColumnIndex = 0;
                }
                else
                {
                    // Decoration logic
                    {
                        if (afterTextLength > beforeTextLength &&
                            !afterPositionIndicesOfLongestCommonSubsequenceHashSet.Contains(runningColumnIndex))
                        {
                            // Insertion
                            afterPositionIndicesOfInsertionHashSet.Add(runningColumnIndex);
                        }
                        else if (beforeTextLength > afterTextLength &&
                                 runningRowIndex >= afterTextLength)
                        {
                            // Deletion
                            beforePositionIndicesOfDeletionHashSet.Add(runningRowIndex);
                        }
                        // TODO: Else if for modification is not working.
                        //
                        // else if (cell.BeforeCharValue != cell.AfterCharValue)
                        // {
                        //     // Modification
                        //     beforePositionIndicesOfModificationHashSet.Add(runningRowIndex);
                        //     afterPositionIndicesOfModificationHashSet.Add(runningColumnIndex);
                        // }
                    }
                }

                if (runningColumnIndex == 0)
                {
                    runningColumnIndex = restoreColumnIndex;
                    runningRowIndex--;
                }
                else
                {
                    runningColumnIndex--;
                }
            }
        }

        var longestCommonSubsequenceValue = new string(longestCommonSubsequenceBuilder
            .ToString()
            .Reverse()
            .ToArray());

        var beforeTextSpans = new List<TextEditorTextSpan>();
        var afterTextSpans = new List<TextEditorTextSpan>();

        // Decoration logic
        {
            // Longest common subsequence
            {
                beforeTextSpans.AddRange(GetTextSpans(
                    beforeResourceUri,
                    beforeSourceText,
                    beforePositionIndicesOfLongestCommonSubsequenceHashSet,
                    (byte)TextEditorDiffDecorationKind.LongestCommonSubsequence));

                afterTextSpans.AddRange(GetTextSpans(
                    afterResourceUri,
                    afterSourceText,
                    afterPositionIndicesOfLongestCommonSubsequenceHashSet,
                    (byte)TextEditorDiffDecorationKind.LongestCommonSubsequence));
            }

            // Insertion
            {
                afterTextSpans.AddRange(GetTextSpans(
                    afterResourceUri,
                    afterSourceText,
                    afterPositionIndicesOfInsertionHashSet,
                    (byte)TextEditorDiffDecorationKind.Insertion));
            }

            // Deletion
            {
                beforeTextSpans.AddRange(GetTextSpans(
                    beforeResourceUri,
                    beforeSourceText,
                    beforePositionIndicesOfDeletionHashSet,
                    (byte)TextEditorDiffDecorationKind.Deletion));
            }

            // Modification
            {
                beforeTextSpans.AddRange(GetTextSpans(
                    beforeResourceUri,
                    beforeSourceText,
                    beforePositionIndicesOfModificationHashSet,
                    (byte)TextEditorDiffDecorationKind.Modification));

                afterTextSpans.AddRange(GetTextSpans(
                    afterResourceUri,
                    afterSourceText,
                    afterPositionIndicesOfModificationHashSet,
                    (byte)TextEditorDiffDecorationKind.Modification));
            }
        }

        var diffResult = new TextEditorDiffResult(
            beforeSourceText,
            afterSourceText,
            diffMatrix,
            highestSourceWeightTuple,
            longestCommonSubsequenceValue,
            beforeTextSpans.ToImmutableList(),
            afterTextSpans.ToImmutableList());

        return diffResult;
    }

    private static int GetCellSourceWeight(
        TextEditorDiffCell[,] diffMatrix,
        int beforeIndex,
        int afterIndex)
    {
        if (beforeIndex > 0 && afterIndex > 0)
            return diffMatrix[beforeIndex - 1, afterIndex - 1].Weight + 1;

        return 1;
    }

    private static int GetRowLargestWeightPriorToCurrentColumn(
        TextEditorDiffCell[,] diffMatrix,
        int beforeIndex,
        int afterIndex)
    {
        if (afterIndex > 0)
            return diffMatrix[beforeIndex, afterIndex - 1].Weight;

        return 0;
    }

    private static List<TextEditorTextSpan> GetTextSpans(
        ResourceUri resourceUri,
        string sourceText,
        HashSet<int> positionIndicesHashSet,
        byte decorationByte)
    {
        var matchTextSpans = new List<TextEditorTextSpan>();

        if (!positionIndicesHashSet.Any())
            return matchTextSpans;

        var sortedPositionIndicesThatMatch = positionIndicesHashSet
            .OrderBy(x => x)
            .ToList();

        // The foreach loop coalesces contiguous indices into a single TextEditorTextSpan
        // and the logic that does the coalescing will not write out the final index without this.
        sortedPositionIndicesThatMatch.Add(int.MaxValue);

        var startingIndexInclusive = sortedPositionIndicesThatMatch.First();
        var endingIndexExclusive = startingIndexInclusive + 1;

        var skipFirstIndex = sortedPositionIndicesThatMatch
            .Skip(1)
            .ToArray();

        foreach (var index in skipFirstIndex)
        {
            if (index == endingIndexExclusive)
            {
                endingIndexExclusive++;
            }
            else
            {
                var textSpan = new TextEditorTextSpan(
                    startingIndexInclusive,
                    endingIndexExclusive,
                    decorationByte,
                    resourceUri,
                    sourceText);

                matchTextSpans.Add(textSpan);

                startingIndexInclusive = index;
                endingIndexExclusive = index + 1;
            }
        }

        return matchTextSpans;
    }
}