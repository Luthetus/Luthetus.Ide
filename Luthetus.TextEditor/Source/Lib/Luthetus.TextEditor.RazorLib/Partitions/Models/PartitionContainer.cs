using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

public record PartitionContainer : IList<RichCharacter> // TODO: I need to handle the partition meta data differently for the text editor. Perhaps it would be preferable to have a generic <see cref="PartitionedImmutableList{TItem}"/> type, but I'm finding it hard to do so without odd looking and confusing code. So, I'm going to copy and paste the attempt at the generic type here, then just change the source code to work for the text editor. (2024-02-25)
{
    public const int EXPANSION_FACTOR = 3; // When a partition runs out space its content is divided amongst some amount of partitions. If expansion factor is 3, then when a partition expands, it will insert 2 addition partitions after itself. Then the original partition splits its content into thirds. And distributes it across itself, and the other 2 newly inserted partitions.

    public PartitionContainer(PartitionContainer other) // record copy constructor
    {
        PartitionSize = other.PartitionSize;
        PartitionList = other.PartitionList;
        PartitionMetadataMap = other.PartitionMetadataMap;
        GlobalMetadata = new GlobalMetadataLazy(this); // Reset the global metadata each time the record 'with' keyword is used.
    }

    public PartitionContainer(int partitionSize)
    {
        if (partitionSize < EXPANSION_FACTOR)
            throw new ApplicationException($"Partition size must be equal to or greater than the {nameof(EXPANSION_FACTOR)}:{EXPANSION_FACTOR}.");

        PartitionSize = partitionSize;
        PartitionList = new ImmutableList<RichCharacter>[] { ImmutableList<RichCharacter>.Empty, }.ToImmutableList();
        PartitionMetadataMap = new PartitionMetadata[]
        {
            new() 
            {
                RelativeCharacterCount = 0,
                TabList = ImmutableList<int>.Empty,
                RowEndingList = new RowEnding[] { new(0, 0, RowEndingKind.EndOfFile) }.ToImmutableList(),
            },
        }.ToImmutableList();
        GlobalMetadata = new GlobalMetadataLazy(this); // TODO: How does one not duplicate this code? It exists in the record copy constructor too. A parameterless constructor was tried and invoked with 'this()' but it gives an error message specific to usage of record copy constructors.
    }

    public int PartitionSize { get; }
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; init; }
    public ImmutableList<PartitionMetadata> PartitionMetadataMap { get; init; } // Track the 'Count' of each partition in this. Therefore, one can lookup whether a partition is full or not. Without iterating through all the partitions just to check a specific one.<br/>The name 'Map' is used here because to get the Count of the 0th index partition, one would read the value at index 0 of this property. In otherwords, each partition index maps to its corresponding Count.
    public GlobalMetadataLazy GlobalMetadata { get; }

    public int GlobalCharacterCount
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

    public RichCharacter this[int globalPositionIndex]
    {
        get
        {
            var runningCount = 0;
            for (int i = 0; i < PartitionMetadataMap.Count; i++)
            {
                var partitionCount = PartitionMetadataMap[i].RelativeCharacterCount;
                if (runningCount + partitionCount > globalPositionIndex)
                {
                    var partition = PartitionList[i];
                    return partition[globalPositionIndex - runningCount];
                }
                else
                {
                    runningCount += partitionCount;
                }
            }
            throw new IndexOutOfRangeException();
        }
        set => throw new NotImplementedException();
    }

    public PartitionContainer Add(RichCharacter richCharacter) =>
        PartitionReducer.ReduceInsert(GlobalCharacterCount, richCharacter, this);

    public PartitionContainer AddRange(IEnumerable<RichCharacter> richCharacterList) =>
        PartitionReducer.ReduceInsertRange(GlobalCharacterCount, richCharacterList, this);

    public PartitionContainer Insert(int globalPositionIndex, RichCharacter richCharacter) =>
        PartitionReducer.ReduceInsert(globalPositionIndex, richCharacter, this);
    
    public PartitionContainer InsertRange(int index, IEnumerable<RichCharacter> itemList) =>
        PartitionReducer.ReduceInsertRange(index, itemList, this);
    
    public PartitionContainer Remove(RichCharacter richCharacter) =>
        PartitionReducer.ReduceRemove(richCharacter, this);
    
    public PartitionContainer RemoveAt(int globalPositionIndex) =>
        PartitionReducer.ReduceRemoveAt(globalPositionIndex, this);
    
    public PartitionContainer RemoveRange(int globalPositionIndex, int count) =>
        PartitionReducer.ReduceRemoveRange(globalPositionIndex, count, this);
    
    public PartitionContainer Clear() =>
        new PartitionContainer(PartitionSize);
    
    public bool Contains(RichCharacter item) =>
        IndexOf(item) != -1;

    public void CopyTo(RichCharacter[] array, int arrayIndex)
    {
        PartitionContainer list = this;
        for (int i = 0; i < list.GlobalCharacterCount; i++)
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
        PartitionContainer list = this;
        for (int i = 0; i < list.GlobalCharacterCount; i++)
        {
            RichCharacter? entry = list[i];
            if (item.Equals(entry))
                return i;
        }
        return -1;
    }

    internal static (ImmutableList<RichCharacter> partition, int partitionIndex, int runningCount) GetOffsetPartition(ImmutableList<ImmutableList<RichCharacter>> partitionList, ImmutableList<PartitionMetadata> partitionMetadataMap, int index)
    {
        var runningCount = 0;
        for (int i = 0; i < partitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = partitionMetadataMap[i].RelativeCharacterCount;
            if (runningCount + currentPartitionCount > index)
                return (partitionList[i], i, runningCount);
            else
                runningCount += currentPartitionCount;
        }
        throw new IndexOutOfRangeException();
    }
    
    internal static int GetRelativePositionIndex(int partitionIndex, ImmutableList<ImmutableList<RichCharacter>> partitionList, ImmutableList<PartitionMetadata> partitionMetadataMap, int globalPositionIndex)
    {
        var offsetInfoPartition = GetOffsetPartition(partitionList, partitionMetadataMap, partitionIndex);
        return globalPositionIndex - offsetInfoPartition.runningCount;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    int ICollection<RichCharacter>.Count => GlobalCharacterCount;
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
}
