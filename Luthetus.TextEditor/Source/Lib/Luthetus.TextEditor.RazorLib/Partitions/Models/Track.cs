using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal partial class Track
{
    public static ImmutableList<PartitionMetadata> Insert(int relativePositionIndex, RichCharacter richCharacter, int partitionIndex, ImmutableList<ImmutableList<RichCharacter>> partitionList, ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        // TabList
        var shiftTabsOutput = ShiftTabs(
            relativePositionIndex,
            richCharacter,
            partitionIndex,
            partitionList,
            partitionMetadataMap,
            true);

        partitionMetadataMap = partitionMetadataMap.SetItem(partitionIndex, partitionMetadataMap[partitionIndex] with 
        {
            TabList = shiftTabsOutput.MutableTabList.ToImmutableList()
        });

        // RowEnding
        var shiftRowsOutput = ShiftRows(
            relativePositionIndex,
            richCharacter,
            partitionIndex,
            partitionList,
            partitionMetadataMap,
            true);

        partitionMetadataMap = partitionMetadataMap.SetItem(partitionIndex, partitionMetadataMap[partitionIndex] with 
        {
            RowEndingList = shiftRowsOutput.RowEndingList,
            RowEndingKindCountList = shiftRowsOutput.RowEndingKindCountList,
            OnlyRowEndingKind = shiftRowsOutput.OnlyRowEndingKind,
        });

        return partitionMetadataMap;
    }

    public static ImmutableList<PartitionMetadata> RemoveAt(int relativePositionIndex, RichCharacter removedRichCharacter, int partitionIndex, ImmutableList<ImmutableList<RichCharacter>> partitionList, ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        // TabList
        var shiftTabsOutput = ShiftTabs(
            relativePositionIndex,
            removedRichCharacter,
            partitionIndex,
            partitionList,
            partitionMetadataMap,
            false);

        partitionMetadataMap = partitionMetadataMap.SetItem(partitionIndex, partitionMetadataMap[partitionIndex] with 
        {
            TabList = shiftTabsOutput.MutableTabList.ToImmutableList()
        });

        // RowEnding
        var shiftRowsOutput = ShiftRows(
            relativePositionIndex,
            removedRichCharacter,
            partitionIndex,
            partitionList,
            partitionMetadataMap,
            false);

        partitionMetadataMap = partitionMetadataMap.SetItem(partitionIndex, partitionMetadataMap[partitionIndex] with
        {
            RowEndingList = shiftRowsOutput.RowEndingList,
            RowEndingKindCountList = shiftRowsOutput.RowEndingKindCountList,
            OnlyRowEndingKind = shiftRowsOutput.OnlyRowEndingKind,
        });

        return partitionMetadataMap;
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

    private static ShiftTabsOutput ShiftTabs(
        int relativePositionIndex,
        RichCharacter richCharacter,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap,
        bool isInsertion)
    {
        var inTabList = partitionMetadataMap[partitionIndex].TabList;
        var mutableTabList = new List<int>();
        var relativeTabIndex = inTabList.FindIndex(x => x >= relativePositionIndex);

        var copyForLoopUpperLimit = relativeTabIndex == -1 ? inTabList.Count : relativeTabIndex; // Copy over unmodified values
        for (int i = 0; i < copyForLoopUpperLimit; i++)
            mutableTabList.Add(inTabList[i]);

        var shiftForLoopLowerLimit = copyForLoopUpperLimit; // Write the shifted values
        for (int i = shiftForLoopLowerLimit; i < inTabList.Count; i++)
        {
            if (!isInsertion && inTabList[i] == relativePositionIndex)
                continue;
            mutableTabList.Add(inTabList[i] + (isInsertion ? +1 : -1));
        }

        if (isInsertion && richCharacter.Value == '\t')
        {
            if (relativeTabIndex == -1)
                mutableTabList.Add(relativePositionIndex);
            else
                mutableTabList.Insert(relativeTabIndex, relativePositionIndex);
        }

        return new(relativeTabIndex, mutableTabList);
    }

    private static ShiftRowsOutput ShiftRows(
        int relativePositionIndex,
        RichCharacter richCharacter,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap,
        bool isInsertion)
    {
        var inRowEndingList = partitionMetadataMap[partitionIndex].RowEndingList; // RowEndingList, create mutable data
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
        var copyForLoopUpperLimit = relativeRowEndingIndex == -1 ? inRowEndingList.Count : relativeRowEndingIndex;
        for (int i = 0; i < copyForLoopUpperLimit; i++) // Copy over unmodified values
        {
            mutableRowEndingList.Add(inRowEndingList[i] with
            {
                StartPositionIndexInclusive = inRowEndingList[i].StartPositionIndexInclusive,
                EndPositionIndexExclusive = inRowEndingList[i].EndPositionIndexExclusive,
            });
        }

        if (isInsertion)
        {
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
        }

        var shiftForLoopLowerLimit = copyForLoopUpperLimit;
        for (int i = shiftForLoopLowerLimit; i < inRowEndingList.Count; i++) // Write the shifted values
        {
            if (!isInsertion && inRowEndingList[i].StartPositionIndexInclusive == relativePositionIndex) // Do not write out the 'removed tab' (if one were removed)
            {
                if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
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
                else if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
                {
                    linefeedCount--;
                }
                continue;
            }

            var rowEnding = inRowEndingList[i];
            mutableRowEndingList.Add(rowEnding with
            {
                StartPositionIndexInclusive = rowEnding.StartPositionIndexInclusive + (isInsertion ? +1 : -1),
                EndPositionIndexExclusive = rowEnding.EndPositionIndexExclusive + (isInsertion ? +1 : -1),
            });
        }

        var rowEndingList = mutableRowEndingList.ToImmutableList();
        rowEndingKindCountList = new (RowEndingKind rowEndingKind, int count)[]
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

        return new(relativeRowEndingIndex, rowEndingList, rowEndingKindCountList, onlyRowEndingKind);
    }
}
