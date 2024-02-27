using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class Track
{
    public static void Insert(int relativePositionIndex, RichCharacter richCharacter, int partitionIndex, ImmutableList<ImmutableList<RichCharacter>> partitionList, ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        { // TabList
            var inTabList = partitionMetadataMap[partitionIndex].TabList;
            var mutableTabList = new List<int>();
            var relativeTabIndex = inTabList.FindIndex(x => x >= relativePositionIndex);
            
            var copyForLoopUpperLimit = relativeTabIndex == -1 ? inTabList.Count : relativeTabIndex; // Copy over unmodified values
            for (int i = 0; i < copyForLoopUpperLimit; i++)
                mutableTabList.Add(inTabList[i]);

            var shiftForLoopLowerLimit = copyForLoopUpperLimit; // Write the shifted values
            for (int i = shiftForLoopLowerLimit; i < inTabList.Count; i++)
                mutableTabList.Add(inTabList[i] + 1);

            if (richCharacter.Value == '\t')
            {
                if (relativeTabIndex == -1)
                    mutableTabList.Add(relativePositionIndex);
                else
                    mutableTabList.Insert(relativeTabIndex, relativePositionIndex);

                partitionMetadataMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
            }
        }

        { // Row related tracking
            var inRowEndingList = partitionMetadataMap[partitionIndex].RowEndingList; // RowEndingList, create mutable data
            var mutableRowEndingList = new List<RowEnding>();
            for (int i = 0; i < inRowEndingList.Count; i++)
                mutableRowEndingList.Add(inRowEndingList[i]);

            var rowEndingKindCountList = partitionMetadataMap[partitionIndex].RowEndingKindCountList; // RowEndingKindCountList, create mutable data
            var carriageReturnCount = 0;
            var linefeedCount = 0;
            var carriageReturnLinefeedCount = 0;
            foreach (var rowEndingKindCount in rowEndingKindCountList)
            {
                switch (rowEndingKindCount.rowEndingKind)
                {
                    case RowEndingKind.CarriageReturn:
                        carriageReturnCount += rowEndingKindCount.count;
                        break;
                    case RowEndingKind.Linefeed:
                        linefeedCount += rowEndingKindCount.count;
                        break;
                    case RowEndingKind.CarriageReturnLinefeed:
                        carriageReturnLinefeedCount += rowEndingKindCount.count;
                        break;
                }
            }

            if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.CarriageReturn));
                carriageReturnCount++;
            }
            else if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                var previousCharacter = '\0';
                if (relativePositionIndex > 0)
                {
                    var partition = partitionList[partitionIndex];
                    previousCharacter = partition[relativePositionIndex - 1].Value;
                }
                else if (partitionIndex > 0)
                {
                    var previousPartition = partitionList[partitionIndex - 1];
                    previousCharacter = previousPartition[^1].Value;
                }

                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = mutableRowEndingList[^1];
                    lineEnding.EndPositionIndexExclusive++;
                    lineEnding.RowEndingKind = RowEndingKind.CarriageReturnLinefeed;
                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.Linefeed));
                    linefeedCount++;
                }
            }

            partitionMetadataMap[partitionIndex].RowEndingList = mutableRowEndingList.ToImmutableList();
            partitionMetadataMap[partitionIndex].RowEndingKindCountList = new (RowEndingKind rowEndingKind, int count)[]
            {
                new (RowEndingKind.CarriageReturn, carriageReturnCount),
                new (RowEndingKind.Linefeed, linefeedCount),
                new (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
            }.ToImmutableList();

            var whereMoreThanOne = partitionMetadataMap[partitionIndex].RowEndingKindCountList.Where(x => x.count > 0).ToList();
            RowEndingKind? onlyRowEndingKind;
            if (whereMoreThanOne.Count == 1)
                onlyRowEndingKind = whereMoreThanOne.Single().rowEndingKind;
            else
                onlyRowEndingKind = null;
            partitionMetadataMap[partitionIndex].OnlyRowEndingKind = onlyRowEndingKind;
        }
    }

    public static void RemoveAt(int relativePositionIndex, RichCharacter removedRichCharacter, int partitionIndex, ImmutableList<ImmutableList<RichCharacter>> partitionList, ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        { // TabList
            var inTabList = partitionMetadataMap[partitionIndex].TabList;
            var mutableTabList = new List<int>();
            var relativeTabIndex = inTabList.FindIndex(x => x >= relativePositionIndex);
            
            for (int i = 0; i < relativeTabIndex; i++) // Copy over unmodified values
                mutableTabList.Add(inTabList[i]);

            for (int i = relativeTabIndex; i < inTabList.Count; i++) // Write the shifted values
            {
                if (inTabList[i] == relativePositionIndex) // Do not write out the 'removed tab' (if one were removed)
                    continue;
                mutableTabList.Add(inTabList[i] - 1);
            }
            partitionMetadataMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
        }


        { // RowEnding
            var inRowEndingList = partitionMetadataMap[partitionIndex].RowEndingList;
            var mutableRowEndingList = new List<RowEnding>();

            var rowEndingKindCountList = partitionMetadataMap[partitionIndex].RowEndingKindCountList; // RowEndingKindCountList, create mutable data
            var carriageReturnCount = 0;
            var linefeedCount = 0;
            var carriageReturnLinefeedCount = 0;
            foreach (var rowEndingKindCount in rowEndingKindCountList)
            {
                switch (rowEndingKindCount.rowEndingKind)
                {
                    case RowEndingKind.CarriageReturn:
                        carriageReturnCount += rowEndingKindCount.count;
                        break;
                    case RowEndingKind.Linefeed:
                        linefeedCount += rowEndingKindCount.count;
                        break;
                    case RowEndingKind.CarriageReturnLinefeed:
                        carriageReturnLinefeedCount += rowEndingKindCount.count;
                        break;
                }
            }

            var relativeRowEndingIndex = inRowEndingList.FindIndex(x => x.StartPositionIndexInclusive >= relativePositionIndex);
            for (int i = 0; i < relativeRowEndingIndex; i++) // Copy over unmodified values
                mutableRowEndingList.Add(inRowEndingList[i]);

            for (int i = relativeRowEndingIndex; i < inRowEndingList.Count; i++) // Write the shifted values
            {
                if (inRowEndingList[i].StartPositionIndexInclusive == relativePositionIndex) // Do not write out the 'removed tab' (if one were removed)
                {
                    if (removedRichCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                    {
                        var partition = partitionList[partitionIndex];
                        if (relativePositionIndex < partition.Count) // relativePositionIndex now points to what was the next character prior to deletion. Check for 'carriage return new line'
                        {
                            var nextRichCharacter = partition[relativePositionIndex];
                            if (nextRichCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
                                carriageReturnLinefeedCount--; // Trust is being given that the text editor will remove both the carriage return, and the newline, one after another. The newline half of 'carriage return new line' just wouldn't encounter itself in the row ending list, but will move on without errors.
                            else
                                carriageReturnCount--;
                        }
                    }
                    else if (removedRichCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
                    {
                        linefeedCount--;
                    }
                    continue;
                }

                var rowEnding = inRowEndingList[i];
                mutableRowEndingList.Add(rowEnding with
                {
                    StartPositionIndexInclusive = rowEnding.StartPositionIndexInclusive - 1,
                    EndPositionIndexExclusive = rowEnding.EndPositionIndexExclusive - 1,
                });
            }

            partitionMetadataMap[partitionIndex].RowEndingList = mutableRowEndingList.ToImmutableList();
            partitionMetadataMap[partitionIndex].RowEndingKindCountList = new (RowEndingKind rowEndingKind, int count)[]
            {
                new (RowEndingKind.CarriageReturn, carriageReturnCount),
                new (RowEndingKind.Linefeed, linefeedCount),
                new (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
            }.ToImmutableList();

            var whereMoreThanOne = partitionMetadataMap[partitionIndex].RowEndingKindCountList.Where(x => x.count > 0).ToList();
            RowEndingKind? onlyRowEndingKind;
            if (whereMoreThanOne.Count == 1)
                onlyRowEndingKind = whereMoreThanOne.Single().rowEndingKind;
            else
                onlyRowEndingKind = null;
            partitionMetadataMap[partitionIndex].OnlyRowEndingKind = onlyRowEndingKind;
        }
    }

    public static List<int> ExpandPartition_Tab(ImmutableList<RichCharacter> partition)
    {
        List<int> mutableList = new();
        for (int i = 0; i < partition.Count; i++)
        {
            if (partition[i].Value == '\t')
                mutableList.Add(i);
        }
        return mutableList;
    }
    
    public static (List<RowEnding> rowEndingList, ImmutableList<(RowEndingKind rowEndingKind, int count)> rowEndingCountList, RowEndingKind? onlyRowEndingKind) ExpandPartition_RowEnding(ImmutableList<RichCharacter> partition)
    {
        var carriageReturnCount = 0;
        var linefeedCount = 0;
        var carriageReturnLinefeedCount = 0;

        List<RowEnding> mutableList = new();
        var previousCharacter = '\0';
        for (int i = 0; i < partition.Count; i++)
        {
            if (partition[i].Value == '\r')
            {
                mutableList.Add(new RowEnding(i, i + 1, RowEndingKind.CarriageReturn));
                carriageReturnCount++;
            }
            else if (partition[i].Value == '\n')
            {
                if (previousCharacter == '\r')
                {
                    var previousCarriageReturn = mutableList[i - 1];
                    mutableList[i - 1] = previousCarriageReturn with
                    {
                        EndPositionIndexExclusive = 1 + previousCarriageReturn.EndPositionIndexExclusive,
                        RowEndingKind = RowEndingKind.CarriageReturnLinefeed,
                    };
                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    mutableList.Add(new RowEnding(i, i + 1, RowEndingKind.Linefeed));
                    linefeedCount++;
                }
            }
            previousCharacter = partition[i].Value;
        }

        var rowEndingKindCountList = new (RowEndingKind rowEndingKind, int count)[]
        {
            new (RowEndingKind.CarriageReturn, carriageReturnCount),
            new (RowEndingKind.Linefeed, linefeedCount),
            new (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
        }.ToImmutableList();

        var whereMoreThanOne = rowEndingKindCountList.Where(x => x.count > 0).ToList();
        RowEndingKind? onlyRowEndingKind;
        if (whereMoreThanOne.Count == 1)
            onlyRowEndingKind = whereMoreThanOne.Single().rowEndingKind;
        else
            onlyRowEndingKind = null;

        return (mutableList, rowEndingKindCountList, onlyRowEndingKind);
    }
}
