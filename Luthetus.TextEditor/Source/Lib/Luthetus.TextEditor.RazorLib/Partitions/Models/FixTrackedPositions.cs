using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class FixTrackedPositions
{
    public static void Add(
        RichCharacter item,
        int indexPartitionFreeSpace,
        ImmutableList<ImmutableList<RichCharacter>> localPartitionList,
        ImmutableList<PartitionRichCharacterMetadata> localPartitionMetadataMap,
        int relativePositionIndex)
    {
        // TabList
        {
            var inTabList = localPartitionMetadataMap[indexPartitionFreeSpace].TabList;
            var mutableTabList = new List<int>();

            for (int i = 0; i < inTabList.Count; i++)
            {
                mutableTabList.Add(inTabList[i]);
            }

            if (item.Value == '\t')
            {
                mutableTabList.Add(relativePositionIndex);

                localPartitionMetadataMap[indexPartitionFreeSpace].TabList =
                    mutableTabList.ToImmutableList();
            }
        }

        // RowEndingList
        {
            var inRowEndingList = localPartitionMetadataMap[indexPartitionFreeSpace].RowEndingList;
            var mutableRowEndingList = new List<RowEnding>();

            for (int i = 0; i < inRowEndingList.Count; i++)
            {
                mutableRowEndingList.Add(inRowEndingList[i]);
            }

            if (item.Value == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                mutableRowEndingList.Add(new(relativePositionIndex, relativePositionIndex + 1, RowEndingKind.CarriageReturn));
            }
            else if (item.Value == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                var previousCharacter = '\0';

                if (relativePositionIndex > 0)
                {
                    var currentPartition = localPartitionList[indexPartitionFreeSpace];
                    previousCharacter = currentPartition[relativePositionIndex - 1].Value;
                }
                else if (indexPartitionFreeSpace > 0)
                {
                    var previousPartition = localPartitionList[indexPartitionFreeSpace - 1];
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

            localPartitionMetadataMap[indexPartitionFreeSpace].RowEndingList =
                mutableRowEndingList.ToImmutableList();
        }
    }

    public static void Insert(
        int globalPositionIndex,
        RichCharacter item,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> outPartitionList,
        ImmutableList<PartitionRichCharacterMetadata> outPartitionMemoryMap)
    {
        // TabList
        {
            var inTabList = outPartitionMemoryMap[partitionIndex].TabList;
            var mutableTabList = new List<int>();

            var relativePositionIndex = PartitionedRichCharacterList.GetRelativePositionIndex(
                partitionIndex, outPartitionList, outPartitionMemoryMap, globalPositionIndex);

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

            if (item.Value == '\t')
            {
                if (relativeTabIndex == -1)
                    mutableTabList.Add(relativePositionIndex);
                else
                    mutableTabList.Insert(relativeTabIndex, relativePositionIndex);

                outPartitionMemoryMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
            }
        }
    }

    public static void RemoveAt(int globalPositionIndex, int indexPartition, ImmutableList<PartitionRichCharacterMetadata> outPartitionMemoryMap)
    {
        // TabList
        {
            var inTabList = outPartitionMemoryMap[indexPartition].TabList;
            var mutableTabList = new List<int>();

            var relativeTabIndex = inTabList.FindIndex(x => x >= globalPositionIndex);

            // Copy over unmodified values
            for (int i = 0; i < relativeTabIndex; i++)
            {
                mutableTabList.Add(inTabList[i]);
            }

            // Write the shifted values
            for (int i = relativeTabIndex; i < inTabList.Count; i++)
            {
                // Do not write out the 'removed tab' (if one were removed)
                if (inTabList[i] == globalPositionIndex)
                    continue;

                mutableTabList.Add(inTabList[i] - 1);
            }

            outPartitionMemoryMap[indexPartition].TabList = mutableTabList.ToImmutableList();
        }
    }

    public static List<int> ExpandPartition(ImmutableList<RichCharacter> partition)
    {
        // TabList
        {
            List<int> mutableList = new();
            {
                // TODO: Don't count the tabs, instead divide the original TabList
                for (int tabCounterIndex = 0; tabCounterIndex < partition.Count; tabCounterIndex++)
                {
                    if (partition[tabCounterIndex].Value == '\t')
                        mutableList.Add(tabCounterIndex);
                }
            }

            return mutableList;
        }
    }
}
