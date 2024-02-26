using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

public record GlobalMetadataLazy
{
    public GlobalMetadataLazy(PartitionContainer container)
    {
        AllText = new Lazy<string>(() =>
        {
            return new string(container.PartitionList
                .SelectMany(x => x.Select(y => y.Value)).ToArray());
        });

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
                        EndPositionIndexExclusive = x.StartPositionIndexInclusive + runningCount,
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
            // This line presumes there will always be at least 1 partition.
            var onlyRowEndingKind = container.PartitionMetadataMap[0].OnlyRowEndingKind;
            if (container.PartitionMetadataMap.All(x => x.OnlyRowEndingKind == onlyRowEndingKind))
                return onlyRowEndingKind;
            else
                return null;
        });
    }

    public int Count { get; set; }

    /// <summary><inheritdoc cref="ITextEditorModel.TabKeyPositionsList"/></summary>
	public Lazy<ImmutableList<int>> TabList { get; }
    /// <summary><inheritdoc cref="ITextEditorModel.RowEndingPositionsList"/></summary>
	public Lazy<ImmutableList<RowEnding>> RowEndingList { get; }
	public Lazy<ImmutableList<(RowEndingKind rowEndingKind, int count)>> RowEndingKindCountList { get; }
    public Lazy<RowEndingKind?> OnlyRowEndingKind { get; init; }
    public Lazy<string> AllText { get; }
}
