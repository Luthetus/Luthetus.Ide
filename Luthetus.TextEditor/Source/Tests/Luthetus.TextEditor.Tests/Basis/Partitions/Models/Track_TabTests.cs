using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_TabTests : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add_Start()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "\tab".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.TabList);
        var tabKeyPosition = partitionMetadata.TabList.Single();
        Assert.Equal(0, tabKeyPosition);
    }

    [Fact]
    public override void Add_Middle()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "a\tb".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.TabList);
        var tabKeyPosition = partitionMetadata.TabList.Single();
        Assert.Equal(1, tabKeyPosition);
    }

    [Fact]
    public override void Add_End()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.TabList);
        var tabKeyPosition = partitionMetadata.TabList.Single();
        Assert.Equal(2, tabKeyPosition);
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        partitionContainer = partitionContainer.Add(
            new RichCharacter { Value = '\t' });

        var globalMetadataLazy = partitionContainer.GlobalMetadata;
        var globalTabList = globalMetadataLazy.TabList.Value;
        var globalAllText = globalMetadataLazy.AllText.Value;

        throw new NotImplementedException();
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        var partitionSize = 3;
        var partitionContainer = new PartitionContainer(partitionSize);
        var text = new string('\t', 4);
        var richCharacterList = text.Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(3, partitionSize);
        Assert.Equal(4, partitionContainer.GlobalCharacterCount);

        // PartitionList
        {
            Assert.Single(partitionContainer.PartitionList);
            var partition = partitionContainer.PartitionList.Single();
            Assert.Equal('\t', partition[0].Value);
            Assert.Equal('\t', partition[1].Value);
            Assert.Equal('\t', partition[2].Value);
            Assert.Equal('\t', partition[3].Value);
        }

        // PartitionMetadataMap
        {
            Assert.Single(partitionContainer.PartitionMetadataMap);
        }

        var globalMetadataLazy = partitionContainer.GlobalMetadata;

        var globalTabList = globalMetadataLazy.TabList.Value;
        Assert.Equal(4, globalTabList.Count);
        Assert.Equal(0, globalTabList[0]);
        Assert.Equal(1, globalTabList[1]);
        Assert.Equal(2, globalTabList[2]);
        Assert.Equal(3, globalTabList[3]);

        var globalAllText = globalMetadataLazy.AllText.Value;
        Assert.Equal(text, globalAllText);

        // Assert that these were not erroneously changed
        {
            var rowEndingList = globalMetadataLazy.RowEndingList.Value;
            Assert.Empty(rowEndingList);
        }
    }
    #endregion

    #region Insert
    [Fact]
    public override void Insert_Start()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "\tab".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.TabList);
        var tabKeyPosition = partitionMetadata.TabList.Single();
        Assert.Equal(0, tabKeyPosition);
    }

    [Fact]
    public override void Insert_Middle()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "a\tb".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.TabList);
        var tabKeyPosition = partitionMetadata.TabList.Single();
        Assert.Equal(1, tabKeyPosition);
    }

    [Fact]
    public override void Insert_End()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.TabList);
        var tabKeyPosition = partitionMetadata.TabList.Single();
        Assert.Equal(2, tabKeyPosition);
    }

    [Fact]
    public override void Insert_Causes_Expansion()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        partitionContainer = partitionContainer.Insert(
            0, new RichCharacter { Value = '\t' });

        var globalMetadataLazy = partitionContainer.GlobalMetadata;
        var globalTabList = globalMetadataLazy.TabList.Value;
        var globalAllText = globalMetadataLazy.AllText.Value;

        throw new NotImplementedException();
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = new string('\t', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        // Reading some state so I see it in debugger
        {
            var globalMetadataLazy = partitionContainer.GlobalMetadata;
            var globalTabList = globalMetadataLazy.TabList.Value;
            var globalAllText = globalMetadataLazy.AllText.Value;
        }

        partitionContainer = partitionContainer.Insert(
            0, new RichCharacter { Value = '\t' });

        // Reading some state so I see it in debugger
        {
            var globalMetadataLazy = partitionContainer.GlobalMetadata;
            var globalTabList = globalMetadataLazy.TabList.Value;
            var globalAllText = globalMetadataLazy.AllText.Value;
        }

        throw new NotImplementedException();
    }
    #endregion
}
