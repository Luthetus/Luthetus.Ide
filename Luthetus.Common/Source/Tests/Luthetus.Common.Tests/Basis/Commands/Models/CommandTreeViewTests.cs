using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="TreeViewCommand"/>
/// </summary>
public class CommandTreeViewTests
{
    /// <summary>
    /// <see cref="TreeViewCommand(string, string, bool, Func{ICommandArgs, Task})"/>
    /// </summary>
    [Fact]
    public async Task Constructor()
    {
        var displayName = "";
        var internalIdentifier = "";
        var shouldBubble = false;

        var incrementThisNumberToOne = 0;

        var doAsyncFunc = new Func<ICommandArgs, Task>(commandArgs =>
        {
            incrementThisNumberToOne++;
            return Task.CompletedTask;
        });

        var treeViewCommand = new TreeViewCommand(
            displayName,
            internalIdentifier,
            shouldBubble,
            doAsyncFunc);

        await treeViewCommand.DoAsyncFunc.Invoke(new CommonCommandArgs());

        Assert.Equal(displayName, treeViewCommand.DisplayName);
        Assert.Equal(internalIdentifier, treeViewCommand.InternalIdentifier);
        Assert.Equal(shouldBubble, treeViewCommand.ShouldBubble);
        Assert.Equal(1, incrementThisNumberToOne);
    }
}