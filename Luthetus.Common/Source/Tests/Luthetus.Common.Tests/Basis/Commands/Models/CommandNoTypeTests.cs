using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="CommandNoType"/>
/// </summary>
public class CommandNoTypeTests
{
    /// <summary>
    /// <see cref="CommandNoType.DisplayName"/>
    /// <see cref="CommandNoType.InternalIdentifier"/>
    /// <see cref="CommandNoType.ShouldBubble"/>
    /// <see cref="CommandNoType.DoAsyncFunc"/>
    /// </summary>
    [Fact]
    public async Task DoAsyncFunc()
    {
        var number = 0;

        var displayName = "Increment Number";
        var internalIdentifier = "increment-number";
        var shouldBubble = false;

        var commandNoType = (CommandNoType)new CommonCommand(
            displayName,
            internalIdentifier,
            shouldBubble,
            commandArgs =>
            {
                number++;
                return Task.CompletedTask;
            });
        
        await commandNoType.DoAsyncFunc.Invoke(new CommonCommandArgs());

        Assert.Equal(1, number);
        Assert.Equal(displayName, commandNoType.DisplayName);
        Assert.Equal(internalIdentifier, commandNoType.InternalIdentifier);
        Assert.Equal(shouldBubble, commandNoType.ShouldBubble);
    }
}
