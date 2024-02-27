using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

public record GlobalMetadataLazy
{
    public GlobalMetadataLazy(PartitionContainer container)
    {
        AllText = new Lazy<string>(() => new string(container.PartitionList.SelectMany(x => x.Select(y => y.Value)).ToArray()));

        TabList = new Lazy<ImmutableList<int>>(() =>
        {
            var tabList = new List<int>();
            var runningCount = 0;
            for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
            {
                var metadata = container.PartitionMetadataMap[i];
                tabList.AddRange(metadata.TabList.Select(x => x + runningCount));
                runningCount += metadata.RelativeCharacterCount;
            }
            return tabList.ToImmutableList();
        });

        RowEndingList = new Lazy<ImmutableList<RowEnding>>(() =>
        {
            var rowEndingList = new List<RowEnding>();
            var runningCount = 0;
            for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
            {
                var metadata = container.PartitionMetadataMap[i];
                rowEndingList.AddRange(metadata.RowEndingList.Select(x => x with 
                    {
                        StartPositionIndexInclusive = x.StartPositionIndexInclusive + runningCount,
                        EndPositionIndexExclusive = x.EndPositionIndexExclusive + runningCount,
                    }));
                runningCount += metadata.RelativeCharacterCount;
            }
            return rowEndingList.ToImmutableList();
        });

        RowEndingKindCountList = new Lazy<ImmutableList<(RowEndingKind rowEndingKind, int count)>>(() =>
        {
            var carriageReturnRunningCount = 0;
            var linefeedRunningCount = 0;
            var carriageReturnLinefeedRunningCount = 0;
            for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
            {
                var metadata = container.PartitionMetadataMap[i];
                foreach (var rowEndingKindCount in metadata.RowEndingKindCountList)
                {
                    switch (rowEndingKindCount.rowEndingKind)
                    {
                        case RowEndingKind.CarriageReturn:
                            carriageReturnRunningCount += rowEndingKindCount.count;
                            break;
                        case RowEndingKind.Linefeed:
                            linefeedRunningCount += rowEndingKindCount.count;
                            break;
                        case RowEndingKind.CarriageReturnLinefeed:
                            carriageReturnLinefeedRunningCount += rowEndingKindCount.count;
                            break;
                    }
                }
            }
            return new (RowEndingKind rowEndingKind, int count)[]
            {
                new (RowEndingKind.CarriageReturn, carriageReturnRunningCount),
                new (RowEndingKind.Linefeed, linefeedRunningCount),
                new (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedRunningCount),
            }.ToImmutableList();
        });

        OnlyRowEndingKind = new Lazy<RowEndingKind?>(() =>
        {
            var partitionsWithRowEndings = container.PartitionMetadataMap.Where(x => x.RowEndingList.Count > 0).ToList();

            var onlyRowEndingKind = (RowEndingKind?)null;
            if (partitionsWithRowEndings.Count > 0)
            {
                var sampleOnlyRowEndingKind = container.PartitionMetadataMap[0].OnlyRowEndingKind;
                if (partitionsWithRowEndings.All(x => x.OnlyRowEndingKind == sampleOnlyRowEndingKind))
                    onlyRowEndingKind = sampleOnlyRowEndingKind;
            }

            return onlyRowEndingKind;
        });
    }

    public int Count { get; set; }
	public Lazy<ImmutableList<int>> TabList { get; } // ITextEditorModel.TabKeyPositionsList
    public Lazy<ImmutableList<RowEnding>> RowEndingList { get; } // ITextEditorModel.RowEndingPositionsList
    public Lazy<ImmutableList<(RowEndingKind rowEndingKind, int count)>> RowEndingKindCountList { get; }
    public Lazy<RowEndingKind?> OnlyRowEndingKind { get; init; }
    public Lazy<string> AllText { get; }
}
