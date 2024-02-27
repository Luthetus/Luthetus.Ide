using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class AdhocTests
{
    [Fact]
    public void AddRange()
    {
        var text = @"namespace BlazorCrudApp.Wasm.Pages;

public class MyClass
{
	
}
".ReplaceLineEndings("\n");

        var partitionContainer = new PartitionContainer(5_000).AddRange(text.Select(x => new RichCharacter { Value = x }));

        var globalMetadata = partitionContainer.GlobalMetadata;
        Assert.Equal(text, globalMetadata.AllText.Value);
        Assert.Equal(64, globalMetadata.GlobalCharacterCount.Value);
        Assert.Equal(RowEndingKind.Linefeed, globalMetadata.OnlyRowEndingKind.Value);

        // globalMetadata.RowEndingKindCountList
        {
            var carriageReturnKindCount = globalMetadata.RowEndingKindCountList.Value[0];
            Assert.Equal(0, carriageReturnKindCount.count);

            var linefeedKindCount = globalMetadata.RowEndingKindCountList.Value[1];
            Assert.Equal(6, linefeedKindCount.count);

            var carriageReturnLinefeedKindCount = globalMetadata.RowEndingKindCountList.Value[2];
            Assert.Equal(0, carriageReturnLinefeedKindCount.count);
        }

        // globalMetadata.RowEndingList
        {
            var rowEndingList = globalMetadata.RowEndingList.Value;
            Assert.Equal(6, rowEndingList.Count);

            var i = 0;
            Assert.Equal(new RowEnding(35, 36, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(36, 37, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(57, 58, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(59, 60, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(61, 62, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(63, 64, RowEndingKind.Linefeed), rowEndingList[i++]);
        }

        // globalMetadata.TabList
        {
            var tabList = globalMetadata.TabList.Value;
            Assert.Equal(60, tabList.Single());
        }
    }
    
    [Fact]
    public void AddRange_THEN_Insert()
    {
        var text = @"namespace BlazorCrudApp.Wasm.Pages;

public class MyClass
{
	
}
".ReplaceLineEndings("\n");

        var partitionContainer = new PartitionContainer(5_000)
            .AddRange(text.Select(x => new RichCharacter { Value = x }))
            .Insert(60, new RichCharacter { Value = '\n' });

        var globalMetadata = partitionContainer.GlobalMetadata;
        Assert.Equal(text.Insert(60, "\n"), globalMetadata.AllText.Value);
        Assert.Equal(65, globalMetadata.GlobalCharacterCount.Value);
        Assert.Equal(RowEndingKind.Linefeed, globalMetadata.OnlyRowEndingKind.Value);

        // globalMetadata.RowEndingKindCountList
        {
            var carriageReturnKindCount = globalMetadata.RowEndingKindCountList.Value[0];
            Assert.Equal(0, carriageReturnKindCount.count);

            var linefeedKindCount = globalMetadata.RowEndingKindCountList.Value[1];
            Assert.Equal(7, linefeedKindCount.count);

            var carriageReturnLinefeedKindCount = globalMetadata.RowEndingKindCountList.Value[2];
            Assert.Equal(0, carriageReturnLinefeedKindCount.count);
        }

        // globalMetadata.RowEndingList
        {
            var rowEndingList = globalMetadata.RowEndingList.Value;
            Assert.Equal(7, rowEndingList.Count);

            var i = 0;
            Assert.Equal(new RowEnding(35, 36, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(36, 37, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(57, 58, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(59, 60, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(60, 61, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(62, 63, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(64, 65, RowEndingKind.Linefeed), rowEndingList[i++]);
        }

        // globalMetadata.TabList
        {
            var tabList = globalMetadata.TabList.Value;
            Assert.Equal(61, tabList.Single());
        }
    }
}
