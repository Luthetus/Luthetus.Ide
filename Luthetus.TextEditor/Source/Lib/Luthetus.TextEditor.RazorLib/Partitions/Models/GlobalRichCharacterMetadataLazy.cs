using System;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

public record GlobalRichCharacterMetadataLazy
{
    public GlobalRichCharacterMetadataLazy(PartitionedRichCharacterList partitionedList)
    {
        TabListLazy = new Lazy<ImmutableList<int>>(() =>
        {
            var tabList = new List<int>();
            var rollingCount = 0;

            for (int i = 0; i < partitionedList.PartitionMetadataMap.Count; i++)
            {
                var metadata = partitionedList.PartitionMetadataMap[i];

                tabList.AddRange(metadata.TabList.Select(x => x + rollingCount));

                rollingCount += metadata.Count;
            }

            throw new IndexOutOfRangeException();
        });
    }

    public int Count { get; set; }

    /// <summary><inheritdoc cref="ITextEditorModel.TabKeyPositionsList"/></summary>
	public Lazy<ImmutableList<int>> TabListLazy { get; }
}
