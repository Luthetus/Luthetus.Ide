using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModelModifier : ITextEditorModel
{
    public void __Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count >= globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.Count >= PartitionSize)
                {
                    __SplitIntoTwoPartitions(i);
                    i--;
                    continue;
                }

                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.Insert(relativePositionIndex, richCharacter);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    /// <summary>
    /// If either character or decoration byte are 'null', then the respective
    /// collection will be left unchanged.
    /// 
    /// i.e.: to change ONLY a character value invoke this method with decorationByte set to null,
    ///       and only the <see cref="CharList"/> will be changed.
    /// </summary>
    public void __SetItem(
        int globalPositionIndex,
        RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.SetItem(relativePositionIndex, richCharacter);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    public void __SetDecorationByte(
        int globalPositionIndex,
        byte decorationByte)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
        inPartition.RichCharacterList[relativePositionIndex].DecorationByte = decorationByte;
    }

    public void __RemoveAt(int globalPositionIndex)
    {
        if (globalPositionIndex >= CharCount)
            return;

        int indexOfPartitionWithContent = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithContent = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithContent == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithContent == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithContent];
        var outPartition = inPartition.RemoveAt(relativePositionIndex);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithContent,
            outPartition);
    }

    private void __InsertNewPartition(int partitionIndex)
    {
        _partitionList = _partitionList.Insert(partitionIndex, new TextEditorPartition(Array.Empty<RichCharacter>().ToImmutableList()));
    }

    private void __SplitIntoTwoPartitions(int partitionIndex)
    {
        var originalPartition = _partitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + PartitionSize % 2;
        var secondUnevenSplit = PartitionSize / 2;

        // Replace old
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(0)
                .Take(firstUnevenSplit)
                .ToImmutableList());

            _partitionList = _partitionList.SetItem(
                partitionIndex,
                partition);
        }

        // Insert new
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(firstUnevenSplit)
                .Take(secondUnevenSplit)
                .ToImmutableList());

            _partitionList = _partitionList.Insert(
                partitionIndex + 1,
                partition);
        }
    }

    public void __InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        var richCharacterEnumerator = richCharacterList.GetEnumerator();

        while (richCharacterEnumerator.MoveNext())
        {
            int indexOfPartitionWithAvailableSpace = -1;
            int relativePositionIndex = -1;
            var runningCount = 0;
            TextEditorPartition? partition;

            for (int i = 0; i < _partitionList.Count; i++)
            {
                partition = _partitionList[i];

                if (runningCount + partition.Count >= globalPositionIndex)
                {
                    if (partition.Count >= PartitionSize)
                    {
                        __SplitIntoTwoPartitions(i);
                        i--;
                        continue;
                    }

                    relativePositionIndex = globalPositionIndex - runningCount;
                    indexOfPartitionWithAvailableSpace = i;
                    break;
                }
                else
                {
                    runningCount += partition.Count;
                }
            }

            if (indexOfPartitionWithAvailableSpace == -1)
                throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

            if (relativePositionIndex == -1)
                throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

            partition = _partitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.Count;

            var richCharacterBatchInsertList = new List<RichCharacter> { richCharacterEnumerator.Current };

            while (richCharacterBatchInsertList.Count < partitionAvailableSpace && richCharacterEnumerator.MoveNext())
            {
                richCharacterBatchInsertList.Add(richCharacterEnumerator.Current);
            }

            var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
            var outPartition = inPartition.InsertRange(relativePositionIndex, richCharacterBatchInsertList);

            _partitionList = _partitionList.SetItem(
                indexOfPartitionWithAvailableSpace,
                outPartition);

            globalPositionIndex += richCharacterBatchInsertList.Count;
        }
    }

    /// <summary>
    /// This method modifies the <see cref="TextEditorPartition"/>.
    /// The method only understands singular char values, 
    /// as opposed to the text editor which interprets "\r\n" as a single character,
    /// while encompassing 2 'char' values.<br/><br/>
    /// 
    /// One needs to be cautious with this method. The line ending: "\r\n"
    /// (or any other '2 char' long character),
    /// one can remove 1 of the two characters, and the other will still remain.<br/><br/>
    /// 
    /// If the text editor tells this method to remove "\r\n",
    /// then that is a count of 2 here. Even though for the text
    /// editor, it would describe "\r\n" as a count of 1.
    /// </summary>
    public void __RemoveRange(int globalPositionIndex, int count)
    {
        int deletedCount = 0;

        // The inner for loop needs to remember its place when the while loop, loops.
        int i = 0;
        int rememberCountBeforeRemoveFromPartition = 0;
        int indexOfPartitionWithContent = -1;
        int relativePositionIndex = -1;
        int runningCount = 0;

        while (true)
        {
            if (globalPositionIndex >= CharCount)
                return;

            for (; i < _partitionList.Count; i++)
            {
                TextEditorPartition? partition = _partitionList[i];

                if (runningCount + partition.Count > globalPositionIndex)
                {
                    // This is the partition we want to modify.
                    relativePositionIndex = globalPositionIndex - runningCount;
                    indexOfPartitionWithContent = i;
                    rememberCountBeforeRemoveFromPartition = partition.Count;
                    break;
                }
                else
                {
                    runningCount += partition.Count;
                }
            }

            if (indexOfPartitionWithContent == -1)
                throw new LuthetusTextEditorException("if (indexOfPartitionWithContent == -1)");

            if (relativePositionIndex == -1)
                throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

            // At this point, the first partition with some, or all, of the content to remove has been found.
            //
            // Outside of the while loop all the 'for' loop variables were declared.
            // This lets us remove from this partition, while continuing to loop
            // over further partitions, in the case that there was more content to remove,
            // that was on other partitions.
            //
            // With the variable 'rememberCountBeforeRemoveFromPartition' we can store the
            // current count of richCharacters in the partition, prior to removing anything.
            // This is useful, because the for loop can continue as though nothing happened.
            {
                var inPartition = _partitionList[indexOfPartitionWithContent];

                var ableToDeleteCount = inPartition.RichCharacterList.Count - relativePositionIndex;

                var countToDelete = ableToDeleteCount < count
                    ? ableToDeleteCount
                    : count;

                globalPositionIndex += rememberCountBeforeRemoveFromPartition;
                runningCount += rememberCountBeforeRemoveFromPartition;
                deletedCount += countToDelete;
                count -= countToDelete;

                var outPartition = inPartition.RemoveRange(relativePositionIndex, countToDelete);

                _partitionList = _partitionList.SetItem(
                    indexOfPartitionWithContent,
                    outPartition);
            }

            if (count == 0)
                return;
            if (i == _partitionList.Count)
                return;
            if (deletedCount == count)
                return;
        }
    }

    public void __Add(RichCharacter richCharacter)
    {
        __Insert(CharCount, richCharacter);
    }
}

