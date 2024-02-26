using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

public record GlobalMetadataLazy
{
    public GlobalMetadataLazy(PartitionContainer partitionedList)
    {
        AllText = new Lazy<string>(() =>
        {
            return new string(partitionedList.PartitionList
                .SelectMany(x => x.Select(y => y.Value)).ToArray());
        });

        TabList = new Lazy<ImmutableList<int>>(() =>
        {
            var tabList = new List<int>();
            var runningCount = 0;

            for (int i = 0; i < partitionedList.PartitionMetadataMap.Count; i++)
            {
                var metadata = partitionedList.PartitionMetadataMap[i];

                tabList.AddRange(metadata.TabList.Select(x => x + runningCount));

                runningCount += metadata.Count;
            }

            return tabList.ToImmutableList();
        });

        RowEndingList = new Lazy<ImmutableList<RowEnding>>(() =>
        {
            var rowEndingList = new List<RowEnding>();
            var runningCount = 0;

            for (int i = 0; i < partitionedList.PartitionMetadataMap.Count; i++)
            {
                var metadata = partitionedList.PartitionMetadataMap[i];

                rowEndingList.AddRange(metadata.RowEndingList.Select(x => x with 
                    {
                        StartPositionIndexInclusive = x.StartPositionIndexInclusive + runningCount,
                        EndPositionIndexExclusive = x.StartPositionIndexInclusive + runningCount,
                    }));

                runningCount += metadata.Count;
            }

            return rowEndingList.ToImmutableList();
        });
    }

    public int Count { get; set; }

    /// <summary><inheritdoc cref="ITextEditorModel.TabKeyPositionsList"/></summary>
	public Lazy<ImmutableList<int>> TabList { get; }
    /// <summary><inheritdoc cref="ITextEditorModel.RowEndingPositionsList"/></summary>
	public Lazy<ImmutableList<RowEnding>> RowEndingList { get; }
	public Lazy<string> AllText { get; }
}
