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

        var partitionContainer = new PartitionContainer(5_000)
            .ToPartitionContainerModifier()
            .AddRange(text.Select(x => new RichCharacter { Value = x }));

        Assert.Equal(text, partitionContainer.AllText);
        Assert.Equal(64, partitionContainer.Count);
        Assert.Equal(RowEndingKind.Linefeed, partitionContainer.OnlyRowEndingKind);

        // partitionContainer.RowEndingKindCountList
        {
            var carriageReturnKindCount = partitionContainer.RowEndingKindCountList[0];
            Assert.Equal(0, carriageReturnKindCount.count);

            var linefeedKindCount = partitionContainer.RowEndingKindCountList[1];
            Assert.Equal(6, linefeedKindCount.count);

            var carriageReturnLinefeedKindCount = partitionContainer.RowEndingKindCountList[2];
            Assert.Equal(0, carriageReturnLinefeedKindCount.count);
        }

        // partitionContainer.RowEndingList
        {
            var rowEndingList = partitionContainer.RowEndingList;
            Assert.Equal(7, rowEndingList.Count);

            var i = 0;
            Assert.Equal(new RowEnding(35, 36, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(36, 37, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(57, 58, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(59, 60, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(61, 62, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(63, 64, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(64, 64, RowEndingKind.EndOfFile), rowEndingList[i++]);
        }

        // partitionContainer.TabList
        {
            var tabList = partitionContainer.TabList;
            Assert.Equal(60, tabList.Single());
        }
    }
    
    [Fact]
    public void AddRange_THEN_Insert_CarriageReturn()
    {
        var text = @"namespace BlazorCrudApp.Wasm.Pages;

public class MyClass
{
	
}
".ReplaceLineEndings("\r");

        var partitionContainer = new PartitionContainer(5_000);

        var partitionContainerModifier = partitionContainer.ToPartitionContainerModifier();

        partitionContainerModifier
            .AddRange(text.Select(x => new RichCharacter { Value = x }))
            .Insert(60, new RichCharacter { Value = '\r' });

        partitionContainer = partitionContainerModifier.ToPartitionContainer();

        Assert.Equal(text.Insert(60, "\r"), partitionContainer.AllText);
        Assert.Equal(65, partitionContainer.Count);
        Assert.Equal(RowEndingKind.CarriageReturn, partitionContainer.OnlyRowEndingKind);

        // partitionContainer.RowEndingKindCountList
        {
            var carriageReturnKindCount = partitionContainer.RowEndingKindCountList[0];
            Assert.Equal(7, carriageReturnKindCount.count);

            var linefeedKindCount = partitionContainer.RowEndingKindCountList[1];
            Assert.Equal(0, linefeedKindCount.count);

            var carriageReturnLinefeedKindCount = partitionContainer.RowEndingKindCountList[2];
            Assert.Equal(0, carriageReturnLinefeedKindCount.count);
        }

        // partitionContainer.RowEndingList
        {
            var rowEndingList = partitionContainer.RowEndingList;
            Assert.Equal(8, rowEndingList.Count);

            var i = 0;
            Assert.Equal(new RowEnding(35, 36, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(36, 37, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(57, 58, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(59, 60, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(60, 61, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(62, 63, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(64, 65, RowEndingKind.CarriageReturn), rowEndingList[i++]);
            Assert.Equal(new RowEnding(65, 65, RowEndingKind.EndOfFile), rowEndingList[i++]);
        }

        // partitionContainer.TabList
        {
            var tabList = partitionContainer.TabList;
            Assert.Equal(61, tabList.Single());
        }
    }
    
    [Fact]
    public void AddRange_THEN_Insert_Linefeed()
    {
        var text = @"namespace BlazorCrudApp.Wasm.Pages;

public class MyClass
{
	
}
".ReplaceLineEndings("\n");

        var partitionContainer = new PartitionContainer(5_000)
            .ToPartitionContainerModifier()
            .AddRange(text.Select(x => new RichCharacter { Value = x }))
            .Insert(60, new RichCharacter { Value = '\n' })
            .ToPartitionContainer();

        Assert.Equal(text.Insert(60, "\n"), partitionContainer.AllText);
        Assert.Equal(65, partitionContainer.Count);
        Assert.Equal(RowEndingKind.Linefeed, partitionContainer.OnlyRowEndingKind);

        // partitionContainer.RowEndingKindCountList
        {
            var carriageReturnKindCount = partitionContainer.RowEndingKindCountList[0];
            Assert.Equal(0, carriageReturnKindCount.count);

            var linefeedKindCount = partitionContainer.RowEndingKindCountList[1];
            Assert.Equal(7, linefeedKindCount.count);

            var carriageReturnLinefeedKindCount = partitionContainer.RowEndingKindCountList[2];
            Assert.Equal(0, carriageReturnLinefeedKindCount.count);
        }

        // partitionContainer.RowEndingList
        {
            var rowEndingList = partitionContainer.RowEndingList;
            Assert.Equal(8, rowEndingList.Count);

            var i = 0;
            Assert.Equal(new RowEnding(35, 36, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(36, 37, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(57, 58, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(59, 60, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(60, 61, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(62, 63, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(64, 65, RowEndingKind.Linefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(65, 65, RowEndingKind.EndOfFile), rowEndingList[i++]);
        }

        // partitionContainer.TabList
        {
            var tabList = partitionContainer.TabList;
            Assert.Equal(61, tabList.Single());
        }
    }

    [Fact]
    public void AddRange_THEN_Insert_CarriageReturnLinefeed()
    {
        var text = @"namespace BlazorCrudApp.Wasm.Pages;

public class MyClass
{
	
}
".ReplaceLineEndings("\r\n");

        var partitionContainer = new PartitionContainer(5_000)
            .ToPartitionContainerModifier()
            .AddRange(text.Select(x => new RichCharacter { Value = x }))
            .Insert(60, new RichCharacter { Value = '\r' })
            .Insert(61, new RichCharacter { Value = '\n' })
            .ToPartitionContainer();

        Assert.Equal(text.Insert(60, "\r\n"), partitionContainer.AllText);
        Assert.Equal(72, partitionContainer.Count);
        Assert.Equal(RowEndingKind.CarriageReturnLinefeed, partitionContainer.OnlyRowEndingKind);

        // partitionContainer.RowEndingKindCountList
        {
            var carriageReturnKindCount = partitionContainer.RowEndingKindCountList[0];
            Assert.Equal(0, carriageReturnKindCount.count);

            var linefeedKindCount = partitionContainer.RowEndingKindCountList[1];
            Assert.Equal(0, linefeedKindCount.count);

            var carriageReturnLinefeedKindCount = partitionContainer.RowEndingKindCountList[2];
            Assert.Equal(7, carriageReturnLinefeedKindCount.count);
        }

        // partitionContainer.RowEndingList
        {
            var rowEndingList = partitionContainer.RowEndingList;
            Assert.Equal(8, rowEndingList.Count);

            var i = 0;
            Assert.Equal(new RowEnding(35, 37, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(37, 39, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(59, 61, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(60, 62, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(64, 66, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(67, 69, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(70, 72, RowEndingKind.CarriageReturnLinefeed), rowEndingList[i++]);
            Assert.Equal(new RowEnding(72, 72, RowEndingKind.EndOfFile), rowEndingList[i++]);
        }

        // partitionContainer.TabList
        {
            var tabList = partitionContainer.TabList;
            Assert.Equal(66, tabList.Single());
        }
    }
}
