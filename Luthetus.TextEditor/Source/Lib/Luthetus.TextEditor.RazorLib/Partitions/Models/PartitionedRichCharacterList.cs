using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

/// <summary>
/// TODO: I need to handle the partition meta data differently for the text editor...
/// ...Perhaps it would be preferable to have a generic 'PartitionedImmutableList&gt;RichCharacter&lt;'
/// type, but I'm finding a it hard to do so without odd looking and confusing code.
/// So, I'm going to copy and paste the attempt at the generic type here, then just
/// change the source code to work for the text editor. (2024-02-25)
/// </summary>
public record PartitionedRichCharacterList : IList<RichCharacter>
{
    /// <summary>
    /// When a partition runs out space its content is divided amongst some amount of partitions.
    /// If expansion factor is 3, then when a partition expands,
    /// it will insert 2 addition partitions after itself.
    /// Then the original partition splits its content into thirds.
    /// And distributes it across itself, and the other 2 newly inserted partitions.
    /// </summary>
    public const int EXPANSION_FACTOR = 3;

    public PartitionedRichCharacterList(int partitionSize)
    {
        if (partitionSize < EXPANSION_FACTOR)
            throw new ApplicationException($"Partition size must be equal to or greater than the {nameof(EXPANSION_FACTOR)}:{EXPANSION_FACTOR}.");

        PartitionSize = partitionSize;
    }

    public RichCharacter this[int index]
    {
        get
        {
            var rollingCount = 0;

            for (int i = 0; i < PartitionRichCharacterMetadataMap.Count; i++)
            {
                var currentPartitionCount = PartitionRichCharacterMetadataMap[i].Count;

                if (rollingCount + currentPartitionCount > index)
                {
                    var partition = PartitionList[i];
                    return partition[index - rollingCount];
                }
                else
                {
                    rollingCount += currentPartitionCount;
                }
            }

            throw new IndexOutOfRangeException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public int PartitionSize { get; }
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; init; } = ImmutableList<ImmutableList<RichCharacter>>.Empty;

    /// <summary>
    /// Track the 'Count' of each partition in this.
    /// Therefore, one can lookup whether a partition is full or not.
    /// Without iterating through all the partitions just to check a specific one.
    /// <br/>
    /// The name 'Map' is used here because to get the Count of the 0th index partition,
    /// one would read the value at index 0 of this property.
    /// In otherwords, each partition index maps to its corresponding Count.
    /// </summary>
    public ImmutableList<PartitionRichCharacterMetadata> PartitionRichCharacterMetadataMap { get; init; } = ImmutableList<PartitionRichCharacterMetadata>.Empty;

    public int Count
    {
        get
        {
            if (PartitionList.Count == 0)
                return 0;

            var count = 0;

            foreach (var partition in PartitionList)
            {
                count += partition.Count;
            }

            return count;
        }
    }

    public bool IsReadOnly => true;

    public PartitionedRichCharacterList Add(RichCharacter value)
    {
        var indexPartitionFreeSpace = -1;

        for (int i = 0; i < PartitionRichCharacterMetadataMap.Count; i++)
        {
            int count = PartitionRichCharacterMetadataMap[i].Count;

            if (count != PartitionSize)
                indexPartitionFreeSpace = i;
        }

        if (indexPartitionFreeSpace == -1)
        {
            var partition = new RichCharacter[] { value }.ToImmutableList();

            return this with
            {
                PartitionList = PartitionList.Add(partition),
                PartitionRichCharacterMetadataMap = PartitionRichCharacterMetadataMap.Add(new(partition.Count))
            };
        }
        else
        {
            var partition = PartitionList[indexPartitionFreeSpace];
            partition = partition.Add(value);

            return this with
            {
                PartitionList = PartitionList.SetItem(indexPartitionFreeSpace, partition),
                PartitionRichCharacterMetadataMap = PartitionRichCharacterMetadataMap.SetItem(indexPartitionFreeSpace, new(partition.Count))
            };
        }
    }

    /// <summary>
    /// TODO: Make <see cref="AddRange"/> optimized if needed. Currently it just foreach invokes the <see cref="Add"/> version.
    /// </summary>
    public PartitionedRichCharacterList AddRange(IEnumerable<RichCharacter> itemList)
    {
        var partitionedImmutableList = this;

        foreach (var item in itemList)
        {
            partitionedImmutableList = partitionedImmutableList.Add(item);
        }

        return partitionedImmutableList;
    }

    /// <summary>
    /// TODO: Should this method maintain the partitions, but just clear them?...
    /// Or should it return an entirely new instance so that all the memory is freed?
    /// As of this comment I'm going to return an entirely new instance with the same <see cref="PartitionSize"/>.
    /// (2024-02-24).
    /// </summary>
    public PartitionedRichCharacterList Clear()
    {
        return new PartitionedRichCharacterList(PartitionSize);
    }

    public bool Contains(RichCharacter item)
    {
        return IndexOf(item) != -1;
    }

    public void CopyTo(RichCharacter[] array, int arrayIndex)
    {
        PartitionedRichCharacterList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i];
        }
    }

    public IEnumerator<RichCharacter> GetEnumerator()
    {
        foreach (var partition in PartitionList)
        {
            foreach (var item in partition)
            {
                yield return item;
            }
        }
    }

    public int IndexOf(RichCharacter item)
    {
        PartitionedRichCharacterList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            RichCharacter? entry = list[i];
            if (item.Equals(entry))
                return i;
        }

        return -1;
    }

    public PartitionedRichCharacterList Insert(int index, RichCharacter item)
    {
        var outPartitionedImmutableList = this;

        if (outPartitionedImmutableList.PartitionRichCharacterMetadataMap.Count == 0)
            return outPartitionedImmutableList.Add(item);

        var rollingCount = 0;
        var indexPartition = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        var offset = 0;

        for (int i = 0; i < outPartitionedImmutableList.PartitionRichCharacterMetadataMap.Count; i++)
        {
            var currentPartitionCount = outPartitionedImmutableList.PartitionRichCharacterMetadataMap[i].Count;

            if (rollingCount + currentPartitionCount >= index)
            {
                indexPartition = i;

                if (currentPartitionCount == outPartitionedImmutableList.PartitionSize)
                {
                    outPartitionedImmutableList = outPartitionedImmutableList.ExpandPartition(indexPartition);
                    i -= 1;
                    continue;
                }

                partition = outPartitionedImmutableList.PartitionList[i];
                offset = index - rollingCount;
                break;
            }
            else
            {
                rollingCount += currentPartitionCount;
            }
        }

        if (partition is null)
            throw new IndexOutOfRangeException();

        partition = partition.Insert(offset, item);

        var outPartitionList = outPartitionedImmutableList.PartitionList.SetItem(
            indexPartition,
            partition);

        var outPartitionMemoryMap = outPartitionedImmutableList.PartitionRichCharacterMetadataMap.SetItem(
            indexPartition,
            new(outPartitionedImmutableList.PartitionRichCharacterMetadataMap[indexPartition].Count + 1));

        return outPartitionedImmutableList with
        {
            PartitionList = outPartitionList,
            PartitionRichCharacterMetadataMap = outPartitionMemoryMap,
        };
    }
    
    public PartitionedRichCharacterList InsertRange(int index, IEnumerable<RichCharacter> itemList)
    {
        var partitionedImmutableList = this;

        foreach (var item in itemList)
        {
            partitionedImmutableList = partitionedImmutableList.Insert(index++, item);
        }

        return partitionedImmutableList;
    }

    public PartitionedRichCharacterList Remove(RichCharacter item)
    {
        PartitionedRichCharacterList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(item))
                return RemoveAt(i);
        }

        return this;
    }

    public PartitionedRichCharacterList RemoveAt(int index)
    {
        var rollingCount = 0;
        var indexPartition = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        var offset = 0;

        for (int i = 0; i < PartitionRichCharacterMetadataMap.Count; i++)
        {
            var currentPartitionCount = PartitionRichCharacterMetadataMap[i].Count;

            if (rollingCount + currentPartitionCount > index)
            {
                indexPartition = i;
                partition = PartitionList[i];
                offset = index - rollingCount;
                break;
            }
            else
            {
                rollingCount += currentPartitionCount;
            }
        }

        if (partition is null)
            throw new IndexOutOfRangeException();

        partition = partition.RemoveAt(offset);

        var outPartitionList = PartitionList.SetItem(indexPartition, partition);

        var outPartitionMemoryMap = PartitionRichCharacterMetadataMap.SetItem(
            indexPartition,
            new(PartitionRichCharacterMetadataMap[indexPartition].Count - 1));

        return this with
        {
            PartitionList = outPartitionList,
            PartitionRichCharacterMetadataMap = outPartitionMemoryMap,
        };
    }
    
    public PartitionedRichCharacterList RemoveRange(int index, int count)
    {
        var partitionedImmutableList = this;

        for (int i = 0; i < count; i++)
        {
            partitionedImmutableList = partitionedImmutableList.RemoveAt(index);
        }

        return partitionedImmutableList;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<RichCharacter>.Add(RichCharacter item) => Add(item);
    void ICollection<RichCharacter>.Clear() => Clear();

    bool ICollection<RichCharacter>.Remove(RichCharacter item)
    {
        var index = IndexOf(item);

        if (index == -1)
            return false;

        RemoveAt(index);
        return true;
    }

    void IList<RichCharacter>.Insert(int index, RichCharacter item) => Insert(index, item);
    void IList<RichCharacter>.RemoveAt(int index) => RemoveAt(index);

    private PartitionedRichCharacterList ExpandPartition(int index)
    {
        var outPartitionedImmutableList = this;

        if (index < 0 || index >= outPartitionedImmutableList.PartitionList.Count)
            throw new IndexOutOfRangeException();

        var outPartitionList = outPartitionedImmutableList.PartitionList;

        // inPartition Contains original text.
        // The goal is to split the text across 3 partitions.
        // Allot the original content in 1/3.
        //
        // Only 2 partitions need to be inserted because the third partition will just be the
        // original partition, but with its contents change.
        var inPartition = outPartitionList[index];

        // The idealSplit likely will not be enough due to integer math losing decimals.
        // Therefore, give the middle partition any remainder.
        var idealSplit = inPartition.Count / EXPANSION_FACTOR;

        // Determine the impact of integer math loss of decimal places
        var charactersLost = inPartition.Count - (idealSplit * EXPANSION_FACTOR);

        var replaceOriginalPartition = inPartition;

        var partitionNewList = new List<ImmutableList<RichCharacter>>();
        for (int i = 0; i < EXPANSION_FACTOR; i++)
        {
            if (i == 0)
            {
                replaceOriginalPartition = inPartition
                    .Take(idealSplit)
                    .ToImmutableList();
            }
            else if (i == 1)
            {
                var partitionNew = inPartition
                    .Skip(idealSplit)
                    .Take(idealSplit + charactersLost)
                    .ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
            else
            {
                var partitionNew = inPartition
                    .Skip(idealSplit * i + charactersLost)
                    .Take(idealSplit)
                    .ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
        }

        outPartitionList = outPartitionList.SetItem(index, replaceOriginalPartition);
        outPartitionList = outPartitionList.InsertRange(index + 1, partitionNewList);

        outPartitionedImmutableList = outPartitionedImmutableList with
        {
            PartitionList = outPartitionList
        };

        var newPartitionRichCharacterMetadata = partitionNewList.Select(x => new PartitionRichCharacterMetadata(x.Count));

        var outPartitionMemoryMap = outPartitionedImmutableList.PartitionRichCharacterMetadataMap
            .InsertRange(index + 1, newPartitionRichCharacterMetadata);

        for (int i = index; i < (index + EXPANSION_FACTOR); i++)
        {
            var partition = outPartitionedImmutableList.PartitionList[i];
            outPartitionMemoryMap = outPartitionMemoryMap.SetItem(i, new (partition.Count));
        }

        return outPartitionedImmutableList with
        {
            PartitionRichCharacterMetadataMap = outPartitionMemoryMap
        };
    }
}
