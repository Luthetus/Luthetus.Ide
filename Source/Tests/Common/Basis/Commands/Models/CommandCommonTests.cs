using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="CommonCommand"/>
/// </summary>
public class CommandCommonTests
{
    /// <summary>
    /// <see cref="CommonCommand(Func{ICommandArgs, Task}, string, string, bool)"/>
    /// </summary>
    [Fact]
    public async Task Constructor()
    {
        var number = 0;
        
        var displayName = "Increment Number";
        var internalIdentifier = "increment-number";
        var shouldBubble = false;

        var commandCommon = new CommonCommand(
            displayName,
            internalIdentifier,
            shouldBubble,
            commandArgs => 
            {
                number++;
                return Task.CompletedTask;
            });

        await commandCommon.CommandFunc.Invoke(new CommonCommandArgs());

        Assert.Equal(1, number);
        Assert.Equal(displayName, commandCommon.DisplayName);
        Assert.Equal(internalIdentifier, commandCommon.InternalIdentifier);
        Assert.Equal(shouldBubble, commandCommon.ShouldBubble);
    }
}