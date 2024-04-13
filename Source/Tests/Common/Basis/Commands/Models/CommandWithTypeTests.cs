using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="CommandWithType{T}"/>
/// </summary>
public class CommandWithTypeTests
{
    /// <summary>
    /// <see cref="CommandNoType.DisplayName"/>
    /// <see cref="CommandNoType.InternalIdentifier"/>
    /// <see cref="CommandNoType.ShouldBubble"/>
    /// <see cref="CommandNoType.CommandFunc"/>
    /// </summary>
    [Fact]
    public async Task DoAsyncFunc()
    {
        var number = 0;

        var displayName = "Increment Number";
        var internalIdentifier = "increment-number";
        var shouldBubble = false;

        var commandWithType = (CommandWithType<CommonCommandArgs>)new CommonCommand(
            displayName,
            internalIdentifier,
            shouldBubble,
            commandArgs =>
            {
                number++;
                return Task.CompletedTask;
            });

        await commandWithType.CommandFunc.Invoke(new CommonCommandArgs());

        Assert.Equal(1, number);
        Assert.Equal(displayName, commandWithType.DisplayName);
        Assert.Equal(internalIdentifier, commandWithType.InternalIdentifier);
        Assert.Equal(shouldBubble, commandWithType.ShouldBubble);
    }
}
