using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public class TextEditorDiffResult
{
    private TextEditorDiffResult(
        string inText,
        string outText,
        TextEditorDiffCell[,] diffMatrix,
        (int sourceWeight, int inIndex, int outIndex) highestSourceWeightTuple,
        string longestCommonSubsequence,
        ImmutableList<TextEditorTextSpan> inLongestCommonSubsequenceTextSpanList,
        ImmutableList<TextEditorTextSpan> outLongestCommonSubsequenceTextSpanList)
    {
        InText = inText;
        OutText = outText;
        DiffMatrix = diffMatrix;
        HighestSourceWeightTuple = highestSourceWeightTuple;
        LongestCommonSubsequence = longestCommonSubsequence;
        InResultTextSpanList = inLongestCommonSubsequenceTextSpanList;
        OutResultTextSpanList = outLongestCommonSubsequenceTextSpanList;
    }

    public string InText { get; }
    public string OutText { get; }
    public TextEditorDiffCell[,] DiffMatrix { get; }
    public (int sourceWeight, int beforeIndex, int afterIndex) HighestSourceWeightTuple { get; }
    public string LongestCommonSubsequence { get; }
    public ImmutableList<TextEditorTextSpan> InResultTextSpanList { get; }
    public ImmutableList<TextEditorTextSpan> OutResultTextSpanList { get; }

    /// <summary>
    /// This method aims to implement the "An O(ND) Difference Algorithm"
    /// <br/><br/>
    /// Watching https://www.youtube.com/watch?v=9n8jI2267MM
    /// </summary>
    public static TextEditorDiffResult Calculate(
        ResourceUri inResourceUri,
        string inText,
        ResourceUri outResourceUri,
        string outText)
    {
        // Need to build a square two dimensional array.
        //
        // Envisioning that:
        //     -Each character of 'inText' represents a row.
        //     -Each character of 'outText' represents a column.
        
        var squaredSize = Math.Max(inText.Length, outText.Length);

        var diffMatrix = new TextEditorDiffCell[squaredSize, squaredSize];

        (int sourceWeight, int inIndex, int outIndex) highestSourceWeightTuple = (-1, -1, -1);

        for (int inIndex = 0; inIndex < inText.Length; inIndex++)
        {
            char inCharValue = inText[inIndex];

            for (int outIndex = 0; outIndex < outText.Length; outIndex++)
            {
                char outCharValue = outText[outIndex];

                var weight = GetLargestWeightPriorToCurrentPosition(diffMatrix, inIndex, outIndex);
                var isSourceWeight = false;

                if (inCharValue == outCharValue)
                {
                    var cellSourceWeight = weight + 1;

                    if (cellSourceWeight > weight)
                    {
                        weight = cellSourceWeight;
                        isSourceWeight = true;
                    }

                    if (cellSourceWeight > highestSourceWeightTuple.sourceWeight)
                        highestSourceWeightTuple = (cellSourceWeight, inIndex, outIndex);
                }

                diffMatrix[inIndex, outIndex] = new TextEditorDiffCell(
                    inCharValue, outCharValue, weight, isSourceWeight);
            }

            for (int fabricatedOutIndex = outText.Length; fabricatedOutIndex < squaredSize; fabricatedOutIndex++)
            {
                // This for loop sets the cells in the fabricated column in order to create a square matrix
                // in cases where (outTextLength < inTextLength)

                var rowLargestWeightPriorToCurrentColumn = GetLargestWeightPriorToCurrentPosition(
                    diffMatrix,
                    inIndex,
                    fabricatedOutIndex);

                diffMatrix[inIndex, fabricatedOutIndex] = new TextEditorDiffCell(
                    inCharValue,
                    null,
                    rowLargestWeightPriorToCurrentColumn,
                    false);
            }
        }

        for (int fabricatedInIndex = inText.Length; fabricatedInIndex < squaredSize; fabricatedInIndex++)
        {
            // This for loop sets the cells in the fabricated row in order to create a square matrix
            // in cases where (inTextLength < outTextLength)
            //
            // TODO: This logic should to be removed. Instead of looking at this algorithm from the perspective of 'before' and 'after' text. It might be best to have the perspective be 'vertical' and 'horizontal' text. Then the 'vertical' text is equal to the longer of the two string inputs. By having the row count defined by the longer of the two strings you only would ever fabricate columns. This would allow for a lot of code to be removed.

            for (int fabricatedOutIndex = 0; fabricatedOutIndex < squaredSize; fabricatedOutIndex++)
            {
                var rowLargestWeightPriorToCurrentColumn = GetLargestWeightPriorToCurrentPosition(
                    diffMatrix,
                    fabricatedInIndex,
                    fabricatedOutIndex);

                diffMatrix[fabricatedInIndex, fabricatedOutIndex] = new TextEditorDiffCell(
                    null,
                    null,
                    rowLargestWeightPriorToCurrentColumn,
                    false);
            }
        }

        // The abbreviation "lcs" is to mean "Longest Common Subsequence"
        var lcsBuilder = new StringBuilder();

        var inPositionIndicesLcsHashSet = new HashSet<int>();
        var outPositionIndicesLcsHashSet = new HashSet<int>();

        var inPositionIndicesOfDeletionHashSet = new HashSet<int>();
        var outPositionIndicesInsertionHashSet = new HashSet<int>();

        var inPositionIndicesOfModificationHashSet = new HashSet<int>();
        var outPositionIndicesOfModificationHashSet = new HashSet<int>();

        // Read the LongestCommonSubsequence by backtracking to the highest weights
        {
            var runningRowIndex = highestSourceWeightTuple.inIndex;
            var runningColumnIndex = highestSourceWeightTuple.outIndex;

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
                    targetSourceWeight--;

                    if (cell.BeforeCharValue != cell.AfterCharValue)
                        throw new ApplicationException($"The {nameof(cell.BeforeCharValue)}:'{cell.BeforeCharValue}' was not equal to the {nameof(cell.AfterCharValue)}:'{cell.AfterCharValue}'");

                    lcsBuilder.Append(cell.BeforeCharValue);

                    // Decoration logic for longest common subsequence
                    {
                        inPositionIndicesLcsHashSet.Add(runningRowIndex);
                        outPositionIndicesLcsHashSet.Add(runningColumnIndex);
                    }

                    restoreColumnIndex = runningColumnIndex - 1;

                    // Forces the row index decrementation then a column index restoration.
                    runningColumnIndex = 0;
                }
                else
                {
                    // Decoration logic
                    {
                        if (outText.Length > inText.Length && !outPositionIndicesLcsHashSet.Contains(runningColumnIndex))
                            outPositionIndicesInsertionHashSet.Add(runningColumnIndex); // Insertion
                        else if (inText.Length > outText.Length && runningRowIndex >= outText.Length)
                            inPositionIndicesOfDeletionHashSet.Add(runningRowIndex); // Deletion
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

        var longestCommonSubsequenceValue = new string(lcsBuilder
            .ToString()
            .Reverse()
            .ToArray());

        var inTextSpans = new List<TextEditorTextSpan>();
        var outTextSpans = new List<TextEditorTextSpan>();

        // Decoration logic
        {
            // Longest common subsequence
            {
                inTextSpans.AddRange(GetTextSpans(
                    inResourceUri,
                    inText,
                    inPositionIndicesLcsHashSet,
                    (byte)TextEditorDiffDecorationKind.LongestCommonSubsequence));

                outTextSpans.AddRange(GetTextSpans(
                    outResourceUri,
                    outText,
                    outPositionIndicesLcsHashSet,
                    (byte)TextEditorDiffDecorationKind.LongestCommonSubsequence));
            }

            // Insertion
            {
                outTextSpans.AddRange(GetTextSpans(
                    outResourceUri,
                    outText,
                    outPositionIndicesInsertionHashSet,
                    (byte)TextEditorDiffDecorationKind.Insertion));
            }

            // Deletion
            {
                inTextSpans.AddRange(GetTextSpans(
                    inResourceUri,
                    inText,
                    inPositionIndicesOfDeletionHashSet,
                    (byte)TextEditorDiffDecorationKind.Deletion));
            }

            // Modification
            {
                inTextSpans.AddRange(GetTextSpans(
                    inResourceUri,
                    inText,
                    inPositionIndicesOfModificationHashSet,
                    (byte)TextEditorDiffDecorationKind.Modification));

                outTextSpans.AddRange(GetTextSpans(
                    outResourceUri,
                    outText,
                    outPositionIndicesOfModificationHashSet,
                    (byte)TextEditorDiffDecorationKind.Modification));
            }
        }

        var diffResult = new TextEditorDiffResult(
            inText,
            outText,
            diffMatrix,
            highestSourceWeightTuple,
            longestCommonSubsequenceValue,
            inTextSpans.ToImmutableList(),
            outTextSpans.ToImmutableList());

        return diffResult;
    }

    private static int GetLargestWeightPriorToCurrentPosition(
        TextEditorDiffCell[,] diffMatrix,
        int inIndex,
        int outIndex)
    {
        var largestWeight = 0;

        for (int rowI = 0; rowI < inIndex; rowI++)
        {
            for (int columnI = 0; columnI < outIndex; columnI++)
            {
                var currentWeight = diffMatrix[rowI, columnI].Weight;

                if (currentWeight > largestWeight)
                    largestWeight = currentWeight;
            }
        }

        return largestWeight;
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