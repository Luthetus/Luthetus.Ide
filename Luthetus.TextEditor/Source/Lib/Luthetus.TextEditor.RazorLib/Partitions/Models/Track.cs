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
    }

    public static List<int> ExpandPartition(ImmutableList<RichCharacter> partition)
    {
        // TabList
        {
            List<int> mutableList = new();
            {
                // TODO: Don't count the tabs, instead divide the original TabList
                for (int i = 0; i < partition.Count; i++)
                {
                    if (partition[i].Value == '\t')
                        mutableList.Add(i);
                }
            }

            return mutableList;
        }
    }
}
