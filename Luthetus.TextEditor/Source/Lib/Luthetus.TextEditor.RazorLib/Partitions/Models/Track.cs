using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class Track
{
    public static void Add(
        int relativePositionIndex,
        RichCharacter richCharacter,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        // TabList
        {
            var inTabList = partitionMetadataMap[partitionIndex].TabList;
            var mutableTabList = new List<int>();

            for (int i = 0; i < inTabList.Count; i++)
            {
                mutableTabList.Add(inTabList[i]);
            }

            if (richCharacter.Value == '\t')
            {
                mutableTabList.Add(relativePositionIndex);
                partitionMetadataMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
            }
        }

        // RowEndingList
        {
            var inRowEndingList = partitionMetadataMap[partitionIndex].RowEndingList;
            var mutableRowEndingList = new List<RowEnding>();

            for (int i = 0; i < inRowEndingList.Count; i++)
            {
                mutableRowEndingList.Add(inRowEndingList[i]);
            }

            if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.CarriageReturn));
            }
            else if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                var previousCharacter = '\0';

                if (relativePositionIndex > 0)
                {
                    var currentPartition = partitionList[partitionIndex];
                    previousCharacter = currentPartition[relativePositionIndex - 1].Value;
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
                }
                else
                {
                    mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.Linefeed));
                }
            }

            partitionMetadataMap[partitionIndex].RowEndingList = mutableRowEndingList.ToImmutableList();
        }
    }

    public static void Insert(
        int relativePositionIndex,
        RichCharacter richCharacter,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        // TabList
        {
            var inTabList = partitionMetadataMap[partitionIndex].TabList;
            var mutableTabList = new List<int>();

            var relativeTabIndex = inTabList.FindIndex(x => x >= relativePositionIndex);

            // Copy over unmodified values
            var copyForLoopUpperLimit = relativeTabIndex == -1 ? inTabList.Count : relativeTabIndex;
            for (int i = 0; i < copyForLoopUpperLimit; i++)
            {
                mutableTabList.Add(inTabList[i]);
            }

            // Write the shifted values
            var shiftForLoopLowerLimit = copyForLoopUpperLimit;
            for (int i = shiftForLoopLowerLimit; i < inTabList.Count; i++)
            {
                mutableTabList.Add(inTabList[i] + 1);
            }

            if (richCharacter.Value == '\t')
            {
                if (relativeTabIndex == -1)
                    mutableTabList.Add(relativePositionIndex);
                else
                    mutableTabList.Insert(relativeTabIndex, relativePositionIndex);

                partitionMetadataMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
            }
        }

        // RowEndingList
        {
            var inRowEndingList = partitionMetadataMap[partitionIndex].RowEndingList;
            var mutableRowEndingList = new List<RowEnding>();

            for (int i = 0; i < inRowEndingList.Count; i++)
            {
                mutableRowEndingList.Add(inRowEndingList[i]);
            }

            if (richCharacter.Value == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.CarriageReturn));
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
                }
                else
                {
                    mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.Linefeed));
                }
            }

            partitionMetadataMap[partitionIndex].RowEndingList = mutableRowEndingList.ToImmutableList();
        }
    }

    public static void RemoveAt(
        int relativePositionIndex,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap)
    {
        // TabList
        {
            var inTabList = partitionMetadataMap[partitionIndex].TabList;
            var mutableTabList = new List<int>();

            var relativeTabIndex = inTabList.FindIndex(x => x >= relativePositionIndex);

            // Copy over unmodified values
            for (int i = 0; i < relativeTabIndex; i++)
            {
                mutableTabList.Add(inTabList[i]);
            }

            // Write the shifted values
            for (int i = relativeTabIndex; i < inTabList.Count; i++)
            {
                // Do not write out the 'removed tab' (if one were removed)
                if (inTabList[i] == relativePositionIndex)
                    continue;

                mutableTabList.Add(inTabList[i] - 1);
            }

            partitionMetadataMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
        }

        // RowEnding
        {
            var inRowEndingList = partitionMetadataMap[partitionIndex].RowEndingList;
            var mutableRowEndingList = new List<RowEnding>();

            var relativeRowEndingIndex = inRowEndingList.FindIndex(x => x.StartPositionIndexInclusive >= relativePositionIndex);

            // Copy over unmodified values
            for (int i = 0; i < relativeRowEndingIndex; i++)
            {
                mutableRowEndingList.Add(inRowEndingList[i]);
            }

            // Write the shifted values
            for (int i = relativeRowEndingIndex; i < inRowEndingList.Count; i++)
            {
                // Do not write out the 'removed tab' (if one were removed)
                if (inRowEndingList[i].StartPositionIndexInclusive == relativePositionIndex)
                    continue;

                var rowEnding = inRowEndingList[i];
                mutableRowEndingList.Add(rowEnding with
                {
                    StartPositionIndexInclusive = rowEnding.StartPositionIndexInclusive - 1,
                    EndPositionIndexExclusive = rowEnding.EndPositionIndexExclusive - 1,
                });
            }

            partitionMetadataMap[partitionIndex].RowEndingList = mutableRowEndingList.ToImmutableList();
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
    
    public static List<RowEnding> ExpandPartition_RowEnding(ImmutableList<RichCharacter> partition)
    {
        List<RowEnding> mutableList = new();
        var previousCharacter = '\0';

        for (int i = 0; i < partition.Count; i++)
        {
            if (partition[i].Value == '\r')
            {
                mutableList.Add(new RowEnding(i, i + 1, RowEndingKind.CarriageReturn));
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
                }
                else
                {
                    mutableList.Add(new RowEnding(i, i + 1, RowEndingKind.Linefeed));
                }
            }

            previousCharacter = partition[i].Value;
        }

        return mutableList;
    }
}
